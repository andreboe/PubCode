// !! FChecksum: CB2ED6EE27F307F31789AA6307994DF54AA4137811AC4C2FA031BD52834BA22A3CDAFAA9D310CE5A9444EE4E2C8B06FA59DFD1CA7839AE4A37947577CA078B66

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.BaseTypeExt_v2.cs

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
using System.IO;
using System.Linq;

namespace PubCode.BaseTypeExt_v2 //v2021.4.27.1
{
    public static class BaseTypeExt
    {
        /// <summary>
        /// Wrapper to simplify the process of splitting a string using another string as a separator
        /// </summary>
        /// <param name="s">Splits a string into substrings based on the strings in an array.</param>
        /// <param name="separator">A string that delimits the substrings in this string</param>
        /// <returns></returns>
        public static string[] Split(this string s, string separator)
        {
            return s.Split(new string[] { separator }, StringSplitOptions.None);
        }

        /// <summary>
        /// Wrapper to simplify the process of splitting a string using another string array as a separator
        /// </summary>
        /// <param name="s">Splits a string into substrings based on the strings in an array.</param>
        /// <param name="separator">A string array that delimits the substrings in this string</param>
        /// <returns></returns>
        public static string[] Split(this string s, string[] separator)
        {
            return s.Split(separator, StringSplitOptions.None);
        }

        public static string GetTempDirectoryName()
        {
            var tempFolder = System.IO.Path.GetTempFileName();
            File.Delete(tempFolder);
            Directory.CreateDirectory(tempFolder);
            return tempFolder;
        }

        public static bool Contains(this string[] values, String value, StringComparison comparisonType = StringComparison.CurrentCultureIgnoreCase)
        {
            return values.Where(x => !x.Equals(Path.GetExtension(value), comparisonType)).ToArray().Length != 0;
        }

        public static string[] Append(this string[] ary1, string[] ary2)
        {
            var tmp = ary1.ToList();
            tmp.AddRange(ary2);
            return tmp.ToArray();
        }
    }
}
