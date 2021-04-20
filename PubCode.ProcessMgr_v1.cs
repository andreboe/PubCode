// !! FChecksum: 185DE8327BF6BCB5B848114A3B98435EF35BCE138BB92E0A8B6311AE34138C7D6498BDB2D1A8DCF0422422F3D8725DE84D42EBDC0CCB3CC0DBCC1C199F905FF1

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.ProcessMgr_v1.cs

//DO NOT MODIFY. THIS FILE IS FROZEN. IF NEED BE, A NEW STANDALONE VERSION WILL BE ISSUED.
//////////////////////////////////////////////////////////////////////////////////////////

//ALL USAGE OF THIS FILE IN PROJECTS SHALL BE DONE BY LINKING THE FILE. NOT COPYING IT.
//////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace PubCode.ProcessMgr_v1 //v2021.4.20.1
{
    public static class ProcessMgr
    {
        public static RunResult Run(string paramJSON, List<string> messageBackToCaller, string errorCrumb = "")
        {
            if (Debugger.IsAttached && paramJSON == null)
            {
                MessageBox.Show("DEBUG!");
                return null;
            }
            errorCrumb += "|PubCode.ProcessMgr_v1.cs~Run";
            RunParam param = null;
            if (paramJSON != null)
                param = new JavaScriptSerializer().Deserialize<ProcessMgr.RunParam>(paramJSON);
            return Run(param, messageBackToCaller, errorCrumb);
        }

        public static RunResult Run(RunParam param, List<string> messageBackToCaller, string errorCrumb = "")
        {
            if (Debugger.IsAttached && param==null)
            {
                MessageBox.Show("DEBUG!");
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
                    MessageBox.Show("GO FIND IT; " + errTxt);
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
