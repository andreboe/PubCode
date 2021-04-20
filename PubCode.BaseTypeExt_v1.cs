// !! FChecksum: E68F52256B31F76890F3EB95E136F2146C1D0CF4957CEE80B24C34B58E0C6D8C0CA2538F31975182589C50CE5F2A485728D2A9A737E219F5A3D776E8B4D0D17F

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.BaseTypeExt_v1.cs

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

namespace PubCode.BaseTypeExt_v1 //v2021.4.20.1
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
    }
}
