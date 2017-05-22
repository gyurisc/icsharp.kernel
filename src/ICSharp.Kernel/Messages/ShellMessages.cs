using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharp.Kernel.Messages
{
    public class ShellMessages
    {
        internal static ShellMessage Deserialize(string msg_type, string messageJson)
        {
            switch (msg_type)
            {
                case "execute_request":
                    return JsonConvert.DeserializeObject<ExecuteRequest>(messageJson);
                case "execute_reply_ok":
                    return JsonConvert.DeserializeObject<ExecuteReplyOk>(messageJson);
                
                case "execute_reply_error":
                    return JsonConvert.DeserializeObject<ExecuteReplyError>(messageJson);
        
                case "object_info_request":
                    return JsonConvert.DeserializeObject<ObjectInfoRequest>(messageJson);

                case "inspect_request":
                    return JsonConvert.DeserializeObject<InspectRequest>(messageJson);

                case "complete_request":
                    return JsonConvert.DeserializeObject<CompleteRequest>(messageJson);

                case "complete_reply":
                    return JsonConvert.DeserializeObject<CompleteReply>(messageJson);
                
                case "intellisense_request":
                    return JsonConvert.DeserializeObject<IntellisenseRequest>(messageJson);
                
                case "history_request":
                    return JsonConvert.DeserializeObject<HistoryRequest>(messageJson);

                case "history_reply":
                    return JsonConvert.DeserializeObject<HistoryReply>(messageJson);
                
                case "connect_request":
                    return JsonConvert.DeserializeObject<ConnectRequest>(messageJson);

                case "connect_reply":
                    return JsonConvert.DeserializeObject<ConnectReply>(messageJson);
                
                case "kernel_info_request":
                    return JsonConvert.DeserializeObject<KernelRequest>(messageJson);

                case "kernel_info_reply":
                    return JsonConvert.DeserializeObject<KernelReply>(messageJson);
                
                case "shutdown_request":
                    return JsonConvert.DeserializeObject<ShutdownRequest>(messageJson);

                case "shutdown_reply":
                    return JsonConvert.DeserializeObject<ShutdownReply>(messageJson);
                
                case "comm_open":
                    return JsonConvert.DeserializeObject<CommOpen>(messageJson);
                
                case "comm_info_request":
                    return JsonConvert.DeserializeObject<CommInfoRequest>(messageJson);

                default:
                    throw new Exception($"Unsupported messageType: {msg_type} ");
            }
        }
    }
}
