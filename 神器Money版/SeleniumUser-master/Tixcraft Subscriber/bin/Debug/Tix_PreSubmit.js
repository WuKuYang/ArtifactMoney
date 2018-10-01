function TESTDown(){

    var veryfiSrc = document.getElementById("yw0");
    //alert(veryfiSrc.src);
    $.ajax({
        url: veryfiSrc.src,
        dataType: 'html',
        success: function(data)
        {
            alert(data);
        }
    });
}

var g_IsClickedSubmit = false;
//每隔幾秒執行一次這個函數
function CallBackToDetectVeryfiCode( )
{
    //code
    var veryfiCodeFeild = document.getElementById("TicketForm_verifyCode");
    if(veryfiCodeFeild !=undefined)
    {
        if(veryfiCodeFeild.value.length >= 4)
        {
            //alert("detected!!");
            var submit_out = document.getElementById("TicketForm");
            if(submit_out !=undefined)
            {
                if(g_IsClickedSubmit == false)
                {
                    submit_out.submit();
                    g_IsClickedSubmit = true;
                }
            }
        }
    }
    //code
}

function getCookie(c_name)
{
    var i,x,y,ARRcookies=document.cookie.split(";");
    for (i=0;i<ARRcookies.length;i++)
    {
        x=ARRcookies[i].substr(0,ARRcookies[i].indexOf("="));
        y=ARRcookies[i].substr(ARRcookies[i].indexOf("=")+1);
        x=x.replace(/^\s+|\s+$/g,"");
        if (x==c_name)
        {
            return unescape(y);
        }
    }
}

function setCookie(c_name,value,exdays)
{
    var exdate=new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value=escape(value) + ((exdays==null) ? "" : "; expires="+exdate.toUTCString());
    document.cookie=c_name + "=" + c_value;
}

function xpath(query)
{
    return document.evaluate(query, document, null,XPathResult.UNORDERED_NODE_SNAPSHOT_TYPE, null);
}



//==========================================================================================================
//                                 流程相關副程式
//===========================================================================================================





//*****************************************
// 流程 ==> 取得所有座位(有效可按)，之後隨機進入
//*****************************************

function GoSeatByRndom()
{
    var mHref = document.location.href;
    if(StringContain(mHref , "area"))
    {

    }
    else
    {
        return;
    }
    // == 取得所有座位 (格式 ==>  "位置名稱^網址" ) ==
    var AllSeatsInformation = GetSeatURLs();
    if(AllSeatsInformation != undefined)
    {
        var maxNum = AllSeatsInformation.length - 1;
        var minNum = 0;
        var nRandom = Math.floor(Math.random() * (maxNum - minNum + 1)) + minNum;
        if(AllSeatsInformation.length > nRandom)
        {
            var strSplitURL = AllSeatsInformation[nRandom].split("^");
            var strSeatText = strSplitURL[0];
            var strSeatURL = strSplitURL[1];
            GoURL(strSeatURL);
        }
        else
        {
            RefreshWindow();
        }
    }
}

//*****************************************
// 流程 ==> 取得所有座位(有效可按)，之後隨機進入
//*****************************************
function GoSeatByRndom_Matched(strSeatInformationText)
{
    var mHref = document.location.href;
    if(StringContain(mHref , "area"))
    {

    }
    else
    {
        return;
    }
    // == 取得所有座位 (格式 ==>  "位置名稱^網址" ) ==
    var AllSeats = GetSeatURLs();
    // == 指定座位，把符合座位的搜尋名稱全數找出來 --> 模糊查找，例如 : 紅2A5800區 ...紅2A4200區，只須給定字串 紅2A即可搜出==
    var SeatsByFilter = [];
    if(AllSeats != undefined)
    {
        for(is = 0; is<AllSeats.length; is++)
        {
            var strSplitURL = AllSeats[is].split("^");
            var strSeatText = strSplitURL[0];
            if(StringContain( strSeatText , strSeatInformationText))
            {
                SeatsByFilter.push(AllSeats[is]);
            }
        }
    }
    //模糊查找完畢後，隨便選一個座位進入
    var AllSeatsInformation = SeatsByFilter;
    //紀錄是否有進入網頁成功，沒有的話，隨機選座位進入！
    var bIsEntrySuccessful = false;
    if(AllSeatsInformation != undefined)
    {
        var maxNum = AllSeatsInformation.length - 1;
        var minNum = 0;
        var nRandom = Math.floor(Math.random() * (maxNum - minNum + 1)) + minNum;
        if(AllSeatsInformation.length > nRandom)
        {
            var strSplitURL = AllSeatsInformation[nRandom].split("^");
            var strSeatText = strSplitURL[0];
            var strSeatURL = strSplitURL[1];
            GoURL(strSeatURL);
            bIsEntrySuccessful = true;
        }
    }
    if(bIsEntrySuccessful == false)
    {
        GoSeatByRndom();
    }
}

