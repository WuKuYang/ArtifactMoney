using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageViewer
{
    public partial class EMImageViewer : Form
    {
        public EMImageViewer MsgBox;

        public enum ViewerMode { Viewer, Setting };
        public enum ViewerFocus {A , B };

        public static ViewerMode nowMode = ViewerMode.Viewer;
        public static ViewerFocus Watch = ViewerFocus.A;
        
        public double Viewer(Bitmap Image)
        {
            nowMode = ViewerMode.Viewer;
            mSourceImage = new Bitmap(Image);
            MsgBox = new EMImageViewer(); 
            MoveToPrimaryScreen();
            MsgBox.TopLevel = true;
            MsgBox.TopMost = true;
            MsgBox.ShowDialog();
            return Distance * Pixel_To_mm;
        }  
        public double Viewer(Bitmap ViewA , Bitmap ViewB)
        {
            nowMode = ViewerMode.Viewer;
            mViewImage_A = new Bitmap(ViewA);
            mViewImage_B = new Bitmap(ViewB); 
            MsgBox = new EMImageViewer();
            MoveToPrimaryScreen();
            MsgBox.TopLevel = true;
            MsgBox.TopMost = true;
            MsgBox.ShowDialog();
            return AveraeDistance;
        }


        public void ViewerSetting(Bitmap Image)
        {
            nowMode = ViewerMode.Setting;
            mSourceImage = new Bitmap(Image);
            MsgBox = new EMImageViewer();
            MoveToPrimaryScreen();
            MsgBox.TopLevel = true;
            MsgBox.TopMost = true;
            MsgBox.ShowDialog();
        }
        private void MoveToPrimaryScreen()
        {
            Screen[] a = Screen.AllScreens;
            if (a.Length > 0)
            {

                foreach (Screen MyScreen in Screen.AllScreens) //對所有的螢幕
                {
                    if (MyScreen.Primary) //找到不是主要的第一個螢幕
                    {

                        //MsgBox.Location = new Point((int)(MyScreen.WorkingArea.X + 407), (int)(MyScreen.WorkingArea.Y + 295));
                        MsgBox.StartPosition = FormStartPosition.CenterScreen;

                        break;
                    }
                }
            }
        }
        
        public double getDistance(PointF p1, PointF p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            double line = Math.Sqrt(dx * dx + dy * dy);
            return line;
        }
         

        static Bitmap mViewImage_A = new Bitmap(1600, 900, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        static Bitmap mViewImage_B = new Bitmap(1600, 900, System.Drawing.Imaging.PixelFormat.Format24bppRgb);  

        //原圖
        static Bitmap mSourceImage = new Bitmap(1600, 900, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //Show圖用
        static Bitmap mImageShowImage = new Bitmap(1600, 900, System.Drawing.Imaging.PixelFormat.Format24bppRgb); 

        //  用來顯示小地圖模式
        static Bitmap mMapImage = new Bitmap(286, 271, System.Drawing.Imaging.PixelFormat.Format24bppRgb); 
        //  小地圖模式的ROI
        static Rectangle mMapToImage_ROI = new Rectangle();  
        static Rectangle mMapROI = new Rectangle(); 

        bool Is_Map_MouseClickDown = false; 
        bool Is_Map_clickStart = false; 
        bool Is_Map_clickEnd = false;

        public EMImageViewer()
        {
            InitializeComponent();
        }
        
        Point startP = new Point();
        
        Point EndP = new Point();
        
        bool IsMouseClickDown = false;
        
        bool IsclickStart = false;
        
        bool IsclickEnd = false;

        public static double Distance = 0;  //Pixels
        public static double AveraeDistance = 0;    // mms
        
        /// <summary>
        /// 1 pixel 對應 mm
        /// </summary>
        //public static double Pixel_To_mm = 0.15931;
        public static double Pixel_To_mm = 0.0051546391752577;

        public static double ViewA_Pixel_To_mm = 0.0051546391752577;
        public static double ViewB_Pixel_To_mm = 0.0091546391752577;
        
        private void PreviewImage_MouseDown(object sender, MouseEventArgs e)
        {
            int diff = 10;
            Point Temp = new Point(e.X, e.Y);
            if (Math.Abs(getDistance(Temp, startP)) < diff)
            {
                IsclickStart = true;
                startP = new Point(e.X, e.Y);
            }
            else if (Math.Abs(getDistance(Temp, EndP)) < diff)
            {
                IsclickEnd = true;
                EndP = new Point(e.X, e.Y);
            }
            else
            {
                startP = new Point(e.X, e.Y);
            } 
            IsMouseClickDown = true;
        }

        private void PreviewImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseClickDown)
            {
                if (IsclickStart)
                {
                    startP = new Point(e.X, e.Y);
                }
                else if (IsclickEnd)
                {
                    EndP = new Point(e.X, e.Y);
                }
                else
                {
                    EndP = new Point(e.X, e.Y);
                }

                //DrawImageLine(mImage, startP, EndP);
                DrawImageLine(mImageShowImage, startP, EndP);
                GC.Collect();
            }
        }

        private void PreviewImage_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseClickDown = false;
            IsclickStart = false;
            IsclickEnd = false;
            lblMB.ForeColor = lblMA.ForeColor = Color.Black;
        }
        
        public void DrawImageLine(Bitmap Image,Point p1,Point p2)
        {
            Distance = getDistance(p1, p2);
            if (Watch == ViewerFocus.A)
            {
                lblPA.Text = Distance.ToString("F1");
                lblMA.Text = (Distance * Pixel_To_mm).ToString("F4");
                lblMA.ForeColor = Color.Red;
            }
            else 
            {
                lblPB.Text = Distance.ToString("F1");
                lblMB.Text = (Distance * Pixel_To_mm).ToString("F4");
                lblMB.ForeColor = Color.Red;
            }




            if (lblMA.Text == "lblMA" && lblMB.Text == "lblMB")
            {
                lblAvgmm.Text = "0";
            }
            else if (lblMA.Text == "lblMA")
            {
                double dDepthB = 0; double.TryParse(lblMB.Text, out dDepthB);
                lblAvgmm.Text = dDepthB.ToString("F4");
            }
            else if (lblMB.Text == "lblMB")
            {
                double dDepthA = 0; double.TryParse(lblMA.Text, out dDepthA);
                lblAvgmm.Text = dDepthA.ToString("F4");
            }
            else if (lblMB.Text != "" && lblMA.Text != "")
            {
                double dDepthA = 0; double.TryParse(lblMA.Text, out dDepthA);
                double dDepthB = 0; double.TryParse(lblMB.Text, out dDepthB);
                double dAvg = (dDepthA + dDepthB) / 2;
                lblAvgmm.Text = dAvg.ToString("F4");
            }
            
            double dAvgDepth = 0; double.TryParse(lblAvgmm.Text, out dAvgDepth);
            AveraeDistance = dAvgDepth;

            int circleSize = 5;
            int diffSize = 20;
            Bitmap Temp = new Bitmap(Image);
            Graphics g = Graphics.FromImage(Temp);
            //g.Clear(Color.Black);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.DrawLine(new Pen(Color.White, 1.5f), p1, p2);

            if (nowMode == ViewerMode.Setting)
            {
                g.DrawString("Setting\nPlease Select 1 mm Distance.\n" + Distance.ToString("F2") + " pixel", new Font("Verdana", 14F, FontStyle.Regular), new SolidBrush(Color.Red), new Point(0, 0));
                Pixel_To_mm = 1 / Distance;
            }
            else
            { 
                g.DrawString("Viewer\nDistance:" + (Distance * Pixel_To_mm).ToString("F2") + "mm (" + Distance.ToString("F2") + " pixel)", new Font("Verdana", 14F, FontStyle.Regular), new SolidBrush(Color.Red), new Point(0, 0));
            } 
            g.FillEllipse(new SolidBrush(Color.Red), p1.X - circleSize / 2, p1.Y - circleSize / 2, circleSize, circleSize);
            g.FillEllipse(new SolidBrush(Color.Red), p2.X - circleSize / 2, p2.Y - circleSize / 2, circleSize, circleSize);
            g.DrawEllipse(new Pen(Color.Yellow, 0.5f), p1.X - diffSize / 2, p1.Y - diffSize / 2, diffSize, diffSize);
            g.DrawEllipse(new Pen(Color.Yellow, 0.5f), p2.X - diffSize / 2, p2.Y - diffSize / 2, diffSize, diffSize);
            g.Dispose();
            PreviewImage.Image = Temp;
        }
         
        public void DrawRectangle(Bitmap Image, Rectangle mRect)
        {
            if (Image == null)
            {
                return;
            }
            Bitmap Temp = new Bitmap(Image);
            Graphics g = Graphics.FromImage(Temp); 
            Pen selPen = new Pen(Color.Blue);
            g.DrawRectangle(selPen, mRect);
            g.Dispose();
            ptbImageMap.Image = Temp;
        }

        public Bitmap DrawROI(Bitmap Image, Rectangle mRect)
        {
            if (Image == null)
            {
                return null;
            }
            //Bitmap mROIImage = new Bitmap(PreviewImage.Width , PreviewImage.Height);
            //Graphics g = Graphics.FromImage(mROIImage);
            //g.DrawImage(Image, mRect.X, mRect.Y, mRect, GraphicsUnit.Point); 
            return Image.Clone(mRect, Image.PixelFormat);  
        }

        public  Bitmap cropAtRect(Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            g.Dispose();
            g = null;
            GC.Collect();
            return nb;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //DrawImageLine(mImage, startP, EndP);

            if (Watch == ViewerFocus.A)
            {
                mSourceImage = mViewImage_A;
            }
            else
            {
                mSourceImage = mViewImage_B;
            }
            mMapImage = new Bitmap(mSourceImage, ptbImageMap.Size);

            double dROIScaleW = (double)PreviewImage.Width / (double)mSourceImage.Width;
            double dROIScaleH = (double)PreviewImage.Height / (double)mSourceImage.Height;
            double dSw = (double)ptbImageMap.Width / (double)mSourceImage.Width;
            double dSh = (double)ptbImageMap.Height / (double)mSourceImage.Height;

            double dMapWidthROI = PreviewImage.Width * dROIScaleW * dSw;
            double dMapHeighROI = PreviewImage.Height * dROIScaleH * dSh; 

            mMapROI = new Rectangle(100, 100, (int)dMapWidthROI, (int)dMapHeighROI);
            DrawRectangle(mMapImage, mMapROI);  
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        { 
            Distance = 0;
            this.Close();
        }

        private void ptbImageMap_MouseDown(object sender, MouseEventArgs e)
        {
            Is_Map_MouseClickDown = true; 
        }

        private void ptbImageMap_MouseMove(object sender, MouseEventArgs e)
        { 
            Point mTemp = new Point(e.X, e.Y);
            if (Is_Map_MouseClickDown)
            { 
                Point pCenter = new Point(mTemp.X - mMapROI.Width/2   , mTemp.Y - mMapROI.Height/2); 
                mMapROI.X = pCenter.X;
                mMapROI.Y = pCenter.Y;
                Calculate_ImageROI(ref mMapROI, ref mMapToImage_ROI, mSourceImage.Size);    
                DrawRectangle(mMapImage ,  mMapROI);
                //PreviewImage.Image = DrawROI(mImage, mMapToImage_ROI);
                PreviewImage.Image = mImageShowImage = cropAtRect(mSourceImage, mMapToImage_ROI); 
            }
        }

        private void ptbImageMap_MouseUp(object sender, MouseEventArgs e)
        { 
            Is_Map_MouseClickDown = false;
        }

        public void Calculate_ImageROI(ref Rectangle mMapROI, ref Rectangle mImageROI, Size sImageSize)
        {  
            double dSw = (double)ptbImageMap.Width / (double)sImageSize.Width;
            double dSh = (double)ptbImageMap.Height / (double)sImageSize.Height;

            //mImageROI.Width = (int)(mMapROI.Width / dSw);
            //mImageROI.Height = (int)(mMapROI.Height / dSh);
            mImageROI.Width =PreviewImage.Width;
            mImageROI.Height = PreviewImage.Height;

            mImageROI.X = (int)(mMapROI.X / dSw);
            mImageROI.Y = (int)(mMapROI.Y / dSh);  

        }

        private void PreviewImage_Click(object sender, EventArgs e)
        {

        }

        private void btnView_1_Click(object sender, EventArgs e)
        {
            Watch = ViewerFocus.A;
            ChangeViewSide(Watch);
        }

        private void btnView_2_Click(object sender, EventArgs e)
        {
            Watch = ViewerFocus.B;
            ChangeViewSide(Watch);
        }

        public void ChangeViewSide(ViewerFocus p)
        {

            if (Watch == ViewerFocus.A)
            {
                mSourceImage = mViewImage_A;
                Pixel_To_mm = ViewA_Pixel_To_mm;
            }
            else
            {
                mSourceImage = mViewImage_B;
                Pixel_To_mm = ViewB_Pixel_To_mm;
            }
            mMapImage = new Bitmap(mSourceImage, ptbImageMap.Size);
             
            //顯示左邊大圖 更新
            PreviewImage.Image = null;
            ptbImageMap.Image = mMapImage;
            PreviewImage.Refresh();
            ptbImageMap.Refresh();

        }


    }
}
