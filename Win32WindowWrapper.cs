using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Rebar_Revit
{
    /// <summary>
    /// Simple IWin32Window wrapper around a native Revit window handle.
    /// Used to set the owner for modeless WinForms so they integrate with Revit window.
    /// </summary>
    public class Win32WindowWrapper : IWin32Window
    {
        private readonly IntPtr _hwnd;

        public Win32WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public IntPtr Handle => _hwnd;
    }
}
