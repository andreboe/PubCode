// !! FChecksum: E4A1C2A795F21034D73CCD5430DE44276213BBB38E1037661CA3D02D8CC2A91CE21040BC2541389A3E11018C756E932326D1C1694CB77221249CEFA66F16212D

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCodeUpdateEXE_v1.UpdateEXE.cs

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
using System.IO;
using System.Reflection;

namespace PubCode.UpdateEXE_v1 //v2021.4.20.1
{
    public static class UpdateEXE
    {
        private const bool DISABLE_TRY_CATCH = false;

        public static void UpdateCurrentlyRunningExecutable_withLatestFromOfficialDistributionSource(string officialDistributionSourceFilePath, string errorCrumb = "")
        {
            errorCrumb += "|PubCodeUpdateEXE_v1.UpdateEXE.cs~UpdateCurrentlyRunningExecutable_withLatestFromOfficialDistributionSource";
            var distroFileName = Path.GetFileName(officialDistributionSourceFilePath);
            string runningApp_FilePath = Assembly.GetEntryAssembly().Location;
            string runningApp_Directory = System.IO.Path.GetDirectoryName(runningApp_FilePath);
            string runningApp_FileName = System.IO.Path.GetFileName(runningApp_FilePath);
            if (!distroFileName.Equals(runningApp_FileName, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.Error.WriteLine($"Error."
                    + $"\r\nThe application you are running are named \"{Path.GetFileNameWithoutExtension(runningApp_FileName)}\"."
                    + $"\r\nThis does not match the allowed name \"{Path.GetFileNameWithoutExtension(distroFileName)}\".");
            }
            else if (!UpdateEXE.successfullyVerifiedExistenceOfFile(officialDistributionSourceFilePath))
            {
                Console.Error.WriteLine($"Error. The master copy of the application cannot be found {officialDistributionSourceFilePath}.");
            }
            else if (UpdateEXE.DirectoriesAreTheSame(runningApp_Directory, Path.GetDirectoryName(officialDistributionSourceFilePath), errorCrumb))
            {
                Console.Error.WriteLine($"Error. You are running the master copy of the application. This cannot be updated from the master copy as it is itself {officialDistributionSourceFilePath}.");
            }
            else if ("bin".Equals(Directory.GetParent(runningApp_Directory).Name, StringComparison.CurrentCultureIgnoreCase))
            {
                if (Debugger.IsAttached)
                    Console.Error.WriteLine($"Error. You are running from the Visual Studio binary output folder. Update parameter is not supported for this development folder. In fact... you are even running this app from inside of Visual Studio in Debug Mode...");
                else
                    Console.Error.WriteLine($"Error. You are running from the Visual Studio binary output folder. Update parameter is not supported for this development folder.");
            }
            else if ("bin".Equals(Directory.GetParent(runningApp_Directory).Name, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.Error.WriteLine($"Error. You are running from the Visual Studio binary output folder. Update parameter is not supported for this development folder.");
            }
            else
            {
                if (DISABLE_TRY_CATCH)
                    UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER____withTryCatchAndWritesToStdErr(officialDistributionSourceFilePath, errorCrumb);
                else
                    try
                    {
                        UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER____withTryCatchAndWritesToStdErr(officialDistributionSourceFilePath, errorCrumb);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR: {ex.Message}");
                    }
            }
        }

        private static void UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER____withTryCatchAndWritesToStdErr(string officialDistributionSourceFilePath, string errorCrumb)
        {
            errorCrumb += "|PubCodeUpdateEXE_v1.UpdateEXE.cs~UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER____withTryCatchAndWritesToStdErr";
            if (Debugger.IsAttached)
                UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER(officialDistributionSourceFilePath, errorCrumb);
            else
            {
                if (DISABLE_TRY_CATCH)
                    UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER(officialDistributionSourceFilePath, errorCrumb);
                else
                    try
                    {
                        UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER(officialDistributionSourceFilePath, errorCrumb);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"ERROR: {ex.Message}");
                    }
            }
        }

        private static void UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER(string officialDistributionSourceFilePath, string errorCrumb)
        {
            errorCrumb += "|PubCodeUpdateEXE_v1.UpdateEXE.cs~UpdatePersonalCopyInWindowsToolsFolder_fromOfficialDistributionSource_INNER";
            string runningApp_FilePath = Assembly.GetEntryAssembly().Location;
            string runningApp_Directory = System.IO.Path.GetDirectoryName(runningApp_FilePath);

            string officialDistributionSourceFileName = System.IO.Path.GetFileName(officialDistributionSourceFilePath);
            string runningApp_FileName = System.IO.Path.GetFileName(runningApp_FilePath);
            if (!runningApp_FileName.Equals(officialDistributionSourceFileName, StringComparison.CurrentCultureIgnoreCase))
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; The name of the program you are running are {runningApp_FileName}.\r\nThe name in the official distribution folder is {officialDistributionSourceFileName}.\r\n\r\nUnable to proceed with update.");

            bool runningApp_IS_SAME_AS_localToolPath = runningApp_Directory.Equals(runningApp_Directory, StringComparison.CurrentCultureIgnoreCase);

            if (!System.IO.File.Exists(officialDistributionSourceFilePath))
                Console.WriteLine($"ERROR: Cannot find the executable in the official source location {officialDistributionSourceFilePath}");
            else
            {
                var targetFilePath = runningApp_FilePath;

                var err = copyNewBinary(officialDistributionSourceFilePath, targetFilePath);

                if (err != null)
                    Console.WriteLine($"ERROR: {err.Message}");
                else
                {
                    string version_NEW_COPY = FileVersionInfo.GetVersionInfo(officialDistributionSourceFilePath).FileVersion;

                    Console.WriteLine($"Successfully Updated from official distribution.");
                    Console.WriteLine();
                    Console.WriteLine($"File Updated: {targetFilePath}");
                    Console.WriteLine($"   Old: {Assembly.GetExecutingAssembly().GetName().Version}");
                    Console.WriteLine($"   New: {version_NEW_COPY}");
                    Console.WriteLine();
                    Console.WriteLine($"Official Distribution:    {officialDistributionSourceFilePath}");
                    Console.WriteLine($"Currently running:        {targetFilePath}");
                }
            }
        }

        private static Exception copyNewBinary(string EXE_fromOneclickShare, string EXE_RunningApp)
        {
            if (DISABLE_TRY_CATCH)
                copyNewBinary_INNER(EXE_fromOneclickShare, EXE_RunningApp);
            else
                try
                {
                    copyNewBinary_INNER(EXE_fromOneclickShare, EXE_RunningApp);
                }
                catch (Exception ex)
                {
                    return ex;
                }
            return null;
        }


        private static void copyNewBinary_INNER(string EXE_fromOneclickShare, string EXE_RunningApp)
        {
            var PDB_fromOneclickShare = System.IO.Path.ChangeExtension(EXE_fromOneclickShare, ".pdb");
            var PDB_RunningApp = System.IO.Path.ChangeExtension(EXE_RunningApp, ".pdb");
            //
            var TXT_fromOneclickShare = System.IO.Path.ChangeExtension(EXE_fromOneclickShare, ".txt");
            var TXT_RunningApp = System.IO.Path.ChangeExtension(EXE_RunningApp, ".txt");
            //
            string EXE_appFolder = System.IO.Path.GetDirectoryName(EXE_RunningApp);
            string EXE_appName = System.IO.Path.GetFileNameWithoutExtension(EXE_RunningApp);
            string EXE_appExtension = System.IO.Path.GetExtension(EXE_RunningApp);
            string EXE_archivePath = System.IO.Path.Combine(EXE_appFolder, EXE_appName + "_OldVersion" + EXE_appExtension);

            if (System.IO.File.Exists(EXE_archivePath))
                System.IO.File.Delete(EXE_archivePath);

            System.IO.File.Move(EXE_RunningApp, EXE_archivePath);

            System.IO.File.Copy(EXE_fromOneclickShare, EXE_RunningApp, true);
            System.IO.File.Copy(PDB_fromOneclickShare, PDB_RunningApp, true);
            System.IO.File.Copy(TXT_fromOneclickShare, TXT_RunningApp, true);
        }

        private static bool DirectoriesAreTheSame(string dirPath1, string dirPath2, string errorCrumb)
        {
            errorCrumb += "|PubCodeUpdateEXE_v1.UpdateEXE.cs~DirectoriesAreTheSame";
            try
            {
                return DirectoriesAreTheSame_INNER(dirPath1, dirPath2, errorCrumb);
            }
            catch (Exception ex)
            {
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; \r\n{new string('`', 20)}Failure comparing '{dirPath1}' with '{dirPath2}'", ex);
            }
        }


        private static bool DirectoriesAreTheSame_INNER(string dirPath1, string dirPath2, string errorCrumb)
        {
            errorCrumb += "|PubCodeUpdateEXE_v1.UpdateEXE.cs~DirectoriesAreTheSame_INNER";
            string folderHash1 = null;
            string folderHash2 = null;
            //set value for folderHash1
            if (DISABLE_TRY_CATCH)
                folderHash1 = folderHASH(dirPath1, errorCrumb);
            else
                try
                {
                    folderHash1 = folderHASH(dirPath1, errorCrumb);
                }
                catch //(Exception ex)
                {
                    //throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; \r\n{new string('`', 20)}Failed reading contents of folder {dirPath1}", ex);
                    return false;
                }
            //set value for folderHash2
            if (DISABLE_TRY_CATCH)
                folderHash2 = folderHASH(dirPath2, errorCrumb);
            else
                try
                {
                    folderHash2 = folderHASH(dirPath2, errorCrumb);
                }
                catch (Exception ex)
                {
                    //throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; \r\n{new string('`', 20)}Failed reading contents of folder {dirPath2}", ex);
                    return false;
                }

            if (folderHash1 == null)
            {
                //throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; Unable to calculate checksum for {dirPath1}");
                return false;
            }

            if (folderHash2 == null)
            {
                //throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; Unable to calculate checksum for {dirPath2}");
                return false;
            }

            return folderHash1 == folderHash2;
        }

        private static string folderHASH(string folderPath, string errorCrumb)
        {
            errorCrumb += "|PubCodeUpdateEXE_v1.UpdateEXE.cs~folderHASH";
            try
            {
                return folderHASH_INNER(folderPath, errorCrumb);
            }
            catch (Exception ex)
            {
                throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; \r\n{new string('`', 20)}Failed calculating folder hash for '{folderPath}'", ex);
            }
        }

        private static string folderHASH_INNER(string folderPath, string errorCrumb)
        {
            errorCrumb += "|PubCodeUpdateEXE_v1.UpdateEXE.cs~folderHASH_INNER";
            var rawMaterialForIdentificationOfDirectory = new List<string>();
            foreach (var filePath in Directory.GetFiles(folderPath))
            {
                var fileName = Path.GetFileName(filePath);
                var fileInfo = new FileInfo(filePath);
                long fileInfo_Length = -1;
                try
                {
                    fileInfo_Length = fileInfo.Length;
                }
                catch (Exception ex)
                {
                    throw new Exception($"FAILURE/ERROR; ###errorCrumb:[[{errorCrumb}]]###; \r\n{new string('`', 20)}Failed on fileInfo.Length for '{filePath}'", ex);
                }
                var fileInfo_LastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
                var fileInfo_CreationTimeUtc = fileInfo.CreationTimeUtc;
                rawMaterialForIdentificationOfDirectory.Add($"{fileName} {fileInfo_Length} {fileInfo_LastWriteTimeUtc} {fileInfo_CreationTimeUtc}".ToUpper());
            }
            foreach (var dirPath in Directory.GetDirectories(folderPath))
            {
                var dirName = Path.GetFileName(dirPath);
                var dirInfo = new DirectoryInfo(dirName);
                rawMaterialForIdentificationOfDirectory.Add($"{dirName} {dirInfo.LastWriteTimeUtc} {dirInfo.CreationTimeUtc}".ToUpper());
            }
            rawMaterialForIdentificationOfDirectory.Sort();
            var rawMaterialSTR = string.Join("|", rawMaterialForIdentificationOfDirectory);
            return SHA512(rawMaterialSTR);
        }

        private static string SHA512(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return hashedInputStringBuilder.ToString();
            }
        }

        private static bool successfullyVerifiedExistenceOfFile(string filePath)
        {
            //This handles permission exceptions...
            var exists = false;
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (DISABLE_TRY_CATCH)
                {
                    if (System.IO.File.Exists(filePath))
                        exists = true;
                }
                else
                    try
                    {
                        if (System.IO.File.Exists(filePath))
                            exists = true;
                    }
                    catch
                    {
                        exists = false;
                    }
            }
            return exists;
        }
    }
}
