// !! FChecksum: 2CF243B9EF4C371CDDC783FF261E15AA338E54352A12C32B3F3B03DD4AA42359E851B3534640915FF1010A74E6C7A759ED48FB7C3F5BE9B7A886E9F6C477FD36

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.AppParams_v3.AppParams.cs

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

namespace PubCode.AppParams_v3 //v2021.4.24.1
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

        private string ReservedArgSeparator { get; } = default;
        public string[] ReservedArgument_Ary { get; } = default;
        public string ReservedArgument_Str { get; } = default;
        public string Environment_CommandLine { get; } = default;

        #region Constructor
        public AppParams(string[] args): this(aliasesForCommands: null, args: args)
        {
        }

        public AppParams(
             string[,] aliasesForCommands,
             string[] args,
             string[] acceptedParams = null,
             bool displayErrorMessage = false,
             bool allowHiddenArguments = true,
             string reservedArgSeparator = null,
             string environment_CommandLine = null)
        {
            ReservedArgSeparator = reservedArgSeparator;
            Environment_CommandLine = environment_CommandLine;
            var recalc_normal_args = new List<string>();
            var reservedArgument_List = new List<string>();
            if (args != null)
            {
                var mode_reserved_args = false;
                for (int x = 0; x < args.Length; x++)
                {
                    if (mode_reserved_args)
                        reservedArgument_List.Add(args[x]);
                    else
                    {
                        if (reservedArgSeparator != null && args[x] != null && args[x].Equals(reservedArgSeparator, StringComparison.CurrentCultureIgnoreCase))
                            mode_reserved_args = true;
                        else
                            recalc_normal_args.Add(args[x]);
                    }
                }
            }
            args = recalc_normal_args.ToArray();
            ReservedArgument_Ary = reservedArgument_List.ToArray();
            if (reservedArgSeparator != null)
            {
                var posOfArgSeparator = Environment_CommandLine.IndexOf(" " + reservedArgSeparator);
                if (posOfArgSeparator != -1)
                    ReservedArgument_Str = Environment_CommandLine.Substring(posOfArgSeparator + 1 + 1).Trim();
            }

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
        public string ToString_AsLongStringInLogs(bool ignoreReservedArgs = true, bool ignoreOptions = true, bool ignoreModeAsItIsForExceptionMessage = true)
        {
            return stringAsLongStringInLogs($"{ToStringExt(skipParamLess: true, ignoreReservedArgs: ignoreReservedArgs, ignoreOptions: ignoreOptions, ignoreModeAsItIsForExceptionMessage: ignoreModeAsItIsForExceptionMessage)}");
        }

        private static string stringAsLongStringInLogs(string txt)
        {
            txt = $"{txt}";
            var txtAry = txt.Replace("\r\n", "`").Split(new string[] { "`" }, StringSplitOptions.None);
            var txtAsLongStringInLogs = new System.Text.StringBuilder();
            for (var i = 0; i < txtAry.Length; i++)
            {
                var t = txtAry[i].Trim();
                while (t.StartsWith("\t") || t.EndsWith("\t"))
                {
                    while (t.StartsWith("\t"))
                        t = t.Substring(1).Trim();
                    while (t.EndsWith("\t"))
                        t = t.Substring(0, t.Length - 1).Trim();
                }
                txtAsLongStringInLogs.Append(t);
            }
            return txtAsLongStringInLogs.ToString().Trim();
        }


        public override string ToString()
        {
            return ToStringExt(skipParamLess: false, ignoreReservedArgs: false, ignoreOptions: false, ignoreModeAsItIsForExceptionMessage: false);
        }

        public string ToStringExt(bool skipParamLess, bool ignoreReservedArgs, bool ignoreOptions, bool ignoreModeAsItIsForExceptionMessage)
        {
            var txt = new List<string>();
            //
            if (!ignoreModeAsItIsForExceptionMessage)
                if (validationFailedAgainstAcceptedParamList())
                    txt.Add("Validation failed against the Accepted Parameter List");
            //
            if (true) //(ReservedArgSeparator != null)
            {
                if (!ignoreReservedArgs)
                {
                    if (ReservedArgSeparator == null)
                        txt.Add($"Reserved Argument Separator: (null)");
                    else
                        txt.Add($"Reserved Argument Separator: \"{ReservedArgSeparator}\"");

                    if (ReservedArgument_Ary == null)
                        txt.Add($"Reserved Argument Ary: (null)");
                    else if (ReservedArgument_Ary.Length == 0)
                        txt.Add($"Reserved Argument Ary: (empty array)");
                    else
                        for (var i = 0; i < ReservedArgument_Ary.Length; i++)
                            txt.Add($"Reserve Argument Ary[{i}]: {ReservedArgument_Ary[i]}");

                    if (ReservedArgument_Str == null)
                        txt.Add($"Reserved Argument Str:   (null)");
                    else if (ReservedArgument_Str == "")
                        txt.Add($"Reserved Argument Str:   (empty string)");
                    else
                        txt.Add($"Reserved Argument Str:   {ReservedArgument_Str}");
                }
            }
            //
            if (!skipParamLess)
            {
                if (this.ParamLess == null)
                {
                    if (!ignoreModeAsItIsForExceptionMessage)
                        txt.Add($"ParamLess: null");
                }
                else
                {
                    if (!ignoreModeAsItIsForExceptionMessage)
                    {
                        if (this.ParamLess.ParamData == null)
                        {
                            txt.Add($"ParamLess.ParamData: null");
                        }
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
            }
            //
            if (this.Params == null)
                txt.Add($"Params: null");
            else
            {
                if (!ignoreModeAsItIsForExceptionMessage)
                    txt.Add($"Params: {pluralFunction_rows(Params.Count)}");
                foreach (Param itm in Params)
                    txt.Add($"{itm.ToStringExt(acceptedParams, hideKeyName: false, ignoreOptions: ignoreOptions)}");
            }
            //
            if (ignoreModeAsItIsForExceptionMessage)
                return string.Join(" -~- ", txt);
            else
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
