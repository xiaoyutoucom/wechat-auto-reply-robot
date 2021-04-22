using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TestWechat
{
    public class XMLHelper
    {
        public static XmlNode ParseMsg(string msg)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(msg);
            return xmlDoc.SelectSingleNode("/msg");
        }
    }
}
