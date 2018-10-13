using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TixWin
{

    public class WindowSnap
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(string lpDriverName, string lpDeviceName, string lpOutput, string lpInitData);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern int BitBlt(IntPtr desthDC, int srcX, int srcY, int srcW, int srcH, IntPtr srchDC, int destX, int destY, int op);

        [DllImport("gdi32.dll")]
        private static extern int DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern int DeleteObject(IntPtr hObj);

        public const uint WM_CHAR = 0x102;

        public const uint WM_LBUTTONDOWN = 0x201;
        public const uint WM_LBUTTONUP = 0x202;
        public const uint MK_LBUTTON = 0x0001;
        public const uint WM_SETCURSOR = 0x20;
        public const uint WM_MOUSEACTIVATE = 0x21;
        public const uint WM_MOUSEMOVE = 0x200;
        public const uint BM_CLICK = 0x00F5;
        public const int WM_ACTIVATE = 0x06;
        public const int WM_NCLBUTTONDOWN = 0xA1;

        //当用户释放鼠标左键同时光标某个窗口在非客户区十发送此消息    

        public const int WM_NCLBUTTONUP = 0xA2;

        //一个窗口获得焦点   
        public const int WM_SETFOCUS = 0x07;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);
        public static IntPtr CreateLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
            //return (IntPtr)((HiWord << 0) | (LoWord));
        }
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        static public Bitmap CaptureControl(IntPtr SourceDC, int SourceWidth, int SourceHeight)
        {
            return Capture(SourceDC, SourceWidth, SourceHeight);
        }

        

        static public Bitmap CaptureScreen(IntPtr hwd)
        {
            //IntPtr screen = CreateDC("DISPLAY", "", "", "");
            //int sourceWidth = GetDeviceCaps(screen, 8);
            //int sourceHeight = GetDeviceCaps(screen, 10);
            Bitmap ret = Capture(hwd, 0, 0);
            //DeleteDC(screen);
            return ret;
        }

        static private Bitmap Capture(IntPtr SourceDC, int SourceWidth, int SourceHeight)
        {
            //IntPtr destDC;
            //IntPtr BMP, BMPOld;
            //destDC = CreateCompatibleDC(SourceDC);
            //BMP = CreateCompatibleBitmap(SourceDC, SourceWidth, SourceHeight);
            //BMPOld = SelectObject(destDC, BMP);
            //BitBlt(destDC, 0, 0, SourceWidth, SourceHeight, SourceDC, 0, 0, 13369376);
            //BMP = SelectObject(destDC, BMPOld);
            //DeleteDC(destDC);
            //Bitmap ret = Image.FromHbitmap(BMP);
            //DeleteObject(BMP);
            //return ret;

            System.Drawing.Rectangle rctForm = System.Drawing.Rectangle.Empty;
            using (System.Drawing.Graphics grfx = System.Drawing.Graphics.FromHdc(GetWindowDC(SourceDC)))
            {
                rctForm = System.Drawing.Rectangle.Round(grfx.VisibleClipBounds);
            }
            Rect NotepadRect = new Rect();
            GetWindowRect(SourceDC, ref NotepadRect);
            Bitmap myImage = new Bitmap(rctForm.Width, rctForm.Height);
            Graphics g = Graphics.FromImage(myImage);
            g.CopyFromScreen(NotepadRect.Left, NotepadRect.Top, 0, 0, new Size(rctForm.Width, rctForm.Height));
            IntPtr dc1 = g.GetHdc();
            g.ReleaseHdc(dc1);
            return myImage;
        }

    }
}
