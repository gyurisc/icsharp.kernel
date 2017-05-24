[![Build Status](https://travis-ci.org/gyurisc/icsharp.kernel.svg?branch=master)](https://travis-ci.org/gyurisc/icsharp.kernel)
[![License](https://img.shields.io/badge/License-BSD%203--Clause-blue.svg)](https://opensource.org/licenses/BSD-3-Clause)

# ICSharp.Kernel

This is a [Roslyn](https://github.com/dotnet/roslyn) based C# kernel for [Jupyter](http://jupyter.org/). View the [Feature Notebook](CSharp_Jupyter_Notebook.ipynb) for features that are currently working in this kernel. 

# Building 

 Open ICSharpKernel.sln in Visual Studio 2017 on Windows or Mac and Build, or from the command line type msbuild.

# Installation (Windows)
1. Install [Anaconda](http://continuum.io/downloads)
2. Install [Jupyter](http://jupyter.readthedocs.org/en/latest/install.html)
3. Clone the repository git clone https://github.com/gyurisc/icsharp.kernel
4. Build the project
4. Run IfSharp.exe

# Manual Installation (Mac)
1. Install [Anaconda](http://continuum.io/downloads)
2. Install [Jupyter](http://jupyter.readthedocs.org/en/latest/install.html)
3. Install [Mono](http://www.mono-project.com/download/) (tested 4.2.4)
...


# Todo 

 - Make Printers work (can print lists and tables )
 - Make intellisense work. 


# Notes 

The code is based on [IfSharp](https://github.com/fsprojects/IfSharp) kernel. 
