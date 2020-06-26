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
            public static bool UpDate(string CPUid, SQLBoolean sSwitch)
            {
                if (IsExist(CPUid))
                {
                    String cmdText = string.Format(" UPDATE {0}  SET state = '{1}'  WHERE CPUName='{2}'", HDDTable, sSwitch.ToString(), CPUid);
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


    }



    /// <summary>
    /// Google 雲端 SQL 的資料結構與連線方式
    /// </summary>
    public class TixcraftSQLServer
    {
        //string dbHost = "35.192.60.56";//資料庫位址
        //string dbHost = "35.202.147.14";//資料庫位址 --> 20190425
        //string dbHost = "35.188.51.136";//資料庫位址 --> 20190903 
        string dbHost = "34.68.9.175";//資料庫位址 --> 20190918 承儒
        string dbUser = "tgymchef";//資料庫使用者帳號
        string dbPass = "skeat9999";//資料庫使用者密碼
        string dbName = "MyTest";//資料庫名稱 DB      
        string connStr = "";
        public MySqlConnection conn;
        public MySqlCommand cmd;
        public void Connect() 
        { 
            connStr = "server=" + dbHost + ";uid=" + dbUser + ";pwd=" + dbPass + ";database=" + dbName;
            //進行連線
            conn = new MySqlConnection(connStr);
            conn.Open();
        }
        public void DisConnect()
        {
            conn.Close();
        }

    }

}
