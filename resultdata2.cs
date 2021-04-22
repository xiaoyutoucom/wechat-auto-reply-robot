using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechat
{
    public class NewslistItem
    {
        /// <summary>
        /// 亲爱的{appellation}你好，我叫{robotname}，性别{robotsex}，来自{hometown}，正在从事{robotwork}工作。{constellation}的我，爱好{robothobby}也喜欢和人类做朋友！
        /// </summary>
        public string reply { get; set; }
    }

    public class resultdata2
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string datatype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<NewslistItem> newslist { get; set; }
    }

}
