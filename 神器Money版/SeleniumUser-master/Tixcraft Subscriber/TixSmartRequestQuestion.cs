using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Tixcraft_Subscriber
{
    public class TixListOptions
    { 
        private string[] m_Data = { "A", "B", "C", "D", "E" };
        public List<string> lstResult = new List<string>();
        public void GetAllSorting(List<string> strDatas)
        { 
            this.m_Data = strDatas.ToArray();
             
            lstResult.Clear();
            for (int i = 0; i < m_Data.Length; i++)
            {
                lstResult.Add(m_Data[i]);
            }
            Dictionary<string, int> dic = new Dictionary<string, int>();
            for (int i = 0; i < m_Data.Length; i++)
            {
                //Console.WriteLine(m_Data[i]);//如果不需要打印单元素的组合，将此句注释掉
                dic.Add(m_Data[i], i);
            }
            GetString(dic);
            //Console.ReadLine();  
        }
        private void GetString(Dictionary<string, int> dd)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> kv in dd)
            {
                for (int i = kv.Value + 1; i < m_Data.Length; i++)
                {
                    Console.WriteLine(kv.Key + m_Data[i]);
                    lstResult.Add(kv.Key + m_Data[i]);
                    dic.Add(kv.Key + m_Data[i], i);
                }
            }
            if (dic.Count > 0) GetString(dic);
        } 
    }

    public class TixQuestionBot
    {

        /// <summary>
        /// 所有排列組合可能
        /// </summary>
        public List<string> Answers_Resoult = new List<string>();

        /// <summary>
        /// 所有的選項
        /// </summary>
        public List<List<string>> Oprions_Result = new List<List<string>>();
        /// <summary>
        /// 是否有新的回答？
        /// </summary>
        public bool bIsHaveNewAnswer = false;
        /// <summary>
        /// 目前答案回答到第幾個
        /// </summary>
        public int g_GetAnswerIndex = 0;
        /// <summary>
        /// 目前回答到的答案是 .... 
        /// </summary>
        public string Current_Answer = "";
        /// <summary>
        /// 從選項池中取出一個答案，每呼叫一次就自動增加一次
        /// </summary>
        /// <returns></returns>
        public string Get_a_AnswerFromOptionPool()
        {
            string strReturnAnswer = "";
            if (Answers_Resoult != null)
            {
                if (Answers_Resoult.Count > 0 && g_GetAnswerIndex < Answers_Resoult.Count)
                {
                    strReturnAnswer = Answers_Resoult[g_GetAnswerIndex];
                    g_GetAnswerIndex++;
                } 
                if (g_GetAnswerIndex >= Answers_Resoult.Count)
                { 
                    bIsHaveNewAnswer = false; 
                }
            }
            else 
            {
                bIsHaveNewAnswer = false;
            }
            Current_Answer = strReturnAnswer;
            return strReturnAnswer;

        }

        /// <summary>
        /// 判斷題目是否為排序題型
        /// </summary>
        /// <param name="lstQuestionText"></param>
        /// <returns></returns>
        private bool CheckQIsSorting(List<string> lstQuestionText)
        {

            bool bIsSorting = false;
            foreach (string p in lstQuestionText)
            {
                if (p.Contains("排序") || p.Contains("排列") || p.Contains("順序"))
                {
                    bIsSorting = true;
                }
            }
            return bIsSorting;
        }

        /// <summary>
        /// 切割選擇題選項出來
        /// </summary>
        /// <param name="lstNeedSplitOptions">丟入要分割的選項，不能包含題目；問題的結尾說明，只能丟選項</param>
        /// <returns></returns>
        private List<string> Options_Split_From_Quest(List<string> lstSrc)
        {
            List<string> lstResoultOptions = new List<string>();

            List<string> lstNeedSplitOptions = new List<string>();
            //過濾每行如果字元太少，就有機率不是選項
            for (int i = 0; i < lstSrc.Count; i++)
            {
                if (lstSrc[i].Length > 2)
                {
                    lstNeedSplitOptions.Add(lstSrc[i]);
                }
            }
            // 0 = 空白
            // 1 = .  (半形小數點)
            // 2 = )  (半形右側括弧)
            // 3 = -  (短槓)
            // 4 = ） (全形右側括弧)
            int[] iDotArry = { 0, 0, 0, 0, 0 };
            for (int i = 0; i < lstNeedSplitOptions.Count; i++)
            {
                if (lstNeedSplitOptions[i].Contains(" ")) { iDotArry[0]++; }
                if (lstNeedSplitOptions[i].Contains(".")) { iDotArry[1]++; }
                if (lstNeedSplitOptions[i].Contains(")")) { iDotArry[2]++; }
                if (lstNeedSplitOptions[i].Contains("-")) { iDotArry[3]++; }
                if (lstNeedSplitOptions[i].Contains("）")) { iDotArry[4]++; }
            }

            if (lstNeedSplitOptions.Count != iDotArry.Max())
            {
                lstResoultOptions = lstNeedSplitOptions;
                return lstResoultOptions;
            }

            char strRealMeanText = '#';    //有意義的斷句，此部分如果此有意義，左側應該就是選項
            /*
            1A 歌舞青春
            2B 鐘樓怪人
            3C 開膛手傑克
            4D 伊莉莎白
            5E 三劍客
             */
            for (int i = 0; i < iDotArry.Length; i++)
            {
                //如果字符出現兩次以上，而且是統計完後最大值(代表出現次數最多,那就當他有意義吧！)
                if (iDotArry[i] >= 2 && iDotArry[i] == iDotArry.Max())
                {
                    if (i == 0) strRealMeanText = ' ';
                    if (i == 1) strRealMeanText = '.';
                    if (i == 2) strRealMeanText = ')';
                    if (i == 3) strRealMeanText = '-';
                    if (i == 4) strRealMeanText = '）';
                    break;
                }
            }

            //開始進行切割
            if (strRealMeanText != '#')
            {
                for (int i = 0; i < lstNeedSplitOptions.Count; i++)
                {
                    string[] sp = lstNeedSplitOptions[i].Split(strRealMeanText);
                    string s = sp[0];
                    string r = Regex.Replace(s, @"[\W_]+", ""); //濾除所有特殊符號

                    int iTextLength_Min = 0;
                    int iTextLength_Max = 20;
                    if (s.Length > iTextLength_Min && s.Length < iTextLength_Max)
                    {
                        lstResoultOptions.Add(r);
                    }
                }
            }

            return lstResoultOptions;
        }

        /// <summary>
        /// 取得暴力破解的字典 (列出所有可能性)
        /// </summary>
        /// <param name="lstQuestionText"></param>
        /// <returns>(所有結果)</returns>
        public List<string> GetOptions_Answers(List<string> lstQuestionText, ref string strQuestionType)
        {
            g_GetAnswerIndex = 0;
            List<string> lstQuestResult = new List<string>(); 
            List<List<string>> lstOptions = new List<List<string>>();

            //判斷是否為排序類型題目
            if (CheckQIsSorting(lstQuestionText) == true)
            {
                //"排序題";

                lstOptions = GetSelectQuestion_Options(lstQuestionText, ref strQuestionType);
                strQuestionType = "排序題 .." + strQuestionType;

                if (lstOptions.Count == 1)
                {
                    TixListOptions tix = new TixListOptions(); 
                    tix.GetAllSorting(lstOptions[0]);
                    lstQuestResult = tix.lstResult;
                }
            }
            else
            {
                //選擇題
                lstOptions = GetSelectQuestion_Options(lstQuestionText, ref strQuestionType);

                #region 選擇題的暴力破解方法，全題排列組合可能性列出
                int iQuestionCount = lstOptions.Count;              //題目數量
                List<int> lstOptionsCount = new List<int>();        //選擇題數量
                for (int i = 0; i < iQuestionCount; i++)
                {
                    lstOptionsCount.Add(lstOptions[i].Count);
                }

                if (iQuestionCount == 1)
                {
                    for (int i = 0; i < lstOptions[0].Count; i++)
                    {
                        lstQuestResult.Add(lstOptions[0][i]);
                    }
                }
                else if (iQuestionCount == 2)
                {

                    for (int i = 0; i < lstOptions[0].Count; i++)   // for a
                    {
                        for (int x = 0; x < lstOptions[1].Count; x++)   // for b
                        {
                            lstQuestResult.Add(lstOptions[0][i] + lstOptions[1][x]);
                        }
                    }
                }
                else if (iQuestionCount == 3)
                {

                    for (int i = 0; i < lstOptions[0].Count; i++)   // for a
                    {
                        for (int x = 0; x < lstOptions[1].Count; x++)   // for b
                        {
                            for (int j = 0; j < lstOptions[2].Count; j++)   // for c
                            {
                                lstQuestResult.Add(lstOptions[0][i] + lstOptions[1][x] + lstOptions[2][j]);
                            }
                        }
                    }
                }
                #endregion

            }
            //string strQuest1Options = "";
            //for (int x = 0; x < lstOptions.Count; x++)
            //{
            //    strQuest1Options += Environment.NewLine + "====第 " + x + " 題的選項池 ====";
            //    for (int j = 0; j < lstOptions[x].Count; j++)
            //    {
            //        strQuest1Options += Environment.NewLine + lstOptions[x][j];
            //    } 
            //} 
            this.Answers_Resoult = lstQuestResult;
            if (lstQuestResult.Count > 0) bIsHaveNewAnswer = true;
            return lstQuestResult;
        }

        /// <summary>
        /// 給定一個範圍區域，由此副程式進行偵測題型，並且篩選出可能的"選項"
        /// </summary>
        /// <param name="lstSourceQuestionText"></param>
        /// <param name="sReturnQuestType"></param>
        /// <returns></returns>
        private List<List<string>> GetSelectQuestion_Options(List<string> lstSourceQuestionText, ref string sReturnQuestType)
        {
            List<List<string>> lstOptionsPool = new List<List<string>>();

            #region 問題數量分析 (單選、多選)

            //搜尋問題數量
            List<int> lstOptionsQuestion = new List<int>();  //問題
            for (int i = 0; i < lstSourceQuestionText.Count; i++)
            {
                string p = lstSourceQuestionText[i];
                if (p.Contains("?") || p.Contains("？"))    // 半形 & 全形 都算數
                {
                    lstOptionsQuestion.Add(i);
                }
            }

            //搜尋題目結尾端
            List<int> lstOptionsEnd = new List<int>();  //題目結尾
            for (int i = 0; i < lstSourceQuestionText.Count; i++)
            {
                string p = lstSourceQuestionText[i];
                if (p.Contains("答案框") || p.Contains("答案"))
                {
                    lstOptionsEnd.Add(i);
                }
            }

            //計算題目數量
            int iQuestionCount = 0;
            if (lstOptionsEnd.Count > 0 && lstOptionsQuestion.Count > 0)
            {
                for (int i = 0; i < lstOptionsQuestion.Count; i++)
                {
                    if (lstOptionsEnd[lstOptionsEnd.Count - 1] - lstOptionsQuestion[i] > 0)
                    {
                        iQuestionCount++;
                    }
                }
            }
            else if (lstOptionsEnd.Count == 0 && lstOptionsQuestion.Count > 0)
            {
                iQuestionCount = lstOptionsQuestion.Count;
            }
            #endregion

            string strTypeSystem = "";
            //如果有找到題目 & 有找到說明 = 有可能是一個完整的題目敘述。此時有可能是單選或多選或三選
            if (lstOptionsEnd.Count >= 0 && lstOptionsQuestion.Count > 0)
            {
                #region 有題目、有解說，或有題目，沒解說，都可能是選擇題
                //文本末端會解釋該怎麼填寫考試答案，由該行往上算直到最後一個問題所在列。
                //如果太短，那代表有可能選項包含在題目內，例如 : 
                /* 
                    1.金鍾仁2.金俊勉3.嘟暻秀4.金?錫5.金鐘大6.Willis，誰的年齡最大？
                    請在答案框輸入編號 
                 */
                // 計算文本距離
                int iTextLineRange = 0;
                if (lstOptionsEnd.Count > 0)
                {
                    //有頭 有尾
                    iTextLineRange = lstOptionsEnd[lstOptionsEnd.Count - 1] - lstOptionsQuestion[lstOptionsQuestion.Count - 1];
                }
                else if (lstOptionsEnd.Count == 0)
                {
                    //有頭 沒尾
                    iTextLineRange = lstSourceQuestionText.Count;
                }

                strTypeSystem += string.Format("行數差距 : {0}", iTextLineRange);

                //紀錄分割完的所有選項
                lstOptionsPool.Clear();
                if (iQuestionCount == 1)
                {
                    #region == 單選題 ==

                    if (iTextLineRange >= 3) // 針對文本超過4行以上的選擇題進行處理 
                    {
                        sReturnQuestType = "單選題，且分行超過 3 行";
                        //===單選題處理==並且必須是多行型態的選項
                        int iQuestStartIdx = lstOptionsQuestion[0]; //問題開始
                        int iQuestEndIdx = 0;                      //問題結束

                        if (lstOptionsEnd.Count > 0)
                        {
                            //有頭 有尾
                            //iQuestEndIdx = lstOptionsEnd[0];
                            iQuestEndIdx = lstOptionsEnd[lstOptionsEnd.Count - 1];
                        }
                        else if (lstOptionsEnd.Count == 0)
                        {
                            //有頭 沒尾
                            int iFindEndLine = 0; //查看哪一行不是空白，以最後一行為主
                            for (int i = 0; i < lstSourceQuestionText.Count; i++)
                            {
                                string strLineText = lstSourceQuestionText[i];
                                string strClearText = Regex.Replace(strLineText, @"[\W_]+", ""); //濾除所有特殊符號 
                                if (strClearText.Length > 0)
                                {
                                    iFindEndLine = i + 1;
                                }
                            }
                            iQuestEndIdx = iFindEndLine;
                        }

                        #region    裁切有可能是選項的區域
                        List<string> lstNeedSplit = new List<string>();
                        for (int i = iQuestStartIdx + 1; i < iQuestEndIdx; i++)
                        {
                            lstNeedSplit.Add(lstSourceQuestionText[i]);
                        }
                        #endregion

                        List<string> lstOptionsResult = Options_Split_From_Quest(lstNeedSplit);
                        lstOptionsPool.Add(lstOptionsResult);

                    }
                    else
                    {
                        //===單選題處理==但是題目跟答案在同一行上，要從一行進行分析 
                        sReturnQuestType = "單選題，但是文本段落不足，需橫向分析";

                        List<string> lstResultOption = new List<string>();
                        for (int i = 0; i < lstSourceQuestionText.Count; i++)
                        {
                            string strLineText = lstSourceQuestionText[i];
                            string strRPText = Regex.Replace(strLineText, @"[\W_]+", "#"); //替代所有符合，用以查找代表有意義的文字
                            string[] sp = strRPText.Split('#');
                            //對有意義的符號直接切割
                            foreach (string t in sp)
                            {
                                if (t.Length > 0 && t.Length < 5)
                                {
                                    lstResultOption.Add(t);
                                }
                            }
                            //抓取切割完畢的最後一字
                            foreach (string t in sp)
                            {
                                if (t.Length > 0 && t.Length < 5)
                                {
                                    lstResultOption.Add(t[t.Length - 1].ToString());
                                }
                            }
                        }
                        if (lstResultOption.Count > 0) lstOptionsPool.Add(lstResultOption);

                    }

                    #endregion
                }
                else if (iQuestionCount == 2)
                {
                    #region == 2 選題處理 ==

                    if (iTextLineRange >= 3) // 針對文本超過4行以上的選擇題進行處理 
                    {
                        sReturnQuestType = "雙選題，且分行超過 3 行";
                        //===雙選題處理== 分析第一題選項
                        int iQuestStartIdx = lstOptionsQuestion[0];         //問題開始 (問題1 ~ 問題2 行距離 = 選項 2  )
                        int iQuestEndIdx = lstOptionsQuestion[1];           //問題結束

                        #region    裁切有可能是選項的區域 -- 切割第一題
                        List<string> lstNeedSplit = new List<string>();
                        for (int i = iQuestStartIdx + 1; i < iQuestEndIdx; i++)
                        {
                            lstNeedSplit.Add(lstSourceQuestionText[i]);
                        }
                        List<string> lstOptionsResult = Options_Split_From_Quest(lstNeedSplit);
                        #endregion
                        lstOptionsPool.Add(lstOptionsResult);


                        //===雙選題處理== 分析第二題選項
                        iQuestStartIdx = lstOptionsQuestion[1];             //問題開始 (問題2 ~ 填答說明 行距離 = 選項 2  )
                        iQuestEndIdx = 0;                                   //問題結束
                        if (lstOptionsEnd.Count > 0)
                        {
                            //有頭 有尾
                            //iQuestEndIdx = lstOptionsEnd[0];
                            iQuestEndIdx = lstOptionsEnd[lstOptionsEnd.Count - 1];
                        }
                        else if (lstOptionsEnd.Count == 0)
                        {
                            //有頭 沒尾
                            int iFindEndLine = 0; //查看哪一行不是空白，以最後一行為主
                            for (int i = 0; i < lstSourceQuestionText.Count; i++)
                            {
                                string strLineText = lstSourceQuestionText[i];
                                string strClearText = Regex.Replace(strLineText, @"[\W_]+", ""); //濾除所有特殊符號 
                                if (strClearText.Length > 0)
                                {
                                    iFindEndLine = i + 1;
                                }
                            }
                            iQuestEndIdx = iFindEndLine;
                        }

                        #region    裁切有可能是選項的區域 -- 切割第二題
                        List<string> lstNeedSplit_Two = new List<string>();
                        for (int i = iQuestStartIdx + 1; i < iQuestEndIdx; i++)
                        {
                            lstNeedSplit_Two.Add(lstSourceQuestionText[i]);
                        }
                        List<string> lstOptionsResult_Two = Options_Split_From_Quest(lstNeedSplit_Two);
                        #endregion
                        lstOptionsPool.Add(lstOptionsResult_Two);

                    }
                    else
                    {
                        //===雙選題處理==但是題目跟答案在同一行上，要從一行進行分析
                        sReturnQuestType = "雙選題，但是段落長度不足，須改為橫向分析";
                    }
                    #endregion
                }
                else if (iQuestionCount == 3)
                {
                    #region == 三選題處理 ==

                    if (iTextLineRange >= 3) // 針對文本超過4行以上的選擇題進行處理 
                    {
                        sReturnQuestType = "3 選題，且分行超過 3 行";
                        //=== 3 選題處理== 分析第一題選項
                        int iQuestStartIdx = lstOptionsQuestion[0];         //問題開始 (問題1 ~ 問題2 行距離 = 選項 2  )
                        int iQuestEndIdx = lstOptionsQuestion[1];           //問題結束

                        #region    裁切有可能是選項的區域 -- 切割第一題
                        List<string> lstNeedSplit = new List<string>();
                        for (int i = iQuestStartIdx + 1; i < iQuestEndIdx; i++)
                        {
                            lstNeedSplit.Add(lstSourceQuestionText[i]);
                        }
                        List<string> lstOptionsResult = Options_Split_From_Quest(lstNeedSplit);
                        #endregion
                        lstOptionsPool.Add(lstOptionsResult);


                        //===3 選題處理== 分析第二題選項
                        iQuestStartIdx = lstOptionsQuestion[1];             //問題開始 (問題2 ~ 問題3 行距離 = 選項 2  )
                        iQuestEndIdx = lstOptionsQuestion[2];               //問題結束

                        #region    裁切有可能是選項的區域 -- 切割第二題
                        List<string> lstNeedSplit_Two = new List<string>();
                        for (int i = iQuestStartIdx + 1; i < iQuestEndIdx; i++)
                        {
                            lstNeedSplit_Two.Add(lstSourceQuestionText[i]);
                        }
                        List<string> lstOptionsResult_Two = Options_Split_From_Quest(lstNeedSplit_Two);
                        #endregion
                        lstOptionsPool.Add(lstOptionsResult_Two);


                        //===3 選題處理== 分析第二題選項
                        iQuestStartIdx = lstOptionsQuestion[2];             //問題開始 (問題3 ~ 填答說明 行距離 = 選項 2  )
                        iQuestEndIdx = 0;                                   //問題結束
                        if (lstOptionsEnd.Count > 0)
                        {
                            //有頭 有尾
                            iQuestEndIdx = lstOptionsEnd[lstOptionsEnd.Count - 1];
                            //iQuestEndIdx = lstOptionsEnd[0];
                        }
                        else if (lstOptionsEnd.Count == 0)
                        {
                            //有頭 沒尾
                            int iFindEndLine = 0; //查看哪一行不是空白，以最後一行為主
                            for (int i = 0; i < lstSourceQuestionText.Count; i++)
                            {
                                string strLineText = lstSourceQuestionText[i];
                                string strClearText = Regex.Replace(strLineText, @"[\W_]+", ""); //濾除所有特殊符號 
                                if (strClearText.Length > 0)
                                {
                                    iFindEndLine = i + 1;
                                }
                            }
                            iQuestEndIdx = iFindEndLine;
                        }

                        #region    裁切有可能是選項的區域 -- 切割第三題
                        List<string> lstNeedSplit_Three = new List<string>();
                        for (int i = iQuestStartIdx + 1; i < iQuestEndIdx; i++)
                        {
                            lstNeedSplit_Three.Add(lstSourceQuestionText[i]);
                        }
                        List<string> lstOptionsResult_Three = Options_Split_From_Quest(lstNeedSplit_Three);
                        #endregion
                        lstOptionsPool.Add(lstOptionsResult_Three);

                    }
                    else
                    {
                        //===3 選題處理==但是題目跟答案在同一行上，要從一行進行分析
                        sReturnQuestType = "3 選題，段落不足，需橫向分析";
                    }

                    #endregion
                }
                #endregion

            }
            else if (lstOptionsQuestion.Count == 0)
            {
                #region 完全看不懂題目再問什麼，就直接選可能的括弧當作選項，包含排序題

                sReturnQuestType = "無法辨識的題型，採任意切割";
                List<string> lstNeedSplit = new List<string>();

                lstNeedSplit = lstSourceQuestionText;

                int iCase_LR = 0;   //左右括弧系列
                for (int i = 0; i < lstSourceQuestionText.Count; i++)
                {
                    if (lstSourceQuestionText[i].Contains("(") && lstSourceQuestionText[i].Contains(")"))
                    {
                        iCase_LR++;
                    }
                }

                int iCase_R = 0;   //右括弧系列
                for (int i = 0; i < lstSourceQuestionText.Count; i++)
                {
                    if (lstSourceQuestionText[i].Contains(")"))
                    {
                        iCase_R++;
                    }
                }
                int iCase_DOT = 0;  //逗點系列
                for (int i = 0; i < lstSourceQuestionText.Count; i++)
                {
                    string strTemp = lstSourceQuestionText[i];
                    string[] sp = strTemp.Split('.');
                    if (sp.Length > 1)
                    {
                        if (sp[0].Length > 0)
                        {
                            iCase_DOT++;
                        }
                    }
                }
                int iCase_GUN = 0;  //-- 系列
                for (int i = 0; i < lstSourceQuestionText.Count; i++)
                {
                    string strTemp = lstSourceQuestionText[i];
                    string[] sp = strTemp.Split('-');
                    if (sp.Length > 1)
                    {
                        if (sp[0].Length > 0)
                        {
                            iCase_GUN++;
                        }
                    }
                }

                List<int> lstTable = new List<int>();
                lstTable.Add(iCase_LR);
                lstTable.Add(iCase_R);
                lstTable.Add(iCase_DOT);
                lstTable.Add(iCase_GUN);

                char cSplitKeyWord = '=';

                if (iCase_LR > 2 || iCase_R > 2)
                {
                    cSplitKeyWord = ')';
                }
                else if (iCase_DOT > 3)
                {
                    cSplitKeyWord = '.';
                }
                else if (iCase_GUN > 2)
                {
                    cSplitKeyWord = '-';
                }

                List<string> lstOptionsResult = new List<string>();
                for (int i = 0; i < lstNeedSplit.Count; i++)
                {
                    string strTemp = lstNeedSplit[i];
                    if (strTemp.Contains(cSplitKeyWord))
                    {
                        string[] sp = strTemp.Split(cSplitKeyWord);
                        if (sp.Length > 0)
                        {
                            string strOp = Regex.Replace(sp[0], @"[\W_]+", ""); //取代所有特殊符號
                            if (strOp.Length > 0 && strOp.Length < 20) lstOptionsResult.Add(strOp);
                        }
                    }
                }
                #endregion

                lstOptionsPool.Add(lstOptionsResult);
            }
            this.Oprions_Result = lstOptionsPool;
            return lstOptionsPool;
        }
         
    }

}
