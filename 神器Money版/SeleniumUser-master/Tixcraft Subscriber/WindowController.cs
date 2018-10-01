using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
namespace TixWin
{
    public class WindowController
    {

        public static object WindowLocker = new object();
        public static  object objLockerFocus = new object();
        private enum CommandShow : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
        }

        public IntPtr hwnd = IntPtr.Zero;
        public Rectangle Resize_ROI = new Rectangle(); 


        #region == dll import == 

        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int WS_SHOWNORMAL = 1;  

        /// <summary>
        /// 设置目标窗体大小，位置
        /// </summary>
        /// <param name="hWnd">目标句柄</param>
        /// <param name="x">目标窗体新位置X轴坐标</param>
        /// <param name="y">目标窗体新位置Y轴坐标</param>
        /// <param name="nWidth">目标窗体新宽度</param>
        /// <param name="nHeight">目标窗体新高度</param>
        /// <param name="BRePaint">是否刷新窗体</param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);

        #endregion

        public WindowController(IntPtr _hwnd)
        {
            hwnd = _hwnd;
        }
        public WindowController()
        { 
        
        }
        private int WindowChange(CommandShow cmd)
        {
            int iReturn = -1;
            if (this.hwnd != IntPtr.Zero)
            {
                return ShowWindow(this.hwnd, (int)cmd);
            }
            else
            {
                return iReturn;
            }
        }

        //== 視窗控制 ==
        public bool SetToForegroundWindow()
        {
            ShowWindowAsync(this.hwnd, WS_SHOWNORMAL);
            return SetForegroundWindow(this.hwnd);
        }
        public int Hide()
        {
            return WindowChange(CommandShow.SW_HIDE);
        }
        public int ShowNormal()
        {
            return WindowChange(CommandShow.SW_SHOWNORMAL);
        }
        public int Normal()
        {
            return WindowChange(CommandShow.SW_NORMAL);
        }
        public int ShowMinImized()
        {
            return WindowChange(CommandShow.SW_SHOWMINIMIZED);
        }
        public int ShowMaxImized()
        {
            return WindowChange(CommandShow.SW_SHOWMAXIMIZED);
        }
        public int MaxImized()
        {
            return WindowChange(CommandShow.SW_MAXIMIZE);
        } 
        public int ShowNoActivate()
        {
            return WindowChange(CommandShow.SW_SHOWNOACTIVATE);
        }
        public int Show()
        {
            return WindowChange(CommandShow.SW_SHOW);
        }
        public int Minimize()
        {
            return WindowChange(CommandShow.SW_MINIMIZE);
        }
        public int ShowMinNoActive()
        {
            return WindowChange(CommandShow.SW_SHOWMINNOACTIVE);
        }
        public int ShowNA()
        {
            return WindowChange(CommandShow.SW_SHOWNA);
        }
        public int ReStore()
        {
            return WindowChange(CommandShow.SW_RESTORE);
        }
        public int ShowDefault()
        {
            return WindowChange(CommandShow.SW_SHOWDEFAULT);
        }
        public int ForceMinImize()
        {
            return WindowChange(CommandShow.SW_SHOWDEFAULT);
        }
        public int ReSize(int left, int top, int width, int heigh)
        {
            Resize_ROI.X = left;
            Resize_ROI.Y = top;
            Resize_ROI.Width = width;
            Resize_ROI.Height = heigh;

            return MoveWindow(this.hwnd, left, top, width, heigh, true);
        }

        //== 滑鼠控制 ==
        public int iOffset_Y = 123;
        public int iOffset_X = 6;

        public IntPtr Mouse_MoveTo(int x, int y)
        { 
            return WindowSnap.SendMessage(this.hwnd, WindowSnap.WM_MOUSEMOVE, IntPtr.Zero, WindowSnap.CreateLParam(x, y));
        }
        public IntPtr Mouse_LeftButton_Down(int x, int y)
        {
            return WindowSnap.SendMessage(this.hwnd, WindowSnap.WM_LBUTTONDOWN, new IntPtr(WindowSnap.MK_LBUTTON), WindowSnap.CreateLParam(x, y));
        }
        public IntPtr Mouse_LeftButton_Up(int x, int y)
        {
            return WindowSnap.SendMessage(this.hwnd, WindowSnap.WM_LBUTTONUP, new IntPtr(WindowSnap.MK_LBUTTON), WindowSnap.CreateLParam(x, y));
        }

        public IntPtr Mouse_MoveTo_ByOffset(int x, int y)
        {
            x = x + iOffset_X;
            y = y + iOffset_Y;
            return Mouse_MoveTo(x, y);
        }
        public IntPtr Mouse_LeftButton_Down_ByOffset(int x, int y)
        {
            x = x + iOffset_X;
            y = y + iOffset_Y;
            return Mouse_LeftButton_Down( x, y);
        }
        public IntPtr Mouse_LeftButton_Up_ByOffset(int x, int y)
        {
            x = x + iOffset_X;
            y = y + iOffset_Y;
            return Mouse_LeftButton_Up(x, y);
        } 
        //== 字串發送 ==
        public void SendString(string Input)
        {
            byte[] chars = (new System.Text.ASCIIEncoding()).GetBytes(@"Cd \");  //要写的信息
            for (int i = 0; i < chars.Length; i++)
            {
                WindowSnap.SendMessage(this.hwnd, WindowSnap.WM_CHAR, (int)chars[i], 0);   //以字符发送
            }


            //byte[] ch = (ASCIIEncoding.ASCII.GetBytes(Input));
            //for (int i = 0; i < ch.Length; i++)
            //{
            //    WindowSnap.SendMessage(this.hwnd, WindowSnap.WM_CHAR, (IntPtr)ch[i], (IntPtr)0);
            //}
        }

        //== 影像擷取 ==
        public Bitmap GetScreenShotBy_WindowHwnd()
        {
            Bitmap bTmpSrc = null;
            lock (objLockerFocus)
            {
                this.ReSize(Resize_ROI.X, Resize_ROI.Y, Resize_ROI.Width, Resize_ROI.Height); // 絕對位置
                this.ShowNormal(); // 秀出
                this.SetToForegroundWindow(); //前景
                Thread.Sleep(3);
                bTmpSrc = WindowSnap.CaptureScreen(); // 全桌面取影像
            }
            
            //== 製作有Offset的ROI
            Rectangle rect_offset = new Rectangle();
            rect_offset.X = Resize_ROI.X + 6;
            rect_offset.Y = Resize_ROI.Y + 122;
            rect_offset.Width = Resize_ROI.Width - 6;
            rect_offset.Height = Resize_ROI.Height - 122;

              
            Image<Bgr, byte> temp = new Image<Bgr, byte>(bTmpSrc);

            bTmpSrc.Dispose();
            bTmpSrc = null;

            //== 取得ROI影像 ==
            temp.ROI = rect_offset;
            Bitmap bResult = new Bitmap(temp.Bitmap);

            temp.Dispose();
            temp = null;

            return bResult;


        }

    }
}
