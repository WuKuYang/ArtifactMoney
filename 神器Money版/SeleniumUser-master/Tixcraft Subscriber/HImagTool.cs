using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HalconDotNet;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics; //使用Halcon
namespace HTool
{
    public class HImagTool
    {


        [DllImport("kernel32", EntryPoint = "RtlMoveMemory", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern void CopyMemory(int Destination, int Source, int length);

        public HImagTool()
        {
            HOperatorSet.SetSystem("neighborhood", 4);
        }

        public class PassMode
        {
            /// <summary>
            /// 要使用遮罩模式 : 字串"HPass" : 高通濾波; ， 字串"LPass" : 低通濾波
            /// </summary>
            public string PassMD;
            /// <summary> 
            /// Cutoff frequency. 
            /// Suggested values: 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0
            /// Restriction: Frequency >= 0 
            /// </summary>
            public double Frequency;
            /// <summary>
            /// Normalizing factor of the filter. 
            /// List of values: 'none', 'n' 
            /// </summary>
            public string Norm;
            /// <summary>
            /// Location of the DC term in the frequency domain. 
            /// List of values: 'dc_center', 'dc_edge', 'rft' 
            /// </summary>
            public string Mode;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="_PassMode">
            /// 要使用濾波模式 : 字串"HPass" : 高通濾波; ， 字串"LPass" : 低通濾波</param>
            /// <param name="_Frequency">
            /// Cutoff frequency. 
            /// Suggested values: 0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0
            /// Restriction: Frequency >= 0 </param>
            /// <param name="_Norm">
            /// Normalizing factor of the filter. 
            /// List of values: 'none', 'n' </param>
            /// <param name="_Mode">
            /// Location of the DC term in the frequency domain. 
            /// List of values: 'dc_center', 'dc_edge', 'rft' </param>
            public PassMode(string _PassMode, double _Frequency, string _Norm, string _Mode)
            {
                PassMD = _PassMode;
                Frequency = _Frequency;
                Norm = _Norm;
                Mode = _Mode;
            }
        }


        /// <summary> 
        /// 彩色Bitmap轉HImage
        /// </summary> 
        /// <param name="bImage"></param> 
        /// <returns></returns> 
        public HImage Bitmap_To_HImage_24(Bitmap bImage)
        {
            Bitmap bImage24;
            BitmapData bmData = null;
            Rectangle rect;
            IntPtr pBitmap;
            IntPtr pPixels;
            HImage hImage = new HImage();

            rect = new Rectangle(0, 0, bImage.Width, bImage.Height);
            bImage24 = new Bitmap(bImage.Width, bImage.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bImage24);
            g.DrawImage(bImage, rect);
            g.Dispose();

            bmData = bImage24.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            pBitmap = bmData.Scan0;
            pPixels = pBitmap;
            hImage.GenImageInterleaved(pPixels, "bgr", bImage.Width, bImage.Height, -1, "byte", 0, 0, 0, 0, -1, 0);
            bImage24.UnlockBits(bmData);
            bImage24.Dispose();
            GC.Collect();
            return hImage;
        }

        /// <summary> 
        /// 灰階Bitmap轉HImage
        /// </summary> 
        /// <param name="bImage"></param> 
        /// <returns></returns> 
        public HImage Bitmap_To_HImage_8(Bitmap bImage)
        {
            Bitmap bImage8;
            BitmapData bmData = null;
            Rectangle rect;
            IntPtr pBitmap;
            IntPtr pPixels;
            HImage hImage = new HImage();
            rect = new Rectangle(0, 0, bImage.Width, bImage.Height);
            bImage8 = new Bitmap(bImage.Width, bImage.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bImage8);
            g.DrawImage(bImage, rect);
            g.Dispose();
            bmData = bImage8.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            pBitmap = bmData.Scan0;
            pPixels = pBitmap;
            hImage.GenImage1("byte", bImage.Width, bImage.Height, pPixels);
            bImage8.UnlockBits(bmData);
            bImage8.Dispose();
            GC.Collect();
            return hImage;
        }


        public HObject HImageConvertFromBitmap32(Bitmap bmp)
        {
            HObject ho_Image;
            HOperatorSet.GenEmptyObj(out ho_Image);
            unsafe
            {
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                unsafe
                {
                    HOperatorSet.GenImageInterleaved(out ho_Image, bmpData.Scan0, "bgrx", bmp.Width, bmp.Height, -1, "byte", bmp.Width, bmp.Height, 0, 0, -1, 0);
                }
                bmp.UnlockBits(bmpData);
                return ho_Image;
            }

        }


        /// <summary>
        /// 進行Hobject轉型為HImage格式(彩色)
        /// </summary>
        /// <param name="hobject"></param>
        /// <param name="image"></param>
        public void Hobject_To_Himage(HObject hobject, ref HImage image)
        {
            HTuple pointer, type, width, height;
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
        }

        /// <summary>
        /// 進行Hobject轉型為HImage格式(灰階)
        /// </summary>
        /// <param name="hobject"></param>
        /// <param name="image"></param>
        public void Hobject_To_RGBHimage(HObject hobject, ref HImage image)
        {
            HTuple pointerRed, pointerGreen, pointerBlue, type, width, height;
            HOperatorSet.GetImagePointer3(hobject, out pointerRed, out  pointerGreen, out  pointerBlue, out  type, out  width, out  height);
            image.GenImage3(type, width, height, pointerRed, pointerGreen, pointerBlue);
        }


        public Bitmap HImage_To_Bitmap(HImage HSrcImg)
        {
            HImage img1 = new HImage();
            IntPtr pt;
            int mwidth, mheight;
            string mtype = "";
            Bitmap img;
            ColorPalette pal;
            int i;
            const int Alpha = 255;
            BitmapData bitmapData;
            Rectangle rect;
            int[] ptr = new int[2];
            int PixelSize;
            img1 = HSrcImg;
            pt = img1.GetImagePointer1(out mtype, out mwidth, out mheight);
            img = new Bitmap(mwidth, mheight, PixelFormat.Format8bppIndexed);
            pal = img.Palette;
            for (i = 0; i <= 255; i++)
            {
                pal.Entries[i] = Color.FromArgb(Alpha, i, i, i);
            }
            img.Palette = pal;
            rect = new Rectangle(0, 0, mwidth, mheight);
            bitmapData = img.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            PixelSize = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
            ptr[0] = bitmapData.Scan0.ToInt32();
            ptr[1] = pt.ToInt32();
            if (mwidth % 4 == 0)
                CopyMemory(ptr[0], ptr[1], mwidth * mheight * PixelSize);
            else
            {
                for (i = 0; i < mheight; i++)
                {
                    ptr[1] += mwidth;
                    CopyMemory(ptr[0], ptr[1], mwidth * PixelSize);
                    ptr[0] += bitmapData.Stride;
                }
            }
            img.UnlockBits(bitmapData);
            img1.Dispose();
            GC.Collect();
            return img;
        }

        #region 影像處理函數

        /// <summary>
        /// 進行傅立葉轉換，把圖像轉成頻率域，在使用高/低通濾波方式濾除，最後再從頻率域把圖像轉回來。
        /// </summary>
        /// <param name="HSrc">原始圖</param>
        /// <param name="FFTPassMode">濾除模式(須設定高/低通、頻率、模式...等)</param>
        /// <returns>濾除雜訊後的圖</returns>
        public HImage Filter_FFT_Pass(HImage HSrc, PassMode FFTPassMode)
        {
            // Local iconic variables  
            HObject ho_Image, ho_pass, ho_ImageFFT;
            HObject ho_ImageConvol, ho_ImageResult;
            // Local control variables 
            HTuple hv_Width, hv_Height;

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_pass);
            HOperatorSet.GenEmptyObj(out ho_ImageFFT);
            HOperatorSet.GenEmptyObj(out ho_ImageConvol);
            HOperatorSet.GenEmptyObj(out ho_ImageResult);

            ho_Image.Dispose();
            ho_Image = HSrc;    //使用開放出來的參數
            //取得影像長寬
            HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
            //低通遮罩
            ho_pass.Dispose();
            if (FFTPassMode.PassMD == "HPass")
            {
                HOperatorSet.GenHighpass(out ho_pass, FFTPassMode.Frequency, FFTPassMode.Norm, FFTPassMode.Mode, hv_Width, hv_Height);
            }
            else if (FFTPassMode.PassMD == "LPass")
            {
                HOperatorSet.GenLowpass(out ho_pass, FFTPassMode.Frequency, FFTPassMode.Norm, FFTPassMode.Mode, hv_Width, hv_Height);
            }
            //圖像=>頻率域
            ho_ImageFFT.Dispose();
            HOperatorSet.FftGeneric(ho_Image, out ho_ImageFFT, "to_freq", 1, "none", "dc_edge", "complex");
            //濾波
            ho_ImageConvol.Dispose();
            HOperatorSet.ConvolFft(ho_ImageFFT, ho_pass, out ho_ImageConvol);
            //從頻率域還原成圖
            ho_ImageResult.Dispose();
            HOperatorSet.FftGeneric(ho_ImageConvol, out ho_ImageResult, "from_freq", -1, "none", "dc_edge", "byte");

            ho_Image.Dispose();
            ho_pass.Dispose();
            ho_ImageFFT.Dispose();
            ho_ImageConvol.Dispose();
            HImage image = new HImage();
            Hobject_To_Himage(ho_ImageResult, ref image);
            return image;
        }


        public List<Bitmap> SplitVerifyCode(Bitmap HSrc, int iThreshold = 255, int iAreaMin = 80, int iAreaMax = 1000)
        {
            List<Bitmap> lstChars = new List<Bitmap>(); //結果回傳

            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            long SP_O = 0;

            // Local iconic variables 

            HObject ho_RGB_source;
            HObject ho_Gray_Source;

            HObject ho_Reg_Sort = null;
            HObject ho_selCh = null;
            HObject ho_roi = null;
            HObject ho_Gray_Result = null;
            HObject ho_ImageReduced = null, ho_ImagePart = null;
            HObject ho_RegionFillUp = null;
            // Local control variables 

            HTuple hv_Width, hv_Height, hv_countObject;

            HTuple hv_objChar = new HTuple(), hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RGB_source);
            HOperatorSet.GenEmptyObj(out ho_Gray_Source);
            HOperatorSet.GenEmptyObj(out ho_Reg_Sort);
            HOperatorSet.GenEmptyObj(out ho_selCh);
            HOperatorSet.GenEmptyObj(out ho_roi);
            HOperatorSet.GenEmptyObj(out ho_Gray_Result);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);



