using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechat
{
    public class Contact
    {
        [DisplayName("昵称")]
        public virtual string NickName { get; set; }
        [DisplayName("微信wxid")]
        public string WxId { get; set; }
        [DisplayName("微信号")]
        public string WxNum { get; set; }
        [DisplayName("备注")]
        public string RemarkName { get; set; }
    }
}
