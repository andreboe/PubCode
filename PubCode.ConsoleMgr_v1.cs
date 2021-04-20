// !! FChecksum: CA5B9D4F2F39CDCE6570EC791D3F4E2CE6157F8F2A2E91166DC9BE0102606D9FB2AA59C66CCAE1AA57ABEC83C319E9F4028BD1F81DEA7AF3BC17D683C112FEF3

//Official Location: https://github.com/andreboe/PubCode/blob/main/PubCode.ConsoleMgr_v1.cs

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

namespace PubCode.ConsoleMgr_v1 //v2021.4.20.1
{
    public class ConsoleMgr : IDisposable
    {
        public ErrorMgr myError { get; set; }

        public class ErrorMgr : IDisposable
        {
            private ConsoleMgr myConsole = null;
            public ErrorMgr(ConsoleMgr myConsole)
            {
                this.myConsole = myConsole;
            }

            public void WriteLine<T>(T txt)
            {
                myConsole.ClearDOTModeIfSet();
                Console.Error.WriteLine(txt);
            }

            public void Dispose() {}
        }

        public ConsoleMgr()
        {
            myError = new ErrorMgr(this);
        }

        private bool mode_DOT = false;

        private void GoToStartOfLine_and_ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void ClearDOTModeIfSet()
        {
            if (mode_DOT)
            {
                mode_DOT = false;
                GoToStartOfLine_and_ClearCurrentConsoleLine();
            }
        }

        public void WriteLine<T>(T txt)
        {
            ClearDOTModeIfSet();
            Console.WriteLine(txt);
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public void WriteDOT(string value)
        {
            if (!mode_DOT)
                mode_DOT = true;
            Console.Write(value);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        public void Dispose()
        {
            ClearDOTModeIfSet();
        }
    }
}
