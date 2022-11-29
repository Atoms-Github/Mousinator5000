


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using UnManaged;
using System.Windows.Forms;
using System.CodeDom;

namespace Mousinator5000
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WS_EX_TRANSPARENT = 0x00000020;
        const int GWL_EXSTYLE = (-20);


        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);
        
        private static bool MSetCursorPos(int X, int Y)
        {
            Console.WriteLine("Newpos " + X + " " + Y);

            return SetCursorPos(X, Y);
        }
        private static Point GetScreenSize()
        {
            var resolution = Screen.PrimaryScreen.Bounds;
            return new Point(resolution.Width, resolution.Height);
        }
        private string current = "";
        static string lettersList = "lrksmwt[nhp.oaue]i,dgc";
        private static int code_length = 3;

        Dictionary<string, Point> screenDict = new Dictionary<string, Point>();

            static double combs = Math.Pow(lettersList.Length, code_length);
            int boxCountOneDim = (int) Math.Pow(combs, 0.5);


        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        public MainWindow()
        {
            InitializeComponent();
            UpdateZoom();

            WindowStyle = WindowStyle.None;
            Topmost = true;
            AllowsTransparency = true;
            Opacity = 0.7;
            WindowState = WindowState.Maximized;


            foreach (var a in new[] { KeyModifier.Alt, KeyModifier.None})
            {
                foreach (var b in new[] { KeyModifier.Shift, KeyModifier.None })
                {
                    foreach (var c in new[] { KeyModifier.Ctrl, KeyModifier.None })
                    {
                        foreach (var d in new[] { KeyModifier.Win, KeyModifier.None })
                        {
                            foreach (var e in new[] {Key.F13, Key.F17 })
                            {
                                var result = a | b | c | d;
                                var _hotKey = new HotKey(e, result, OnHotKeyHandler);
                            }
                        }
                    }
                }
            }



            
            foreach (var a in lettersList)
            {

            foreach (var b in lettersList)
            {

            foreach (var c in lettersList)
            {
                        String key = a + "" + b + "" + c + "";

                        screenDict[key] = new Point(screenDict.Count % boxCountOneDim, screenDict.Count / boxCountOneDim);
                        Console.WriteLine("Added " + screenDict[key].x + " " + screenDict[key].y);
            }
            }
            }



            
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowExTransparent(hwnd);
        }
        private void UpdateZoom()
        {
        }
        private void OnGridPressed(int num)
        {
            var letter = lettersList[num];
            current = current + letter;
            if (current.Length == code_length)
            {
                Point targetCoords = screenDict[current];
                MoveMouseToBox(targetCoords);

                current = "";
                
            }
        }
        private void MoveMouseToBox(Point box)
        {
            Point screenSize = GetScreenSize();
            MSetCursorPos(screenSize.x / boxCountOneDim * box.x, screenSize.y /boxCountOneDim * box.y);
        }
        private void OnMonitorMove(bool right)
        {

        }
        private void OnGoBack()
        {
        }
        private void Reset()
        {
            current = "";
        }
        private void OnHotKeyHandler(HotKey hotKey)
        {
            // Order: Shift Ctrl Alt Win
            var num = ModifiersToInt(hotKey.KeyModifiers, KeyModifier.Shift, KeyModifier.Ctrl, KeyModifier.Alt, KeyModifier.Win) ;
            var rightHand = hotKey.Key == Key.F17;
            if (rightHand)
            {
                num += 16;
            }
            Console.WriteLine("Hotkey " + num);
            if (num == 31) {
                Reset();
            }
            else if (num == 30)
            {
                OnGoBack();
            }
            else if (num == 29)
            {
                OnMonitorMove(true);
            }
            else if (num == 28)
            {
                OnMonitorMove(false);
            }
            else
            {
                OnGridPressed(num);
            }
        }
        private int ModifiersToInt(KeyModifier source, KeyModifier a, KeyModifier b, KeyModifier c, KeyModifier d)
        {
            int result = 0;
            var a_val = source & a;
            var b_val = source & b;
            var c_val = source & c;
            var d_val = source & d;
            if (a_val != 0)
            {
                result += 1;
            }
            if (b_val != 0)
            {
                result += 2;
            }

            if (c_val != 0)
            {
                result += 4;
            }
            if (d_val != 0)
            {
                result += 8;
            }
            return result;
        }
    }
}
