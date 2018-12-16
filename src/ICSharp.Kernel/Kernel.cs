using System;
using Newtonsoft.Json;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharp.Kernel.Messages;
using System.Reflection;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ICSharp.Kernel
{
    public class Kernel
    {
        private ConnectionInformation connectionInformation;
        private RouterSocket hbSocket;
        private RouterSocket controlSocket;
        private PublisherSocket ioSocket;
        private RouterSocket shellSocket;
        private RouterSocket stdinSocket;
        private List<Payload> payload;
        private NuGetManager nugetManager;
        private int executionCount;
        private KernelMessage lastMessage;
        private string headerCode;
        private HMACSHA256 hmac;

        public Kernel(ConnectionInformation connectionInformation)
        {
            this.connectionInformation = connectionInformation;

            // heartbeat
            hbSocket = new RouterSocket();
            hbSocket.Bind($"{connectionInformation.transport}://{connectionInformation.ip}:{connectionInformation.hb_port}");

            // control 
            controlSocket = new RouterSocket();
            controlSocket.Bind($"{connectionInformation.transport}://{connectionInformation.ip}:{connectionInformation.control_port}");

            // stdin 
            stdinSocket = new RouterSocket();
            stdinSocket.Bind($"{connectionInformation.transport}://{connectionInformation.ip}:{connectionInformation.stdin_port}");

            // iopub 
            ioSocket = new PublisherSocket();
            ioSocket.Bind($"{connectionInformation.transport}://{connectionInformation.ip}:{connectionInformation.iopub_port}");

            // shell 
            shellSocket = new RouterSocket();
            shellSocket.Bind($"{connectionInformation.transport}://{connectionInformation.ip}:{connectionInformation.shell_port}");

            payload = new List<Payload>();
            nugetManager = new NuGetManager(new FileInfo(".").FullName);
            executionCount = 0;
            lastMessage = null;

            headerCode = getHeaderCode();
            hmac = new HMACSHA256(Encoding.UTF8.GetBytes(connectionInformation.key));
        }

        internal string getHeaderCode()
        {
            var file = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var dir = file.Directory.FullName;
            var includeFile = Path.Combine(dir, "Include.csx");
            var code = File.ReadAllText(includeFile);
            return code;
        }

        internal string sign(string[] list)
        {
            if (connectionInformation.key == "")
            {
                return "";
            }
            else
            {
                hmac.Initialize();
                foreach (var s in list)
                {
                    transformPart(s);
                }

                hmac.TransformFinalBlock(new byte[] { }, 0, 0);
                return BitConverter.ToString(hmac.Hash).Replace("-", "").ToLower();
            }
        }

        private void transformPart(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            byte[] outputBytes = new byte[bytes.Length];
            var num = hmac.TransformBlock(bytes, 0, bytes.Length, null, 0);
        }

        internal List<byte[]> recvAll(NetMQSocket socket)
        {
            return socket.ReceiveMultipartBytes();
        }

        internal KernelMessage recvMessage(NetMQSocket socket)
        {
            // receive all parts of the message
            var message = recvAll(socket).ToArray();
            var asStrings = message.Select(s => decode(s)).ToArray();

            // find the delimiter between IDS and MSG
            var idx = Array.IndexOf(asStrings, "<IDS|MSG>");
            var idents = message.Take(idx).ToArray();
            var messageList = asStrings.Skip(idx + 1).Take(asStrings.Length - 1 - idx).ToArray();

            // detect a malformed message
            if (messageList.Length < 4) throw new Exception("Malformed message");

            var hmac = messageList[0];
            var headerJson = messageList[1];
            var parentHeaderJson = messageList[2];
            var metadata = messageList[3];

            var contentJson = messageList.Length >= 5 ? messageList[4] : "{}";

            var header = JsonConvert.DeserializeObject<Header>(headerJson);
            var parentHeader = JsonConvert.DeserializeObject<Header>(parentHeaderJson);
            var metaDataDict = deserializeDict(metadata);
            var content = ShellMessages.Deserialize(header.msg_type, contentJson);

            var calculated_signature = sign(new string[] { headerJson, parentHeaderJson, metadata, contentJson });

            if (calculated_signature != hmac)
            {
                throw new Exception("Wrong message signature");
            }

            lastMessage = new KernelMessage()
            {
                Identifiers = idents.ToList(),
                HmacSignature = hmac,
                Header = header,
                ParentHeader = parentHeader,
                Metadata = metadata,
                Content = content
            };

            return lastMessage;
        }

        public Header createHeader(string messageType, KernelMessage sourceEnvelope)
        {
            var header = new Header()
            {
                msg_type = messageType,
                msg_id = Guid.NewGuid().ToString(),
                session = sourceEnvelope.Header.session,
                username = sourceEnvelope.Header.username
            };

            return header;
        }

        public void sendMessage(NetMQSocket socket, KernelMessage envelope, string messageType, object content)
        {
            var header = createHeader(messageType, envelope);
            var msg = new NetMQMessage();

            foreach (var ident in envelope.Identifiers)
            {
                msg.Append(ident);
            }

            var _header = serialize(header);
            var _parent_header = serialize(envelope.Header);
            var _meta = "{}";
            var _content = serialize(content);
            var _signature = sign(new string[] { _header, _parent_header, _meta, _content });

            msg.Append(encode("<IDS|MSG>"));
            msg.Append(encode(_signature));
            msg.Append(encode(_header));
            msg.Append(encode(_parent_header));
            msg.Append(encode(_meta));
            msg.Append(encode(_content));
            socket.SendMultipartMessage(msg);
        }

        public void sendState(KernelMessage envelope, string state)
        {
            sendMessage(ioSocket, envelope, "status", new { execution_state = state });
        }

        public void sendStateBusy(KernelMessage envelope)
        {
            sendState(envelope, "busy");
        }

        public void sendStateIdle(KernelMessage envelope)
        {
            sendState(envelope, "idle");
        }

        public void kernelInfoRequest(KernelMessage msg, KernelRequest content)
        {
            var reply = new KernelReply()
            {
                protocol_version = "4.0.0",
                implementation = "icsharp",
                implementation_version = "4.0.0",
                banner = "",
                help_links = new KernelReply_HelpLink[] { },
                language = "csharp",
                language_info = new KernelReply_LanguageInfo()
                {
                    name = "csharp",
                    version = "6.0.0.0",
                    mimetype = "text/x-csharp",
                    file_extension = ".cs",
                    pygments_lexer = "",
                    codemirror_mode = "",
                    nbconvert_exporter = ""
                }
            };
            sendStateBusy(msg);
            sendMessage(shellSocket, msg, "kernel_info_reply", reply);
        }

        public void sendDisplayData(string contentType, object displayItem, string messageType)
        {
            if (lastMessage != null)
            {
                var d = new Dictionary<string, object>();
                d.Add(contentType, displayItem);

                var reply = new Pyout() { execution_count = executionCount, data = d, metadata = new Dictionary<string, object>() };
                sendMessage(ioSocket, lastMessage, messageType, reply);
            }
        }

        public void pyout(object message)
        {
            sendDisplayData("text/plain", message, "pyout");
        }

        public string preprocessCode(string code)
        {
            logMessage(code);

            var preprocessing = nugetManager.Preprocess(code);
            var newCode = String.Join("\n", preprocessing.FilteredLines);

            if (preprocessing.HelpLines.Length > 0)
            {
                var icsharpHelp = "IC# notebook directives: " + Environment.NewLine +
                    "#r\t\tAdd a metadata reference to specified assembly and all its dependencies, e.g. #r \"myLib.dll\"." + Environment.NewLine +
                    "#load\t\tLoad specified script file and execute it, e.g. #load \"myScript.csx\".";
            }

            if (preprocessing.CsiOutputLines.Length > 0)
            {
                pyout("csioutput is not yet supported!");
            }

            if (preprocessing.NuGetLines.Length > 0)
            {
                var message = "#N Nuget is not yet supported.";
                pyout(message);
            }
            return newCode;
        }

        internal void executeRequest(KernelMessage msg, ExecuteRequest content)
        {
            Evaluation.sbOut.Clear();
            Evaluation.sbErr.Clear();
            Evaluation.sbPrint.Clear();
            payload.Clear();

            if (!content.silent) executionCount++;

            // sending busy 
            sendStateBusy(msg);
            sendMessage(ioSocket, msg, "pyin", new Pyin() { code = content.code, execution_count = executionCount });

            // preprocess
            var newCode = preprocessCode(content.code);

            if (!String.IsNullOrEmpty(newCode))
            {
                // evaluate 
                try
                {
                    (var value, var errors) = Evaluation.EvalInteractionNonThrowing(newCode);

                    if (errors.Length > 0)
                    {
                        var sb = errors.Aggregate(new StringBuilder(), (ag, n) => ag.AppendLine(n));
                        sendError(sb.ToString(), msg);
                    }
                    var executeReply = new ExecuteReplyOk()
                    {
                        status = "ok",
                        execution_count = executionCount,
                        payload = payload.ToArray(),
                        user_variables = new Dictionary<string, object>(),
                        user_expressions = new Dictionary<string, object>()
                    };

                    sendMessage(shellSocket, msg, "execute_reply", executeReply);

                    // send all the data
                    if (!content.silent)
                    {
                        var lastExpression = Evaluation.GetLastExpression();

                        if (lastExpression != null)
                        {
                            var printedResult = Printers.PrintVariable(lastExpression);
                            sendDisplayData(printedResult.ContentType, printedResult.Data, "pyout");
                        }
                    }
                }
                catch (AggregateException ae)
                {
                    var esb = new StringBuilder();
                    esb.AppendLine(ae.Message);

                    foreach (var e in ae.InnerExceptions)
                    {
                        esb.AppendLine(e.Message);
                    }

                    sendError(esb.ToString(), msg);
                }
                catch (Exception ex)
                {
                    var error = $"Expression evaluation failed: {ex.Message}\r\n {ex.StackTrace}";
                    sendError(error, msg);
                }
            }

            if (Evaluation.sbPrint.Length > 0)
            {
                sendDisplayData("text/plain", Evaluation.sbPrint.ToString(), "display_data");
            }

            sendStateIdle(msg);
        }

        internal void sendError(string error, KernelMessage msg)
        {
            var executeReply = new ExecuteReplyError()
            {
                status = "error",
                execution_count = executionCount,
                ename = "generic",
                evalue = error,
                traceback = new string[] { }
            };

            sendMessage(shellSocket, msg, "execute_reply", executeReply);
            sendMessage(ioSocket, msg, "stream", new { name = "stderr", data = error });
            logMessage(error);
        }

        public void intellisenseRequest(KernelMessage msg, IntellisenseRequest content)
        {
            var codeCells = JsonConvert.DeserializeObject<List<String>>(content.text);
            codeCells.Add(headerCode);

            var position = JsonConvert.DeserializeObject<BlockType>(content.block);
            var lineOffset = 0;
           
            foreach (var line in codeCells.Take(position.selectedIndex + 1))
            {
                lineOffset += line.Split('\n').Length;
            }

            var realLineNumber = position.line + lineOffset + 1;
            var codeString = string.Join("\n", codeCells);


            // TODO: Finish implementing intellisense... 
            // Evaluation.GetDeclarations(codeString, realLineNumber, position.ch);
            var newContent = new CompleteReply()
            {
                matched_text = "matched text",
                filter_start_index = 0,
                matches = new object(),
                status = "ok"
            };

            sendDisplayData("errors", new object(), "display_data");
            sendMessage(shellSocket, msg, "complete_reply", newContent);
        }

        private void completeRequest(KernelMessage msg, CompleteRequest r)
        {
            // using intellisense request instead.
        }

        public void connectRequest(KernelMessage msg, ConnectRequest content)
        {
            var reply = new ConnectReply()
            {
                hb_port = connectionInformation.hb_port,
                iopub_port = connectionInformation.iopub_port,
                shell_port = connectionInformation.shell_port,
                stdin_port = connectionInformation.stdin_port
            };

            logMessage("connectRequest()");
            sendMessage(shellSocket, msg, "connect_reply", reply);
        }

        public void shutdownRequest(KernelMessage msg, ShutdownRequest content)
        {
            logMessage("shutdown request");
            var reply = new ShutdownReply() { restart = true };
            sendMessage(shellSocket, msg, "shutdown_reply", reply);
            System.Environment.Exit(0);
        }

        public void historyRequest(KernelMessage msg, HistoryRequest content)
        {
            var reply = new HistoryReply() { history = new List<string>() };
            sendMessage(shellSocket, msg, "history_reply", reply);
        }

        public void objectInfoRequest(KernelMessage msg, ObjectInfoRequest content)
        {
            // TODO: Implement this... 
        }

        public void inspectRequest(KernelMessage msg, InspectRequest content)
        {
            var reply = new InspectReply() { status = "ok", found = false, data = new Dictionary<string, object>(), metadata = new Dictionary<string, object>() };
            sendMessage(shellSocket, msg, "inspect_reply", reply);
        }

        internal void doShell()
        {
            try
            {
                var preprocessedCode = preprocessCode(headerCode); // not sure if this is needed 

                Evaluation.EvalInteraction(preprocessedCode);

                logMessage(Evaluation.sbErr.ToString());
                logMessage(Evaluation.sbOut.ToString());
            }
            catch (Exception ex)
            {
                // failed at executing header code. 
                handleException(ex);
            }


            while (true)
            {
                var msg = recvMessage(shellSocket);

                try
                {
                    switch (msg.Content)
                    {
                        case KernelRequest r:
                            kernelInfoRequest(msg, r);
                            break;
                        case ExecuteRequest r:
                            executeRequest(msg, r);
                            break;
                        case CompleteRequest r:
                            completeRequest(msg, r);
                            break;
                        case IntellisenseRequest r:
                            intellisenseRequest(msg, r);
                            break;
                        case ConnectRequest r:
                            connectRequest(msg, r);
                            break;
                        case ShutdownRequest r:
                            shutdownRequest(msg, r);
                            break;
                        case HistoryRequest r:
                            historyRequest(msg, r);
                            break;
                        case ObjectInfoRequest r:
                            objectInfoRequest(msg, r);
                            break;
                        case InspectRequest r:
                            inspectRequest(msg, r);
                            break;
                        default:
                            logMessage($"Unexpected content type on shell. msg_type is {msg.Header.msg_type} ");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    handleException(ex);
                }
            }
        }

        internal void doControl()
        {
            while (true)
            {
                var msg = recvMessage(controlSocket);

                try
                {
                    switch (msg.Content)
                    {
                        case ShutdownRequest r:
                            shutdownRequest(msg, r);
                            break;
                        default:
                            logMessage($"Unexpected content type on control. msg_type is {msg.Header.msg_type}.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    handleException(ex);
                    throw;
                }
            }
        }

        internal void doHeartbeat()
        {
            try
            {
                while (true)
                {
                    var hb = hbSocket.ReceiveMultipartBytes();
                    hbSocket.SendMultipartBytes(hb);
                }
            }
            catch (Exception ex)
            {
                handleException(ex);
            }
        }

        internal void ClearDisplay()
        {
            if (lastMessage != null)
            {
                var clear = new ClearOutput()
                {
                    wait = false,
                    stedrr = true,
                    stdout = true,
                    other = true
                };

                sendMessage(ioSocket, lastMessage, "clear_output", clear);
            }
        }

        internal void AddPayload(string text)
        {
            var pl = new Payload()
            {
                html = "",
                source = "page",
                start_line_number = 1,
                text = text
            };

            payload.Add(pl);
        }

        public void SendDisplayData(string contentType, object displayItem)
        {
            sendDisplayData(contentType, displayItem, "display_data");
        }

        internal void logMessage(string msg)
        {
            var fileName = "shell.log";
            var messages = msg.Split('\r', '\n')
                .Where(l => !String.IsNullOrWhiteSpace(l))
                .Select(i => $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {i}")
                .ToArray();

            try
            {
                File.AppendAllLines(fileName, messages);
            }
            catch
            {
                // do nothing 
            }

        }

        internal void handleException(Exception ex)
        {
            string msg = ex.ToString();
            logMessage(msg);
        }

        internal string decode(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        internal byte[] encode(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        internal Dictionary<string, string> deserializeDict(string str)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(str);
        }

        internal string serialize(object obj)
        {
            var ser = new JsonSerializer();
            var sw = new StringWriter();
            ser.Serialize(sw, obj);
            return sw.ToString();
        }

        internal void StartAsync()
        {
            Task.Run(() => { doShell(); });
            Task.Run(() => { doControl(); });
        }

    }
}
