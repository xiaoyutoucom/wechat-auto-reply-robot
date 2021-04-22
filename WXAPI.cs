using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWechat
{
    public class WXAPI
    {
        public static IntPtr FriendOrGroup = (IntPtr)0x10099;
        public static IntPtr LoginOk = (IntPtr)0x1000BB;
        public static IntPtr Message = (IntPtr)0x10000;
        public static IntPtr GroupMember = (IntPtr)0x10098;
        public IntPtr WXHandle { get; set; }

        public string ContactFile { get; set; }

        public WXAPI(IntPtr handle, string file)
        {
            WXHandle = handle;
            ContactFile = file;
        }

        // 设置通讯接口
        public void SetCommunicationInterface(IntPtr handle)
        {
            WatchLog_Print(0xB0001, handle.ToString());
        }

        // 开启消息监听
        public void StartMonitorMsg()
        {
            WatchLog_Print(0xA0000, ContactFile);
        }

        // 获取我的消息
        public void GetMyInfo()
        {
            WatchLog_Print(0x10004, ContactFile);
        }

        // 关闭消息监听
        public void EndMonitorMsg()
        {
            WatchLog_Print(0xA0001, ContactFile);
        }

        // 通过微信号发送消息
        public void SendMsgWithWXAccount(string account, string content)
        {
            WatchLog_Print(0x10003, account + "`" + content + "`68");
        }

        // 通过昵称发送消息
        public void SendMsgWithNickname(string nickname, string content)
        {
            WatchLog_Print(0x10003, nickname + "`" + content + "`140");
        }

        // 通过微信ID发送消息
        public void SendMsgWithWIXD(string wxid, string content)
        {
            WatchLog_Print(0x10003, wxid + "`" + content + "`16");
        }

        // 枚举聊天室
        public void EnumChatRoom()
        {
            WatchLog_Print(0x10002, ContactFile);
        }

        // 枚举聊天室成员
        public void EnumChatRoomMember(string chatroomID)
        {
            WatchLog_Print(0xA10002, chatroomID + "=" + ContactFile);
        }

        // 枚举内容
        public void EnumContact()
        {
            WatchLog_Print(0x10001, ContactFile);
        }
        public void AutoAgreeFriendOn()
        {
            WatchLog_Print(0xA0002, ContactFile, ImportFromDLL.WM_KEYDOWN);
        }
        public void AutoAgreeFriendOff()
        {
            WatchLog_Print(0xA0003, ContactFile, ImportFromDLL.WM_KEYDOWN);
        }
        public void SendGroupMsgAperson(string gwxid, string nickname, string awxid, string msg)
        {
            WatchLog_Print(0x100033, gwxid + "`" + nickname + " " + msg + "`" + awxid);
        }
        public void AddToMyGroup(string gwxid, string awxid)
        {
            WatchLog_Print(0x10008, gwxid + "=" + awxid);
        }
        // 消息打印
        public void WatchLog_Print(int logType, string logStr, int mark = ImportFromDLL.WM_COPYDATA)
        {
            ImportFromDLL.COPYDATASTRUCT msg = new ImportFromDLL.COPYDATASTRUCT
            {
                cbData = Encoding.Default.GetBytes(logStr).Length + 1, //长度 注意不要用strText.Length;  
                lpData = logStr
            };
            ImportFromDLL.SendMessage(WXHandle, mark, (IntPtr)logType, ref msg);
        }
    }
}
