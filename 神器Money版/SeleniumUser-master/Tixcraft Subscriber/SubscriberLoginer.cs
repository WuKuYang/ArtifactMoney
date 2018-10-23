using Recipe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TSubscriber;
using SQLTest;
using WinForm_Test;
using System.Net;
using TixWin;
using System.Diagnostics;
using VIPGeneral;
using System.Threading;
using System.IO;
using HalconDotNet;

namespace Tixcraft_Subscriber
{
    public partial class SubscriberLoginer : Form
    {
        public SubscriberLoginer()
        {
            InitializeComponent();
        }

        public string g_AnswerServer = "AAAAAAAAAA";
        public const string Server_A = "AAAAAAAAAA";
        public const string Server_B = "BBBBBBBBBB";

        public TSRecipe USERData = new TSRecipe();
        public IPRecipe OCRRecipe = new IPRecipe();

        public string SavePath = "C:\\TixcraftFB_users_data.txt";
        public string OcrSavePath = "C:\\OCRServerIP.txt";

        public bool g_bIsMountProxy = false;

        public TixcraftCookieServer CookieServer = null;
        TixcraftSubscriber Tixcraft = new TixcraftSubscriber(); 
        public FastHttpWebDriver FastHttpDriver = new FastHttpWebDriver();

        List<FastHttpWebDriver> FBBrowser = new List<FastHttpWebDriver>();
        public Process[] g_ChromeDrives = null;

        public List<TixcraftSubscriber.Activity> g_AllShow = new List<TixcraftSubscriber.Activity>();
        public int g_iSelectShowIndex = -1;

        //聊天室
        List<string> g_PixelPinAddress = new List<string>();

        private delegate void udRefreshText(Control ctr, string msg);
        private udRefreshText degRefreshText; 
        private delegate void udControlEnable(Control ctr, bool bIsEnable);
        private udControlEnable degControlEnable;
        private void SubscriberLoginer_Load(object sender, EventArgs e)
        {

            CookieServer = new TixcraftCookieServer();
            CookieServer.Connect();
            //連線到Google雲端資料庫
            string strMyCPUid = "";
            //Show出等待視窗，並且等待事件結束後自動消失
            LoadingBox frmWaitBox = new LoadingBox((obj, args) =>
            {
                TixcraftSQL.ConnectToSQL();
                //取得本機端硬體序號
                strMyCPUid = HDDInformation.GetDevice(); 
            }, 600, "硬體認證中...", false, true);
            frmWaitBox.ShowDialog();

            //string strMyCPUid = HDDInformation.GetDevice(); 
            //委派
            degRefreshText = RefreshText;
            degControlEnable = ControlEnable;
            USERData.Config_Path = SavePath;
            OCRRecipe.Config_Path = OcrSavePath;
            OCRRecipe.LoadRecipeFromXml(ref OCRRecipe);
            txtIP.Text = OCRRecipe.IP;
            LoadingBox DownLoadShow = new LoadingBox((obj, args) =>
            {
                ShowAllActivity();
            }, 600, "取得節目資訊...", false, true);
            DownLoadShow.ShowDialog();

            //判斷認證
            if (TixcraftSQL.HDDDatabase.IsAllow("CPUID_ver1022")) 
            //if (true)
            {
                
                #region 認證成功 
                lblHDDNumber.Text = string.Format("硬體資訊 - 硬體序號 : {0} , 開通狀態 : 開通", strMyCPUid);


                SQLBoolean tmpAllow = SQLBoolean.on; 
                //若已開通，則直接修改即可(修改時間戳記...)
                if (TixcraftSQL.HDDDatabase.IsExist(strMyCPUid))
                {
                    TixcraftSQL.HDDDatabase.UpDate(strMyCPUid, tmpAllow, DateTime.Now.ToString()); 
                }
                else 
                { 
                    TixcraftSQL.HDDDatabase.Add(strMyCPUid, tmpAllow, DateTime.Now.ToString());
                }

                pnlControlTable.Visible = true;
                #endregion

            }
            else
            {
                #region 認證失敗 
                Clipboard.SetData(DataFormats.Text, strMyCPUid);
                lblHDDNumber.Text = string.Format("硬體序號 : {0}  開通狀態 : 尚未開通(請洽軟體開發者)", strMyCPUid);
                #region 上傳時間戳記 + 硬體序號
                SQLBoolean tmpAllow = SQLBoolean.off;
                if (TixcraftSQL.HDDDatabase.IsExist(strMyCPUid))
                {
                    //若已有硬體資訊，則直接修改即可(修改時間戳記...)
                    TixcraftSQL.HDDDatabase.UpDate(strMyCPUid, tmpAllow, DateTime.Now.ToString());
                }
                else
                {
                    //若沒有硬體資訊，則新增一筆資料上去
                    TixcraftSQL.HDDDatabase.Add(strMyCPUid, tmpAllow, DateTime.Now.ToString());
                }
                #endregion
                pnlControlTable.Visible = false;
                
                MessageBox.Show("您沒有權限使用這個軟體！序號尚未開通，請把硬體序號給管理人員協助開通，已幫您將序號 : " + strMyCPUid + " 複製到剪貼簿上！");
                #endregion
            }
            //TransferPixelPin();
            //ReadPixelPinUsers("add.txt");
            ReadPixelPinUsersBySQL();
            this.TopMost = false;
        }


