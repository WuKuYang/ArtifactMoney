using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YDMCSDemo
{
    public static class AutoKeyIn
    {
        public static FormMain frm = new FormMain();

        public static void AppInfo()
        {
            frm.button_YDM_SetAppInfo_Click(null , null);
        }
        public static void Login()
        {
            frm.button_YDM_Login_Click(null, null);
        }

        public static string AutoCheckCode(Byte[] bCodeImage)
        {
            return frm.YDM_EasyDecodeByByte(bCodeImage);
        }

    }
}
