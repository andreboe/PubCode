// !! FChecksum: 0C3259A74E5AFD42F1E1A5F344F9E8845A1E9B90B2FE0331E3247A2664F36BA4882ED7AB55A8F34EAAAF8C3851D07731F55817C43A58FA6C8423F36BCDA99782

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.SubApp_v2.cs

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

using PubCode.AppParams_v3;
using PubCode.ConsoleMgr_v1;
//using SubApp_N1_22B08752;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace PubCode.SubApp_v2 //v2021.5.2.1
{
    internal class SubAppAttribute : Attribute
    {
        internal string Cmd { get; }

        internal SubAppAttribute(string cmd)
        {
            Cmd = cmd;
        }

        internal static bool isAllowedToRun(Type type, bool isDefaultSubApp, ref string[] args)
        {
            var myAttr_Cmd = new List<string>();
            foreach (var attr in Attribute.GetCustomAttributes(type))
                myAttr_Cmd.Add(((dynamic)attr).Cmd);
            if (myAttr_Cmd.Count != 1) throw new NotImplementedException("myAttr_Cmd.Count != 1");
            var myAttr_Cmd_Singular = myAttr_Cmd[0];

            bool enableSubApp;
            if (isDefaultSubApp)
                enableSubApp = true;
            else
            {
                enableSubApp = false;
                using (var tmpAppParams = new AppParams(args))
                {
                    if (tmpAppParams.ParamLess != null)
                    {
                        foreach (var paramTxt in tmpAppParams.ParamLess.ParamData)
                        {
                            if (paramTxt.Equals(myAttr_Cmd_Singular, StringComparison.OrdinalIgnoreCase))
                            {
                                var args_asList = new List<string>();
                                bool adjustedArgs = false;
                                for (var i = 0; i < args.Length; i++)
                                {
                                    if (!adjustedArgs && args[i].Equals(myAttr_Cmd_Singular, StringComparison.OrdinalIgnoreCase))
                                        adjustedArgs = true;
                                    else
                                        args_asList.Add(args[i]);
                                }
                                if (!adjustedArgs)
                                    Console.Error.WriteLine("Error during parsing of parameters");
                                else
                                {
                                    enableSubApp = true;
                                    args = args_asList.ToArray();
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return enableSubApp;
        }

        internal static void RunAppropriateApp(string[] args, ConsoleMgr myConsole, ResourceManager myResourceManager) => RunAppropriateApp(null, args, myConsole, myResourceManager);
        internal static void RunAppropriateApp(string defaultApp, string[] args, ConsoleMgr myConsole, ResourceManager myResourceManager)
        {
            //ResourceManager myResourceManager = new ResourceManager(typeof(AppResources).ToString(), typeof(AppResources).Assembly);

            var constructors = new List<Type>();
            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly()
              .GetTypes()
              .Where(myType => myType.Name.EndsWith("SubApp", StringComparison.OrdinalIgnoreCase))
              )
                foreach (var prop in type.GetCustomAttributes(typeof(SubAppAttribute), inherit: false))
                    constructors.Add(type);

            var constructors_Default = new List<Type>();
            var constructors_NotDefault = new List<Type>();
            foreach (var type in constructors)
            {
                var myAttr_Cmd = new List<string>();
                foreach (var attr in Attribute.GetCustomAttributes(type))
                    myAttr_Cmd.Add(((dynamic)attr).Cmd);
                if (myAttr_Cmd.Count != 1) throw new NotImplementedException("myAttr_Cmd.Count != 1");
                var myAttr_Cmd_Singular = myAttr_Cmd[0];

                if (myAttr_Cmd_Singular.Equals(defaultApp, StringComparison.OrdinalIgnoreCase))
                    constructors_Default.Add(type);
                else
                    constructors_NotDefault.Add(type);
            }

            bool done = false;
            foreach (var type in constructors_NotDefault)
                if (!done)
                    done = (bool)type.GetMethod("Try").Invoke(null, new object[] { false, args, myConsole, myResourceManager });
            foreach (var type in constructors_Default)
                if (!done)
                    done = (bool)type.GetMethod("Try").Invoke(null, new object[] { true, args, myConsole, myResourceManager });
            if (!done)
                myConsole.myError.WriteLine("Unable to start application engine");
        }
    }

    internal class SubCmdAttribute : Attribute
    {
        internal string AppCmd { get; }

        internal SubCmdAttribute(string appCmd)
        {
            AppCmd = appCmd;
        }
    }

    internal abstract class AppCmdBase : IDisposable
    {
        internal abstract string ParamTXTRaw { get; }
        internal abstract string ParamHelpTXT { get; }

        internal bool isParamTXT(AppParams myParams)
        {
            bool ret = false;
            if (myParams.Params != null)
                foreach (var param in myParams.Params)
                    if (isParamTXT(param.ParamKey))
                        ret = true;
            return ret;
        }

        internal bool isParamTXT(string[] args)
        {
            bool ret = false;
            foreach (var arg in args)
                if (isParamTXT(arg))
                    ret = true;
            return ret;
        }


        internal bool isParamTXT(string paramTXT)
        {
            if (string.IsNullOrWhiteSpace(paramTXT) && string.IsNullOrWhiteSpace(ParamTXTRaw))
                return true;
            else if (string.IsNullOrWhiteSpace(paramTXT))
                return false;
            else if (string.IsNullOrWhiteSpace(ParamTXTRaw))
                return false;
            else
                return paramTXT.Equals(ParamTXT_Slash, StringComparison.OrdinalIgnoreCase) || paramTXT.Equals(ParamTXT_Dash, StringComparison.OrdinalIgnoreCase);
        }

        private string getRaw()
        {
            var txt = ParamTXTRaw;
            if (txt == null)
                return null;
            else
            {
                if (txt.StartsWith("-") || txt.StartsWith("/"))
                {
                    txt = txt.Substring(1);
                }
                return txt;
            }
        }

        internal string ParamTXT_Slash { get { return "/" + getRaw(); } }
        internal string ParamTXT_Dash { get { return "-" + getRaw(); } }

        public void Dispose() { }
    }

    internal abstract class AppCmdRegBase : AppCmdBase
    {
        internal abstract void ExecuteAppCmd();
    }

    internal abstract class AppCmdK1Base : AppCmdBase
    {
        internal bool isParamTXT_andExecuteIfNeeded(AppParams myParams, ConsoleMgr myConsole)
        {
            bool ret = false;
            if (myParams.Params != null)
            {
                foreach (var param in myParams.Params)
                    if (isParamTXT(param.ParamKey))
                    {
                        ExecuteAppCmd(myParams, myConsole);
                        ret = true;
                    }
            }
            else
            {
                ExecuteAppCmd(myParams, myConsole);
                ret = true;
            }
            return ret;
        }

        internal new bool isParamTXT(AppParams myParams)
        {
            bool ret = false;
            if (myParams.Params != null)
                foreach (var param in myParams.Params)
                    if (isParamTXT(param.ParamKey))
                        ret = true;
            return ret;
        }

        internal abstract void ExecuteAppCmd(AppParams myParams, ConsoleMgr myConsole);
    }

    internal abstract class AppCmdFileNameBase : AppCmdBase
    {
        internal bool isParamTXT_andExecuteIfNeeded(string arg_command, string[] arg_fileList_aryOfString)
        {
            if (!isParamTXT(arg_command))
                return false;
            else
            {
                ExecuteAppCmd(arg_fileList_aryOfString);
                return true;
            }
        }

        internal void ExecuteAppCmd(string[] arg_fileList_aryOfString)
        {
            if (arg_fileList_aryOfString != null)
                foreach (var arg_row_fileName in arg_fileList_aryOfString)
                    ExecuteAppCmd(arg_row_fileName);
        }

        internal abstract void ExecuteAppCmd(string arg_row_fileName);
    }

    internal abstract class AppCmdFileNameAryBase : AppCmdBase
    {
        internal bool isParamTXT_andExecuteIfNeeded(string arg_command, string[] arg_fileList)
        {
            if (!isParamTXT(arg_command))
                return false;
            else
            {
                ExecuteAppCmd(arg_fileList);
                return true;
            }
        }

        internal abstract void ExecuteAppCmd(string[] arg_fileList);
    }

    internal abstract class AppCmdTupleBase : AppCmdBase
    {
        internal bool isParamTXT_andExecuteIfNeeded(string arg_command, Tuple<string, string>[] arg_fileList_Tuple)
        {
            if (!isParamTXT(arg_command))
                return false;
            else
            {
                ExecuteAppCmd(arg_fileList_Tuple);
                return true;
            }
        }

        internal abstract void ExecuteAppCmd(Tuple<string, string>[] arg_fileList_Tuple);
    }

    internal abstract class AppCmdN1Z2Base : AppCmdBase
    {
        internal abstract string ExecuteAppCmd();
        internal abstract string ReturnCmd { get; }
    }
}
