using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Management;
using System.Net.NetworkInformation;
namespace SQLTest
{
    public static class HDDInformation
    {
        public static string GetDevice()
        {
            string DevicesInfo = "";
            ManagementObjectSearcher cpuInfo = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject cpu in cpuInfo.Get())
            {
                DevicesInfo = cpu["ProcessorId"].ToString();
            }
            //PC Name + 完整CPU
            //DevicesInfo = System.Windows.Forms.SystemInformation.ComputerName + "_" + DevicesInfo;
            //CPU後四碼
            //DevicesInfo = DevicesInfo.Substring(DevicesInfo.Length - 4, 4); 
            //PCName  + CPU後四碼
            DevicesInfo = System.Windows.Forms.SystemInformation.ComputerName + DevicesInfo.Substring(DevicesInfo.Length - 4, 4);


            //網卡號碼
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            List<string> macList = new List<string>();
            foreach (var nic in nics)
            {
                // 因為電腦中可能有很多的網卡(包含虛擬的網卡)，
                // 我只需要 Ethernet 網卡的 MAC
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    macList.Add(nic.GetPhysicalAddress().ToString());
                }
            }


            if (macList.Count > 0)
            {
                return DevicesInfo + macList[0];
            }
            else
            {
                return DevicesInfo + " do not have hdd.";
            }
        }

    }
}
