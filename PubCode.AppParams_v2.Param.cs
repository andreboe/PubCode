// !! FChecksum: 3D8E3780591E5C8552BBA659BB96B273B17BDE27BA4ED22228132697B0F56E921A5670A5EDFDFF9F8361B6AC071485266EDA633D2120FA2F288D20598F30260B

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.AppParams_v2.Param.cs

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

namespace PubCode.AppParams_v2 //v2021.4.20.1
{
    public class Param : IDisposable
    {
        public string ParamKey { get; } = null;
        public List<string> ParamData { get; set; } = null;

        public Param(string paramKey, bool normalizeParamKey = true)
        {
            if (normalizeParamKey) paramKey = Param.normalizeParamKey(paramKey);
            ParamKey = paramKey;
            ParamData = new List<string>();
        }

        public override string ToString()
        {
            var txt = new System.Text.StringBuilder();
            //
            txt.Append($"/{ParamKey}");
            if (ParamData != null && ParamData.Count > 0)
                txt.Append($"\"{string.Join("\" \"", ParamData)}\"");
            //
            return txt.ToString();
        }

        public string ToStringExt(string[] acceptedParams, bool hideKeyName = false, bool ignoreOptions = false)
        {
            var txt = new List<string>();
            //
            var flagForERROR = false;
            if (acceptedParams != null)
            {
                flagForERROR = true;
                if (ParamKey.Equals(AppParams.HIDDEN_PARAM_01, StringComparison.CurrentCultureIgnoreCase))
                    flagForERROR = false;
                else
                    foreach (var acceptedParam in acceptedParams)
                        if (ParamKey.Equals(acceptedParam, StringComparison.CurrentCultureIgnoreCase))
                        {
                            flagForERROR = false;
                            break;
                        }
            }
            //
            if (!hideKeyName)
            {
                if (flagForERROR)
                    // do not change "Not a valid parameter" right now as it's hardcoded twice in the app
                    txt.Add($"\tParamKey: {ParamKey}                   Not a valid parameter!");
                else
                    txt.Add($"\tParamKey: {ParamKey}");
            }
            //
            if (!ignoreOptions)
            {
                if (ParamData == null)
                    txt.Add($"\t\tParamData: null");
                else
                {
                    txt.Add($"\t\tParamData: {ParamData.Count} rows");
                    foreach (var itm in ParamData)
                        txt.Add($"\t\t\t{itm}");
                }
            }
            //
            return string.Join("\r\n", txt);
        }

        public void Dispose()
        {
        }

        public static string normalizeParamKey(string paramKey)
        {
            if (paramKey != null && paramKey.StartsWith("-"))
                paramKey = "/" + paramKey.Substring(1);
            return paramKey;
        }
    }
}
