// !! FChecksum: 5E4C24EF1D2185B7E9326D6FBA94032E5B4D48B49B7E8CE3C66803A58A1798893CF085AC897234EE542A4B3960B7535537CBA75327B7E48D705FAFF732F5554E

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.ProcessMgr_v1.cs

//DO NOT MODIFY. THIS FILE IS FROZEN. IF NEED BE, A NEW STANDALONE VERSION WILL BE ISSUED.
//////////////////////////////////////////////////////////////////////////////////////////

//ALL USAGE OF THIS FILE IN PROJECTS SHALL BE DONE BY LINKING THE FILE. NOT COPYING IT.
//////////////////////////////////////////////////////////////////////////////////////////

#region License; This is free and unencumbered software released into the public domain.
/// This is free and unencumbered software released into the public domain.
/// 
/// Anyone is free to copy, modify, publish, use, compile, sell, or
/// distribute this software, either in source code form or as a compiled
/// binary, for any purpose, commercial or non-commercial, and by any
/// means.
/// 
/// In jurisdictions that recognize copyright laws, the author or authors
/// of this software dedicate any and all copyright interest in the
/// software to the public domain. We make this dedication for the benefit
/// of the public at large and to the detriment of our heirs and
/// successors. We intend this dedication to be an overt act of
/// relinquishment in perpetuity of all present and future rights to this
/// software under copyright law.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
/// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
/// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
/// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
/// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
/// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
/// OTHER DEALINGS IN THE SOFTWARE.
/// 
/// For more information, please refer to <https://unlicense.org/>
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace PubCode.ProcessMgr_v1 //v2021.10.23.1
{
    public static class ProcessMgr
    {
        public static RunResult Run(string paramJSON, List<string> messageBackToCaller, string errorCrumb = "")
        {
            if (Debugger.IsAttached && paramJSON == null)
            {
                Debugger.Break(); //MessageBox.Show("DEBUG!");
                return null;
            }
            errorCrumb += "|PubCode.ProcessMgr_v1.cs~Run";
            RunParam param = null;
            if (paramJSON != null)
#if NETFRAMEWORK
                param = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<ProcessMgr.RunParam>(paramJSON);
#elif NET
                param = System.Text.Json.JsonSerializer.Deserialize<ProcessMgr.RunParam>(paramJSON);
#else
                throw new NotImplementedException();
#endif
            return Run(param, messageBackToCaller, errorCrumb);
        }

        public static RunResult Run(RunParam param, List<string> messageBackToCaller, string errorCrumb = "")
        {
            if (Debugger.IsAttached && param==null)
            {
                Debugger.Break(); //MessageBox.Show("DEBUG!");
                return null;
            }
            errorCrumb += "|PubCode.ProcessMgr_v1.cs~Run";
            ValidateParameters(param, errorCrumb);
            return RunProccess(param, messageBackToCaller, errorCrumb);
        }

        [DataContract] public class RunParam
        {
            [DataMember] public string AppPath { get; set; }
            [DataMember] public string AppArgs { get; set; }
            [DataMember] public string WorkingDirectory { get; set; }

            public RunParam()
            {
            }
            public RunParam(string appPath, string appArgs, string workingDirectory)
            {
                AppPath = appPath;
                AppArgs = appArgs;
                WorkingDirectory = workingDirectory;
            }
            public override string ToString()
            {
                return $"AppPath={AppPath}, AppArgs={AppArgs}, WorkingDirectory={WorkingDirectory}";
            }
        }

        [DataContract] public class RunResult
        {
            [DataMember] public string OutputString { get; set; }
            [DataMember] public string ErrorString { get; set; }
            [DataMember] public int ExitCode { get; set; }
        }

        private static RunResult RunProccess(RunParam param, List<string> messageBackToCaller, string errorCrumb)
        {
            errorCrumb += "|PubCode.ProcessMgr_v1.cs~RunProccess";
            var prmOutput = new RunResult();

            string OutputString;
            string ErrorString;
            int ExitCode;
            try
            {
                var appArgs = param.AppArgs;
                if (appArgs == "\"\"")
                    appArgs = "";

                messageBackToCaller.Add($"### RunProcess ### {param.AppPath} {appArgs}");
                using (var myProcess = new System.Diagnostics.Process())
                {
                    ProcessStartInfo startInfo;
                    startInfo = new ProcessStartInfo()
                    {
                        FileName = param.AppPath,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = appArgs,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    if (!string.IsNullOrWhiteSpace(param.WorkingDirectory))
                        startInfo.WorkingDirectory = param.WorkingDirectory;

                    myProcess.StartInfo = startInfo;
                    myProcess.Start();
                    OutputString = myProcess.StandardOutput.ReadToEnd();
                    ErrorString = myProcess.StandardError.ReadToEnd();
                    myProcess.WaitForExit();
                    ExitCode = myProcess.ExitCode;
                    myProcess.Close();
                }

                prmOutput.OutputString = OutputString;
                prmOutput.ErrorString = ErrorString;
                prmOutput.ExitCode = ExitCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; RunProccess(({param})", ex);
            }

            return prmOutput;
        }

        private static void ValidateParameters(RunParam param, string errorCrumb)
        {
            errorCrumb += "|PubCode.ProcessMgr_v1.cs~ValidateParameters";
            if (param == null || string.IsNullOrWhiteSpace(param.AppPath))
            {
                var errTxt = $"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; AppPath is empty string. ValidateParameters({param})";
                if (Debugger.IsAttached)
                {
                    Debugger.Break(); //MessageBox.Show("GO FIND IT; " + errTxt);
                    return;
                }
                throw new Exception(errTxt);
            }
        }

        private static readonly Random _random = new Random();
        private static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