        public void UpdateToSQL_PixelPinAccounts()
        {
            if (VIPGeneral.Window.VPMessageBox.ShowQuestion("從檔案載入表格，分割後將上傳 ?") == true)
            {
                string strUpdatePixelAcc = txtPixelPinUpdateTable.Text; 
                string[] strMyText = strUpdatePixelAcc.Split('\n');
                VIPGeneral.Window.VPProgressControl pdb = new VIPGeneral.Window.VPProgressControl();
                pdb.MaxValue = strMyText.Length;
                pdb.Start();
                foreach (string p in strMyText)
                {
                    pdb.Text = p;
                    if (p.Split('\t').Length >= 2)  //格式檢查
                    {
                        if (TixcraftSQL.FBAccountDatabase.IsExist(p) == false)
                        {
                            TixcraftSQL.FBAccountDatabase.Add(p, SQLBoolean.on, DateTime.Now.ToString());
                        }
                    }
                    pdb.Next();
                }
                pdb.Close();
            }
        }
        public void ReadPixelPinUsersBySQL( )
        { 
             List<FBStruct> lstAllUsers = TixcraftSQL.FBAccountDatabase.GetTotalAccount(); 
             List<string> lstResult = new List<string>();
             int icount = 1;
             foreach (FBStruct fb in lstAllUsers)
             {
                 lstResult.Add(icount.ToString("D3") + ","+fb.Account);
                 icount++;
             }
             g_PixelPinAddress = lstResult;
            lstPixelPinAddress.Items.Clear();
            foreach (string p in g_PixelPinAddress)
            {
                lstPixelPinAddress.Items.Add(p);
            }
            label9.Text = label9.Text + " 雲端帳號數 : " + lstAllUsers.Count.ToString();
        }

        public void ReadPixelPinUsers(string strPath)
        { 
            g_PixelPinAddress = ReadFileLines(strPath ); 
            lstPixelPinAddress.Items.Clear();
            foreach (string p in g_PixelPinAddress)
            {
                lstPixelPinAddress.Items.Add(p);
            }
        }

