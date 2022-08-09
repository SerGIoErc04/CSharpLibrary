using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace UniversalHotkeys
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            Keys k = Keys.Control | Keys.Shift | Keys.M;
            WindowsShell.RegisterHotKey(this, k);

        }




        // Use SetForegroundWindow for Win32
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        // Use GetForegroundWindow for Win32
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();




        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WindowsShell.WM_HOTKEY)
            {

                // Actions here
                listBox1.Items.Add(String.Format("[Detected] ({0})",
                    DateTime.Now.ToString("HH:mm:ss:ffff dd/MM/yyyy")
                    ));
                listBox1.SelectedIndex = listBox1.Items.Count - 1;



                // Showcase focus traversal
                IntPtr fromWindow = GetForegroundWindow();
                SetForegroundWindow(this.Handle);

                Thread.Sleep(5000);

                SetForegroundWindow(fromWindow);



            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            WindowsShell.UnregisterHotKey(this);

        }

    }

    public class WindowsShell
    {
        #region fields
        public static int MOD_ALT = 0x1;
        public static int MOD_CONTROL = 0x2;
        public static int MOD_SHIFT = 0x4;
        public static int WM_HOTKEY = 0x312;
        #endregion

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static int keyId;
        public static void RegisterHotKey(Form f, Keys key)
        {
            int modifiers = 0;

            if ((key & Keys.Alt) == Keys.Alt)
                modifiers = modifiers | WindowsShell.MOD_ALT;

            if ((key & Keys.Control) == Keys.Control)
                modifiers = modifiers | WindowsShell.MOD_CONTROL;

            if ((key & Keys.Shift) == Keys.Shift)
                modifiers = modifiers | WindowsShell.MOD_SHIFT;

            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
            keyId = f.GetHashCode();
            RegisterHotKey((IntPtr)f.Handle, keyId, (int)(uint)modifiers, (int)(uint)k);
        }

        private delegate void Func();

        public static void UnregisterHotKey(Form f)
        {
            try
            {
                UnregisterHotKey(f.Handle, keyId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }

}