//*****************************************
//**************回傳座位的網頁URL  -> 全部  *********
//*****************************************
function GetSeatURLs()
{
    var mReturnURLs = [];
    var mHref = document.location.href;
    if(StringContain(mHref , "area"))
    {

    }
    else
    {
        return;
    }

    var lstStrListSeatURL = GetAreaUrlList();
    var mSeatElement = document.getElementsByTagName('a');
    //掃過網頁上所有元素
    for( iElemIdx = 0 ; iElemIdx < mSeatElement.length ; iElemIdx++)
    {
        var strSeatText = mSeatElement[iElemIdx].text;// 取得網頁上文字 --> 例如 紅2D區5800
        var strSeatElementID = mSeatElement[iElemIdx].id;  // 紅2D區5800的 --->對應ID
        //JS內所有元素(包含網址)
        for (iEach = 0; iEach < lstStrListSeatURL.length; iEach++)
        {
            var strElement = lstStrListSeatURL[iEach].split('"');
            var SeatID = strElement[1];
            var SeatURL = strElement[3];
            if(strSeatElementID == SeatID)
            {
                mReturnURLs.push(strSeatText + "^"+"https://tixcraft.com/" + SeatURL);
            }
        }
    }
    return mReturnURLs;
}

//*****************************************
//************** 取得所有位置資訊   *********
//*****************************************
function GetAreaUrlList()
{
    iIndexOfArray = -1;
    for(i = 0; i < document.scripts.length ; i++)
    {

        if(StringContain(document.scripts[i].text , "areaUrlList"))
        {
            iIndexOfArray = i;
            break;
        }
    }
    JS_Text = "";
    if(iIndexOfArray > -1)
    {
        var JS_Text = document.scripts[iIndexOfArray].text;
        var iTableArray = JS_Text.indexOf("areaUrlList");   //javascript的程式碼的位置陣列
        if (iTableArray == -1)
        {
            return null;
        }
        var ReturnList = [];

        var strAfterArray = JS_Text.substr(iTableArray);
        var iTableArrayLeft = strAfterArray.indexOf("{"); //陣列頭);
        var iTableArrayRight = strAfterArray.indexOf("}"); //陣列尾
        iTableArrayLeft++;
        iTableArrayRight--;
        var strSubText = strAfterArray.substring(iTableArrayLeft,iTableArrayRight+1);
        strSubText = strSubText.replace(/\\/g, "");
        var strSp = strSubText.split(',');
        return strSp;
    }
    else
    {

    }
}

