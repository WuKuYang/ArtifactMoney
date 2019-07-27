using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Recipe
{
    public class EConfig<T>
    {
        /*
         *  2016 - 12 - 30 Demon Created.
         * 此份類別將自動對子類別進行參數存檔功能
         * 使用方式 :
         * 在UI下使用一個Controls包含所有子項目(例如 textbox , checkbox , label )
         *  使用時，需要將UI上控件名稱(name) 命名與 您參數下欄位名稱 (需要為public公開形式)
         *  
         * 
         * 例如 :
         *  class A
         *  {
         *      public int MyFeild = 1;
         *  }
         *  
         * 則 textbox1 要命名為 MyFeild ，這樣即可進行自動搜尋欄位且填入 / 輸出！ 
         */
        private string g_Path = "";

        /// <summary>
        /// XML will save to this path
        /// </summary>
        public string Config_Path { get { return g_Path; } set { g_Path = value; } }

        public void GetRecipeFromUI(T clsRecipe, Control formGroupBox)
        {
            if (formGroupBox.Controls.Count > 0)
            {
                foreach (Control aControl in formGroupBox.Controls)
                {
                    GetRecipeFromUI(clsRecipe, aControl);
                }
            }
            else
            {
                FieldInfo[] clsFieldInfos = clsRecipe.GetType().GetFields();
                //取得欄位資訊。
                foreach (FieldInfo clsFInfo in clsFieldInfos)
                {
                    //參數物件上的名字。 
                    string strRecipeValueName = clsFInfo.Name;
                    dynamic ObjUI = formGroupBox;
                    if (ObjUI.Name == strRecipeValueName)
                    {
                        Type FileType = (clsFInfo.GetValue(clsRecipe)).GetType();
                        if (FileType == typeof(int))
                        {
                            clsFInfo.SetValue(clsRecipe, Convert.ToInt32(ObjUI.Text));
                        }
                        else if (FileType == typeof(float))
                        {
                            clsFInfo.SetValue(clsRecipe, Convert.ToSingle(ObjUI.Text));
                        }
                        else if (FileType == typeof(double))
                        {
                            clsFInfo.SetValue(clsRecipe, Convert.ToDouble(ObjUI.Text));
                        }
                        else if (FileType == typeof(bool))
                        {
                            clsFInfo.SetValue(clsRecipe, Convert.ToBoolean(ObjUI.Checked));
                        }
                        else if (FileType == typeof(string))
                        {
                            clsFInfo.SetValue(clsRecipe, Convert.ToString(ObjUI.Text));
                        }
                        else if (FileType == typeof(decimal))
                        {
                            clsFInfo.SetValue(clsRecipe, Convert.ToDecimal(ObjUI.Text));
                        }
                    }
                    //dynamic Value = clsFInfo.GetValue(clsObj);
                }
            }
        }

        public void SetRecipeToUI(T clsRecipe, Control formGroupBox)
        {
            if (formGroupBox.Controls.Count > 0)
            {
                foreach (Control aControl in formGroupBox.Controls)
                {
                    SetRecipeToUI(clsRecipe, aControl);
                }
            }
            else
            {
                FieldInfo[] clsFieldInfos = clsRecipe.GetType().GetFields();
                //取得欄位資訊。
                foreach (FieldInfo clsFInfo in clsFieldInfos)
                {
                    //參數物件上的名字。 
                    string strRecipeValueName = clsFInfo.Name;
                    dynamic ObjUI = formGroupBox;
                    if (ObjUI.Name == strRecipeValueName)
                    {
                        Type FileType = (clsFInfo.GetValue(clsRecipe)).GetType();
                        var value = clsFInfo.GetValue(clsRecipe);
                        if (FileType == typeof(int))
                        {
                            ObjUI.Text = value.ToString();
                        }
                        else if (FileType == typeof(float))
                        {
                            ObjUI.Text = value.ToString();
                        }
                        else if (FileType == typeof(double))
                        {
                            ObjUI.Text = value.ToString();
                        }
                        else if (FileType == typeof(bool))
                        {
                            ObjUI.Checked = Convert.ToBoolean(value);
                        }
                        else if (FileType == typeof(string))
                        {
                            ObjUI.Text = value.ToString();
                        }
                        else if (FileType == typeof(decimal))
                        {
                            ObjUI.Text = value.ToString();
                        }
                        //var value = clsFInfo.GetValue(clsRecipe);
                        //ObjUI.Text = value.ToString();
                    }
                    //dynamic Value = clsFInfo.GetValue(clsObj);
                }
            }
        }

        public bool LoadRecipeFromXml(ref T clsRecipe, Control ControlsOnUI)
        {
            try
            {
                string strPath = g_Path;

                if (File.Exists(strPath))
                {
                    //反序列化。
                    using (FileStream oFileStream = new FileStream(strPath, FileMode.Open))
                    {
                        System.Xml.Serialization.XmlSerializer clsSerializer = new System.Xml.Serialization.XmlSerializer(clsRecipe.GetType());
                        clsRecipe = (T)clsSerializer.Deserialize(oFileStream);
                    }
                    SetRecipeToUI(clsRecipe, ControlsOnUI);
                    return true;
                }
                else
                {
                    MessageBox.Show(String.Format("[ Config ] Failed to load. ( do not have config ) , now will use default config"));
                    SetRecipeToUI(clsRecipe, ControlsOnUI);
                    return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("讀參數錯誤 Permissions not enough , please run as Admin..!!!");
                return false;
            }
        }

        public bool LoadRecipeFromXml(ref T clsRecipe)
        {
            try
            {
                string strPath = g_Path;

                if (File.Exists(strPath))
                {
                    //反序列化。
                    using (FileStream oFileStream = new FileStream(strPath, FileMode.Open))
                    {
                        System.Xml.Serialization.XmlSerializer clsSerializer = new System.Xml.Serialization.XmlSerializer(clsRecipe.GetType());
                        clsRecipe = (T)clsSerializer.Deserialize(oFileStream);
                    }
                    return true;
                }
                else
                {
                    MessageBox.Show(String.Format("[ Config ] - 找不到存檔可以讀！"));
                    return false;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("讀參數錯誤 Permissions not enough , please run as Admin..!!!");
                return false;
            }
        }

        public bool SaveRecipeToXml(T clsRecipe, Control ControlsOnUI)
        {
            string strPath = g_Path;
            try
            {
                GetRecipeFromUI(clsRecipe, ControlsOnUI);
                using (FileStream oFileStream = new FileStream(strPath, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer ClsSerializer = new System.Xml.Serialization.XmlSerializer(clsRecipe.GetType());
                    ClsSerializer.Serialize(oFileStream, clsRecipe);
                    oFileStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("[  設定檔  ] 儲存失敗. Permissions not enough , please run as Admin..!!! : " + strPath);
                return false;
            }
        }

        public bool SaveRecipeToXml(T clsRecipe)
        {
            string strPath = g_Path;
            try
            {
                using (FileStream oFileStream = new FileStream(strPath, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer ClsSerializer = new System.Xml.Serialization.XmlSerializer(clsRecipe.GetType());
                    ClsSerializer.Serialize(oFileStream, clsRecipe);
                    oFileStream.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("[  設定檔  ] 儲存失敗. Permissions not enough , please run as Admin..!!! : " + strPath);
                return false;
            }
        }

    }

    public class logindata
    {
        public string Address = "";
        public string passwod = "";
        public string LoginMode = "";
    }

    public class TSRecipe : EConfig<TSRecipe>
    {
        public List< logindata >USER_Infor = new List<logindata>(); 
    }


    public class IPRecipe : EConfig<IPRecipe>
    {
        public string IP = "127.0.0.1";
        public int WebBrowser_Width = 500;

    }

}
