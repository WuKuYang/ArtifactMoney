using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace TSubscriber
{
    /// <summary>
    /// Html分析
    /// </summary>
    public class HtmlAnalyze
    {
        #region 私有函式
        private static List<SWebElement> FindElementByAttribute(string strCode, string webValue, string Attribute)
        {
            List<SWebElement> myclass = new List<SWebElement>();
            int Index = 0;
            SWebElement data = null;
            while (Index != -1)
            {
                data = FindSubAttribute(strCode, webValue, Attribute);
                if (data != null)
                {
                    Index = data.ELocation.End;
                    strCode = strCode.Remove(0, Index);
                    //data = RemoveTag(data, webclass);
                    myclass.Add(data);
                }
                else
                {
                    break;
                }


            }
            return myclass;
        }

        private static SWebElement FindSubAttribute(string strCode, string webValue, string Attribute)
        {
            int class_Index = strCode.IndexOf(" "+Attribute + "=\"" + webValue + "\"");
            if (class_Index != -1)
            {
                SWebElement Element = getTagInfomation(strCode, class_Index);
                return Element;
            }

            return null;
        }

        private static List<SWebElement> FindElementByClass(string strCode, string webclass)
        {
            List<SWebElement> myclass = new List<SWebElement>();
            int Index = 0;
            SWebElement data = null;
            while (Index != -1)
            {
                data = FindSubClass(strCode, webclass);
                if (data != null)
                {
                    Index = data.ELocation.End;
                    strCode = strCode.Remove(0, Index);
                    //data = RemoveTag(data, webclass);
                    myclass.Add(data);
                }
                else
                {
                    break;
                }


            }
            return myclass;
        }

        private static SWebElement FindSubClass(string strCode, string webclass)
        {
            int class_Index = strCode.IndexOf("class=\"" + webclass + "\"");
            if (class_Index != -1)
            {
                SWebElement Element = getTagInfomation(strCode, class_Index);
                return Element;
            }

            return null;
        }

        private static List<SWebElement> FindElementByType(string strCode, string webtype)
        {
            List<SWebElement> myclass = new List<SWebElement>();
            int Index = 0;
            SWebElement data = null;
            while (Index != -1)
            {
                data = FindSubClass(strCode, webtype);
                if (data != null)
                {
                    Index = data.ELocation.End;
                    strCode = strCode.Remove(0, Index);
                    //data = RemoveTag(data, webclass);
                    myclass.Add(data);
                }
                else
                {
                    break;
                }


            }
            return myclass;
        }

        private static SWebElement FindSubType(string strCode, string webtype)
        {
            int class_Index = strCode.IndexOf("type=\"" + webtype + "\"");
            if (class_Index != -1)
            {
                SWebElement Element = getTagInfomation(strCode, class_Index);
                return Element;
            }

            return null;
        }

        private static List<SWebElement> FindElementByID(string strCode, string webID)
        {
            List<SWebElement> myID = new List<SWebElement>();
            int Index = 0;
            SWebElement data = null;
            while (Index != -1)
            {
                data = FindSubID(strCode, webID);
                if (data != null)
                {
                    Index = data.ELocation.End;
                    strCode = strCode.Remove(0, Index);
                    //data = RemoveTag(data, webclass);
                    myID.Add(data);
                }
                else
                {
                    break;
                }


            }
            return myID;
        }

        private static SWebElement FindSubID(string strCode, string webid)
        {
            int class_Index = strCode.IndexOf("id=\"" + webid + "\"");
            if (class_Index != -1)
            {
                SWebElement Element = getTagInfomation(strCode, class_Index);
                return Element;
            }

            return null;
        }

        private static int FindWebElementStartIndex(string strCode, int SearchIndex, out string tag)
        {
            int Element_Index = SearchIndex;

            bool FindStart = false;
            //尋找class起點
            while (!FindStart)
            {
                if (strCode[Element_Index] == '<')
                {
                    FindStart = true;
                    string[] data = strCode.Remove(0, Element_Index + 1).Split(' ');
                    if (data.Length != 0)
                    {
                        tag = data[0];
                        #region 檢查Tag的正確性
                        int tagCheck = tag.IndexOf(">");
                        if (tagCheck != -1)
                        {
                            tag = tag.Substring(0, tagCheck);
                        }
                        #endregion
                    }
                    else
                    {
                        tag = "";
                    }
                    return Element_Index;
                }
                Element_Index--;
            }
            tag = "";
            return -1;
        }

        private static int FindWebElementEndIndex(string strCode, int StartIndex, string tag)
        {
            string data = strCode.Remove(0, StartIndex);

            int Tag_Length = 2 + tag.Length;
            int tempValue = data.IndexOf("/" + tag + ">");
            if (tempValue == -1)
            {
                int End = data.IndexOf(">");
                return StartIndex + End;
            }
            int Element_Index = tempValue + Tag_Length + StartIndex;

            MatchCollection TagStart = Regex.Matches(strCode, "<" + tag);
            MatchCollection TagEnd = Regex.Matches(strCode, "</" + tag);
            
            List<ElementIndex> eData = new List<ElementIndex>();
            
            #region 取得所有Tag頭
            for (int i = 0; i < TagStart.Count; i++)
            {
                ElementIndex temp = new ElementIndex();
                temp.Index = TagStart[i].Index;
                temp.etype = ElementType.Start;
                eData.Add(temp);
            }
            #endregion

            #region 取得所有Tag尾

            for (int i = 0; i < TagEnd.Count; i++)
            {
                ElementIndex temp = new ElementIndex();
                temp.Index = TagEnd[i].Index;
                temp.etype = ElementType.End;
                eData.Add(temp);
            }
            #endregion

            #region 所有Tag排序
            for (int i = 0; i < eData.Count; i++)
            {
                for (int j = 0; j < eData.Count; j++)
                {
                    if (eData[i].Index < eData[j].Index)
                    {
                        ElementIndex temp = eData[i];
                        eData[i] = eData[j];
                        eData[j] = temp;
                    }
                }
            }
            #endregion

            int SelectIndex = 0;
            for (int i = 0; i < eData.Count - 1; i++)
            {
                if (eData[i].Index == StartIndex)
                {
                    SelectIndex = i;
                    int Element_Weight = 0;
                    for (int j = SelectIndex + 1; j < eData.Count; j++)
                    {
                        //<?> ++ 
                        //</?> --
                        if (eData[j].etype == ElementType.Start)
                        {
                            Element_Weight++;
                        }
                        else
                        {
                            Element_Weight--;
                        }
                        if (Element_Weight < 0)
                        {
                            Element_Index = eData[j].Index + Tag_Length;
                            break;
                        }
                    }
                    break;
                }
            }
            return Element_Index;
        }

        private static SWebElement getTagInfomation(string strCode, int Search_Index)
        {
            string tag = "";
            string data = strCode;
            int Element_Start_Index = FindWebElementStartIndex(data, Search_Index, out tag);
            int Element_End_Index = FindWebElementEndIndex(data, Element_Start_Index, tag);

            SWebElement element = new SWebElement();
            element.ELocation.Start = Element_Start_Index;
            element.ELocation.End = Element_End_Index;
            element.TagName = tag;
            element.Context = data.Substring(Element_Start_Index, Element_End_Index - Element_Start_Index + 1);
            element.ElementName = RemoveTag(element.Context, element.TagName);
            return element;
        }

        private static string RemoveTag(string strCode, string tag)
        {
            string patternTS = "<" + tag + ".*?>";
            string patternTE = "<\\/" + tag + ">";
            MatchCollection TagS = Regex.Matches(strCode, patternTS);
            MatchCollection TagE = Regex.Matches(strCode, patternTE);
            int SubIndexS = -1;
            int SubIndexE = -1;
            if (TagS.Count > 0)
            {
                SubIndexS = TagS[0].Index + TagS[0].Value.Length;
            }
            else
            {
                SubIndexS = 0;
            }
            if (TagE.Count > 0)
            {
                SubIndexE = TagE[TagE.Count - 1].Index;
            }
            else
            {
                SubIndexE = strCode.Length;
            }
            strCode = strCode.Substring(SubIndexS, SubIndexE - SubIndexS);
            return strCode;
        }

        private static List<SWebElement> FindElementByTag(string strCode, string webTag)
        {
            
            string pattern = "<" + webTag + ".*?>";
            List<SWebElement> Tags = new List<SWebElement>();
            List<string> MatchTag = FindTag(strCode, webTag, false);
            for (int i = 0; i < MatchTag.Count; i++)
            {
                Tags.Add(getTagInfomation(MatchTag[i], 0));
            }
            return Tags;
        }
        private static List<string> FindTag(string strCode, string tag, bool Remove = true)
        {
            List<string> Tags = new List<string>();

            string pattern = "<" + tag + ".*?>";
            MatchCollection match = Regex.Matches(strCode, pattern);
            for (int i = 0; i < match.Count; i++)
            {
                int EndIndex = FindWebElementEndIndex(strCode, match[i].Index, tag);
                string str = strCode.Substring(match[i].Index, EndIndex - match[i].Index + 1);
                if (Remove)
                {
                    str = RemoveTag(str, tag);
                }
                Tags.Add(str);
            }
            return Tags;
        }
        
        #endregion

        public static List<SWebElement> FindElement(string strCode, WebBy FindFunction)
        {
            switch (FindFunction.FMode)
            {
                case FindElementMode.Class:
                    return FindElementByAttribute(strCode, FindFunction.value, "class");
                case FindElementMode.ID:
                    return FindElementByAttribute(strCode, FindFunction.value, "id");
                case FindElementMode.Type:
                    return FindElementByAttribute(strCode, FindFunction.value, "type");
                case FindElementMode.Title:
                    return FindElementByAttribute(strCode, FindFunction.value, "title");
                case FindElementMode.SelfDefine:
                    return FindElementByAttribute(strCode, FindFunction.value, FindFunction._SelfDefineName);
                case FindElementMode.Tag:
                    return FindElementByTag(strCode, FindFunction.value);
            }
            return null;
        }

        
    }

    #region Html相關元素
    public enum ElementType { Start, End }

    public enum FindElementMode { ID, Class, Tag, Type, Title, SelfDefine };

    public class WebBy
    {
        public FindElementMode FMode = FindElementMode.Class;
        public string value = "";
        public string _SelfDefineName = "";

        public WebBy()
        { }
        public WebBy(FindElementMode m, string v)
        {
            FMode = m;
            value = v;
        }
        public WebBy(FindElementMode m, string d, string v)
        {
            FMode = m;
            _SelfDefineName = d;
            value = v;
        }
        public static WebBy ID(string itofind)
        {
            return new WebBy(FindElementMode.ID, itofind);
        }
        public static WebBy Class(string itofind)
        {
            return new WebBy(FindElementMode.Class, itofind);
        }
        public static WebBy Tag(string itofind)
        {
            return new WebBy(FindElementMode.Tag, itofind);
        }
        public static WebBy Type(string itofind)
        {
            return new WebBy(FindElementMode.Type, itofind);
        }
        public static WebBy Title(string itofind)
        {
            return new WebBy(FindElementMode.Title, itofind);
        }
        public static WebBy SelfDefineName(string _SelfDefineName, string itofind)
        {
            return new WebBy(FindElementMode.SelfDefine, _SelfDefineName, itofind);
        }
    }

    public class ElementLocation
    {
        public int Start;
        public int End;
    }

    public class SWebElement
    {
        public string TagName = "";

        public string ElementName = "";

        public string Context = "";

        public ElementLocation ELocation = new ElementLocation();

        public string Class
        {
            get
            {
                return GetAttribute(" class=");
            }
        }

        public string id
        {
            get
            {
                return GetAttribute(" id=");
            }
        }
        public string value
        {
            get
            {
                return GetAttribute(" value=");
            }
        }
        public string Name
        {
            get
            {
                return GetAttribute(" name=");
            }
        }
        public string URL
        {
            get
            {
                return GetAttribute("href=");
            }
        }
        public SWebElement()
        { 
        
        }
        public SWebElement(string text)
        {
            Context = text;
            
        }
        
        public string GetAttribute(string attributeName)
        {
            int attributeSearchEnd = Context.IndexOf(">");
            int attribute_Start = Context.IndexOf(attributeName);
            if ((attribute_Start != -1) && (attribute_Start < attributeSearchEnd))
            {
                string attributeInfo = Context.Substring(attribute_Start);
                attribute_Start = attributeInfo.IndexOf("\"");
                attributeInfo = attributeInfo.Substring(attribute_Start + 1);
                attribute_Start = attributeInfo.IndexOf("\"");
                return attributeInfo.Substring(0, attribute_Start);
            }
            return "";
        }

    }

    public class ElementIndex
    {
        public int Index = 0;
        public ElementType etype = ElementType.Start;

    }
    #endregion
}
