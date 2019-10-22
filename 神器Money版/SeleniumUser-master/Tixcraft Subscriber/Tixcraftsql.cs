using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SQLTest
{

    public enum SQLBoolean
    {
        on,
        off
    }

    public static class TixcraftSQL
    {
        public static TixcraftSQLServer Server = new TixcraftSQLServer();
        /// <summary>
        /// 連線到Google雲端SQL (一定要先連線)
        /// </summary>
        public static void ConnectToSQL()
        {
            Server.Connect();
        }


        /// <summary>
        /// FB帳號認證的Database
        /// </summary>
        public class FBAccountDatabase 
        {
            private static string FBTableName = "AccountInfo";
            public static void Add(string FBAccount , SQLBoolean sSwitch,string mDateTime)
            { 
                String cmdText = string.Format("INSERT INTO {0} (FBAccount,state,OpenDate) VALUES('{1}','{2}','{3}')", FBTableName, FBAccount, sSwitch.ToString(), mDateTime);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            } 
            public static void Del(string FBAccount)
            { 
                String cmdText = string.Format("delete from {0} where FBAccount='{1}'", FBTableName, FBAccount);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }  
            public static void DelAllAccount(string FBAccount , string Passwd)
            {
                if (Passwd == "54088")
                {
                    String cmdText = string.Format("delete from {0} where true", FBTableName);
                    //製作指令
                    Server.cmd = new MySqlCommand(cmdText, Server.conn);
                    Server.cmd.ExecuteNonQuery();
                }
            } 
            public static bool UpDate(string FBAccount, SQLBoolean sSwitch)
            {
                if (IsExist(FBAccount))
                {
                    String cmdText = string.Format(" UPDATE {0}  SET state = '{1}'  WHERE FBAccount='{2}'", FBTableName, sSwitch.ToString(), FBAccount);
                    //製作指令
                    Server.cmd = new MySqlCommand(cmdText, Server.conn);
                    Server.cmd.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool IsExist(string FBAccount)
            {
                //String cmdText = "INSERT INTO MyTix (name,FreeUse) VALUES('幹兩點了','no')";

                String cmdText = string.Format("SELECT * FROM {0} WHERE FBAccount='{1}'", FBTableName, FBAccount);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<FBStruct> lstFB = new List<FBStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s; 
                    }
                    if (strField.Length == 3)
                    {
                        FBStruct FBTmp = new FBStruct();
                        FBTmp.Account = strField[0];
                        FBTmp.OnOff = strField[1];
                        FBTmp.mDateTime = strField[2];
                        lstFB.Add(FBTmp);
                    }

                }
                reader.Close(); //一定要關掉，只能有一個    
                if (lstFB.Count > 0)
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            public static bool IsAllow(string FBAccount)
            {
                String cmdText = string.Format("SELECT * FROM {0} WHERE FBAccount='{1}'  AND state='on'", FBTableName, FBAccount);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<FBStruct> lstFB = new List<FBStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        FBStruct FBTmp = new FBStruct();
                        FBTmp.Account = strField[0];
                        FBTmp.OnOff = strField[1];
                        FBTmp.mDateTime = strField[2];
                        lstFB.Add(FBTmp);
                    }

                }
                reader.Close(); //一定要關掉，只能有一個    
                if (lstFB.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static List<FBStruct> GetInformation(string FBAccount)
            {
                String cmdText = string.Format("SELECT * FROM {0} WHERE FBAccount='{1}'", FBTableName, FBAccount);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<FBStruct> lstFB = new List<FBStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        FBStruct FBTmp = new FBStruct();
                        FBTmp.Account = strField[0];
                        FBTmp.OnOff = strField[1];
                        FBTmp.mDateTime = strField[2];
                        lstFB.Add(FBTmp);
                    }

                }
                reader.Close(); //一定要關掉，只能有一個    
                return lstFB;
            }
            public static List<FBStruct> GetTotalAccount()
            {
                String cmdText = string.Format("SELECT * FROM {0}", FBTableName);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<FBStruct> lstFB = new List<FBStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        FBStruct FBTmp = new FBStruct();
                        FBTmp.Account = strField[0];
                        FBTmp.OnOff = strField[1];
                        FBTmp.mDateTime = strField[2];
                        lstFB.Add(FBTmp);
                    }

                }
                reader.Close(); //一定要關掉，只能有一個    
                return lstFB;
            } 
        }

        public class HDDDatabase 
        {
            private static string HDDTable = "HDDInfo";
            public static void Add(string CPUid, SQLBoolean sSwitch, string mDateTime)
            {
                String cmdText = string.Format("INSERT INTO {0} (CPUName,state,OpenDate) VALUES('{1}','{2}','{3}')", HDDTable, CPUid, sSwitch.ToString(), mDateTime);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }
            public static void Del(string CPUid)
            {
                String cmdText = string.Format("delete from {0} where CPUName='{1}'", HDDTable, CPUid);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }
            public static void DelAllAccount(string Passwd)
            {
                if (Passwd == "54088")
                {
                    String cmdText = string.Format("delete from {0} where true", HDDTable);
                    //製作指令
                    Server.cmd = new MySqlCommand(cmdText, Server.conn);
                    Server.cmd.ExecuteNonQuery();
                }
            }

            public static bool UpDate(string CPUid, SQLBoolean sSwitch, string sDateTime)
            {
                if (IsExist(CPUid))
                {
                    String cmdText = string.Format("UPDATE {0}  SET state = '{1}', OpenDate = '{2}'WHERE CPUName='{3}'", HDDTable, sSwitch.ToString(), sDateTime, CPUid);
                    //製作指令
                    Server.cmd = new MySqlCommand(cmdText, Server.conn);
                    Server.cmd.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }  
            public static bool IsExist(string CPUid)
            {
                String cmdText = string.Format("SELECT * FROM {0} WHERE CPUName='{1}'", HDDTable, CPUid);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<HDDStruct> lstCUPid = new List<HDDStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        HDDStruct CPUidTmp = new HDDStruct();
                        CPUidTmp.CPUid = strField[0];
                        CPUidTmp.OnOff = strField[1];
                        CPUidTmp.mDateTime = strField[2];
                        lstCUPid.Add(CPUidTmp);
                    } 
                }
                reader.Close(); //一定要關掉，只能有一個    
                if (lstCUPid.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool IsAllow(string CPUid)
            {
                String cmdText = string.Format("SELECT * FROM {0} WHERE CPUName='{1}'  AND state='on'", HDDTable, CPUid);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<HDDStruct> lstCUPid = new List<HDDStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        HDDStruct CPUidTmp = new HDDStruct();
                        CPUidTmp.CPUid = strField[0];
                        CPUidTmp.OnOff = strField[1];
                        CPUidTmp.mDateTime = strField[2];
                        lstCUPid.Add(CPUidTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    
                if (lstCUPid.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                //return true; // 20180323 --> 全開不鎖了
            }
            public static List<HDDStruct> GetInformation(string CPUid)
            {
                String cmdText = string.Format("SELECT * FROM {0} WHERE CPUName='{1}'", HDDTable, CPUid);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<HDDStruct> lstCUPid = new List<HDDStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        HDDStruct CPUidTmp = new HDDStruct();
                        CPUidTmp.CPUid = strField[0];
                        CPUidTmp.OnOff = strField[1];
                        CPUidTmp.mDateTime = strField[2];
                        lstCUPid.Add(CPUidTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    
                return lstCUPid;
            }
            public static List<HDDStruct> GetTotalAccount()
            {
                String cmdText = string.Format("SELECT * FROM {0}", HDDTable);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<HDDStruct> lstCUPid = new List<HDDStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        HDDStruct CPUidTmp = new HDDStruct();
                        CPUidTmp.CPUid = strField[0];
                        CPUidTmp.OnOff = strField[1];
                        CPUidTmp.mDateTime = strField[2];
                        lstCUPid.Add(CPUidTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    
                return lstCUPid;
            }
        }

        public class ShareAnswerDatabase
        {
            private static string AnsTable = "ShareAnswer";
            public static void Add(string SAnswer, string mDateTime)
            {
                String cmdText = string.Format("INSERT INTO {0} (Ans,OpenDate) VALUES('{1}','{2}')", AnsTable, SAnswer, mDateTime);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }
            public static void Del(string SAnswer)
            {
                String cmdText = string.Format("delete from {0} where Ans='{1}'", AnsTable, SAnswer);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }
            public static void Clear(string Passwd)
            {
                if (Passwd == "54088")
                {
                    String cmdText = string.Format("delete from {0} where true", AnsTable);
                    //製作指令
                    Server.cmd = new MySqlCommand(cmdText, Server.conn);
                    Server.cmd.ExecuteNonQuery();
                }
            }
            public static List<SAStruct> GetTotalAccount()
            {
                String cmdText = string.Format("SELECT * FROM {0}", AnsTable);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<SAStruct> lstAnswers = new List<SAStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 2)
                    {
                        SAStruct CPUidTmp = new SAStruct();
                        CPUidTmp.MAnswer = strField[0];
                        CPUidTmp.mDateTime = strField[1];
                        lstAnswers.Add(CPUidTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    
                return lstAnswers;
            }
        } 

        public class CookieDatabase
        {
            private static string CookieTable = "TixCookiesInfo";
            public static void Add(string email, string Cookiepkg, string mDateTime)
            {
                String cmdText = string.Format("INSERT INTO {0} (email,cookie ,time ) VALUES('{1}','{2}','{3}')", CookieTable, email, Cookiepkg, mDateTime);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }
            public static void Del(string mEmail)
            {
                String cmdText = string.Format("delete from {0} where email='{1}'", CookieTable, mEmail);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }
            public static void DelAllAccount(string Passwd)
            {
                if (Passwd == "54088")
                {
                    String cmdText = string.Format("delete from {0} where true", CookieTable);
                    //製作指令
                    Server.cmd = new MySqlCommand(cmdText, Server.conn);
                    Server.cmd.ExecuteNonQuery();
                }
            }
            public static bool UpDate(string myEmail,string myCookiepkg)
            {
                if (IsExist(myEmail))
                {
                    String cmdText = string.Format(" UPDATE {0}  SET cookie = '{1}'  WHERE email='{2}'", CookieTable, myCookiepkg, myEmail);
                    //製作指令
                    Server.cmd = new MySqlCommand(cmdText, Server.conn);
                    Server.cmd.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool IsExist(string myEmail)
            {
                String cmdText = string.Format("SELECT * FROM {0} WHERE email='{1}'", CookieTable, myEmail);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<CookieStruct> lstCookietmp = new List<CookieStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        CookieStruct CookieTmp = new CookieStruct();
                        CookieTmp.Email = strField[0];
                        CookieTmp.CookiePkg = strField[1];
                        CookieTmp.mDateTime = strField[2];
                        lstCookietmp.Add(CookieTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    
                if (lstCookietmp.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }  
            public static List<CookieStruct> GetTotalCookieHistory()
            {
                String cmdText = string.Format("SELECT * FROM {0}", CookieTable);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<CookieStruct> lstCookieTmp = new List<CookieStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        CookieStruct CkTmp = new CookieStruct();
                        CkTmp.Email = strField[0];
                        CkTmp.CookiePkg = strField[1];
                        CkTmp.mDateTime = strField[2];
                        lstCookieTmp.Add(CkTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    
                return lstCookieTmp;
            }
        }


        public class ProxyDatabase
        {
            private static string AnsTable = "ProxyInfo";
            public static void Add(string sProxy, string sRegion , string mDateTime)
            {
                String cmdText = string.Format("INSERT INTO {0} (sProxy,sRegion,sTime) VALUES('{1}','{2}','{3}')", AnsTable, sProxy, sRegion ,  mDateTime );
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }
            public static void Del(string sProxy)
            {
                String cmdText = string.Format("delete from {0} where sProxy='{1}'", AnsTable, sProxy);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                Server.cmd.ExecuteNonQuery();
            }
            public static void Clear(string Passwd)
            {
                if (Passwd == "54088")
                {
                    String cmdText = string.Format("delete from {0} where true", AnsTable);
                    //製作指令
                    Server.cmd = new MySqlCommand(cmdText, Server.conn);
                    Server.cmd.ExecuteNonQuery();
                }
            }
            public static List<ProxyStruct> GetProxyList()
            {
                String cmdText = string.Format("SELECT * FROM {0}", AnsTable);
                //製作指令
                Server.cmd = new MySqlCommand(cmdText, Server.conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = Server.cmd.ExecuteReader(); //execure the reader
                List<ProxyStruct> lstAnswers = new List<ProxyStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 3)
                    {
                        ProxyStruct ProxyPackageTmp = new ProxyStruct();
                        ProxyPackageTmp.ProxyIP = strField[0];
                        ProxyPackageTmp.Region = strField[1];
                        ProxyPackageTmp.mDateTime = strField[2];
                        lstAnswers.Add(ProxyPackageTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    
                return lstAnswers;
            }
        } 

    }



    /// <summary>
    /// Google 雲端 SQL 的資料結構與連線方式
    /// </summary>
    public class TixcraftSQLServer
    {
        //string dbHost = "35.192.60.56";//資料庫位址 --> 武庫帳號
        //string dbHost = "35.192.81.49";//資料庫位址 --> 大翔帳號 --> skeat520
        //string dbHost = "35.202.189.89";//資料庫位址 --> 武庫帳號 --> 4a0g0137 
        //string dbHost = "35.193.144.193";//資料庫位址 --> 武庫帳號 --> 4a0g0137 --> 精文帳號f
        //string dbHost = "35.202.147.14";//資料庫位址 --> 20190425
        //string dbHost = "35.188.51.136";//資料庫位址 --> 20190903 
        string dbHost = "104.198.137.247";//資料庫位址 --> 20190918 承儒
        string dbUser = "tgymchef";//資料庫使用者帳號
        string dbPass = "skeat9999";//資料庫使用者密碼
        string dbName = "MyTest";//資料庫名稱 DB      
        string connStr = "";
        public bool IsConnect = false;
        public MySqlConnection conn;
        public MySqlCommand cmd;
        public void Connect()
        {
            connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
            //進行連線
            conn = new MySqlConnection(connStr);
            conn.Open();
            IsConnect = true;
        }
        public void DisConnect()
        {
            conn.Close();
        }

        /// <summary>
        /// 下載考試答案 ( DB : ShareAnswer )
        /// </summary>
        /// <returns></returns>
        public string DownLoadTestAnswerFromSQL()
        {
            if (this.IsConnect)
            {
                String cmdText = string.Format("SELECT * FROM {0}", "ShareAnswer");
                //製作指令
                cmd = new MySqlCommand(cmdText, conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = cmd.ExecuteReader(); //execure the reader
                List<SAStruct> lstAnswers = new List<SAStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 2)
                    {
                        SAStruct CPUidTmp = new SAStruct();
                        CPUidTmp.MAnswer = strField[0];
                        CPUidTmp.mDateTime = strField[1];
                        lstAnswers.Add(CPUidTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    

                List<SAStruct> lstTmpAllList = lstAnswers;
                string strNewestAnswer = "";
                if (lstTmpAllList.Count > 0)
                {
                    strNewestAnswer = lstTmpAllList[lstTmpAllList.Count - 1].MAnswer;
                    //lblSystemMsg.Text = "下載成功!(" + lstTmpAllList[lstTmpAllList.Count - 1].mDateTime + ")";
                }
                else
                {
                    strNewestAnswer = "";
                    //lblSystemMsg.Text = "沒有答案...";
                }
                return strNewestAnswer;
            }
            else
                return "";
        }

        /// <summary>
        /// 下載考試答案 ( DB : ShareAnswer ) ( 有區分 A & B , 代表周六、周日 )
        /// </summary>
        /// <returns></returns>
        public string DownLoadTestAnswerFromSQL(string indexServer = "AAAAAAAAAA")
        {
            if (this.IsConnect)
            {
                String cmdText = string.Format("SELECT * FROM {0}", "ShareAnswer");
                //製作指令
                cmd = new MySqlCommand(cmdText, conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = cmd.ExecuteReader(); //execure the reader
                List<SAStruct> lstAnswers = new List<SAStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 2)
                    {
                        SAStruct CPUidTmp = new SAStruct();
                        CPUidTmp.MAnswer = strField[0];
                        CPUidTmp.mDateTime = strField[1];
                        lstAnswers.Add(CPUidTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    

                List<SAStruct> lstTmpAllList = lstAnswers;
                //從文字過濾
                List<SAStruct> lstFilterList = new List<SAStruct>();
                for (int i = 0; i < lstTmpAllList.Count; i++)
                {
                    if (lstTmpAllList[i].MAnswer.Contains(indexServer))
                    {
                        lstFilterList.Add(lstTmpAllList[i]);
                    }
                }

                string strNewestAnswer = "";
                if (lstFilterList.Count > 0)
                {
                    string[] spG = lstFilterList[lstFilterList.Count - 1].MAnswer.Split(',');
                    if (spG.Length > 1)
                    {
                        strNewestAnswer = spG[1];
                        //strNewestAnswer = lstFilterList[lstTmpAllList.Count - 1].MAnswer.Split(',')[1];
                    }
                    else 
                    {
                        strNewestAnswer = "";
                    }
                    
                    //lblSystemMsg.Text = "下載成功!(" + lstTmpAllList[lstTmpAllList.Count - 1].mDateTime + ")";
                }
                else
                {
                    strNewestAnswer = "";
                    //lblSystemMsg.Text = "沒有答案...";
                }
                return strNewestAnswer;
            }
            else
                return "";
        }

        public  bool Add_Answer(string SAnswer, string mDateTime)
        {
            bool bIsSuccessful = false;
            try
            { 
                String cmdText = string.Format("INSERT INTO {0} (Ans,OpenDate) VALUES('{1}','{2}')", "ShareAnswer", SAnswer, mDateTime);
                //製作指令
                cmd = new MySqlCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
                bIsSuccessful = true;
            }
            catch (Exception)
            {
                bIsSuccessful = false;
            }
            return bIsSuccessful;
        }

    }


    public class TixcraftCookieServer
    {
        //string dbHost = "35.192.60.56";//資料庫位址 --> 武庫帳號
        //string dbHost = "35.192.81.49";//資料庫位址 --> 大翔帳號 --> skeat520
        //string dbHost = "35.202.189.89";//資料庫位址 --> 武庫帳號 --> 4a0g0137 
        //string dbHost = "35.193.144.193";//資料庫位址 --> 武庫帳號 --> 4a0g0137 --> 精文帳號f
        //string dbHost = "35.202.147.14";//資料庫位址 --> 20190425
        //string dbHost = "35.188.51.136";//資料庫位址 --> 20190903 - 4a0g0136@stust.edu.tw
        string dbHost = "104.198.137.247";//資料庫位址 --> 20190918 承儒
        string dbUser = "tgymchef";//資料庫使用者帳號
        string dbPass = "skeat9999";//資料庫使用者密碼
        string dbName = "MyTest";//資料庫名稱 DB      
        string connStr = "";
        bool IsConnect = false;
        public MySqlConnection conn;
        public MySqlCommand cmd;
        private static string CookieTable = "TixCookiesInfo";
        public void Connect()
        {
            connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
            //進行連線
            conn = new MySqlConnection(connStr);
            conn.Open();
            IsConnect = true;
        }
        public void DisConnect()
        {
            conn.Close();
        }
        /// <summary>
        /// 新增物件化連線給Multi Thread使用 ( 避免conn共用即可閃掉崩潰狀態 )
        /// </summary>
        /// <returns></returns>
        public string DownLoadTestAnswerFromSQL()
        {
            if (this.IsConnect)
            {
                String cmdText = string.Format("SELECT * FROM {0}", "ShareAnswer");
                //製作指令
                cmd = new MySqlCommand(cmdText, conn);
                //使用reader進行讀取 ( 只能一次!! )
                MySqlDataReader reader = cmd.ExecuteReader(); //execure the reader
                List<SAStruct> lstAnswers = new List<SAStruct>();
                while (reader.Read())
                {
                    string[] strField = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        String s = reader.GetString(i);
                        strField[i] = s;
                    }
                    if (strField.Length == 2)
                    {
                        SAStruct CPUidTmp = new SAStruct();
                        CPUidTmp.MAnswer = strField[0];
                        CPUidTmp.mDateTime = strField[1];
                        lstAnswers.Add(CPUidTmp);
                    }
                }
                reader.Close(); //一定要關掉，只能有一個    

                List<SAStruct> lstTmpAllList = lstAnswers;
                string strNewestAnswer = "";
                if (lstTmpAllList.Count > 0)
                {
                    strNewestAnswer = lstTmpAllList[lstTmpAllList.Count - 1].MAnswer;
                    //lblSystemMsg.Text = "下載成功!(" + lstTmpAllList[lstTmpAllList.Count - 1].mDateTime + ")";
                }
                else
                {
                    strNewestAnswer = "";
                    //lblSystemMsg.Text = "沒有答案...";
                }
                return strNewestAnswer;
            }
            else
                return "";
        }
        public void Add(string email, string Cookiepkg, string mDateTime)
        {
            String cmdText = string.Format("INSERT INTO {0} (email,cookie ,time ) VALUES('{1}','{2}','{3}')", CookieTable, email, Cookiepkg, mDateTime);
            //製作指令
            cmd = new MySqlCommand(cmdText, conn);
            cmd.ExecuteNonQuery();
        }

        public bool UpDate(string myEmail, string myCookiepkg)
        {
            if (IsExist(myEmail))
            {
                String cmdText = string.Format(" UPDATE {0}  SET cookie = '{1}'  WHERE email='{2}'", CookieTable, myCookiepkg, myEmail);
                //製作指令
                cmd = new MySqlCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsExist(string myEmail)
        {
            String cmdText = string.Format("SELECT * FROM {0} WHERE email='{1}'", CookieTable, myEmail);
            //製作指令
            cmd = new MySqlCommand(cmdText, conn);
            //使用reader進行讀取 ( 只能一次!! )
            MySqlDataReader reader = cmd.ExecuteReader(); //execure the reader
            List<CookieStruct> lstCookietmp = new List<CookieStruct>();
            while (reader.Read())
            {
                string[] strField = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    String s = reader.GetString(i);
                    strField[i] = s;
                }
                if (strField.Length == 3)
                {
                    CookieStruct CookieTmp = new CookieStruct();
                    CookieTmp.Email = strField[0];
                    CookieTmp.CookiePkg = strField[1];
                    CookieTmp.mDateTime = strField[2];
                    lstCookietmp.Add(CookieTmp);
                }
            }
            reader.Close(); //一定要關掉，只能有一個    
            if (lstCookietmp.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public List<CookieStruct> GetTotalCookieHistory()
        {
            String cmdText = string.Format("SELECT * FROM {0}", CookieTable);
            //製作指令
            cmd = new MySqlCommand(cmdText, conn);
            //使用reader進行讀取 ( 只能一次!! )
            MySqlDataReader reader = cmd.ExecuteReader(); //execure the reader
            List<CookieStruct> lstCookieTmp = new List<CookieStruct>();
            while (reader.Read())
            {
                string[] strField = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    String s = reader.GetString(i);
                    strField[i] = s;
                }
                if (strField.Length == 3)
                {
                    CookieStruct CkTmp = new CookieStruct();
                    CkTmp.Email = strField[0];
                    CkTmp.CookiePkg = strField[1];
                    CkTmp.mDateTime = strField[2];
                    lstCookieTmp.Add(CkTmp);
                }
            }
            reader.Close(); //一定要關掉，只能有一個    
            return lstCookieTmp;
        }
        public void Del(string mEmail)
        {
            String cmdText = string.Format("delete from {0} where email='{1}'", CookieTable, mEmail);
            //製作指令
             cmd = new MySqlCommand(cmdText,conn);
             cmd.ExecuteNonQuery();
        }
        public void EnableAutoFlag(bool bIsEnable)
        {

            if (bIsEnable)
            {
                if (IsExist("OpenAutoFlag") == false)
                {
                    Del("CloseAuto");
                    Add("OpenAutoFlag", "OpenAutoFlag", "OpenAutoFlag");
                }
            }
            else
            { 
                if (IsExist("CloseAuto") == false)
                {
                    Del("OpenAutoFlag");
                    Add("CloseAuto", "CloseAuto", "CloseAuto");
                }
            } 
        }
        public bool GetAutoFlag()
        {
            if (IsExist("OpenAutoFlag") == true && IsExist("CloseAuto") == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
