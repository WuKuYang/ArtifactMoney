using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace YDMCSDemo
{
    public partial class FormMain : Form
    {
        bool isLogin = false;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            textBox_FileName.Text = Application.StartupPath + "\\getimage.jpg";
        }

        public void button_YDM_SetAppInfo_Click(object sender, EventArgs e)
        {
            // 测试时可直接使用默认的软件ID密钥，但要享受开发者分成必须使用自己的软件ID和密钥
            // 1. http://www.yundama.com/index/reg/developer 注册开发者账号
            // 2. http://www.yundama.com/developer/myapp 添加新软件
            // 3. 使用添加的软件ID和密钥进行开发，享受丰厚分成

            int nAppId;         // 软件ＩＤ，开发者分成必要参数。登录开发者后台【我的软件】获得！
            string lpAppKey;    // 软件密钥，开发者分成必要参数。登录开发者后台【我的软件】获得！

            nAppId = parseInt(textBox_AppId.Text);
            lpAppKey = textBox_AppKey.Text;

            YDMWrapper.YDM_SetAppInfo(nAppId, lpAppKey);

            //MessageBox.Show("初始化成功");
        }

        public  void button_YDM_Login_Click(object sender, EventArgs e)
        {
            // 注意这里是普通会员账号，不是开发者账号，注册地址 http://www.yundama.com/index/reg/user
            // 开发者可以联系客服领取免费调试题分

            string username, password;
            int ret;

            username = textBox_UserName.Text;
            password = textBox_PassWord.Text;

            // 返回云打码用户UID，大于零为登录成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            ret = YDMWrapper.YDM_Login(username, password);

            if (ret > 0)
            {
                isLogin = true;
                //MessageBox.Show("登陆成功");
            }
            else
            {
                isLogin = false;
                MessageBox.Show("登陆失败，错误代码：" + ret.ToString(), "登陆失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_YDM_DecodeByPath_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(1))
                return;

            string lpFilePath;
            int nCodeType;
            int nCaptchaId;
            StringBuilder pCodeResult = new StringBuilder(new string(' ', 30)); // 分配30个字节存放识别结果

            textBox_cid.Text = "正在识别...";
            textBox_Result.Text = "";
            this.Refresh();

            // 图片路径
            lpFilePath = textBox_FileName.Text;

            // 例：1004表示4位字母数字，不同类型收费不同。请准确填写，否则影响识别率。在此查询所有类型 http://www.yundama.com/price.html
            nCodeType = parseInt(textBox_CodeType.Text);  

            // 返回验证码ID，大于零为识别成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            nCaptchaId = YDMWrapper.YDM_DecodeByPath(lpFilePath, nCodeType, pCodeResult);

            textBox_Result.Text = pCodeResult.ToString();
            textBox_cid.Text = nCaptchaId.ToString();
        }

        private void button_YDM_EasyDecodeByPath_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(2))
                return;

            int nCodeType, nCaptchaId, nAppId, nTimeOut;
            string username, password, lpFilePath, lpAppKey;
            StringBuilder pCodeResult = new StringBuilder(new string(' ', 30)); // 分配30个字节存放识别结果

            textBox_cid.Text = "正在识别...";
            textBox_Result.Text = "";
            this.Refresh();

            // 一键版本无需调用 YDM_SetAppInfo 和 YDM_Login，但需传入软件ID密钥等4个参数
            username = textBox_UserName.Text;
            password = textBox_PassWord.Text;
            nAppId = parseInt(textBox_AppId.Text);
            lpAppKey = textBox_AppKey.Text;

            // 图片路径
            lpFilePath = textBox_FileName.Text;

            // 例：1004表示4位字母数字，不同类型收费不同。请准确填写，否则影响识别率。在此查询所有类型 http://www.yundama.com/price.html
            nCodeType = parseInt(textBox_CodeType.Text);   

            // 超时时间，单位：秒
            nTimeOut = 60;

            // 返回验证码ID，大于零为识别成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            nCaptchaId = YDMWrapper.YDM_EasyDecodeByPath(username, password, nAppId, lpAppKey, lpFilePath, nCodeType, nTimeOut, pCodeResult);

            textBox_Result.Text = "一键识别成功：" + pCodeResult.ToString();
            textBox_cid.Text = nCaptchaId.ToString();
        }

        public string YDM_EasyDecodeByByte(Byte[] bCodeImage)
        {

            if (!checkInputParam(2))
                return "";

            int nCodeType, nCaptchaId, nAppId, nTimeOut;
            string username, password, lpFilePath, lpAppKey;
            StringBuilder pCodeResult = new StringBuilder(new string(' ', 30)); // 分配30个字节存放识别结果

            textBox_cid.Text = "正在识别...";
            textBox_Result.Text = "";
            this.Refresh();

            // 一键版本无需调用 YDM_SetAppInfo 和 YDM_Login，但需传入软件ID密钥等4个参数
            username = textBox_UserName.Text;
            password = textBox_PassWord.Text;
            nAppId = parseInt(textBox_AppId.Text);
            lpAppKey = textBox_AppKey.Text;

            // 图片路径
            lpFilePath = textBox_FileName.Text;

            // 例：1004表示4位字母数字，不同类型收费不同。请准确填写，否则影响识别率。在此查询所有类型 http://www.yundama.com/price.html
            nCodeType = parseInt(textBox_CodeType.Text);

            // 超时时间，单位：秒
            nTimeOut = 3;

            // 返回验证码ID，大于零为识别成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            nCaptchaId = YDMWrapper.YDM_EasyDecodeByBytes(username, password, nAppId, lpAppKey, bCodeImage,bCodeImage.Length , nCodeType, nTimeOut, pCodeResult);

            textBox_Result.Text = "一键识别成功：" + pCodeResult.ToString();
            textBox_cid.Text = nCaptchaId.ToString();
            return pCodeResult.ToString();
        }


        private void button_YDM_UploadByPath_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(1))
                return;

            string lpFilePath;
            int nCodeType;
            int nCaptchaId;

            textBox_cid.Text = "正在上传...";
            textBox_Result.Text = "";
            this.Refresh();

            // 图片路径
            lpFilePath = textBox_FileName.Text;

            // 例：1004表示4位字母数字，不同类型收费不同。请准确填写，否则影响识别率。在此查询所有类型 http://www.yundama.com/price.html
            nCodeType = parseInt(textBox_CodeType.Text);

            // 返回验证码ID，大于零为识别成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            nCaptchaId = YDMWrapper.YDM_UploadByPath(lpFilePath, nCodeType);

            if (nCaptchaId > 0)
            {
                textBox_Result.Text = "上传成功，正在拉取识别结果...";
                button_YDM_UploadByPath.Enabled = false;
                button_YDM_UploadByBytes.Enabled = false;

                // 每秒拉取一次识别结果，请参考【timer_Result_Tick】
                timer_Result.Interval = 1000;
                timer_Result.Start();
            }

            textBox_cid.Text = nCaptchaId.ToString();
        }

        private void button_YDM_DecodeByBytes_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(1))
                return;

            int nCodeType, nCaptchaId;
            StringBuilder pCodeResult = new StringBuilder(new string(' ', 30)); // 分配30个字节存放识别结果

            textBox_cid.Text = "正在识别...";
            textBox_Result.Text = "";
            this.Refresh();

            // 读取验证码图片
            FileInfo fi = new FileInfo(textBox_FileName.Text);
            long len = fi.Length;
            FileStream fs = new FileStream(textBox_FileName.Text, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[len];
            fs.Read(buffer, 0, (int)len);
            fs.Close();

            // 例：1004表示4位字母数字，不同类型收费不同。请准确填写，否则影响识别率。在此查询所有类型 http://www.yundama.com/price.html
            nCodeType = parseInt(textBox_CodeType.Text);

            // 返回验证码ID，大于零为识别成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            nCaptchaId = YDMWrapper.YDM_DecodeByBytes(buffer, (int)len, nCodeType, pCodeResult);

            textBox_Result.Text = pCodeResult.ToString();
            textBox_cid.Text = nCaptchaId.ToString();
        }

        private void button_YDM_EasyDecodeByBytes_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(2))
                return;

            string username, password, lpAppKey;
            int nCodeType, nCaptchaId, nAppId, nTimeOut;
            StringBuilder pCodeResult = new StringBuilder(new string(' ', 30)); // 分配30个字节存放识别结果

            textBox_cid.Text = "正在识别...";
            textBox_Result.Text = "";
            this.Refresh();

            // 读取验证码图片
            FileInfo fi = new FileInfo(textBox_FileName.Text);
            long len = fi.Length;
            FileStream fs = new FileStream(textBox_FileName.Text, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[len];
            fs.Read(buffer, 0, (int)len);
            fs.Close();

            // 一键版本无需调用 YDM_SetAppInfo 和 YDM_Login，但需传入软件ID密钥等4个参数
            username = textBox_UserName.Text;
            password = textBox_PassWord.Text;
            nAppId = parseInt(textBox_AppId.Text);
            lpAppKey = textBox_AppKey.Text;

            // 例：1004表示4位字母数字，不同类型收费不同。请准确填写，否则影响识别率。在此查询所有类型 http://www.yundama.com/price.html
            nCodeType = parseInt(textBox_CodeType.Text);

            // 超时时间，单位：秒
            nTimeOut = 60;

            // 返回验证码ID，大于零为识别成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            nCaptchaId = YDMWrapper.YDM_EasyDecodeByBytes(username, password, nAppId, lpAppKey, buffer, (int)len, nCodeType, nTimeOut, pCodeResult);

            textBox_Result.Text = "一键识别成功：" + pCodeResult.ToString();
            textBox_cid.Text = nCaptchaId.ToString();
        }

        private void button_YDM_UploadByBytes_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(1))
                return;

            int nCodeType, nCaptchaId;

            textBox_cid.Text = "正在上传...";
            textBox_Result.Text = "";
            this.Refresh();

            // 读取验证码图片
            FileInfo fi = new FileInfo(textBox_FileName.Text);
            long len = fi.Length;
            FileStream fs = new FileStream(textBox_FileName.Text, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[len];
            fs.Read(buffer, 0, (int)len);
            fs.Close();

            // 例：1004表示4位字母数字，不同类型收费不同。请准确填写，否则影响识别率。在此查询所有类型 http://www.yundama.com/price.html
            nCodeType = parseInt(textBox_CodeType.Text);

            // 返回验证码ID，大于零为识别成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            nCaptchaId = YDMWrapper.YDM_UploadByBytes(buffer, (int)len, nCodeType);

            if (nCaptchaId > 0)
            {
                textBox_Result.Text = "上传成功，正在拉取识别结果...";
                button_YDM_UploadByPath.Enabled = false;
                button_YDM_UploadByBytes.Enabled = false;

                // 每秒拉取一次识别结果，请参考【timer_Result_Tick】
                timer_Result.Interval = 1000;
                timer_Result.Start();
            }

            textBox_cid.Text = nCaptchaId.ToString();
        }

        private void timer_Result_Tick(object sender, EventArgs e)
        {
            int nCaptchaId, ret;
            StringBuilder pCodeResult = new StringBuilder(new string(' ', 30)); // 分配30个字节存放识别结果

            // DM_UploadByPath、YDM_UploadByBytes 的返回值
            nCaptchaId = parseInt(textBox_cid.Text);

            // 异步获取结果，本函数需结合异步上传函数 YDM_UploadByPath、YDM_UploadByBytes 使用，本函数一般放在循环体里调用，每调用一次延迟1000毫秒，直到获取到识别结果为止
            // 一般情况下如果返回的错误代码不是 -3002（正在识别）的话，延迟1000毫秒后再调用本函数，直到函数返回零
            ret = YDMWrapper.YDM_GetResult(nCaptchaId, pCodeResult);

            // 错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            if (ret == 0) {
                textBox_Result.Text = "异步识别成功：" + pCodeResult.ToString();
                timer_Result.Stop();    // 停止拉取识别结果
                button_YDM_UploadByPath.Enabled = true;
                button_YDM_UploadByBytes.Enabled = true;
            }
            else if (ret == 3002)
            {
                // 正在识别，继续拉取结果
            } else {
                textBox_Result.Text = "异步识别错误代码：" + ret.ToString();
                timer_Result.Stop();    // 停止拉取识别结果
                button_YDM_UploadByPath.Enabled = true;
                button_YDM_UploadByBytes.Enabled = true;
            }
        }

        private void button_YDM_Report_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(1))
                return;

            int nCaptchaId, ret;
            bool bCorrect;

            nCaptchaId = parseInt(textBox_cid.Text);

            DialogResult result =
                MessageBox.Show("识别正确请点击【是】，识别错误请点击【否】\r\n\r\n恶意报错会导致开发者账号被封或不予结算！识别正确的可以不用提交！", "识别报错", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bCorrect = true;   // 识别正确传入true，识别正确可以忽略不报
            }
            else 
            {
                bCorrect = false;   // 识别错误传入false，云打码将返还题分，恶意报错的开发者账号系统将自动封号！
            }

            // 返回零成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            ret = YDMWrapper.YDM_Report(nCaptchaId, bCorrect);

            if (ret == 0)
            {
                MessageBox.Show("汇报成功！");
            }
            else
            {
                MessageBox.Show("汇报失败！错误代码：" + ret.ToString(), "汇报失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_YDM_EasyReport_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(2))
                return;

            string username, password, lpAppKey;
            int nCaptchaId, nAppId, ret;
            bool bCorrect;

            // 一键版本无需调用 YDM_SetAppInfo 和 YDM_Login，但需传入软件ID密钥等4个参数
            username = textBox_UserName.Text;
            password = textBox_PassWord.Text;
            nAppId = parseInt(textBox_AppId.Text);
            lpAppKey = textBox_AppKey.Text;

            nCaptchaId = parseInt(textBox_cid.Text);

            DialogResult result =
                MessageBox.Show("识别正确请点击【是】，识别错误请点击【否】\r\n\r\n恶意报错会导致开发者账号被封或不予结算！识别正确的可以不用提交！", "识别报错", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                bCorrect = true;   // 识别正确传入true，识别正确可以忽略不报
            }
            else
            {
                bCorrect = false;   // 识别错误传入false，云打码将返还题分，恶意报错的开发者账号系统将自动封号！
            }

            // 返回零成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            ret = YDMWrapper.YDM_EasyReport(username, password, nAppId, lpAppKey, nCaptchaId, bCorrect);

            if (ret == 0)
            {
                MessageBox.Show("汇报成功！");
            }
            else
            {
                MessageBox.Show("汇报失败！错误代码：" + ret.ToString(), "汇报失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_YDM_GetBalance_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(1))
                return;

            string username, password;
            int balance;

            username = textBox_UserName.Text;
            password = textBox_PassWord.Text;

            // 返回用户剩余题分，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            balance = YDMWrapper.YDM_GetBalance(username, password);

            MessageBox.Show("【" + username + "】的账号余额为：" + balance.ToString());
        }

        private void button_YDM_EasyGetBalance_Click(object sender, EventArgs e)
        {
            if (!checkInputParam(2))
                return;

            string username, password, lpAppKey;
            int nAppId, balance;

            // 一键版本无需调用 YDM_SetAppInfo 和 YDM_Login，但需传入软件ID密钥等4个参数
            username = textBox_UserName.Text;
            password = textBox_PassWord.Text;
            nAppId = parseInt(textBox_AppId.Text);
            lpAppKey = textBox_AppKey.Text;

            // 返回用户剩余题分，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            balance = YDMWrapper.YDM_EasyGetBalance(username, password, nAppId, lpAppKey);

            MessageBox.Show("【" + username + "】的账号余额为：" + balance.ToString());
        }

        private void button_SetTimeOut_Click(object sender, EventArgs e)
        {
            int nTimeOut = 0;

            // 超时时间，单位：秒
            nTimeOut = parseInt(textBox_TimeOut.Text);

            YDMWrapper.YDM_SetTimeOut(nTimeOut);

            MessageBox.Show("成功设置超时时间为 " + nTimeOut + " 秒");
        }

        private void button_YDM_Reg_Click(object sender, EventArgs e)
        {
            string RegUser, RegPass, RegMail, RegMobile, RegQQ;
            int uid, ret;

            RegUser = textBox_RegUser.Text;
            RegPass = textBox_RegPass.Text;
            RegMail = textBox_RegMail.Text;
            RegMobile = textBox_RegMobile.Text; // 可留空
            RegQQ = textBox_RegQQ.Text; // 可留空

            // 返回新用户UID，大于零为注册成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            uid = YDMWrapper.YDM_Reg(RegUser, RegPass, RegMail, RegMobile, RegQQ);

            if (uid > 0)
            {
                MessageBox.Show("成功注册新用户，用户名：" + RegUser + "，新用户ID：" + uid.ToString());
            }
            else
            {
                ret = uid;
                MessageBox.Show("注册新用户失败，错误代码：" + ret.ToString(), "注册失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_YDM_EasyReg_Click(object sender, EventArgs e)
        {
            string lpAppKey, RegUser, RegPass, RegMail, RegMobile, RegQQ;
            int nAppId, uid, ret;

            nAppId = parseInt(textBox_AppId.Text);
            lpAppKey = textBox_AppKey.Text;

            RegUser = textBox_RegUser.Text;
            RegPass = textBox_RegPass.Text;
            RegMail = textBox_RegMail.Text;
            RegMobile = textBox_RegMobile.Text; // 可留空
            RegQQ = textBox_RegQQ.Text; // 可留空

            // 返回新用户UID，大于零为注册成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            uid = YDMWrapper.YDM_EasyReg(nAppId, lpAppKey, RegUser, RegPass, RegMail, RegMobile, RegQQ);

            if (uid > 0)
            {
                MessageBox.Show("成功注册新用户，用户名：" + RegUser + "，新用户ID：" + uid.ToString());
            }
            else
            {
                ret = uid;
                MessageBox.Show("注册新用户失败，错误代码：" + ret.ToString(), "注册失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_YDM_Pay_Click(object sender, EventArgs e)
        {
            string PayUser, PayPass, PayCard;
            int ret;

            PayUser = textBox_PayUser.Text;
            PayPass = textBox_PayPass.Text;
            PayCard = textBox_PayCard.Text;

            // 返回充值结果，为零即为充值成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            ret = YDMWrapper.YDM_Pay(PayUser, PayPass, PayCard);

            if (ret > 0)
            {
                MessageBox.Show("充值成功");
            }
            else
            {
                MessageBox.Show("充值失败，错误代码：" + ret.ToString(), "充值失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_YDM_EasyPay_Click(object sender, EventArgs e)
        {
            string lpAppKey, PayUser, PayPass, PayCard;
            int nAppId, ret;

            nAppId = parseInt(textBox_AppId.Text);
            lpAppKey = textBox_AppKey.Text;

            PayUser = textBox_PayUser.Text;
            PayPass = textBox_PayPass.Text;
            PayCard = textBox_PayCard.Text;

            // 返回充值结果，为零即为充值成功，返回其他错误代码请查询 http://www.yundama.com/apidoc/YDM_ErrorCode.html
            ret = YDMWrapper.YDM_EasyPay(PayUser, PayPass, nAppId, lpAppKey, PayCard);

            if (ret > 0)
            {
                MessageBox.Show("充值成功");
            }
            else
            {
                MessageBox.Show("充值失败，错误代码：" + ret.ToString(), "充值失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void linkLabel_Type_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.yundama.com/price.html");
        }

        private void linkLabel_Dev_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.yundama.com/index/reg/developer");
        }

        private void linkLabel_User_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.yundama.com/index/reg/user");
        }

        private void linkLabel_Error_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.yundama.com/apidoc/YDM_ErrorCode.html");
        }

        private void button_Select_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ImgDialog = new OpenFileDialog())
            {
                ImgDialog.Title = "选择图片";
                ImgDialog.Filter = "图片文件 (*.jpg)|*.jpg|所有文件 (*.*)|*.*";
                if (ImgDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBox_FileName.Text = ImgDialog.FileName;
                    pictureBox_Captcha.Image = Image.FromFile(ImgDialog.FileName);
                }
            }
        }

        private bool checkInputParam(int paramtype)
        {
            if (paramtype == 1)
            {
                if (isLogin == true)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("您还未登录云打码账号", "参数错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            else
            {
                if (textBox_AppId.Text.Length == 0 || textBox_AppKey.Text.Length == 0)
                {
                    MessageBox.Show("请填写好【软件ID】【软件密钥】后再试\r\n\r\n提示：一键版本无需初始化和登录", "参数错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                if (textBox_UserName.Text.Length == 0 || textBox_PassWord.Text.Length == 0)
                {
                    MessageBox.Show("请填写好【用户账号】【用户密码】后再试\r\n\r\n提示：一键版本无需初始化和登录", "参数错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }
        }

        private int parseInt(string s)
        {
            try
            {
                return int.Parse(s);
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }

        private void textBox_cid_TextChanged(object sender, EventArgs e)
        {
            if (textBox_cid.Text.Substring(0, 1) == "-")
            {
                textBox_Result.Text = "请在官方开发文档查询错误代码含义";
            }
        }
    }
}