        public List<string> ReadFileLines(string strPath)
        {
            List<string> lstFile = new List<string>();
            try
            { // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                using (StreamReader sr = new StreamReader(strPath, Encoding.Default))     //小寫TXT
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        lstFile.Add(line); 
                    }
                }
            }
            catch (Exception ex)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(ex.Message);
            }
            return lstFile;
        }
        public void ControlEnable(Control ctr, bool bIsEnable)
        {
            ctr.Enabled = bIsEnable;
        }
        public void RefreshText(Control ctr, string msg)
        {
            ctr.Text = msg;
        }


        public void AddAccountIntoRecipe(ref TSRecipe users , string strAcc,string strPwd)
        {
            logindata us = new logindata();
            us.Address = strAcc;
            us.passwod = strPwd;
            users.USER_Infor.Add(us);
        }

        public void AddAccountIntoRecipe(ref TSRecipe users, string strAcc, string strPwd , string LoginMode)
        {
            logindata us = new logindata();
            us.Address = strAcc;
            us.passwod = strPwd;
            us.LoginMode = LoginMode;
            users.USER_Infor.Add(us);
        }
        private void btnLoadAcc_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("重存檔中讀取帳號 ?" ,"帳號確認" , MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                USERData.LoadRecipeFromXml(ref USERData);
                UserToListView(USERData , listBox1);
                this.Text = "讀取成功，目前有 " + USERData.USER_Infor.Count.ToString() + " 組 帳號可使用";
            }
        }

        private void btnSaveAcc_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("儲存該帳號列表 ?", "帳號確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                USERData.SaveRecipeToXml(USERData);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddAccountIntoRecipe(ref USERData, txtACC.Text, txtPWD.Text);
            UserToListView(USERData, listBox1);
        }

        List<Form1> lstFrms = new List<Form1>();
        private void btnOpenBrowsers_Click(object sender, EventArgs e)
        {
            lstFrms.Clear();
            bool bIsOpenBrowserWithGoogleChrome = chkBGoogleChrome.Checked;
            bool bIsRandSeats = ckRandSeats.Checked;
            if (lstShow.SelectedIndex < 0)
            {
                MessageBox.Show("你沒有選節目是要搶什麼?");
                return;
            }
            if (USERData.USER_Infor.Count <= 0)
            {
                MessageBox.Show("帳號數不夠，請點選新增！ 不然我不知道你到底要開幾個視窗。");
                return; 
            }
             

            //打驗證碼的倍率
            string strOCRServerIpAddress = txtIP.Text;
            Task t = Task.Factory.StartNew(()=> 
            {
                Parallel.For(0, USERData.USER_Infor.Count, (i) =>
                {
                    Form1 frm = new Form1();
                    frm.bIsOpenWithGoogleChrome = bIsOpenBrowserWithGoogleChrome;
                    frm.bIsMountProxy = g_bIsMountProxy;

                    string[] strSp = USERData.USER_Infor[i].Address.Split(',');
                    if (strSp.Length >= 2)
                    { 
                        if (USERData.USER_Infor[i].LoginMode == "PP")
                        {
                            frm.LoginMode = LoginType.PixelPin;
                        }
                        else if (USERData.USER_Infor[i].LoginMode == "G+")
                        {
                            frm.LoginMode = LoginType.Google;
                        }
                        frm.strFormatEmailInfo = USERData.USER_Infor[i].Address.Split(',')[1];  //此處目前格式 : gmail / pwd / backup email ----> 帶入 
                    } 
                    frm.strPinelPinPassword = USERData.USER_Infor[i].passwod;
                    frm.OCRRecipe_IP_Address = OCRRecipe.IP;
                    frm.lstShow = g_AllShow;
                    frm.g_ShowSelected = g_iSelectShowIndex;
                    frm.SetRandSeats(bIsRandSeats); 
                    lstFrms.Add(frm);
                });
            });
            t.Wait();

            foreach (Form1 p in lstFrms)
            {
                p.Show();
                Thread.Sleep(1500);
            }

            try
            {
                int iSWidth = Screen.PrimaryScreen.WorkingArea.Width;
                int iSHeigh = Screen.PrimaryScreen.WorkingArea.Height;

                int iBW = 0;
                int iBH = 0;
                int iCount_BX = 0;
                int iCount_BY = 0;

                if (lstFrms.Count > 0)
                {
                    iBW = lstFrms[0].Width;
                    iBH = lstFrms[0].Height;
                    iCount_BX = iSWidth / iBW;
                    iCount_BY = iSHeigh / iBH;
                }

                for (int y = 0; y < lstFrms.Count; y++)
                {
                    lstFrms[y].Left = iSWidth - iBW;
                    if (iCount_BY >= lstFrms.Count)
                        lstFrms[y].Top = iBH * y;
                } 
            }
            catch (Exception ex)
            {
                this.Text = "整理視窗有問題，不要亂按";
            }

            Chromes_Sort_Window();

            foreach (Form1 p in lstFrms)
            {
                p.btnShowTop_Click(null, null);
                //p.LoginPixelPin();
            }

            Chromes_Sort_Window();
        }

        public void UserToListView(TSRecipe users , ListBox lstview)
        {
            lstview.Items.Clear();
            for (int i = 0; i < users.USER_Infor.Count; i++)
            {
                string strMsg = string.Format("{0}=帳號{1}: {2}   .", users.USER_Infor[i].LoginMode, i, users.USER_Infor[i].Address); 
                lstview.Items.Add(strMsg);
            } 
        }

        private void btnDEL_Click(object sender, EventArgs e)
        {

            int index = listBox1.SelectedIndex;
            if (index < 0) return;
            if (USERData.USER_Infor == null) return;
            if (USERData.USER_Infor.Count < index) return;
            USERData.USER_Infor.RemoveAt(listBox1.SelectedIndex);
            UserToListView(USERData, listBox1);
        }

        public void RefreshListBox(ListBox lstBox, List<TixcraftSubscriber.Activity> act)
        {
            lstBox.Items.Clear();
            for (int i = 0; i < act.Count; i++)
            {
                lstBox.Items.Add(act[i].Name);
            }
        }

        private void btnGetShow_Click(object sender, EventArgs e)
        {
            ShowAllActivity();
        }

        public void ShowAllActivity()
        {
            //==============================主要功能區====================================
            //step1.先更新活動  Tixcraft.RefreshActivity();
            Tixcraft.RefreshActivity();
            //sep2.取得所有活動 Tixcraft.GetActivity(Index);

            //==============================記錄cookie=====================================
            Tixcraft.TixcraftWebDriver.Session = FastHttpDriver.Session;
            FastHttpDriver = Tixcraft.TixcraftWebDriver;
            //=============================================================================   
            g_AllShow.Clear();
            for (int i = 0; i < Tixcraft.ActivityCount; i++)
            {
                g_AllShow.Add(Tixcraft.GetActivity(i));
            }
            RefreshListBox(lstShow, g_AllShow);
            //this.Text = "";
            //nSelectMode = SelectMode.Activity;
        }

        private void lstShow_SelectedIndexChanged(object sender, EventArgs e)
        {
            g_iSelectShowIndex = lstShow.SelectedIndex;
            lblBuyInfo.Text = "你點選了第" + g_iSelectShowIndex + "項 :" + g_AllShow[g_iSelectShowIndex].Name;
        }

        private void btnChangeShow_Click(object sender, EventArgs e)
        {
            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    lstFrms[i].g_ShowSelected = g_iSelectShowIndex;
                    lstFrms[i].g_ShowSelected = g_iSelectShowIndex;
                    lstFrms[i].ChangeLabelMessage("切換至 : " + g_AllShow[g_iSelectShowIndex].Name);
                }
            }
        }
         
        public FastHttpWebDriver Login(string strFBAdd, string strFBPwd)
        {
            bool bIsLogin = false;
            mFacebook SelfFaceBook = new mFacebook(strFBAdd, strFBPwd);
            TixcraftSubscriber tmpSubscriber = new TixcraftSubscriber();
            FastHttpWebDriver tmpMiniBrowser = new FastHttpWebDriver();
            //mFacebook.UpdateStatus = UpdateFaceBookStatus;
            if (SelfFaceBook.Login())
            {
                //UpdateLable("帳號驗證中", lblLogin);
                if (TixcraftSQL.FBAccountDatabase.IsAllow(SelfFaceBook.email))
                {
                    tmpMiniBrowser = SelfFaceBook.miniFacebookBrowser;
                    //UpdateLable("等待傳送cookie...", lblLogin);
                    //傳送FB Cookie資料到 Tixcraft
                    if (tmpSubscriber.FacebookLogin(SelfFaceBook.miniFacebookBrowser.Session))
                    {
                        tmpMiniBrowser = tmpSubscriber.TixcraftWebDriver;
                        //UpdateLable("登入拓元成功!", lblLogin);
                        bIsLogin = true;
                    }
                    else
                    {
                        tmpMiniBrowser = tmpSubscriber.TixcraftWebDriver;
                        //UpdateLable("登入拓元失敗!", lblLogin);
                        bIsLogin = false;
                    }
                }
                else
                { 
                    if (TixcraftSQL.FBAccountDatabase.IsExist(SelfFaceBook.email))
                    {
                        TixcraftSQL.FBAccountDatabase.UpDate(SelfFaceBook.email, SQLBoolean.off);
                    }
                    else
                    {
                        TixcraftSQL.FBAccountDatabase.Add(SelfFaceBook.email, SQLBoolean.off, DateTime.Now.ToString());
                    }
                    //UpdateLable("不接受身分登入(無開通)!", lblLogin);
                    bIsLogin = false;
                }
            }
            else
            {
                bIsLogin = false;
            }
            if (bIsLogin)
            {
                return tmpMiniBrowser;
            }
            else
            {
                return null;
            }
        }

        private void btnAutoBuy_Click(object sender, EventArgs e)
        { 
            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    if (lstFrms[i].bIsBrowserBusying == false)
                    {
                        lstFrms[i].button3_Click(null, null);
                    }
                }
            }
        }


        private void ckRandSeats_CheckedChanged(object sender, EventArgs e)
        { 
            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    lstFrms[i].SetRandSeats(ckRandSeats.Checked);
                }
            }
        }

        private void btnSubmitCheckCode_Click(object sender, EventArgs e)
        {

            Task.Factory.StartNew(() =>
            {

                if (lstFrms != null)
                {
                    string strMsg = "";

                    this.Invoke(degControlEnable, btnSubmitCheckCode, false); 
                    Parallel.For(0, lstFrms.Count, i =>
                    {
                        try
                        {
                            strMsg = lstFrms[i].SendCheckCode(txtCheckCodeAnswer.Text);
                        }
                        catch (Exception ex)
                        {

                        }
                    }); // Parallel.For 
                    this.Invoke(degControlEnable, btnSubmitCheckCode, true); 
                    this.Invoke(degRefreshText, lblAnswerVeryCOde, strMsg); 
                    //for (int i = 0; i < lstFrms.Count; i++)
                    //{
                    //    lstFrms[i].button3_Click(null, null);
                    //}
                }
                else
                {
                    this.Invoke(degRefreshText, lblAnswerVeryCOde, "不要亂按"); 
                    //lblAnswerVeryCOde.Text = "不要亂按";
                } 
            });
        }

        private void btnSortForm_Click(object sender, EventArgs e)
        {
            
            try
            { 
                int iSWidth = Screen.PrimaryScreen.WorkingArea.Width;
                int iSHeigh = Screen.PrimaryScreen.WorkingArea.Height;

                int iBW = 0;
                int iBH = 0;
                int iCount_BX = 0;
                int iCount_BY = 0;

                if (lstFrms.Count > 0)
                {
                    iBW = lstFrms[0].Width;
                    iBH = lstFrms[0].Height;
                    iCount_BX = iSWidth / iBW;
                    iCount_BY = iSHeigh / iBH;
                }

                for (int y = 0; y < lstFrms.Count; y++)
                {
                    lstFrms[y].Left = iSWidth- iBW;
                    if(iCount_BY>= lstFrms.Count)
                    lstFrms[y].Top = iBH* y;
                }

                //int iOffsetX = iSWidth - iCount_BX * iBW ;

                //int iFrmIndex = 0;
                //for (int x = iCount_BX; x > 0; x--)
                //{
                //    int iTargetX = iBW * x - iOffsetX;
                //    for (int y = 0; y < iCount_BY; y++)
                //    {
                //        int iTargetY = iBH * y;
                //        if (0 <= iFrmIndex && iFrmIndex < lstFrms.Count && lstFrms.Count > 0)
                //        {
                //            lstFrms[iFrmIndex].Left = iTargetX;
                //            lstFrms[iFrmIndex].Top = iTargetY;
                //        }
                //        iFrmIndex++;
                //    }
                //}
            }
            catch (Exception ex)
            {
                this.Text = "整理視窗有問題，不要亂按";
            }

            #region == 整理網頁瀏覽器位置 ==
            Chromes_Sort_Window();
            #endregion

        }

        private void SubscriberLoginer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (lstFrms != null)
            {
                Parallel.For(0, lstFrms.Count, i =>
                {
                    try
                    {
                        lstFrms[i].CloseBrowser();
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }

        private void btnShareAnswer_Click(object sender, EventArgs e)
        {
            if (txtCheckCodeAnswer.Text != "")
            {
                lblAnswerVeryCOde.Text = "分享中...!";

                if (g_AnswerServer != "")
                {
                    TixcraftSQL.ShareAnswerDatabase.Add(g_AnswerServer + "," + txtCheckCodeAnswer.Text, DateTime.Now.ToString());
                }
                else 
                { 
                    TixcraftSQL.ShareAnswerDatabase.Add(txtCheckCodeAnswer.Text, DateTime.Now.ToString());
                }

                //g_ChatRm.SendMessage("answer", txtCheckCodeAnswer.Text); 
                lblAnswerVeryCOde.Text = "分享成功!..." + txtCheckCodeAnswer.Text;
                txtCheckCodeAnswer.Text = "";
            }
            else
            { 
                lblAnswerVeryCOde.Text = "沒有答案按三小!";
            }
        }

        private void btnDownLoadAnswer_Click(object sender, EventArgs e)
        {
            List<SAStruct> lstTmpAllList = TixcraftSQL.ShareAnswerDatabase.GetTotalAccount();

            //從文字過濾
            List<SAStruct> lstFilterList = new List<SAStruct>();
            for (int i = 0; i < lstTmpAllList.Count; i++)
            {
                if (lstTmpAllList[i].MAnswer.Contains(g_AnswerServer))
                {
                    lstFilterList.Add(lstTmpAllList[i]);
                }
            }



            if (lstFilterList.Count > 0)
            {
                string strNewestAnswer = "";
                if (lstFilterList.Count > 0)
                {
                    string[] spG = lstFilterList[lstFilterList.Count - 1].MAnswer.Split(',');
                    if (spG.Length > 1)
                    {
                        strNewestAnswer = spG[1];
                    }
                    else
                    {
                        strNewestAnswer = "";
                    } 
                }
                else
                {
                    strNewestAnswer = ""; 
                }

                txtCheckCodeAnswer.Text = strNewestAnswer; 
                //txtCheckCodeAnswer.Text = lstTmpAllList[lstTmpAllList.Count - 1].MAnswer; 
                lblAnswerVeryCOde.Text = "下載成功!(" + lstFilterList[lstFilterList.Count - 1].mDateTime + ")";
            }
            else
            {
                txtCheckCodeAnswer.Text = "";
                lblAnswerVeryCOde.Text = "沒有答案...";
            }

        }

        private void btnClearAnswers_Click(object sender, EventArgs e)
        {
            TixcraftSQL.ShareAnswerDatabase.Clear("54088");
            lblAnswerVeryCOde.Text = "已清空雲端上所有答案...";
        }


         

        private void rdbTicket_All_CheckedChanged(object sender, EventArgs e)
        {

        }



        private void SubscriberLoginer_Activated(object sender, EventArgs e)
        {
            
        }

        private void SubscriberLoginer_Validated(object sender, EventArgs e)
        {
            ;
        }

        private void SubscriberLoginer_Layout(object sender, LayoutEventArgs e)
        {
            ;
        }

        private void pnlControlTable_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnSaveIP_Click(object sender, EventArgs e)
        {
            OCRRecipe.IP = txtIP.Text;
            OCRRecipe.SaveRecipeToXml(OCRRecipe);






            //#region Server端讀Cookies

            //string strUserEmail = "a7860710@yahoo.com.tw" + "^" + "2xliriri"; // 20171230 SQL上伺服器EMAIL更新為 EMAIL^PWD ， 多一個^來串接帳號密碼

            //Subscriber myGoogle = new Subscriber();
            //myGoogle.OpenBrowser(true);
            //myGoogle.GoTo("https://tixcraft.com/activity"); 
            //if (this.CookieServer.IsExist(strUserEmail))
            //{
            //    List<CookieStruct> FBusers = this.CookieServer.GetTotalCookieHistory();
            //    CookieStruct MatchUsers = null;
            //    for (int ifx = 0; ifx < FBusers.Count; ifx++)
            //    {
            //        if (FBusers[ifx].Email == strUserEmail)
            //        {
            //            MatchUsers = FBusers[ifx];
            //            break;
            //        }
            //    }
            //    if (MatchUsers != null)
            //    {
            //        //.....將字串型式Cookie轉回來~ 
            //        //格式 : Domain;Url;Name;Value  為一包
            //        //整串格式 Domain;Url;Name;Value^Domain;Url;Name;Value^Domain;Url;Name;Value^Domain;Url;Name;Value

            //        List<Cookie> lstHistoryCookie = new List<Cookie>();

            //        string ServerEmail = MatchUsers.Email.Split('^')[0];
            //        string ServerCookie = MatchUsers.CookiePkg;
            //        string ServerDateTime = MatchUsers.mDateTime;

            //        string[] CookiesPackage = MatchUsers.CookiePkg.Split('^');
            //        for (int iCkIndex = 0; iCkIndex < CookiesPackage.Length; iCkIndex++)
            //        {
            //            // 0 => Domain
            //            // 1 => Url
            //            // 2 => Name
            //            // 3 => Value
            //            string[] strTemp = CookiesPackage[iCkIndex].Split(';');
            //            if (strTemp.Length == 4)
            //            {

            //                myGoogle.Driver.Manage().Cookies.AddCookie(new OpenQA.Selenium.Cookie(strTemp[2], strTemp[3]));

            //            }
            //        }
            //        myGoogle.Driver.Navigate().Refresh();
            //    }
            //}
            //#endregion


        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void txtCheckCodeAnswer_KeyPress(object sender, KeyPressEventArgs e)
        {  
        }

        private void txtCheckCodeAnswer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnShareAnswer_Click(null, null);
            }
        }


        /// <summary>
        /// 視窗 ==> 整理視窗
        /// </summary>
        public void Chromes_Sort_Window()
        {
            //== 螢幕工作區大小 ==
            int iSWidth = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            int iSHeigh = Screen.PrimaryScreen.WorkingArea.Height;

            int iBlock_W = 300;
            int iBlock_H = 1036;

            int iList_W = iSWidth / iBlock_W;
            int iList_H = iSHeigh / 150;

            int iput_count = 0;
            for (int iy = 0; iy < iList_H; iy++)
            {
                for (int ix = 0; ix <= iList_W; ix++)
                {
                     
                    if (iput_count < lstFrms.Count)
                    {
                        lstFrms[iput_count].SubscrEr.HwndController.ShowNormal();
                        lstFrms[iput_count].SubscrEr.HwndController.SetToForegroundWindow();
                        lstFrms[iput_count].SubscrEr.HwndController.ReSize(ix * (iBlock_W+80), iy * 150, iBlock_W, iBlock_H);
                        iput_count++;
                    }
                }
            }
            //Chromes_Hide_CommandWindow();
        }
        /// <summary>
        /// 隱藏所有小黑窗
        /// </summary>
        public void Chromes_Hide_CommandWindow()
        {
            if (g_ChromeDrives == null) return;
            for (int i = 0; i < this.g_ChromeDrives.Length; i++)
            {
                WindowController temp = new TixWin.WindowController();
                temp.hwnd = g_ChromeDrives[i].MainWindowHandle;
                temp.Hide();
            }
        }
        public Process[] GetChromeProcesses()
        {
            List<Process> pResult = new List<Process>();
            this.g_ChromeDrives = Process.GetProcesses();
            foreach (Process p in this.g_ChromeDrives)
            {
                if (p.MainWindowTitle.Contains("chrome"))
                    pResult.Add(p);
            }
            return pResult.ToArray();
        }

        private void btnStop_Buy_Click(object sender, EventArgs e)
        {

            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    if (lstFrms[i].bIsBrowserBusying == true)
                    {
                        lstFrms[i].btnStopBuy_Click(null, null);
                    }
                }
            }
        }

        private void btn_ShowLog_Click(object sender, EventArgs e)
        {
            VPState.ShowViewer();
        }

        private void tpShow_Click(object sender, EventArgs e)
        {

        }

        private void btnGDrive_Account_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetData(DataFormats.Text, "https://docs.google.com/spreadsheets/d/1Q1UOCkQqfr8mWPrwCjq3JZcb_jisg89On_038ij2ByM/edit?pli=1#gid=666626623");
                VIPGeneral.Window.VPMessageBox.ShowInfo("複製到剪貼簿上成功，請使用Ctrl + V貼上");
            }
            catch (Exception)
            {

            }
        }

        private void btnGDrive_ProgramDownload_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetData(DataFormats.Text, "https://drive.google.com/drive/folders/0B3XN0iE0HNeqVjhnaHJBX3Y1Qnc");
                VIPGeneral.Window.VPMessageBox.ShowInfo("複製到剪貼簿上成功，請使用Ctrl + V貼上");
            }
            catch (Exception)
            {

            }
        }

        private void btnSnapShotAll_Click(object sender, EventArgs e)
        {
            bool bIsSaved = false;

            VIPGeneral.Window.VPProgressControl pdb = new VIPGeneral.Window.VPProgressControl();
            pdb.MaxValue = lstFrms.Count-1;
            pdb.Start();
            for (int y = 0; y < lstFrms.Count; y++)
            {
                try
                {

                    lstFrms[y].SubscrEr.HwndController.MaxImized();
                    Thread.Sleep(100);
                    lstFrms[y].SubscrEr.ScreenShot();
                    Thread.Sleep(100);
                    pdb.Next(1);
                    Bitmap myScreenShot = new Bitmap(lstFrms[y].SubscrEr.bSnapShot);
                    if (VIPGeneral.IO.VPFile.ExistsFolder("BuyTicket_Image") == false)
                    {
                        VIPGeneral.IO.VPFile.CreateFolder("BuyTicket_Image");
                    }
                    myScreenShot.Save("BuyTicket_Image" + "\\" + VIPGeneral.Tool.VPWinApi.ComputerName + "__" + y.ToString() + ".bmp");
                }
                catch (Exception)
                {

                }
                bIsSaved = true;
            }
            pdb.Close();
            if (bIsSaved == true)
            {
                VIPGeneral.Window.VPMessageBox.ShowInfo("儲存成功，請進入資料夾內參考圖檔");
                VIPGeneral.IO.VPFile.OpenPath("BuyTicket_Image");
            }
        }

        private void btn_Upload_Image_Click(object sender, EventArgs e)
        { 
            try
            {
                Clipboard.SetData(DataFormats.Text, "https://drive.google.com/drive/folders/1rML-0MygoUsPteVIkSAfH11kHhI3LU38");
                VIPGeneral.Window.VPMessageBox.ShowInfo("複製到剪貼簿上成功，請使用Ctrl + V貼上");
            }
            catch (Exception)
            {

            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void btnSettingAllForm_Click(object sender, EventArgs e)
        {
            bool bIsSetting = false; 
            for (int y = 0; y < lstFrms.Count; y++)
            {
                try
                { 
                    //搶票參數設定
                    lstFrms[y].Set_BuyDataInformation(txtShowTime.Text, txtTicketCount.Text, txtSeatInformation.Text);
                    //額外延遲設定 ( 驗證碼互換時 )
                    int iDelayYW0 = 80; int.TryParse(txtYW0Delay.Text, out iDelayYW0);
                    lstFrms[y].Set_YW0_MouseClickDelayTime(iDelayYW0);
                    bIsSetting = true;
                }
                catch (Exception)
                {

                } 
            }
            if (bIsSetting)
            {
                VIPGeneral.Window.VPMessageBox.ShowInfo("全窗設定完畢！");
            }
        }

        private void btnRunAllWindow_Click(object sender, EventArgs e)
        {
            if (VIPGeneral.Window.VPMessageBox.ShowQuestion("確定要開啟總開關 , 使所有已上線機台開搶 ?") == true)
            { 
                CookieServer.EnableAutoFlag(true);
            }
            ShowAutoFlag();
        }

        private void btnRunAllWindowStop_Click(object sender, EventArgs e)
        {
            CookieServer.EnableAutoFlag(false);
            ShowAutoFlag();
        }

        private void btnRefreshAutoFlag_Click(object sender, EventArgs e)
        {
            ShowAutoFlag();
        }
        public void ShowAutoFlag()
        {
            if (CookieServer.GetAutoFlag())
            {
                lblMsgAutoFlag.Text = "已開啟";
            }
            else
            {
                lblMsgAutoFlag.Text = "關閉中";
            }
        }

        private void lstPixelPinAddress_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string strLoginMode = "PP";
            if (rdoLoaginPP.Checked)
            {
                strLoginMode = "PP";
            }
            else 
            {
                strLoginMode = "G+";
            }
            AddAccountIntoRecipe(ref USERData, lstPixelPinAddress.Items[lstPixelPinAddress.SelectedIndex].ToString(), txtPWD.Text, strLoginMode);
            UserToListView(USERData, listBox1); 
        }
         

        private void btnAutoFillCreditCard_Click(object sender, EventArgs e)
        {
            if (txtCredit_Card_Account.Text == "") { lblCreditCard.Text = "信用卡卡號不能空白"; return; }
            if (txtCredit_Card_Month.Text == "") { lblCreditCard.Text = "月份不能空白"; return; }
            if (txtCredit_Card_Year.Text == "") { lblCreditCard.Text = "年份不能空白"; return; }
            if (txtCredit_Card_CVE.Text == "") { lblCreditCard.Text = "卡片檢查碼不能空白"; return; }

            for (int y = 0; y < lstFrms.Count; y++)
            {
                try
                {
                    //搶票參數設定
                    lstFrms[y].Set_CrediteCardInformation(txtCredit_Card_Account.Text , txtCredit_Card_Month.Text , txtCredit_Card_Year.Text , txtCredit_Card_CVE.Text); 
                }
                catch (Exception)
                {

                }
            } 
        }

        private void btnAutoFillup_AllWindow_Click(object sender, EventArgs e)
        { 
            for (int y = 0; y < lstFrms.Count; y++)
            {
                try
                {
                    //搶票參數設定
                    lstFrms[y].btnAutoFillCreditCard_Click(null, null);
                }
                catch (Exception)
                {

                }
            } 
        }

        private void ckbSwitchPageStepByStep_CheckedChanged(object sender, EventArgs e)
        { 
            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    lstFrms[i].SetSwitchStepByStep(ckbSwitchPageStepByStep.Checked);
                }
            }
        }

        private void lstPixelPinAddress_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnUpDatePixelPinAccount_Click_1(object sender, EventArgs e)
        { 
            UpdateToSQL_PixelPinAccounts();
        }

        private void chkBGoogleChrome_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ckbProxy_CheckedChanged(object sender, EventArgs e)
        {
            g_bIsMountProxy = ckbProxy.Checked;
        }

        private void rd_Answer01_CheckedChanged(object sender, EventArgs e)
        {
            g_AnswerServer = Server_A;
            SetInServerFilterText(g_AnswerServer);

        }

        private void rd_Answer02_CheckedChanged(object sender, EventArgs e)
        {
            g_AnswerServer = Server_B;
            SetInServerFilterText(g_AnswerServer);
        }

        public void SetInServerFilterText(string strCoreText)
        {
            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    lstFrms[i].g_AnswerSwitchText = strCoreText;
                }
            }
        }

        private void btnAutoFillCredidCard_Click(object sender, EventArgs e)
        { 
            if (txtCredit_Card_Account_BK.Text == "") { lblCreditCard.Text = "信用卡卡號不能空白"; return; }
            if (txtCredit_Card_Month_BK.Text == "") { lblCreditCard.Text = "月份不能空白"; return; }
            if (txtCredit_Card_Year_BK.Text == "") { lblCreditCard.Text = "年份不能空白"; return; }
            if (txtCredit_Card_CVE_BK.Text == "") { lblCreditCard.Text = "卡片檢查碼不能空白"; return; }

            for (int y = 0; y < lstFrms.Count; y++)
            {
                try
                {
                    //搶票參數設定
                    lstFrms[y].Set_CrediteCardInformation(txtCredit_Card_Account_BK.Text, txtCredit_Card_Month_BK.Text, txtCredit_Card_Year_BK.Text, txtCredit_Card_CVE_BK.Text);
                }
                catch (Exception)
                {

                }
            } 
        }

        private void txtCredit_Card_Year_TextChanged(object sender, EventArgs e)
        {
            if (txtCredit_Card_Year.Text.Length == 2)
            {
                txtCredit_Card_Year.Text = "20" + txtCredit_Card_Year.Text;
            }
        }

        private void txtCredit_Card_Year_BK_TextChanged(object sender, EventArgs e)
        { 
            if (txtCredit_Card_Year_BK.Text.Length == 2)
            {
                txtCredit_Card_Year_BK.Text = "20" + txtCredit_Card_Year_BK.Text;
            }
        }

        private void tabPage8_Click(object sender, EventArgs e)
        {

        }

        private void rd_PayNone_CheckedChanged(object sender, EventArgs e)
        {

            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    lstFrms[i].PayMode = -1;
                }
            }
        }

        private void rd_PayATM_CheckedChanged(object sender, EventArgs e)
        {

            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    lstFrms[i].PayMode = 0;
                }
            }
        }

        private void rd_PayIBON_CheckedChanged(object sender, EventArgs e)
        {

            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    lstFrms[i].PayMode = 1;
                }
            }
        }

        private void rd_PayCreditCard_CheckedChanged(object sender, EventArgs e)
        { 
            if (lstFrms != null)
            {
                for (int i = 0; i < lstFrms.Count; i++)
                {
                    lstFrms[i].PayMode = 2;
                }
            }
        }
    }
}
