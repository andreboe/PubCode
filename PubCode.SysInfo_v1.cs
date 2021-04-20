// !! FChecksum: 91D0B41FC09FD3EC7BD53B7137771963385260BFDB420400155E58BE3048CDE0865C61B43E782038948AC8DC1EFC6ECB2CCF947CB5B7E797B07F38DE940FC7A6

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.SysInfo_v1.cs

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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PubCode.SysInfo_v1 //v2021.4.20.1
{
    public static class SysInfo
    {
        public static string MachineIdentifier
        {
            get
            {
                return GetServerId_v1();
            }
        }

        private static string GetMacAddress()
        {
            // Credits: https://stackoverflow.com/questions/850650/reliable-method-to-get-machines-mac-address-in-c-sharp
            try
            {
                const int MIN_MAC_ADDR_LENGTH = 12;
                // Dim macAddress = String.Empty
                var macAddress = new List<string>();
                long maxSpeed = -1;
                foreach (System.Net.NetworkInformation.NetworkInterface nic in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up)
                    {
                        if (nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)
                        {
                            string _nic_Description = nic.Description;
                            if (_nic_Description == null) _nic_Description = String.Empty;
                            if (_nic_Description.IndexOf("Loopback", StringComparison.OrdinalIgnoreCase) == -1)
                            {
                                string _nic_Name = nic.Name;
                                if (_nic_Name == null) _nic_Name = String.Empty;
                                if (_nic_Name.IndexOf("Loopback", StringComparison.OrdinalIgnoreCase) == -1)
                                {
                                    // Log.Debug("Found MAC Address: " & nic.GetPhysicalAddress & " Type: " + nic.NetworkInterfaceType)
                                    string tempMac = nic.GetPhysicalAddress().ToString();
                                    if (nic.Speed > maxSpeed && !string.IsNullOrEmpty(tempMac) && tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                                    {
                                        // Log.Debug("New Max Speed = " & nic.Speed & ", MAC: " & tempMac)
                                        maxSpeed = nic.Speed;
                                        // macAddress = tempMac
                                        macAddress.Add(tempMac);
                                    }
                                }
                            }
                        }
                    }
                }

                macAddress.Sort();
                return string.Join(",", macAddress).ToLower();
            }
            catch
            {
                return "ERR";
            }
        }

        private static string GetSystemProductName()
        {
            try
            {
                string location = @"HARDWARE\DESCRIPTION\System\BIOS";
                string name = "SystemProductName";
                using (var localMachineX64View = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64))
                {
                    using (var rk = localMachineX64View.OpenSubKey(location))
                    {
                        if (rk == null)
                            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));
                        var machineGuid = rk.GetValue(name);
                        if (machineGuid == null)
                            throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));
                        return machineGuid.ToString();
                    }
                }
            }
            catch
            {
                return "ERR";
            }
        }

        private static string GetSystemManufacturer()
        {
            try
            {
                string location = @"HARDWARE\DESCRIPTION\System\BIOS";
                string name = "SystemManufacturer";
                using (var localMachineX64View = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64))
                {
                    using (var rk = localMachineX64View.OpenSubKey(location))
                    {
                        if (rk == null)
                            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));
                        var machineGuid = rk.GetValue(name);
                        if (machineGuid == null)
                            throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));
                        return machineGuid.ToString();
                    }
                }
            }
            catch
            {
                return "ERR";
            }
        }

        private static string GetMachineGuid()
        {
            try
            {
                string location = @"SOFTWARE\Microsoft\Cryptography";
                string name = "MachineGuid";
                using (var localMachineX64View = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64))
                {
                    using (var rk = localMachineX64View.OpenSubKey(location))
                    {
                        if (rk == null)
                            throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));
                        var machineGuid = rk.GetValue(name);
                        if (machineGuid == null)
                            throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));
                        return machineGuid.ToString();
                    }
                }
            }
            catch
            {
                return "ERR";
            }
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                for (int i = 0, loopTo = bytes.Length - 1; i <= loopTo; i++)
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        private static string GetServerId_v1()
        {
            string MachineName = Environment.MachineName;
            string MacAddress = GetMacAddress();
            string MachineGuid = GetMachineGuid();
            string SystemProductName = GetSystemProductName();
            string SystemManufacturer = GetSystemManufacturer();
            // 
            return ComputeSha256Hash($"{MachineName}|{MacAddress}|{MachineGuid}|{SystemProductName}|{SystemManufacturer}");
        }
    }
}
