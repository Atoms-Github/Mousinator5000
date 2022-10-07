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
            var resolution = Screen.PrimaryScreen.Bounds;
            Console.WriteLine("Newpos " + X + " " + Y);

            return SetCursorPos(X, Y);
        }

        private List<int[]> currentZoom = new List<int[]>();
        private bool rightHand = false;

        private int[] divisions = new[] { 4, 3 };

        public static void SetWindowExTransparent(IntPtr hwnd)
        {
            var extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
        public MainWindow()
        {
            InitializeComponent();

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
            UpdateZoom();

            

            

            
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowExTransparent(hwnd);
        }
        private void UpdateZoom()
        {
            var resolution = Screen.PrimaryScreen.Bounds;
            foreach (int[] coord in currentZoom)
            {
                var xDivSize = resolution.Width / divisions[0];
                var yDivSize = resolution.Height / divisions[1];

                resolution = new System.Drawing.Rectangle(resolution.Left + coord[0] * xDivSize, resolution.Top+coord[1] * yDivSize, xDivSize, yDivSize);
                
            }
            var xDivSizeNext = resolution.Width / divisions[0];
            var yDivSizeNext = resolution.Height / divisions[1];

            var o = (DrawingBrush)MyCanvas.Background;
            var t = (GeometryDrawing)o.Drawing;
            var th = (RectangleGeometry)t.Geometry;
            var f = th.Rect;
            f.Width = xDivSizeNext;
            f.Height = yDivSizeNext;

            o.Viewport = new Rect(0, 0, xDivSizeNext, yDivSizeNext);

            MyCanvas.Width = resolution.Width;
            MyCanvas.Height = resolution.Height;
            Canvas.SetTop(MyCanvas, resolution.Top);
            Canvas.SetLeft(MyCanvas, resolution.Left);

            if (currentZoom.Count > 0)
            {
                MSetCursorPos(resolution.Left + resolution.Width / 2, resolution.Top + resolution.Height / 2);
            }
        }
        private void OnGridPressed(int[] coords, bool right)
        {
            if (right == rightHand && currentZoom.Count < 6)
            {
                currentZoom.Add(coords);
                UpdateZoom();
                rightHand = !rightHand;
            }
        }
        private void OnMonitorMove(bool right)
        {

        }
        private void OnGoBack()
        {
            
        }
        private void Reset()
        {
            currentZoom.Clear();
            rightHand = false;
            UpdateZoom();
        }
        private void OnHotKeyHandler(HotKey hotKey)
        {
            // Order: Shift Ctrl Alt Win
            var coords = new[] { ModifiersToInt(hotKey.KeyModifiers, KeyModifier.Shift, KeyModifier.Ctrl), ModifiersToInt(hotKey.KeyModifiers, KeyModifier.Alt, KeyModifier.Win) };
            var rightHand = hotKey.Key == Key.F17;
            Console.WriteLine("Coords: " + coords);
            if(coords[1] == 3)
            {
                switch (coords[0])
                {
                    case 0:
                        if (rightHand)
                        {
                            Reset();
                        }
                        else
                        {
                            OnGoBack();
                        }
                        break;
                    case 1:
                        OnMonitorMove(rightHand);
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                OnGridPressed(coords, rightHand);
            }
        }
        private int ModifiersToInt(KeyModifier source, KeyModifier low, KeyModifier hi)
        {
            var low_val = source & low;
            var hi_val = source & hi;
            int result = 0;
            if (low_val != 0)
            {
                result += 1;
            }
            if(hi_val != 0)
            {
                result += 2;
            }
            return result;
        }
    }
}
