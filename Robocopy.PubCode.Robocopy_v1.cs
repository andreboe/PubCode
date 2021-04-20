// !! FChecksum: 8A788E4E37F1C42F5BFA83A5B4BF06314E53CAE7D929F8C42B6013D58D97B62B0BEDB056B80E05974ACDC4A8ACEF868583B42E0A2B1AF24D25D7DF361B3882C0

//Official Location: https://github.com/andreboe/PubCode/blob/main/Robocopy.PubCode.Robocopy_v1.cs

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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PubCode.Robocopy_v1 //v2021.4.20.1
{
    public static class Robocopy
    {
        public static void MIR(string param_ReInit_FROM, string param_ReInit_TO, bool pressEnterToContineBeforeDisplayingResults, string errorCrumb = "")
        {
            errorCrumb += "|Robocopy.PubCode.Robocopy_v1.cs~MIR";
            try
            {
                var unixTimeNow = (int)(DateTime.UtcNow - DateTime.Parse("1970-01-01")).TotalSeconds;
                var logfilePath = Path.Combine(Path.GetTempPath(), $"zRobocopy.PubCode.Robocopy_v1.MIR.{unixTimeNow}.{RandomNumber(1000, 9999)}.log");
                if (File.Exists(logfilePath)) File.Delete(logfilePath);

                string prmOutput_OutputString = "";
                string prmOutput_ErrorString = "";
                string statusText = null;
                string errorText = null;
                
                try
                {
                    RoboCopy_Mirror(param_ReInit_FROM, param_ReInit_TO, out prmOutput_OutputString, out prmOutput_ErrorString, out statusText, out errorText, logfilePath, errorCrumb);
                }
                catch (Exception ex)
                {
                    throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; MIR({param_ReInit_FROM}, {param_ReInit_TO}, {pressEnterToContineBeforeDisplayingResults})", ex);
                }

                string logfileContents = null;
                if (File.Exists(logfilePath))
                {
                    logfileContents = System.IO.File.ReadAllText(logfilePath);
                    File.Delete(logfilePath);
                }

                if (pressEnterToContineBeforeDisplayingResults)
                {
                    Console.WriteLine("Press [Enter] to continue");
                    Console.ReadLine();
                }

                Console.WriteLine($"statusText:             \r\n{new string('-', 50)}\r\n{statusText ?? "null"}\r\n\r\n");
                Console.WriteLine($"errorText:              \r\n{new string('-', 50)}\r\n{errorText ?? "null"}\r\n\r\n");
                Console.WriteLine($"prmOutput_OutputString: \r\n{new string('-', 50)}\r\n{prmOutput_OutputString ?? "null"}\r\n\r\n");
                Console.WriteLine($"prmOutput_ErrorString:  \r\n{new string('-', 50)}\r\n{prmOutput_ErrorString ?? "null"}\r\n\r\n");
                Console.WriteLine($"logfileContents:        \r\n{new string('-', 50)}\r\n{logfileContents ?? "null"}\r\n\r\n");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex}");
            }
        }

        private static void RoboCopy_Mirror(string from_folder, string to_folder, out string prmOutput_OutputString, out string prmOutput_ErrorString, out string statusText, out string errorText, string logfilePath, string errorCrumb)
        {
            errorCrumb += "|Robocopy.PubCode.Robocopy_v1.cs~RoboCopy_Mirror";
            Validate_parameters_and_throw_Exception_if_not_valid_for_Robocopy_mirror_command(from_folder, to_folder, errorCrumb);

            var RobocopyLog_FileNm = logfilePath; // Path.Combine(Path.GetTempPath(), logfileName);
            var RoboCopyArgs = BuildRoboCopyArgString____thisRunsInsideAProcess(from_folder, to_folder, RobocopyLog_FileNm);

            // http://net-informations.com/q/mis/robocopy.html

            int ExitCode = 0;
            try
            {
                ExitCode = RunProccess("robocopy.exe", RoboCopyArgs, out prmOutput_OutputString, out prmOutput_ErrorString, null, errorCrumb);
            }
            catch (Exception ex)
            {
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; RoboCopy_Mirror({from_folder}, {to_folder}, out prmOutput_OutputString, out prmOutput_ErrorString, out statusText, out errorText, out logfilePath)", ex);
            }

            // Process our return codes
            var switchExpr = ExitCode;
            statusText = null;
            errorText = null;
            switch (switchExpr)
            {
                case 0: // Nothing to copy
                    {
                        statusText = $"     No Files Deployed. ExitCode: ({ExitCode}) - See Robocopy logs";
                        break;
                    }

                case 1: // Things copied successfully
                    {
                        statusText = $"     File Deployed. ExitCode: ({ExitCode}) - See Robocopy logs";
                        break;
                    }

                case 2: // Some Extra files or directories were detected. No files were copied Examine the output log for details.
                    {
                        statusText = $"     Some Extra files or directories were detected. No files were copied. ExitCode: ({ExitCode}) - See Robocopy logs";
                        break;
                    }

                case 3: // (2+1) Some files were copied. Additional files were present. No failure was encountered.
                    {
                        statusText = $"     Some files were copied. Additional files were present. No failure was encountered. ExitCode: ({ExitCode}) - See Robocopy logs"; // Oops! there was an issue
                        break;
                    }

                default:
                    {
                        errorText = $"[ERROR] There was an error copying files. See Robocopy logs. Exit Code ({ExitCode}";
                        break;
                    }
            }

            if (prmOutput_OutputString != null)
            {
                while (true)
                {
                    if (true
                        && !prmOutput_OutputString.StartsWith("\r")
                        && !prmOutput_OutputString.StartsWith("\n")
                        && !prmOutput_OutputString.StartsWith("\t")
                        && !prmOutput_OutputString.StartsWith(" ")
                        && true)
                        break;
                    if (prmOutput_OutputString.StartsWith("\r")) prmOutput_OutputString = prmOutput_OutputString.Substring(1);
                    if (prmOutput_OutputString.StartsWith("\n")) prmOutput_OutputString = prmOutput_OutputString.Substring(1);
                    if (prmOutput_OutputString.StartsWith("\t")) prmOutput_OutputString = prmOutput_OutputString.Substring(1);
                    if (prmOutput_OutputString.StartsWith(" ")) prmOutput_OutputString = prmOutput_OutputString.Substring(1);
                }
            }

            if (string.IsNullOrWhiteSpace(statusText)) statusText = null;
            if (string.IsNullOrWhiteSpace(errorText)) errorText = null;
            if (string.IsNullOrWhiteSpace(prmOutput_OutputString)) prmOutput_OutputString = null;
            if (string.IsNullOrWhiteSpace(prmOutput_ErrorString)) prmOutput_ErrorString = null;
        }

        private static int RunProccess(string prmProcessName, string prmArguments, out string prmOutput_OutputString, out string prmOutput_ErrorString, string prmWorkingDirectory, string errorCrumb)
        {
            errorCrumb += "|Robocopy.PubCode.Robocopy_v1.cs~Validate_parameters_and_throw_Exception_if_not_valid_for_Robocopy_mirror_command";
            Console.WriteLine($"Process.Run: \"{prmProcessName}\" {prmArguments}");

            prmOutput_OutputString = "";
            prmOutput_ErrorString = "";

            string retValue = string.Empty;
            string OutputString;
            string ErrorString;
            int ExitCode;
            try
            {
                using (var myProcess = new Process())
                {
                    // Setup the command

                    ProcessStartInfo startInfo;
                    startInfo = new ProcessStartInfo()
                    {
                        FileName = prmProcessName,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = prmArguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    if (prmWorkingDirectory != null)
                    {
                        startInfo.WorkingDirectory = prmWorkingDirectory;
                    }

                    myProcess.StartInfo = startInfo;
                    myProcess.Start();
                    OutputString = myProcess.StandardOutput.ReadToEnd();
                    ErrorString = myProcess.StandardError.ReadToEnd();
                    myProcess.WaitForExit();
                    ExitCode = myProcess.ExitCode;
                    myProcess.Close();
                }

                prmOutput_OutputString = OutputString;
                prmOutput_ErrorString = ErrorString;
            }
            catch (Exception ex)
            {
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; RunProccess(\"{prmProcessName ?? "(null)"}\", \"{prmArguments ?? "(null)"}\", \"{prmOutput_OutputString ?? "(null)"}\", \"{prmOutput_ErrorString ?? "(null)"}\", \"{prmWorkingDirectory??"(null)"}\"), prmArguments={prmArguments}", ex);
            }

            return ExitCode;
        }

        private static string getOSInfo()
        {
            var os = Environment.OSVersion;
            var vs = os.Version;
            string operatingSystem = "";
            if (os.Platform == PlatformID.Win32Windows)
            {
                var switchExpr = vs.Minor;
                switch (switchExpr)
                {
                    case 0:
                        {
                            operatingSystem = "95";
                            break;
                        }

                    case 10:
                        {
                            if ((vs.Revision.ToString() ?? "") == "2222A")
                            {
                                operatingSystem = "98SE";
                            }
                            else
                            {
                                operatingSystem = "98";
                            }

                            break;
                        }

                    case 90:
                        {
                            operatingSystem = "Me";
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                var switchExpr1 = vs.Major;
                switch (switchExpr1)
                {
                    case 3:
                        {
                            operatingSystem = "NT 3.51";
                            break;
                        }

                    case 4:
                        {
                            operatingSystem = "NT 4.0";
                            break;
                        }

                    case 5:
                        {
                            if (vs.Minor == 0)
                            {
                                operatingSystem = "2000";
                            }
                            else
                            {
                                operatingSystem = "XP";
                            }

                            break;
                        }

                    case 6:
                        {
                            if (vs.Minor == 0)
                            {
                                operatingSystem = "Vista";
                            }
                            else if (vs.Minor == 1)
                            {
                                operatingSystem = "7";
                            }
                            else if (vs.Minor == 2)
                            {
                                operatingSystem = "8";
                            }
                            else
                            {
                                operatingSystem = "8.1";
                            }

                            break;
                        }

                    case 10:
                        {
                            operatingSystem = "10";
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            if (!string.IsNullOrEmpty(operatingSystem))
            {
                operatingSystem = "Windows " + operatingSystem;
                if (!string.IsNullOrEmpty(os.ServicePack))
                {
                    operatingSystem += " " + os.ServicePack;
                }
            }

            return operatingSystem;
        }

        private static bool os_is_Win7()
        {
            return getOSInfo().StartsWith("Windows 7");
        }

        private static string BuildRoboCopyArgString____thisRunsInsideAProcess(string prmSourceFolder, string prmTargetFolder, string prmRobocopyLog)
        {
            var strBldr = new StringBuilder();
            strBldr.Append($"\"{prmSourceFolder}\" \"{prmTargetFolder}\" ");
            strBldr.Append("/MIR /R:10 /NODCOPY /E ");
            if (os_is_Win7())
                strBldr.Replace("/NODCOPY", ""); // This option is not compatible with Windows 7
            strBldr.Append($"/LOG+:\"{prmRobocopyLog}\" ");
            return strBldr.ToString();
        }

        private static void Validate_parameters_and_throw_Exception_if_not_valid_for_Robocopy_mirror_command(string from_folder, string to_folder, string errorCrumb)
        {
            errorCrumb += "|Robocopy.PubCode.Robocopy_v1.cs~Validate_parameters_and_throw_Exception_if_not_valid_for_Robocopy_mirror_command";
            if (string.IsNullOrWhiteSpace(from_folder))
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; from_folder is empty string. RoboCopy_Mirror(\"{from_folder}\", \"{to_folder}\")");
            if (string.IsNullOrWhiteSpace(to_folder))
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; to_folder is empty string. RoboCopy_Mirror(\"{from_folder}\", \"{to_folder}\")");
            from_folder = from_folder.Trim();
            to_folder = to_folder.Trim();
            if (new char[] { '\\', '/' }.Contains(from_folder[from_folder.Length - 1]))
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; from_folder are not allowed to end with a \"{from_folder[from_folder.Length - 1]}\" when used as a Robocopy mirror folder parameter. RoboCopy_Mirror(\"{from_folder}\", \"{to_folder}\")");
            if (new char[] { '\\', '/' }.Contains(to_folder[to_folder.Length - 1]))
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; to_folder are not allowed to end with a \"{to_folder[to_folder.Length - 1]}\" when used as a Robocopy mirror folder parameter. RoboCopy_Mirror(\"{from_folder}\", \"{to_folder}\")");
            if (!Directory.Exists(from_folder))
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; from_folder does not exist. RoboCopy_Mirror(\"{from_folder}\", \"{to_folder}\")");
            if (!Directory.Exists(to_folder))
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; to_folder does not exist. RoboCopy_Mirror(\"{from_folder}\", \"{to_folder}\")");
        }

        // Instantiate random number generator.  
        private static readonly Random _random = new Random();

        // Generates a random number within a range.      
        private static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