            HObject ho_Regions = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions1 = null, ho_EmptyObject = null, ho_SelectedRegions = null;
            HObject ho_ObjectSelected = null, ho_Partitioned = null, ho_ObjectSelected2 = null;


            // Local control variables 

            HTuple hv_Number = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Index1 = new HTuple(), hv_Area1 = new HTuple();
            HTuple hv_Number2 = new HTuple();
            HTuple hv_Index2 = new HTuple(), hv_Number3 = new HTuple();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_Partitioned);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected2);


            ho_RGB_source.Dispose();
            //HOperatorSet.ReadImage(out ho_RGB_source, "C:/Users/Administrator/Desktop/拓元圖庫2/AZFull(乾淨版)/goji/9.png");
            //HImage mySource = this.Bitmap_To_HImage_24(HSrc);

            HObject mySource = HImageConvertFromBitmap32(HSrc);

            ho_RGB_source = mySource;
            HOperatorSet.GetImageSize(ho_RGB_source, out hv_Width, out hv_Height);
            ho_Gray_Source.Dispose();
            HOperatorSet.Rgb3ToGray(ho_RGB_source, ho_RGB_source, ho_RGB_source, out ho_Gray_Source
                );

            ho_Regions.Dispose();
            HOperatorSet.VarThreshold(ho_Gray_Source, out ho_Regions, 11, 11, 0.2, 8, "light");
            ho_RegionFillUp.Dispose();
            HOperatorSet.FillUp(ho_Regions, out ho_RegionFillUp);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
            ho_SelectedRegions1.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions1, "area",
                "and", 50, 99999);
            HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number);
            if ((int)(new HTuple(hv_Number.TupleEqual(3))) != 0)
            {
                ho_EmptyObject.Dispose();
                HOperatorSet.GenEmptyObj(out ho_EmptyObject);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_SelectedRegions1, out ho_SelectedRegions,
                    "max_area", 70);
                HOperatorSet.AreaCenter(ho_SelectedRegions, out hv_Area, out hv_Row, out hv_Column);
                for (hv_Index1 = 1; hv_Index1.Continue(hv_Number, 1); hv_Index1 = hv_Index1.TupleAdd(1))
                {
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedRegions1, out ho_ObjectSelected, hv_Index1);
                    HOperatorSet.AreaCenter(ho_ObjectSelected, out hv_Area1, out hv_Row1, out hv_Column1);
                    if ((int)(new HTuple(hv_Area.TupleEqual(hv_Area1))) != 0)
                    {
                        ho_Partitioned.Dispose();
                        HOperatorSet.PartitionDynamic(ho_ObjectSelected, out ho_Partitioned,
                            20, 30);
                        HOperatorSet.CountObj(ho_Partitioned, out hv_Number2);
                        if ((int)(new HTuple(hv_Number2.TupleEqual(2))) != 0)
                        {
                            for (hv_Index2 = 1; hv_Index2.Continue(hv_Number2, 1); hv_Index2 = hv_Index2.TupleAdd(1))
                            {
                                ho_ObjectSelected2.Dispose();
                                HOperatorSet.SelectObj(ho_Partitioned, out ho_ObjectSelected2, hv_Index2);
                                OTemp[SP_O] = ho_EmptyObject.CopyObj(1, -1);
                                SP_O++;
                                ho_EmptyObject.Dispose();
                                HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_ObjectSelected2, out ho_EmptyObject
                                    );
                                OTemp[SP_O - 1].Dispose();
                                SP_O = 0;
                            }
                        }
                    }
                    else
                    {
                        OTemp[SP_O] = ho_EmptyObject.CopyObj(1, -1);
                        SP_O++;
                        ho_EmptyObject.Dispose();
                        HOperatorSet.ConcatObj(OTemp[SP_O - 1], ho_ObjectSelected, out ho_EmptyObject
                            );
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;
                    }
                }
                HOperatorSet.CountObj(ho_EmptyObject, out hv_Number3);
                if ((int)(new HTuple(hv_Number3.TupleEqual(4))) != 0)
                {
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
                    OTemp[SP_O] = ho_SelectedRegions1.CopyObj(1, -1);
                    SP_O++;
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.ConcatObj(ho_EmptyObject, OTemp[SP_O - 1], out ho_SelectedRegions1
                        );
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;


                    ho_Reg_Sort.Dispose();
                    HOperatorSet.SortRegion(ho_SelectedRegions1, out ho_Reg_Sort, "first_point", "true", "column"); //排序由左到右

                    for (hv_objChar = 1; hv_objChar.Continue(hv_Number3, 1); hv_objChar = hv_objChar.TupleAdd(1))
                    {
                        ho_selCh.Dispose();
                        HOperatorSet.SelectObj(ho_Reg_Sort, out ho_selCh, hv_objChar);
                        HOperatorSet.SmallestRectangle1(ho_selCh, out hv_Row1, out hv_Column1, out hv_Row2,
                            out hv_Column2);
                        ho_roi.Dispose();
                        //ROI OBJECT
                        HOperatorSet.GenRectangle1(out ho_roi, hv_Row1 - 5, hv_Column1 - 5, hv_Row2 + 5, hv_Column2 + 5);

                        ho_Gray_Result.Dispose();
                        HOperatorSet.PaintRegion(ho_roi, ho_Gray_Source, out ho_Gray_Result, 0.0, "fill"); //塗成黑色
                        OTemp[SP_O] = ho_Gray_Result.CopyObj(1, -1);
                        SP_O++;
                        ho_Gray_Result.Dispose();
                        HOperatorSet.PaintRegion(ho_selCh, OTemp[SP_O - 1], out ho_Gray_Result, 255.0, "fill"); //把字塗白
                        OTemp[SP_O - 1].Dispose();
                        SP_O = 0;

                        ho_ImageReduced.Dispose();
                        HOperatorSet.ReduceDomain(ho_Gray_Result, ho_roi, out ho_ImageReduced);

                        HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);

                        HImage hChar = new HImage();
                        this.Hobject_To_Himage(ho_ImagePart, ref hChar);
                        Bitmap bTemp = this.HImage_To_Bitmap(hChar);
                        lstChars.Add(bTemp);
                    }
                }

            }
            else if ((int)(new HTuple(hv_Number.TupleEqual(4))) != 0)
            {
                ho_Reg_Sort.Dispose();
                HOperatorSet.SortRegion(ho_SelectedRegions1, out ho_Reg_Sort, "first_point", "true", "column"); //排序由左到右

                for (hv_objChar = 1; hv_objChar.Continue(hv_Number, 1); hv_objChar = hv_objChar.TupleAdd(1))
                {
                    ho_selCh.Dispose();
                    HOperatorSet.SelectObj(ho_Reg_Sort, out ho_selCh, hv_objChar);
                    HOperatorSet.SmallestRectangle1(ho_selCh, out hv_Row1, out hv_Column1, out hv_Row2,
                        out hv_Column2);
                    ho_roi.Dispose();
                    //ROI OBJECT
                    HOperatorSet.GenRectangle1(out ho_roi, hv_Row1 - 5, hv_Column1 - 5, hv_Row2 + 5, hv_Column2 + 5);

                    ho_Gray_Result.Dispose();
                    HOperatorSet.PaintRegion(ho_roi, ho_Gray_Source, out ho_Gray_Result, 0.0, "fill"); //塗成黑色
                    OTemp[SP_O] = ho_Gray_Result.CopyObj(1, -1);
                    SP_O++;
                    ho_Gray_Result.Dispose();
                    HOperatorSet.PaintRegion(ho_selCh, OTemp[SP_O - 1], out ho_Gray_Result, 255.0, "fill"); //把字塗白
                    OTemp[SP_O - 1].Dispose();
                    SP_O = 0;

                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_Gray_Result, ho_roi, out ho_ImageReduced);

                    HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);

                    HImage hChar = new HImage();
                    this.Hobject_To_Himage(ho_ImagePart, ref hChar);
                    Bitmap bTemp = this.HImage_To_Bitmap(hChar);
                    lstChars.Add(bTemp);
                }
            }

            ho_Regions.Dispose();
            ho_SelectedRegions1.Dispose();
            ho_EmptyObject.Dispose();
            ho_SelectedRegions.Dispose();
            ho_ObjectSelected.Dispose();
            ho_ObjectSelected2.Dispose();
            ho_RGB_source.Dispose();
            ho_Gray_Source.Dispose();
            ho_Reg_Sort.Dispose();
            ho_selCh.Dispose();
            ho_roi.Dispose();
            ho_Gray_Result.Dispose();
            ho_ImageReduced.Dispose();
            ho_Regions.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_Partitioned.Dispose();
            ho_RegionFillUp.Dispose();
            if (ho_ImagePart != null)
            {
                ho_ImagePart.Dispose();
            }

            return lstChars;
        }


        /// <summary>
        /// 找出畫面上所有驗證碼圖像
        /// </summary>
        /// <param name="bScreenShot">螢幕截圖影像</param>
        /// <returns></returns>
        public List<Bitmap> SplitVerifyFromScreenShot(Bitmap bScreenShot, ref int x1, ref int y1, ref int x2, ref int y2)
        {
            List<Bitmap> lstResult = new List<Bitmap>();

            // Local iconic variables 
            HObject ho_Image, ho_Image1, ho_Image2, ho_Image3;
            HObject ho_Region, ho_Region1, ho_Region2, ho_RegionIntersection;
            HObject ho_RegionIntersection1;
            HObject ho_Sc, ho_bwRegion, ho_RegionFillUp;
            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_objSelect = null;
            HObject ho_Rectangle = null, ho_ImageReduced = null, ho_ImagePart = null;

            // Local control variables 

            HTuple hv_iCountObjs, hv_Index, hv_y1 = new HTuple();
            HTuple hv_x1 = new HTuple(), hv_y2 = new HTuple(), hv_x2 = new HTuple();

            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
            HOperatorSet.GenEmptyObj(out ho_Sc);
            HOperatorSet.GenEmptyObj(out ho_bwRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_objSelect);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);

            ho_Sc.Dispose();
            //HOperatorSet.ReadImage(out ho_Sc, "C:/_2.png"); 
            HObject mySource = HImageConvertFromBitmap32(bScreenShot);
            ho_Sc = mySource;

            ho_Image1.Dispose();
            ho_Image2.Dispose();
            ho_Image3.Dispose();
            HOperatorSet.Decompose3(ho_Sc, out ho_Image1, out ho_Image2, out ho_Image3
                );
            ho_Region.Dispose();
            HOperatorSet.Threshold(ho_Image1, out ho_Region, 0, 70);
            ho_Region1.Dispose();
            HOperatorSet.Threshold(ho_Image2, out ho_Region1, 0, 160);
            ho_Region2.Dispose();
            HOperatorSet.Threshold(ho_Image3, out ho_Region2, 0, 240);
            ho_RegionIntersection.Dispose();
            HOperatorSet.Intersection(ho_Region, ho_Region1, out ho_RegionIntersection);
            ho_RegionIntersection1.Dispose();
            HOperatorSet.Intersection(ho_RegionIntersection, ho_Region2, out ho_RegionIntersection1
                );


            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionIntersection1, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                "and", 8000, 13000);
            //ho_RegionFillUp.Dispose();
            //HOperatorSet.FillUp(ho_RegionIntersection1, out ho_RegionFillUp);
            //ho_ConnectedRegions.Dispose();
            //HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
            //ho_SelectedRegions.Dispose();
            //HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
            //    "and", 8000, 13000);
            HOperatorSet.CountObj(ho_SelectedRegions, out hv_iCountObjs);
            if (hv_iCountObjs == 1)
            {
                HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_y1, out hv_x1, out hv_y2, out hv_x2);

                //---
                x1 = hv_x1.I;
                y1 = hv_y1.I;
                x2 = hv_x2.I;
                y2 = hv_y2.I;

                int iWidth = x2 - x1;
                int iHeight = y2 - y1;

                //---

                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_y1, hv_x1, hv_y2, hv_x2);
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Sc, ho_Rectangle, out ho_ImageReduced);
                ho_ImagePart.Dispose();
                HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);

                //將切割下來的驗證碼影像轉為Bitmap
                HImage hFullImage = new HImage();
                this.Hobject_To_Himage(ho_ImagePart, ref hFullImage);
                Bitmap bTemp = this.HImage_To_Bitmap(hFullImage);
                lstResult.Add(bTemp);
            }
            ho_Image1.Dispose();
            ho_Image2.Dispose();
            ho_Image3.Dispose();
            ho_Region.Dispose();
            ho_Region1.Dispose();
            ho_Region2.Dispose();
            ho_RegionIntersection.Dispose();
            ho_RegionIntersection1.Dispose();
            ho_Sc.Dispose();
            ho_bwRegion.Dispose();
            ho_RegionFillUp.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
            ho_objSelect.Dispose();
            ho_Rectangle.Dispose();
            ho_ImageReduced.Dispose();
            ho_ImagePart.Dispose();
            return lstResult;
        }


        public List<Bitmap> SplitVerifyFromScreenShotEx(Bitmap bScreenShot, ref int x1, ref int y1, ref int x2, ref int y2)
        {

            List<Bitmap> lstResult = new List<Bitmap>();

            // Local iconic variables 
            HObject ho_Image, ho_Image1, ho_Image2, ho_Image3;
            HObject ho_Region, ho_Region1, ho_Region2, ho_RegionIntersection;
            HObject ho_RegionIntersection1;
            HObject ho_Sc, ho_bwRegion, ho_RegionFillUp;
            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_objSelect = null;
            HObject ho_Rectangle = null, ho_ImageReduced = null, ho_ImagePart = null;

            // Local control variables 

            HTuple hv_iCountObjs, hv_Index, hv_y1 = new HTuple();
            HTuple hv_x1 = new HTuple(), hv_y2 = new HTuple(), hv_x2 = new HTuple();

            // Initialize local and output iconic variables
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
            HOperatorSet.GenEmptyObj(out ho_Sc);
            HOperatorSet.GenEmptyObj(out ho_bwRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_objSelect);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);

            ho_Sc.Dispose();
            //HOperatorSet.ReadImage(out ho_Sc, "C:/_2.png"); 
            HObject mySource = HImageConvertFromBitmap32(bScreenShot);
            ho_Sc = mySource;

            ho_Image1.Dispose();
            ho_Image2.Dispose();
            ho_Image3.Dispose();
            HOperatorSet.Decompose3(ho_Sc, out ho_Image1, out ho_Image2, out ho_Image3
                );
            ho_Region.Dispose();
            HOperatorSet.Threshold(ho_Image1, out ho_Region, 0, 70);
            ho_Region1.Dispose();
            HOperatorSet.Threshold(ho_Image2, out ho_Region1, 0, 160);
            ho_Region2.Dispose();
            HOperatorSet.Threshold(ho_Image3, out ho_Region2, 0, 240);
            ho_RegionIntersection.Dispose();
            HOperatorSet.Intersection(ho_Region, ho_Region1, out ho_RegionIntersection);
            ho_RegionIntersection1.Dispose();
            HOperatorSet.Intersection(ho_RegionIntersection, ho_Region2, out ho_RegionIntersection1
                );
            ho_RegionFillUp.Dispose();
            HOperatorSet.FillUp(ho_RegionIntersection1, out ho_RegionFillUp);
            ho_ConnectedRegions.Dispose();
            HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
            ho_SelectedRegions.Dispose();
            HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                "and", 8000, 13000);
            HOperatorSet.CountObj(ho_SelectedRegions, out hv_iCountObjs);
            if (hv_iCountObjs == 1)
            {
                HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_y1, out hv_x1, out hv_y2, out hv_x2);

                //---
                x1 = hv_x1.I;
                y1 = hv_y1.I;
                x2 = hv_x2.I;
                y2 = hv_y2.I;

                int iWidth = x2 - x1;
                int iHeight = y2 - y1;

                //---

                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_y1, hv_x1, hv_y2, hv_x2);
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Sc, ho_Rectangle, out ho_ImageReduced);
                ho_ImagePart.Dispose();
                HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);

                //將切割下來的驗證碼影像轉為Bitmap
                HImage hFullImage = new HImage();
                this.Hobject_To_Himage(ho_ImagePart, ref hFullImage);
                Bitmap bTemp = this.HImage_To_Bitmap(hFullImage);
                lstResult.Add(bTemp);
            }
            ho_Image1.Dispose();
            ho_Image2.Dispose();
            ho_Image3.Dispose();
            ho_Region.Dispose();
            ho_Region1.Dispose();
            ho_Region2.Dispose();
            ho_RegionIntersection.Dispose();
            ho_RegionIntersection1.Dispose();
            ho_Sc.Dispose();
            ho_bwRegion.Dispose();
            ho_RegionFillUp.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
            ho_objSelect.Dispose();
            ho_Rectangle.Dispose();
            ho_ImageReduced.Dispose();
            ho_ImagePart.Dispose();
            return lstResult;
        }


        #endregion


        /// <summary>
        /// 判斷影像是否有變動 : True 表示沒有更新，False = 有更新(不一樣)
        /// </summary>
        /// <param name="bm"></param>
        /// <param name="bm2"></param>
        /// <param name="iThred"></param>
        /// <returns></returns>
        public bool GetPSNR(Bitmap bm, Bitmap bm2, int iThred = 90)
        {
            int i, j;
            double sum = 0, sum1;

            double G_sum = 0, G_sum1;

            double B_sum = 0, B_sum1;

            int picsizeX = bm.Width;
            int picsizeY = bm.Height;

            for (i = 0; i < picsizeX; i++)
            {
                for (j = 0; j < picsizeY; j++)
                {
                    if(bm.GetPixel(i, j).R!= bm2.GetPixel(i, j).R)
                    {
                        return true;
                    }
                   // sum = sum + (bm.GetPixel(i, j).R - bm2.GetPixel(i, j).R) * (bm.GetPixel(i, j).R - bm2.GetPixel(i, j).R);
                }
            }
            return false;
            //for (i = 0; i < picsizeX; i++)
            //{
            //    for (j = 0; j < picsizeY; j++)
            //    {
            //        G_sum = G_sum + (bm.GetPixel(i, j).G - bm2.GetPixel(i, j).G) * (bm.GetPixel(i, j).G - bm2.GetPixel(i, j).G);
            //    }
            //}
            //for (i = 0; i < picsizeX; i++)
            //{
            //    for (j = 0; j < picsizeY; j++)
            //    {
            //        B_sum = B_sum + (bm.GetPixel(i, j).B - bm2.GetPixel(i, j).B) * (bm.GetPixel(i, j).B - bm2.GetPixel(i, j).B);
            //    }
            //}
            //sum = (sum / (picsizeX * picsizeY));
            //sum1 = 10 * (Math.Log(((255 * 255) / sum), 10));

            //G_sum = (G_sum / (picsizeX * picsizeY));
            //G_sum1 = 10 * (Math.Log(((255 * 255) / G_sum), 10));

            //B_sum = (B_sum / (picsizeX * picsizeY));
            //B_sum1 = 10 * (Math.Log(((255 * 255) / B_sum), 10));

            //double Avg = (sum1 + G_sum1 + B_sum1) / 3;

            //if (sum1 > iThred)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            //statusBar1.Text = "Time:" + DateTime.Now.ToString() + "    " + "[PSNR為]:R: " + sum1 + " G: " + G_sum1 + " B: " + B_sum1 + " Avg: " + Avg;
        }
    }
}
