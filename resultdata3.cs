using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechat
{
    public class Info
    {
        /// <summary>
        /// 姚明的身高是226厘米
        /// </summary>
        public string text { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Info info { get; set; }
    }

    public class resultdata3
    {
        /// <summary>
        /// 
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
    }

}
