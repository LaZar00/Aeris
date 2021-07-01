using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Aeris
{

    public class FolderBrowserDialogEX
    {

        // -----------------------------------------------------------------------------
        //  Helper for FolderBrowserDialog object.
        // -----------------------------------------------------------------------------
        public FolderBrowserDialog folderBrowser;
        public bool Disposed { get; private set; }

        public FolderBrowserDialogEX()
        {
            Tmr = new Timer() { Interval = 200 };
            folderBrowser = new FolderBrowserDialog();
        }

        public void Dispose()
        {
            if (Disposed) return;
            Disposed = true;
            Tmr.Dispose();
            folderBrowser.Dispose();
        }

        private Timer _Tmr;

        public Timer Tmr
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Tmr;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Tmr != null)
                {
                    _Tmr.Tick -= Tmr_Tick;
                }

                _Tmr = value;
                if (_Tmr != null)
                {
                    _Tmr.Tick += Tmr_Tick;
                }
            }
        }

        private const int WM_USER = 1024;
        private const int BFFM_SETEXPANDED = (WM_USER + 106);
        private const int WM_SETFOCUS = 7;
        private const int WM_SETREDRAW = 11;

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, uint msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        [DllImport("user32.dll", EntryPoint = "FindWindowExW")]
        public static extern IntPtr FindWindowExW(IntPtr hWndParent, IntPtr hWndChildAfter, [MarshalAs(UnmanagedType.LPWStr)] string lpszClass, [MarshalAs(UnmanagedType.LPWStr)] string lpszWindow);
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);


        public void Tmr_Tick(object sender, System.EventArgs e)
        {
            // Dim hFb As IntPtr = FindWindowExW(IntPtr.Zero, IntPtr.Zero, "#32770", Nothing)
            IntPtr hFb = GetActiveWindow();

            SendMessage(hFb, WM_SETREDRAW, false, 0);
            if ((hFb != IntPtr.Zero))
            {
                IntPtr hChild = FindWindowExW(hFb, IntPtr.Zero, null, null);
                IntPtr hTreeView = FindWindowExW(hChild, IntPtr.Zero, "SysTreeView32", null);
                while ((hTreeView == IntPtr.Zero))
                {
                    hChild = FindWindowExW(hFb, hChild, null, null);
                    hTreeView = FindWindowExW(hChild, IntPtr.Zero, "SysTreeView32", null);
                }

                if ((SendMessageW(hFb, BFFM_SETEXPANDED, 1, folderBrowser.SelectedPath) == IntPtr.Zero))
                {
                    Tmr.Stop();
                    SendMessageW(hTreeView, WM_SETFOCUS, 0, null);
                }

            }

            SendMessage(hFb, WM_SETREDRAW, true, 0);
        }
    }
}