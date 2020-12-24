using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZeroTierDesktop
{
    public struct CallbackMessage
    {
        public int eventCode;
        /* Pointers to structures that contain details about the 
        subject of the callback */
        public System.IntPtr node;
        public System.IntPtr network;
        public System.IntPtr netif;
        public System.IntPtr route;
        public System.IntPtr path;
        public System.IntPtr peer;
        public System.IntPtr addr;
    }
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CSharpCallback(IntPtr msg);

        [DllImport("zt-shared")]
        public static extern int zts_allow_local_conf(int allowed);
        // [MarshalAs(UnmanagedType.SysUInt)] UInt16 port);
        [DllImport("zt-shared")]
        public static extern int zts_start(string path, CSharpCallback callback, int port);
        // [MarshalAs(UnmanagedType.SysUInt)] 
        
        private CSharpCallback _callback;
        private void MyCallback(IntPtr msg)
        {
            var callbackMessage = Marshal.PtrToStructure<CallbackMessage>(msg);
            Debug.WriteLine("callbackMessage.eventCode: " + callbackMessage.eventCode);
        }
        
        public MainWindow()
        {
            Foo();
            InitializeComponent();
        }

        private void Foo()
        {
            unsafe
            {
                ulong nwid = 0xabfd31bd47bef75e;
                int err;

                _callback = new CSharpCallback(MyCallback);
                //var functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(_callback);
                var currentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var foo = zts_allow_local_conf(1);
                
                if ((err = zts_start(currentDirectory, _callback,9994)) == 1)
                {
                    throw new Exception($"Failed to start/join network [{nwid:X}]");
                }
                Debug.WriteLine(err);
            }
        }
    }
}