//== 進入天數 ==
function GoDays(iDayIndex)
{
    var mHref = document.location.href;
    if(StringContain(mHref , "game"))
    {

    }
    else
    {
        return;
    }
    var MyDays = [];
    var MyDaysURL = [];
    var ListDays = document.getElementsByTagName("tr");
    for(i = 0 ; i < ListDays.length ; i++)
    {
        if(StringContain(ListDays[i].className , "gridc fcTxt"))
        {
            MyDays.push(ListDays[i]);
            var myElementTix = ListDays[i].getElementsByTagName("input");
            if(myElementTix.length > 0)
            {
                if(StringContain(myElementTix[0].value , "立即訂購"))
                {
                    MyDaysURL.push(myElementTix[0]);
                }
            }
        }
    }
    if(MyDaysURL.length > 0) //如果有開放天數的話
    {
        if(iDayIndex < MyDaysURL.length) //如果有點日期成功的話
        {
            MyDaysURL[iDayIndex].click();
        }
        else
        {
            // 如果點失敗日期，則隨機選一個天期進去
            var maxNum = MyDaysURL.length - 1;
            var minNum = 0;
            var nRandom = Math.floor(Math.random() * (maxNum - minNum + 1)) + minNum;
            if(nRandom < MyDaysURL.length)
            {
                MyDaysURL[nRandom].click();
            }
            else
            {
                MyDaysURL[0].click();
            }
        }
    }
    else
    {
        //如果沒有開放天數，那重刷
        RefreshWindow();
    }
}

//*****************************************
//************** 取得登入資訊TOKEN == (提交使用)
//*****************************************
function GetCSRFTOKEN()
{
    iIndexOfArray = -1;
    for(i = 0; i < document.scripts.length ; i++)
    {

        if(StringContain(document.scripts[i].text , "CSRFTOKEN"))
        {
            if(StringContain(document.scripts[i].text , "jQuery.yii.submitForm"))
            {

                iIndexOfArray = i;
                break;
            }
        }
    }
    JS_Text = "";
    if(iIndexOfArray > -1)
    {
        var JS_Text = document.scripts[iIndexOfArray].text;
        var iTableArray = JS_Text.indexOf("jQuery.yii.submitForm");   //javascript的程式碼的位置陣列
        if (iTableArray == -1)
        {
            return null;
        }
        var ReturnList = [];

        var strAfterArray = JS_Text.substr(iTableArray);
        var iTableArrayLeft = strAfterArray.indexOf("{"); //陣列頭);
        var iTableArrayRight = strAfterArray.indexOf("}"); //陣列尾
        iTableArrayLeft++;
        iTableArrayRight--;
        var strSubText = strAfterArray.substring(iTableArrayLeft,iTableArrayRight+1);
        var strCSRFTOKEN_Value = strSubText.split(':')[1];
        strCSRFTOKEN_Value = strCSRFTOKEN_Value.replace("'", "");
        strCSRFTOKEN_Value = strCSRFTOKEN_Value.replace("'", "");
        return strCSRFTOKEN_Value;
    }
    else
    {

    }
}

// FeildName_Count = "TicketForm[ticketPrice][03]" --> 選座位的select option
// tCount = 張數
// 驗證碼答案 = vAnswerCode
function GetPOST_StringFormat( FeildName_Count , tCount , vAnswerCode)
{
    var AreaHref = document.location.href;
    if(StringContain(AreaHref , "area"))
    {
        var CSRFTOKEN = GetCSRFTOKEN();
        var CountFeild = FeildName_Count;
        var TicketCount = tCount;
        var CSRFTOKENFeild = "CSRFTOKEN";
        var VeriyFeild = "TicketForm[verifyCode]";
        var VeriyAnswer = vAnswerCode;
        var AgreeFeild = "TicketForm[agree]";
        var DataStringParameter =
            "CSRFTOKEN" + "=" + CSRFTOKEN + "&" +
            CountFeild + "=" + TicketCount + "&" +
            VeriyFeild + "=" + VeriyAnswer + "&" +
            AgreeFeild + "=" + "1" + "&" +
            "ticketPriceSubmit" + "=" + "E7%A2%BA%E8%AA%8D%E5%BC%B5%E6%95%B8";
        return DataStringParameter;
    }
    else
    {
        return "";
    }
}


//===========================================================================================================
//                                 自動化填單
//===========================================================================================================

