using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TestWechat.Weixin
{
    public class WeMessage
    {
        public string wxid { get; set; }
        public string zsid { get; set; }
        public string MessageContent { get; set; }

        public static WeMessage Parse(string orimess)
        {
            WeMessage wm = new WeMessage();
            wm.wxid = Regex.Match(orimess, @"(?<=ID=)([^\,]+)(?=\,)").ToString();
            wm.zsid = Regex.Match(orimess, @"(?<=ID2=)([^\,]+)(?=\,)").ToString();
            wm.MessageContent = Regex.Match(orimess, @"(?<=,内容=)(.*)", RegexOptions.Singleline).ToString();
            return wm;
        }
    }
}
