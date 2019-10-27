using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Tixcraft_Subscriber
{
    public partial class TixQuestionAI : Form
    {
        public TixQuestionAI()
        {
            InitializeComponent();
        }
        public int g_ItemSelect = 0;
        public List<string> g_SelectFileText = new List<string>();

        public TixQuestionBot g_tQBot = new TixQuestionBot();

        private void TixQuestionAI_Load(object sender, EventArgs e)
        {
            List<string> lstFiles = VIPGeneral.IO.VPFile.GetFilesPathList("拓元問答\\", "*.txt");
            foreach (string p in lstFiles)
            {
                listBox1.Items.Add(p);
            } 
        } 
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSplitOption.Text = "";
            if (listBox1.SelectedIndex != -1)
            {
                g_ItemSelect = listBox1.SelectedIndex;
            }
            if (listBox1.SelectedIndex != -1 && listBox1.Items.Count > 0)
            {
                //讀取考試問題
                ReadQuestion(listBox1.Items[g_ItemSelect].ToString());
                //分析選項 
                string strQuestionType = "尚未分析";
                List<string> lstAnswerResult = g_tQBot.GetOptions_Answers(g_SelectFileText, ref strQuestionType);

                //lstAnswerResult = g_tQBot.RandomSortList(lstAnswerResult);

                string strSystemText = strQuestionType + Environment.NewLine + "排列組合數量 : " + lstAnswerResult.Count + " 種";
                for (int i = 0; i < lstAnswerResult.Count; i++)
                {
                    strSystemText += Environment.NewLine + lstAnswerResult[i];
                }
                txtSystemInfo.Text = strSystemText;
            }

        }

        public void ReadQuestion(string strFilePath)
        {
           string[] sMessage =  VIPGeneral.IO.VPFile.ReadFile(strFilePath , VIPGeneral.IO.VPFile.eVPEncode.UTF8);
           List<string> lstMessage = sMessage.ToList();
           g_SelectFileText = lstMessage;
           string strReturnMessage = "";
           for (int i = 0; i < lstMessage.Count; i++)
           {
               strReturnMessage = strReturnMessage + Environment.NewLine + lstMessage[i];
           } 
           txtQuestion.Text = strReturnMessage;
        }
         

    }
}