//*****************************************
//************** 勾選必要條件 **************
//*****************************************
function PreSubmit(iTCount)
{
    var mHref = document.location.href;
    if(StringContain(mHref , "ticket/ticket"))
    {
        //選擇張數
        var iTicketFeild = document.getElementsByTagName('select'); // --> 控件欄位
        var iTicketCount = document.getElementsByTagName('option'); // --> 控件選項
        if(iTicketCount !=undefined)
        {

            for(i = 0; i < iTicketCount.length ; i++)
            {
                var mySeatText = iTicketCount[i].textContent;
                if(iTCount.toString() == mySeatText) // ==> 要確定有張數，如果出現 " 已無連續 4 座位 " 這種就不會成立！
                {
                    iTicketFeild[0].value = iTCount;
                }
            }
            //如果沒有這個欄位，那就填到滿
            while(iTicketFeild[0].value != iTCount)
            {
                if(iTCount < 0){  break;  }
                iTCount = iTCount - 1;
                iTicketFeild[0].value = iTCount;
            }
        }
        //勾選我同意條款
        var mAgree  = document.getElementById("TicketForm_agree");
        if(mAgree !=undefined)
        {
            mAgree.checked=true;
        }
        var mCode  = document.getElementById("TicketForm_verifyCode");
        if(mCode !=undefined)
        {
            if(mCode.value.toString().length == 4)
            {
                //送出
                var SubmitOut  = document.getElementById("TicketForm");
                if(SubmitOut !=undefined)
                {
                    SubmitOut.submit();
                }
            }
        }
        g_IsClickedSubmit = false;

        //setInterval(CallBackToDetectVeryfiCode, 75);
        CallBackToDetectVeryfiCode();
        Play_Crrect(); //撥放音效
    }
}

//*****************************************
//************** 自動導向刷天數頁面 ********
//*****************************************
function AutoDirect_GAME()
{
    var mHref = document.location.href;
    if(StringContain(mHref , "detail"))
    {
        mHref = mHref.replace("detail", "game");
        GoURL(mHref);
    }
    else
    {
    }
}
//====自動選擇ATM付款====
// 0 => ATM付款   1 = ibon付款  2 = 信用卡
// 參數 : 信用卡 : "Credit Card"
// 參數 : ATM付款 : "ATM"
// 參數 : ibon付款 : "ibon"
function AutoSelect_PayType(payment_type)
{
    var mHref = document.location.href;
    if(StringContain(mHref , "payment"))
    {

        var iPay_Types = document.getElementsByTagName('label');

        for(i = 0; i < iPay_Types.length ; i++)
        {
            //alert(iPay_Types[i].textContent);

            var pay_string = "none_slect";
            if(payment_type == 0) { pay_string = "ATM"; }
            if(payment_type == 1) { pay_string = "ibon"; }
            if(payment_type == 2) { pay_string = "Credit Card"; }

            if(StringContain(iPay_Types[i].textContent , pay_string))
            {
                iPay_Types[i].click();

                setTimeout(function(){
                    var SubmitButton = document.getElementById("submitButton");
                    Play_Coin();
                    SubmitButton.click();
                    //?里的代??在1000ms(1s后?行)
                },1500);

            }
        }
    }
}

//*****************************************
//************** 填寫驗證碼         ********
//*****************************************
function Tix_KeyInVerifyCode(mAnswer)
{
    //填寫驗證碼
    var mVeryfyCode  = document.getElementById("TicketForm_verifyCode");
    if(mVeryfyCode !=undefined)
    {
        mVeryfyCode.value= mAnswer;
    }
}

//===========================================================================================================
//                                基本功能
//===========================================================================================================

//==============================================
// 進入網頁
//==============================================
function GoURL(strURL)
{
    window.location = strURL;
}

function RefreshWindow()
{
    window.location.reload(true);
}

//======================
// 字串搜尋 : 搜尋SMsg內是否有InnerString這組字串
//======================
function StringContain(SMsg , InnerString)
{
    if(SMsg.indexOf(InnerString) > -1)
    {
        return true;
    }
    else
    {
        return false;
    }
}

//== 音效控制 ==
function Play_Coin()
{
    var audio = new Audio("http://taira-komori.jpn.org/sound_os/game01/coin07.mp3");
    audio.play();
}
function Play_Crrect()
{
    var audio = new Audio("http://taira-komori.jpn.org/sound_os/game01/crrect_answer2.mp3");
    audio.play();
}
function Play_Bright()
{
    var audio = new Audio("http://taira-komori.jpn.org/sound/anime01/bright_bell1.mp3");
    audio.play();
}


