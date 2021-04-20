// !! FChecksum: 3BD4FF50C7A743A433EBF44BF6D763FBD7280D75E0255518C88084995E78FA96F25CA34FD046FA2A4C300F5A39F682A2D06396834D3CBB21937C85CB9D0BE1DE

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.AppParams_v1.AppParams.cs

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

namespace PubCode.AppParams_v1 //v2021.4.20.1
{
    public class AppParams : IDisposable
    {
        #region Public Constants... this section will be enhanced in future versions...
        public const string HIDDEN_PARAM_01 = "/params";
        #endregion

        #region Public Properties
        public Param ParamLess { get; set; } = null;
        public List<Param> Params { get; } = null;
        public bool ConsistencyError_NeedToTerminateApp
        {
            get
            {
                return _consistencyError_NeedToTerminateApp;
            }
            private set
            {
                _consistencyError_NeedToTerminateApp = value;
            }
        }
        public bool ValidationFailedAgainstAcceptedParamList
        {
            get
            {
                return _ValidationFailedAgainstAcceptedParamList;
            }
            private set
            {
                _ValidationFailedAgainstAcceptedParamList = value;
            }
        }
        public string ProcessStart_CommandLineArgument
        {
            get
            {
                var argStr = "";
                if (_args.Length != 0)
                    argStr = $"\"{string.Join("\" \"", _args)}\"";
                return argStr;
            }
        }
        #endregion

        #region Public Methods; getParamByParamKey(...) and 
        public Param getParamByParamKey(string paramKey, bool normalizeParamKey = true)
        {
            if (Params == null) return null;
            if (string.IsNullOrWhiteSpace(paramKey)) return null;
            //
            foreach (var param in Params)
                if (param.ParamKey.Equals(paramKey, StringComparison.CurrentCultureIgnoreCase))
                    return param;
            return null;
        }

        public bool containsParamKey(string paramKey, bool normalizeParamKey = true)
        {
            if (Params == null) return false;
            if (string.IsNullOrWhiteSpace(paramKey)) return false;
            if (normalizeParamKey) paramKey = Param.normalizeParamKey(paramKey);
            //
            foreach (var param in Params)
                if (param.ParamKey.Equals(paramKey, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            return false;
        }

        public bool validationFailedAgainstAcceptedParamList()
        {
            // do not change "Not a valid parameter" right now as it's hardcoded twice in the app
            if (this.Params != null)
                foreach (Param itm in Params)
                    if (itm.ToStringExt(acceptedParams).Contains("Not a valid parameter"))
                        return true;
            return false;
        }
        #endregion

        #region Constructor
        public AppParams(
            string[,] aliasesForCommands,
            string[] args,
            string[] acceptedParams = null,
            bool displayErrorMessage = false,
            bool allowHiddenArguments = true)
        {
            #region ApplyAliasesForCommands
            if (args != null)
                if (aliasesForCommands != null)
                    for (var i = 0; i < args.Length; i++)
                    {
                        var txt = args[i];
                        if (txt != null)
                        {
                            var txt_orig = txt;
                            var txt_new = txt;
                            for (int aliasNo = 0; aliasNo <= aliasesForCommands.GetUpperBound(0); aliasNo++)
                                if (txt_orig.Equals(aliasesForCommands[aliasNo, 0], StringComparison.CurrentCultureIgnoreCase)) txt_new = aliasesForCommands[aliasNo, 1];
                            if (!txt_new.Equals(txt_orig, StringComparison.CurrentCultureIgnoreCase))
                                args[i] = txt_new;
                        }
                    }
            #endregion


            this.acceptedParams = acceptedParams;
            _args = args;
            Params = new List<Param>();
            var p = _args.ToList();
            Param newParam = null;
            while (p.Count > 0)
            {
                var txt = p[0];
                p.RemoveAt(0);
                //
                if (txt.StartsWith("-") || txt.StartsWith("/"))
                {
                    if (newParam != null)
                        Params.Add(newParam);
                    newParam = new Param(txt);
                }
                else
                {
                    if (newParam == null)
                    {
                        if (ParamLess == null)
                            ParamLess = new Param("*");
                        ParamLess.ParamData.Add(txt);
                    }
                    else
                        newParam.ParamData.Add(txt);
                }

            }
            if (newParam != null)
            {
                if (newParam.ParamData != null && newParam.ParamData.Count == 0)
                    newParam.ParamData = null;
                Params.Add(newParam);
                newParam = null;
            }

            if (Params != null && Params.Count == 0)
                Params = null;

            //

            ValidationFailedAgainstAcceptedParamList = false;
            if (this.acceptedParams != null)
            {
                if (validationFailedAgainstAcceptedParamList())
                {
                    ValidationFailedAgainstAcceptedParamList = true;
                    if (displayErrorMessage)
                    {
                        Console.WriteLine(this);
                    }
                    ConsistencyError_NeedToTerminateApp = true;
                    return;
                }
            }

            //

            if (allowHiddenArguments)
                if (containsParamKey(HIDDEN_PARAM_01))
                {
                    Console.WriteLine(this);
                    ConsistencyError_NeedToTerminateApp = true;
                    return;
                }
        }
        #endregion

        #region Custom ToString() and ToStringExt(...)
        public override string ToString()
        {
            return ToStringExt(skipParamLess: false);
        }

        public string ToStringExt(bool skipParamLess)
        {
            var txt = new List<string>();
            //
            if (validationFailedAgainstAcceptedParamList())
                txt.Add("Validation failed against the Accepted Parameter List");
            //
            //txt.Add($"ProcessStart_CommandLineArgument: [{this.ProcessStart_CommandLineArgument}]");
            //
            if (!skipParamLess)
            {
                if (this.ParamLess == null)
                    txt.Add($"ParamLess: null");
                else
                {
                    if (this.ParamLess.ParamData == null)
                        txt.Add($"ParamLess.ParamData: null");
                    else
                    {
                        txt.Add($"ParamLess.ParamData: {pluralFunction_rows(ParamLess.ParamData.Count)}");
                        foreach (string itm in ParamLess.ParamData)
                        {
                            txt.Add($"\t{itm}");
                        }
                    }
                }
            }
            //
            if (this.Params == null)
                txt.Add($"Params: null");
            else
            {
                txt.Add($"Params: {pluralFunction_rows(Params.Count)}");
                foreach (Param itm in Params)
                    txt.Add($"{itm.ToStringExt(acceptedParams)}");
            }
            //
            return string.Join("\r\n", txt);
        }
        #endregion

        #region Private and IDisposable Support
        private static string pluralFunction_rows(int cnt)
        {
            if (cnt == 1)
                return $"{cnt} row";
            else
                return $"{cnt} rows";
        }
        #region Private Variables
        private string[] _args = null;
        private bool _consistencyError_NeedToTerminateApp = false;
        private string[] acceptedParams = null;
        private bool _ValidationFailedAgainstAcceptedParamList = false;
        #endregion

        #region IDisposable Support
        public void Dispose()
        {
        }
        #endregion
        #endregion
    }
}
