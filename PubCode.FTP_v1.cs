// !! FChecksum: ED7A6D9392D0E50B26D549AA3A8470F4E5950B67AAE1F3F2FB86D1890E189CD4D7AE1FF65ED97EDF732E86681F154992E21A37F17F237819DDC3EF3B9C66A23A

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.FTP_v1.cs

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
using System.IO;
using System.Linq;
using System.Net;

namespace PubCode.FTP_v1 //v2021.4.27.1
{
    public class FTP: IDisposable
    {
        private string UserId { get; }
        private string Password { get; }

        public FTP(string userId, string password)
        {
            UserId = userId;
            Password = password;
        }

        public bool DirectoryExists_OnFTP(string ftpFolderPath)
        {
            bool isexist = false;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFolderPath);
                request.Credentials = new NetworkCredential(UserId, Password);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    return true;
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                    if (((FtpWebResponse)ex.Response).StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                        return false;
            }
            return isexist;
        }

        public string[] GetFiles_FromFTP(string ftpFolderPath)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(ftpFolderPath);
                ftpRequest.Credentials = new NetworkCredential(UserId, Password);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());
                List<string> directories = new List<string>();
                string line = streamReader.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    var lineArr = line.Split('/');
                    line = lineArr[lineArr.Count() - 1];
                    directories.Add(line);
                    line = streamReader.ReadLine();
                }
                streamReader.Close();
                return directories.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Copy_FileToFTP(string fromFILE, string toFTP)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(UserId, Password);
                client.UploadFile(toFTP, WebRequestMethods.Ftp.UploadFile, fromFILE);
            }
        }

        public void Copy_FTPToFile(string fromFTP, string toFILE)
        {
            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(UserId, Password);
                client.DownloadFile(fromFTP, toFILE);
            }
        }

        public void DeleteFolder_OnFTP(string ftpFolderPath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFolderPath);
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            request.Credentials = new System.Net.NetworkCredential(UserId, Password);
            request.GetResponse().Close();
        }

        public void DeleteFile_OnFTP(string ftpFilePath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            request.Credentials = new System.Net.NetworkCredential(UserId, Password);
            request.GetResponse().Close();
        }

        public void Dispose() { }
    }
}
