using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestWechat
{
    public class IniHelper
    {
        private readonly string _inipath;

        public IniHelper(string iniPath)
        {
            _inipath = iniPath;
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
            int size, string filePath);

        public void IniWriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, _inipath);
        }

        public string IniReadValue(string section, string key)
        {
            var temp = new StringBuilder(500);
            var i = GetPrivateProfileString(section, key, "", temp, 500, _inipath);
            return temp.ToString();
        }
    }
}
