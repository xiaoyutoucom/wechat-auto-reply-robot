using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechat
{
    public class Group : Contact
    {

        public override string NickName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(base.NickName)) return base.NickName;
                if (Members == null || Members.Count <= 0) return base.NickName;
                var max = Members.Count > 4 ? 4 : Members.Count;
                var newName = "";
                for (var i = 0; i < max; i++)
                {
                    newName = newName + Members[i].NickName;
                    if (i != max - 1) newName += ",";
                }
                base.NickName = newName;
                return base.NickName;
            }

            set { base.NickName = value; }
        }

        public BindingList<Contact> Members { get; set; }

        public Contact GetMember(string wxid)
        {
            if (Members == null) return null;
            foreach (Contact item in Members)
            {
                if (item.WxId == wxid) return item;
            }
            return null;
        }
    }
}
