using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Drawing;

namespace OCRInspection
{
    public class OCRServer
    {  

        IPEndPoint ipep = null;
        Socket server = null;
        public string strServerIP = "127.0.0.1";
        int iServerPort = 10000;
        StreamReader sr = null;
        StreamWriter sw = null;
        NetworkStream stream = null;
        object myLockTensorflow = new object();
        public OCRServer() { }
        public OCRServer(string IP , int Port)
        {
            this.iServerPort = Port;
            this.strServerIP = IP;
        } 
        public void Connect()
        { 
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(strServerIP), iServerPort);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(ipep);
            stream = new NetworkStream(server);
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
        }

        public string GetChar(Image OCRImage)
        {
            string strRecv = "";
            try
            {
                lock (myLockTensorflow)
                {
                    //傳圖給Server
                    server.Send(ImageToByte(OCRImage));
                    //等待接收答案
                    strRecv = sr.ReadLine();
                    strRecv = strRecv.Replace("'", "");
                }
            }
            catch (Exception ex)
            {
                this.Connect();
            }
            return strRecv;
        } 
        byte[] ImageToByte(System.Drawing.Image iImage)
        {
            MemoryStream mMemoryStream = new MemoryStream();
            iImage.Save(mMemoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
            return mMemoryStream.ToArray();
        }
    }
}
