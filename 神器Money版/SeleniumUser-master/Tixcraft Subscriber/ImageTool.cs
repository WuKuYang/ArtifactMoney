using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tixcraft_Subscriber
{
    public static class ImageTool
    { 
        public static Bitmap GetBlock(Bitmap clsImage, Rectangle tRect)
        {
            Bitmap clsImageNew = new Bitmap(tRect.Width, tRect.Height);

            Graphics.FromImage(clsImageNew).DrawImage(clsImage, new Rectangle(0, 0, tRect.Width, tRect.Height),
                                                        tRect, GraphicsUnit.Pixel);

            return clsImageNew;
        }
        public static byte[] ImageToBuffer(Image Image, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            if (Image == null) { return null; }
            byte[] data = null;
            using (MemoryStream oMemoryStream = new MemoryStream())
            {
                //建立副本
                using (Bitmap oBitmap = new Bitmap(Image))
                {
                    //儲存圖片到 MemoryStream 物件，並且指定儲存影像之格式
                    oBitmap.Save(oMemoryStream, imageFormat);
                    //設定資料流位置
                    oMemoryStream.Position = 0;
                    //設定 buffer 長度
                    data = new byte[oMemoryStream.Length];
                    //將資料寫入 buffer
                    oMemoryStream.Read(data, 0, Convert.ToInt32(oMemoryStream.Length));
                    //將所有緩衝區的資料寫入資料流
                    oMemoryStream.Flush();
                }
            }
            return data;
        }
    }

}