function Tix_Get_Answer_From_SQL()
{
    var mHref = document.location.href;
    if(StringContain(mHref , "verify"))
    {
        var fData = new FormData();
        var xhr = new XMLHttpRequest();
        xhr.open("GET","https://114.33.187.132/shareAnswer/getAnswer.php");
        xhr.send(fData);
        xhr.onreadystatechange = function()
        {
            if(xhr.readyState == 4)
            {
                if(xhr.status == 200)
                {
                    var sAnswer = "";

                    //判斷是否有無答案(雲端SQL上)
                    if(xhr.responseText == ""){sAnswer = "nodata";}

                    //==下載最新考試答案==
                    else{  sAnswer = Get_Newest_Answer(xhr.responseText); }

                    //==提交==
                    if(sAnswer != undefined)
                    {
                       Tix_KeyInQuestionAnswer(sAnswer);
                    }
                    else
                    {
                        RefreshWindow();
                    }
                    //console.log(xhr.responseText);
                }
                else
                {
                    //console.log("SQL no response");
                    alert("SQL no response");
                }
            }
            else
            {
                alert("case2222");

            }

        };
    }
}

function Get_Newest_Answer(pagesource_php)
{
    var list_SQL = Split_SQL_Response_to_Answer(pagesource_php);
    return list_SQL[list_SQL.length-1];
}

function Split_SQL_Response_to_Answer(pagesource_php)
{
    var strAfterArray =  pagesource_php;
    var myList = [];
    while(true)
    {
        var iTableArrayLeft = strAfterArray.indexOf("{"); //陣列頭);
        var iTableArrayRight = strAfterArray.indexOf("}"); //陣列尾
        if(iTableArrayLeft == -1 || iTableArrayRight == -1)
        {
            break;
        }
        else
        {
            var strSubText = strAfterArray.substring(iTableArrayLeft+1,iTableArrayRight-1);
            var strSp = strSubText.split(',')[0].split(':')[1].replace(new RegExp("\"", "g") , "");
            myList.push(strSp);
            var spNext = strAfterArray.substr(iTableArrayRight+1);
            strAfterArray = spNext;
        }
    }
    return myList;
}


    //======參數設定======= -->有開放的按鈕
    //== 日期設定 ==
    // 0 = 第一天
    // 1 = 第二天
    // 2 = 第三天
    var vFlagDays = 1;

    //== 座位參數設定 ==
    // 空白   = 全數隨機座位
    // 有內容 = 模糊搜尋後隨機抽樣
    var vFlagSeatText = "1000";


    //== 座位參數設定 ==
    //  張數設定
    var vFlagTicketCount = 1;

    //== 付款模式選擇 ==
    // -1 = 不選擇
    // 0  = ATM付款
    // 1  = ibon付款
    // 2  = 信用卡
    var vFlagPayType = -1;



    //===================== 流程控制  =================================

    // == 自動打勾 & 選張數 ==
    PreSubmit(vFlagTicketCount);

    //== 選日期 ==
    GoDays(vFlagDays);
    //== 選座位 ==
    GoSeatByRndom_Matched(vFlagSeatText);
    // == 自動導向到準備頁面 ==
    AutoDirect_GAME();

    // == 打驗證碼 ==
    //Tix_KeyInVerifyCode("test");

    //==自動選擇ATM付款==
    // 0 => ATM付款   1 = ibon付款  2 = 信用卡
    AutoSelect_PayType(vFlagPayType);

    //setInterval(Tix_Get_Answer_From_SQL , 800);



    // == 取得CSRFTOKEN資料 (有登入Google or FB才會有這個欄位 )==
    //CsrCode = GetCSRFTOKEN();
    //alert("CSRFTOKEN is  ===>  " +  CsrCode);
    //unsafeWindow.SomeVarInPage = "areaUrlList";

