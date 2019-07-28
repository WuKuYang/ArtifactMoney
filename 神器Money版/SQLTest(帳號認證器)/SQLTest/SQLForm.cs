using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLTest
{
    public partial class SQLForm : Form
    {
        public SQLForm()
        {
            InitializeComponent();
        }

        List<HDDStruct> g_lstAllCPU = null;
        List<FBStruct> g_lstAllFB = null;
        List<CookieStruct> g_lstAllCookie = null;

        private void SQLForm_Load(object sender, EventArgs e)
        {
            TixcraftSQL.ConnectToSQL();
            btnScanAll_Click(null, null);
            btnScanAllCPUid_Click(null, null);
        }

        private void btnDel_Click(object sender, EventArgs e)
        { 
            string strFBAccount = txtFBAccount.Text;
            TixcraftSQL.FBAccountDatabase.Del(strFBAccount);
            btnScanAll_Click(null, null);
        }

        private void btnInsertInto_Click(object sender, EventArgs e)
        {
            string strFBAccount = txtFBAccount.Text; 
            SQLBoolean tmpAllow = SQLBoolean.off;
            if (chkAllow.Checked)
            {
                tmpAllow = SQLBoolean.on;
            }
            else 
            { 
                tmpAllow = SQLBoolean.off;
            }
            if (TixcraftSQL.FBAccountDatabase.IsExist(strFBAccount))
            {
                TixcraftSQL.FBAccountDatabase.UpDate(strFBAccount, tmpAllow);
            }
            else
            {
                //TixcraftSQL.FBAccountDatabase.Add(strFBAccount, tmpAllow, DateTime.Now.ToString("yyyy/MM/dd"));
                TixcraftSQL.FBAccountDatabase.Add(strFBAccount, tmpAllow, DateTime.Now.ToString()); 
            }
            btnScanAll_Click(null, null);
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            string strFBAccount = txtFBAccount.Text;
            if (TixcraftSQL.FBAccountDatabase.IsAllow(strFBAccount))
            {
                MessageBox.Show(strFBAccount + "\n可以使用");
            }
            else 
            { 
                MessageBox.Show(strFBAccount + "\n無法使用");
            }
        }

        private void btnScanAll_Click(object sender, EventArgs e)
        {
            g_lstAllFB = TixcraftSQL.FBAccountDatabase.GetTotalAccount();
            lstFBAccount.Items.Clear();
            for (int i = 0; i < g_lstAllFB.Count; i++)
            {
                string strAcc = string.Format("帳號:{0}\t\t使用狀態:{1}\t\t開通時間:{2}", g_lstAllFB[i].Account, g_lstAllFB[i].OnOff, g_lstAllFB[i].mDateTime);
                lstFBAccount.Items.Add(strAcc);
            }
        }

        private void btnInserInToCPUid_Click(object sender, EventArgs e)
        {
            string strCPUid = txtCPUid.Text;
            SQLBoolean tmpAllow = SQLBoolean.off;
            if (chkCPUid.Checked)
            {
                tmpAllow = SQLBoolean.on;
            }
            else
            {
                tmpAllow = SQLBoolean.off;
            }
            if (TixcraftSQL.HDDDatabase.IsExist(strCPUid))
            {
                TixcraftSQL.HDDDatabase.UpDate(strCPUid, tmpAllow);
            }
            else
            {
                TixcraftSQL.HDDDatabase.Add(strCPUid, tmpAllow, DateTime.Now.ToString());
            }
            btnScanAllCPUid_Click(null, null);
        }

        private void btnDelCPUid_Click(object sender, EventArgs e)
        {
                string strCPUid = txtCPUid.Text;
                TixcraftSQL.HDDDatabase.Del(strCPUid);
                btnScanAllCPUid_Click(null, null);
        }

        private void btnCheckCPUid_Click(object sender, EventArgs e)
        {
            string strCPUid = txtCPUid.Text;
            if (TixcraftSQL.HDDDatabase.IsAllow(strCPUid))
            {
                MessageBox.Show(strCPUid + "\n可以使用");
            }
            else
            {
                MessageBox.Show(strCPUid + "\n無法使用");
            }
        }

        private void btnScanAllCPUid_Click(object sender, EventArgs e)
        {
            g_lstAllCPU = TixcraftSQL.HDDDatabase.GetTotalAccount();
            lstCPUid.Items.Clear();
            for (int i = 0; i < g_lstAllCPU.Count; i++)
            {
                string strAcc = string.Format("CPUid:{0}\t\t\t使用狀態:{1}\t\t\t開通時間:{2}", g_lstAllCPU[i].CPUid, g_lstAllCPU[i].OnOff, g_lstAllCPU[i].mDateTime);
                lstCPUid.Items.Add(strAcc);
            }
        }

        private void btnGetHDDNum_Click(object sender, EventArgs e)
        {
            txtCPUid.Text = HDDInformation.GetDevice();
        }

        private void lstCPUid_SelectedIndexChanged(object sender, EventArgs e)
        {
            int indexofID = lstCPUid.SelectedIndex;
            if (indexofID >= 0)
            {
                txtCPUid.Text = g_lstAllCPU[indexofID].CPUid;
            } 
        }

        private void lstFBAccount_SelectedIndexChanged(object sender, EventArgs e)
        { 
            int indexofID = lstFBAccount.SelectedIndex;
            if (indexofID >= 0)
            {
                txtFBAccount.Text = g_lstAllFB[indexofID].Account;
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //新增Cookie資料包
            string strEmail = txtEmail.Text;
            string strCookiePkg = txtCookie.Text;
            string strTime = txtTime.Text;
            if (TixcraftSQL.CookieDatabase.IsExist(strEmail))
            {
                TixcraftSQL.CookieDatabase.UpDate(strEmail, strCookiePkg);
            }
            else
            {
                TixcraftSQL.CookieDatabase.Add(strEmail, strCookiePkg, DateTime.Now.ToString());
            }
            btnRefreshCookies_Click(null, null);
        }

        private void btnRefreshCookies_Click(object sender, EventArgs e)
        {
            g_lstAllCookie = TixcraftSQL.CookieDatabase.GetTotalCookieHistory();
            lstCookieServer.Items.Clear();
            for (int i = 0; i < g_lstAllCookie.Count; i++)
            {
                string strAcc = string.Format("email:{0}\t\t\t時間:{1}\t\t\t內容:{2}", g_lstAllCookie[i].Email, g_lstAllCookie[i].mDateTime, g_lstAllCookie[i].CookiePkg);
                lstCookieServer.Items.Add(strAcc);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //刪除Cookie資料包 
            string strEmail = txtEmail.Text;
            TixcraftSQL.CookieDatabase.Del(strEmail);
            btnRefreshCookies_Click(null, null);
        }

        private void lstCookieServer_SelectedIndexChanged(object sender, EventArgs e)
        { 
            int indexofID = lstCookieServer.SelectedIndex;
            if (indexofID >= 0)
            {
                txtEmail.Text = g_lstAllCookie[indexofID].Email;
                txtTime.Text = g_lstAllCookie[indexofID].mDateTime;
                txtCookie.Text = g_lstAllCookie[indexofID].CookiePkg;
            } 
        }

        private void btnClearFB_Click(object sender, EventArgs e)
        {
            string strFBAccount = txtFBAccount.Text;
            TixcraftSQL.FBAccountDatabase.DelAllAccount("", strFBAccount);
            btnScanAll_Click(null, null);
        }

        private void lstFBAccount_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
