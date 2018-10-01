using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace SQLTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        string dbHost = "35.192.60.56";//資料庫位址
        string dbUser = "linbaywugi";//資料庫使用者帳號
        string dbPass = "12345678";//資料庫使用者密碼
        string dbName = "MyTest";//資料庫名稱 DB      
        string connStr = "";
        MySqlConnection conn;
        MySqlCommand cmd;

        /*
          目前使用Database名稱為 : MyTest
             --> 旗下的Table名稱 : MyTix
                     --> MyTix共含2個欄位 name,FreeUse 
         */

        private void Form1_Load(object sender, EventArgs e)
        {
            connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
            //進行連線
            conn = new MySqlConnection(connStr);
            conn.Open();
            //MyTest是Database .
            //MyTix是table , 只有兩個欄位 
        }

        private void btnReadAll_Click(object sender, EventArgs e)
        { 
            //新增一筆資料進入語法 : (新增到Table內)
            //======================================================
            // INSERT INTO MyTix (name,FreeUse) VALUES('yang','no');
            //======================================================
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            String cmdText = "SELECT * FROM MyTix";
            //製作指令
            cmd = new MySqlCommand(cmdText, conn); 
            //使用reader進行讀取 ( 只能一次!! )
            MySqlDataReader reader = cmd.ExecuteReader(); //execure the reader
            string strRead = "";
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    String s = reader.GetString(i);
                    strRead = strRead + s + "\t";
                }
                strRead += "\n";
            }
            reader.Close(); //一定要關掉，只能有一個
            sw.Stop();
            strRead += "\n" + sw.ElapsedMilliseconds.ToString() + "ms";
            MessageBox.Show(strRead);
            //conn.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        { 
            conn.Close();
        }

        private void btnInsertInto_Click(object sender, EventArgs e)
        {
            String cmdText = "INSERT INTO MyTix (name,FreeUse) VALUES('幹兩點了','no')";
            //製作指令
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            String cmdText = "delete from MyTix where true";
            //製作指令
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }
    }
}
