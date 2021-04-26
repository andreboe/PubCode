// !! FChecksum: AF72A804B5D8D037FE3597C58423B9AC5B235C51DA1DE1950F253B2E68943ED82B04CD7F5F944721A7607A262EA2F76C77199A49440B7C663FD23BD401422A7C

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.SubApp_v1.cs

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace PubCode.SubApp_v1 //v2021.4.25.2
{
    class SubAppAttribute : Attribute
    {
        public string Cmd { get; }

        public SubAppAttribute(string cmd)
        {
            Cmd = cmd;
        }

        public static bool isAllowedToRun(Type type, bool isDefaultSubApp, ref string[] args)
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

        public static void RunAppropriateApp(string[] args, ConsoleMgr myConsole, ResourceManager myResourceManager) => RunAppropriateApp(null, args, myConsole, myResourceManager);
        public static void RunAppropriateApp(string defaultApp, string[] args, ConsoleMgr myConsole, ResourceManager myResourceManager)
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
}
