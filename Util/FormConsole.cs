using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public sealed class FormConsole
    {
        private static readonly FormConsole _Instance = new FormConsole();

        private class Win32
        {
            /// <summary>
            /// Allocates a new console for current process.
            /// </summary>
            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            public static extern Boolean AllocConsole();

            /// <summary>
            /// Frees the console.
            /// </summary>
            [System.Runtime.InteropServices.DllImport("kernel32.dll")]
            public static extern Boolean FreeConsole();
        }

        private FormConsole()
        {
            Win32.AllocConsole();  //Allocates a new console for current process.
        }

        public static FormConsole Instance
        {
            get
            {
                return _Instance;
            }
        }

        ~FormConsole()
        {
            Win32.FreeConsole(); // Free the console.
        }
    }
}
