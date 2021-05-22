using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using TestWechat.Weixin;
using System.Speech.Synthesis;
using Npgsql;
using System.Data;

namespace TestWechat
{
    public partial class Form1 : Form
    {
        // MyFiddler的实例副本，用于做消息输出
        public static Form1 instance = null;

        // 微信是否登陆成功的标记
        private bool wechatLoginSuccess = false;                                                      //
        private IntPtr wechatWindowHandler = IntPtr.Zero;                                             //
        private ImportFromDLL.STARTUPINFO si = new ImportFromDLL.STARTUPINFO();                       //
        private ImportFromDLL.PROCESS_INFORMATION pi = new ImportFromDLL.PROCESS_INFORMATION();       //
        //
        private readonly BackgroundWorker _bw;                                                        //多线程读取文件
        private readonly string _dllName;                                                             //dll文件
        private readonly string _logName;                                                             //log文件
        private readonly string _wechatexe;                                                             //log文件
        private WXAPI _api;                                                                             //api
        private readonly IniHelper _iniHelper;
        private BindingList<Contact> _friendList;
        private BindingList<Contact> _publicList;
        private BindingList<Group> _groupList;
        private IntPtr _thisHandler;
        private bool reback =false;
        private bool reback2;
        private bool fanti = false;
        List<string> room = new List<string>();
        private string  reback3 = "1";
        private string reply;
        private int cishu = 0;
        private bool ifsave;
        private string[] split;
        private bool gushi;

        public Form1()
        {
            InitializeComponent();

            instance = this;
            _dllName = Application.StartupPath + "/WeChat.dll";
            _logName = Application.StartupPath + "/msg.ini";
            _wechatexe = Application.StartupPath + "/WeChat/WeChat.exe";
            _iniHelper = new IniHelper(_logName);
            _friendList = new BindingList<Contact>();
            _publicList = new BindingList<Contact>();
            _groupList = new BindingList<Group>(); 
            _thisHandler = this.Handle;
            FileStream fs = new FileStream(@"c:\setting.ini", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);

            sw.WriteLine("[Student]");
            sw.WriteLine("nAge=" + this.Handle);
            sw.Close();
            Debug.WriteLine(this.Handle);
            

            //线程
            _bw = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };
            _bw.DoWork += readMessage;
            _bw.ProgressChanged += readMessaging;
            _bw.RunWorkerCompleted += Compleate;

        }

        private void readMessage(object sender, DoWorkEventArgs e)
        {
            PrintMessage(e.Result as string);
        }

        private void readMessaging(object sender,
            ProgressChangedEventArgs e)
        {
            PrintMessage(e.ProgressPercentage.ToString());
        }

        private void Compleate(object sender, RunWorkerCompletedEventArgs e)
        {
            PrintMessage(e.Result as string);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        public void PrintMessage(String message)
        {
            jsh(message);
            //ifreback(message);
           
            if (this.LogMessageBox.Disposing || this.LogMessageBox.IsDisposed) return;
            if (this.LogMessageBox.InvokeRequired)
            {
                this.LogMessageBox.Invoke(new Action<TextBox, String>((ct, v) => { ct.AppendText(v + Environment.NewLine); }), new object[] { this.LogMessageBox, message });
            }
            else
            {
                this.LogMessageBox.AppendText(message + Environment.NewLine);
            } 
        }


        private void MainWindowShown(object sender, EventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (sender1, e1) =>
            {
                if (ImportFromDLL.CreateProcess(_wechatexe, null, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi))
                {
                    injectDllToWechatHandler();
                    while (!this.wechatLoginSuccess)
                    {
                        wechatWindowHandler = ImportFromDLL.FindWindow("WeChatMainWndForPC", "微信");
                        if (wechatWindowHandler != IntPtr.Zero)
                        {
                            this.wechatLoginSuccess = true;
                            PrintMessage("微信登陆成功!");
                            ImportFromDLL.SetForegroundWindow(wechatWindowHandler);
                            ImportFromDLL.ShowWindow(wechatWindowHandler, 1);

                            //ChangeButtonEnable(this.OpenMonitorButton, true);
                            break;
                        }
                        else
                        {
                            Thread.Sleep(1000);//休眠1秒后再次查看操作结果
                        }
                    }
                }
            };
            bw.RunWorkerCompleted += (sender1, e1) =>
            {
                 
            };
            bw.RunWorkerAsync();
        }

        private void ReadMyinfo()
        {
            string mywxid = null;
            string mywxname = null;
            var bw = new BackgroundWorker();
            bw.DoWork += (sender1, e1) =>
            {
                PrintMessage("我的信息获取工作开始啦！");
                while (true)
                {
                    PrintMessage("真正的开始");
                    mywxid = _iniHelper.IniReadValue("自身微信信息", "微信号");
                    mywxname = _iniHelper.IniReadValue("自身微信信息", "微信昵称");
                    if (!string.IsNullOrEmpty(mywxname))
                    { 
                        break;
                    }
                    Thread.Sleep(1000);
                }

            };
            bw.RunWorkerCompleted += (sender1, e1) =>
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<Form1, String>((ct, v) => { ct.Text = v; }), new object[] { this, mywxname });
                }
                else
                {
                    this.Text = mywxname;
                }

                PrintMessage("我的信息:" + mywxid + "," + mywxname);

                _api.EnumContact();

            };
            bw.RunWorkerAsync();
        }

        // 向微信句柄注入WeChat.dll
        private void injectDllToWechatHandler()
        {

            IntPtr lpRemoteBuf = ImportFromDLL.VirtualAllocEx(pi.hProcess,
                IntPtr.Zero,
                (uint)((_dllName.Length + 1) * Marshal.SizeOf(typeof(char))),
                ImportFromDLL.AllocationType.Commit | ImportFromDLL.AllocationType.Reserve,
                ImportFromDLL.MemoryProtection.ReadWrite);

            IntPtr pfnStartAddr = ImportFromDLL.GetProcAddress(ImportFromDLL.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (pfnStartAddr == IntPtr.Zero)
            {
                PrintMessage("GetProcAddress error\n");
                return;
            }

            byte[] code = Encoding.Default.GetBytes(_dllName);

            if (ImportFromDLL.WriteProcessMemory(pi.hProcess, lpRemoteBuf, code, code.Length, 0))
            {
                PrintMessage("进程注入成功!");
            }
            ImportFromDLL.CreateRemoteThread(pi.hProcess, IntPtr.Zero, 0, pfnStartAddr, lpRemoteBuf, 0, IntPtr.Zero);
            ImportFromDLL.CloseHandle(pi.hProcess);
            ImportFromDLL.CloseHandle(pi.hThread);
        }

        protected override void WndProc(ref Message m)
        { 
            Console.WriteLine(m.Msg + "||" + m.WParam + "||" + ImportFromDLL.WM_COPYDATA); 

            if (m.Msg == ImportFromDLL.WM_COPYDATA)//根据Message.Msg区分消息类型，ImportFromDLL.WM_COPYDATA为发送方定义的消息类型
            {
                ImportFromDLL.COPYDATASTRUCT copyData = (ImportFromDLL.COPYDATASTRUCT)m.GetLParam(typeof(ImportFromDLL.COPYDATASTRUCT));//获取数据
                // PrintMessage(String.Format("WParam:{0} ---- msg:{1}", m.WParam, copyData.lpData));
                Debug.WriteLine("msg:" + copyData.lpData);

                if (m.WParam == WXAPI.LoginOk)
                { 
                    Debug.WriteLine("登录成功");

                    PrintMessage("一切准备就绪！");
                    _api = new WXAPI(wechatWindowHandler, _logName); 

                    //Debug.WriteLine(this.Handle);
                    var timer1 = new System.Timers.Timer
                    {
                        Interval = 100,
                        AutoReset = false,
                        Enabled = true,

                    };
                    timer1.Elapsed += (s, e) =>
                    {
                        _api.SetCommunicationInterface(_thisHandler);
                        _api.StartMonitorMsg();


                    };
                    timer1.Start();
                    var timer = new System.Timers.Timer
                    {
                        Interval = 200,
                        AutoReset = false,
                        Enabled = true,

                    };
                    timer.Elapsed += (s, e) =>
                    {
                        _api.StartMonitorMsg();
                        _api.GetMyInfo();
                         ReadMyinfo();
                    };
                    timer.Start();

                }
                else if (m.WParam == WXAPI.FriendOrGroup)
                {
                    var strs = copyData.lpData.Split(',');
                    if (strs[1].Contains("gh_"))
                    {
                        var item = new Contact
                        {
                            WxId = strs[1],
                            NickName = strs[2],
                            WxNum = strs[3],
                            RemarkName = strs[4] == "(null)" ? "" : strs[4]
                        };
                        _publicList.Add(item);
                    }
                    else
                        if (strs[1].Contains("@chatroom"))
                        {
                            var item = new Group
                            {
                                WxId = strs[1],
                                NickName = strs[2],
                                WxNum = strs[3],
                                RemarkName = strs[4] == "(null)" ? "" : strs[4],
                                Members = new BindingList<Contact>()
                            };
                            _groupList.Add(item);
                        }
                        else
                        {
                            var item = new Contact
                            {
                                WxId = strs[1],
                                NickName = strs[2],
                                WxNum = strs[3],
                                RemarkName = strs[4] == "(null)" ? "" : strs[4]
                            };
                            _friendList.Add(item);

                        }
                }
                else if (m.WParam == WXAPI.Message)
                {
                    try
                    {
                        WeMessage wm = WeMessage.Parse(copyData.lpData);
                        Savedata(wm);
                        PrintMessage(wm.wxid + ":" +wm.zsid + ":" + wm.MessageContent);
                    }
                    catch (Exception xe)
                    {
                        PrintMessage(xe.Message);
                    }
                }
                else if (m.WParam == WXAPI.GroupMember)
                {
                    var strs = copyData.lpData.Split(',');
                    var item = new Contact
                    {
                        WxId = strs[0],
                        NickName = strs[1]
                    };
                    // GetgroupById(groupwxidbox.Text).Members.Add(item);
                }


            }

            base.WndProc(ref m);
        }

        private void Savedata(WeMessage wm)
        {
            if (ifsave)
            {
                if (wm.MessageContent.Contains("查询&"))
                {
                    return;
                }
                if(!wm.MessageContent.Contains("msg"))
                { 
                string DbConn = "Server=localhost;Port=5432;Database=WX;User Id=WX;Password=WX;";
                var pgconn = new NpgsqlConnection(DbConn);
                pgconn.Open();
                string name = "未知";
                string guid = Guid.NewGuid().ToString();
                decimal addtime =decimal.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
                //if (wm.zsid == "wxid_j0xt1meghff322" || wm.zsid == "wxid_j0xt1meghff322")
                //{
                //    name = "程梦真";
                //}
                    if (wm.zsid == "wxid_qoqp89oc6inl12" || wm.zsid == "wxid_qoqp89oc6inl12")
                    {
                        name = "王文静";
                    }


                    if (wm.zsid == "wxid_2dserk6o0soa22" || wm.zsid == "wxid_2dserk6o0soa22")
                {
                    name = "我";
                }

                string ifroom = "1";
                if (String.IsNullOrEmpty(wm.zsid))
                {
                    ifroom = "0";
                }
                else if (wm.wxid=="24352671519@chatroom")
                {
                    ifroom = "1+聊天室";
                }
        
                var cmd = new NpgsqlCommand(@"INSERT INTO chatdata(id, name, text, addtime, roomid, userid,ifroom) VALUES('" + guid+"', '"+name+ "', '" + wm.MessageContent + "', '" + addtime + "', '" + wm.wxid + "', '" + wm.zsid + "','"+ ifroom + "');", pgconn);
                cmd.ExecuteNonQuery();
                pgconn.Close();
                }
              
            }
            return;
        }

        private Group GetgroupById(string id)
        {
            foreach (var item in _groupList)
            {
                if (item.WxId == id)
                {
                    return item;
                }
            }
            return null;
        }

        // 窗体关闭时关闭代理
        private void MainWindowFormClosing(object sender, FormClosingEventArgs e)
        {
            var copydatastruct = new ImportFromDLL.COPYDATASTRUCT();
            ImportFromDLL.SendMessage(wechatWindowHandler, ImportFromDLL.WM_CLOSE, (IntPtr)0, ref copydatastruct);
            System.Environment.Exit(System.Environment.ExitCode);

        }

        private void buttonStartWeixin_Click(object sender, EventArgs e)
        {
            MainWindowShown(null, null);
            buttonStartWeixin.Enabled = false;
        }
        //开启自动聊天
        private void button1_Click(object sender, EventArgs e)
        {
            reback = true;

        }
        private void ifreback(string message)
        {
     
        }

        private void button2_Click(object sender, EventArgs e)
        {
            reback = false;
        }
        private string Bridge(string text)
        {


            string baseUrl = " http://api.qingyunke.com/api.php?key=free&appid=0&msg="+ text;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
            request.Method = "GET";

            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var resStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            return result;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            reback2 = true;
            if(!room.Contains(textBox1.Text))
            { 
            room.Add(textBox1.Text);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            reback2 = false;
            room = new List<string>();
        }
        private void ifreback2(string message)
        {
            if (message.Contains("查询&"))
            {
                where(message.Split('&')[1].Replace("?"," "));
                return;
            }
            if (message.Contains("你大哥芝麻开门") || message.Contains("你大哥说话") || message.Contains("你大哥回来"))
            {
                if (!room.Contains(textBox1.Text))
                {
                    room.Add(message.Split(':')[0]);
                    reback2 = true;
                    textBox2.Text = "1";
                    reback3 = "1";
                }
            }
            else if (message.Contains("逗比机器人芝麻开门") || message.Contains("逗比机器人说话") || message.Contains("逗比机器人回来"))
            {
                if (!room.Contains(textBox1.Text))
                {
                    room.Add(message.Split(':')[0]);
                reback2 = true;
                    textBox2.Text = "2";
                    reback3 = "2";
                }
            }
            else if (message.Contains("小鱼儿芝麻开门") || message.Contains("小鱼儿说话") || message.Contains("小鱼儿回来"))
            {
                    if (!room.Contains(textBox1.Text))
                    {
                        room.Add(message.Split(':')[0]);
                reback2 = true;
                    textBox2.Text = "3";
                    reback3 = "3";
                }
            }
            else if (message.Contains("一起说话芝麻开门") || message.Contains("一起说话") || message.Contains("一起回来"))
            {
                        if (!room.Contains(textBox1.Text))
                        {
                            room.Add(message.Split(':')[0]);
                            reback2 = true;
                            textBox2.Text = "0";
                            reback3 = "0";
                        }
            }
            //if (message.Contains("21786170784@chatroom"))
            //{
            //    return;
            //}

            if (message.Contains(textBox1.Text))
            {
                if (message.Contains("@我叫。。。"))
                {
                    foreach (var item in room)
                    {
                        if (message.Contains(item))
                        {
                            _api.SendMsgWithWIXD(item, "叫老子干哈");
                        }
                    }
                }
                if (message.Contains("拍了拍"))
                {
                    foreach (var item in room)
                    {
                        if (message.Contains(item))
                        {
                            if (message.Contains("拍了拍我"))
                            {
                                if (cishu == 0)
                                {
                                    _api.SendMsgWithWIXD(item, "拍老子干啥??");
                                    cishu++;
                                }
                                else
                                {
                                    _api.SendMsgWithWIXD(item, "拍老子干啥??拍了老子" + cishu + "次了");
                                    cishu++;
                                }
                            }
                            else
                            {
                                _api.SendMsgWithWIXD(item, "对使劲拍");
                            }
                            //else if (message.Contains("于"))
                            //{
                            //    _api.SendMsgWithWIXD(item, "再拍报警了");
                            //}
                            //else
                            //{
                            //    _api.SendMsgWithWIXD(item, "拍拍拍就知道拍拍拍，有本事面对面拍");
                            //}
                        }
                    }
                }
            }
            if (reback2|| reback)
            {
                if (message.Contains("繁体"))
                {
                    fanti = true;
                }
                else if (message.Contains("简体"))
                {
                    fanti = false;
                }
                foreach (var item in room)
                {
                    if (message.Contains(item))
                    {
                        if (message.Contains("闭嘴")|| message.Contains("芝麻关门") || message.Contains("滚"))
                        {
                            room.Remove(item);
                        }
                        else if (message.Contains("主人"))
                        {
                            _api.SendMsgWithWIXD(item, "我的主人是个真诚 坚定 诚实 虚心 谦逊 谨慎 廉洁 无私 正直 慷慨 有趣 天真 自然 坚定 脱俗 清新 帅气 大方 友爱 忠诚 诚意 实在 有品 聪颖 努力 可靠 俊雅 气质 勇敢 自信 得人");
                        }
                        else
                        {
                            int co=message.Split(':').Length;
                            split =message.Split(':');
                            if (reback3 == "3" || reback3 == "0")
                            {
                                string t = Bridge3(message.Split(':')[co - 1]);

                                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                                resultdata3 list = Serializer.Deserialize<resultdata3>(t);

                                if (fanti)
                                {
                                    reply = ToTraditional(list.data.info.text);
                                }
                                else
                                {
                                    reply = ToSimplified(list.data.info.text);
                                }
                            

                                //reply = reply.Replace("小思", "小鱼儿").Replace("{appellation}", "小主人").Replace("天行", "逗比");
                                _api.SendMsgWithWIXD(item, ("" + reply));

                                //_api.SendMsgWithWIXD(item, (reply));
                            }
                            if (reback3 == "1"|| reback3 == "0")
                            {

                                string t = Bridge(message.Split(':')[co - 1]);
                                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                                resultdata list = Serializer.Deserialize<resultdata>(t);

                                if (fanti)
                                {
                                    reply = ToTraditional(list.content);
                                }
                                else
                                {
                                    reply = ToSimplified(list.content);
                                }


                                    reply = list.content.Replace("菲菲", "你大哥");
                                _api.SendMsgWithWIXD(item, ("" + reply));
                                //_api.SendMsgWithWIXD(item, ( reply));
                            }
                             if (reback3 == "2" || reback3 == "0")
                            {

                                string t = Bridge2(message.Split(':')[co - 1]);
                                JavaScriptSerializer Serializer = new JavaScriptSerializer();
                                resultdata2 list = Serializer.Deserialize<resultdata2>(t);
                                if (fanti)
                                {
                                    reply = ToTraditional(list.newslist[0].reply);
                                }
                                else
                                {
                                    reply = ToSimplified(list.newslist[0].reply);
                                }

                                //reply = ToSimplified(list.newslist[0].reply);

                                //reply = reply.Replace("{robotname}", "逗比机器人").Replace("{appellation}", "小主人").Replace("天行", "逗比");
                                 _api.SendMsgWithWIXD(item, ("" + reply));
                                //_api.SendMsgWithWIXD(item, (reply));
                            }


                            /*下面这个程序不支持英文，我刚接触，英文的我还不懂，呵呵*/
                            //this.SaveFile(reply);
                        }
                    }
                }
               
            }
        }
        private void where(string where)
        {

               
                    string DbConn = "Server=localhost;Port=5432;Database=WX;User Id=WX;Password=WX;";
                    var pgconn = new NpgsqlConnection(DbConn);
                    pgconn.Open();

                DataTable dt = new DataTable();
                string sql = "SELECT  name, text, addtime FROM chatdata where roomid = '24352671519@chatroom' " + where;

                var cmd = new NpgsqlCommand(sql, pgconn);
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                adapter.Fill(dt);
            string value = "";
                for (int i=0;i<dt.Rows.Count;i++ )
                    {
                value += "姓名:" + dt.Rows[i][0] + "      " + "内容:" + dt.Rows[i][1] + "      " + "时间:" + dt.Rows[i][2] + "      ";

                    }
            _api.SendMsgWithWIXD("24352671519@chatroom", value);
            pgconn.Close();
               

            return;
        }
        /// <summary>
        /// 生成语音文件的方法
        /// </summary>
        /// <param name="text"></param>
        private void SaveFile(string text)
        {
            SpeechSynthesizer speech;
            speech = new SpeechSynthesizer();
            var dialog = new SaveFileDialog();
            dialog.Filter = "*.wav|*.wav|*.mp3|*.mp3";
            dialog.ShowDialog();

            string path = dialog.FileName;
            if (path.Trim().Length == 0)
            {
                return;
            }
            speech.SetOutputToWaveFile(path);
            speech.Volume = 100;
            speech.Rate = 1;
            speech.Speak(text);
            speech.SetOutputToNull();
            MessageBox.Show("生成成功!在" + path + "路径中！", "提示");

        }

        private string Bridge2(string text)
        {


            string baseUrl = "http://api.tianapi.com/txapi/robot/index?key=fa191b84e15ab45080e2e9b52ab0e970&userid=23123456464&question=" + text;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
            request.Method = "GET";

            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var resStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            return result;
        }
        private string Bridge3(string text)
        {


            string baseUrl = "https://api.ownthink.com/bot?appid=59c6520524e5583fa9609358dd38ed9c&userid=pNFTmlXD&spoken=" + text;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
            request.Method = "GET";

            request.ContentType = "application/json";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var resStream = response.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            return result;
        }
        #region IString 成员
        private const int LOCALE_SYSTEM_DEFAULT = 0x0800;
        private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);

        /// <summary>
        /// 将字符转换成简体中文
        /// </summary>
        /// <param name="source">输入要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string ToSimplified(string source)
        {
            String target = new String(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_SIMPLIFIED_CHINESE, source, source.Length, target, source.Length);
            return target;
        }

        /// <summary>
        /// 讲字符转换为繁体中文
        /// </summary>
        /// <param name="source">输入要转换的字符串</param>
        /// <returns>转换完成后的字符串</returns>
        public static string ToTraditional(string source)
        {
            String target = new String(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, source, source.Length, target, source.Length);
            return target;
        }
        #endregion



        private void button5_Click(object sender, EventArgs e)
        {
            LogMessageBox.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ifsave = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ifsave = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string DbConn = "Server=localhost;Port=5432;Database=WX;User Id=WX;Password=WX;";
            var pgconn = new NpgsqlConnection(DbConn);
            pgconn.Open();
            DataTable dt = new DataTable();
            string sql = "SELECT  text FROM zhihu where text like '%:320606064%'";
            var cmd = new NpgsqlCommand(sql, pgconn);
            NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
            adapter.Fill(dt);
            string value;
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            string id = "", name = "", text = "", title = "", url_token = "", headline = "", content = "";
            decimal created_time = 0M, updated_time = 0M;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                value = dt.Rows[i]["text"].ToString();
                zhihu list = Serializer.Deserialize<zhihu>(value);
                foreach (var item in list.data)
                {
                    id= item.id.ToString();
                    name = item.author.name;
                    url_token = item.author.url_token;
                    headline = item.author.headline;
                    content = item.content;
                    title = item.question.title;
                    created_time = decimal.Parse(item.created_time.ToString());
                    updated_time = decimal.Parse(item.updated_time.ToString());
                    var cmdaaa = new NpgsqlCommand(@"INSERT INTO zhihutext(
            id, name, title, url_token, headline, content, created_time, 
            updated_time) VALUES('" + id + "', '" + name + "', '" + title + "', '" + url_token + "', '" + headline + "', '" + content + "'," + created_time + "," + updated_time + ");", pgconn);
                    cmdaaa.ExecuteNonQuery();
                }
            }
            string guid = Guid.NewGuid().ToString();     
            cmd.ExecuteNonQuery();
            pgconn.Close();
            MessageBox.Show("整理成功");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MessageBox.Show("qq:576165539");
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text=="1")
            {
                reback3 = "1";
            }
            else if (textBox2.Text == "2")
            {
                reback3 = "2";
            }
            else if (textBox2.Text == "0")
            {
                reback3 = "0";
            }
            else
            {
                reback3 = "3";
            }
        }

        private void jsh(string message)
        {
            if((message.Contains("笑话")|| message.Contains  ("故事") || message.Contains("段子"))&&gushi==true)
            { 
            int co = message.Split(':').Length;
            split = message.Split(':');
            string t = Bridge2(message.Split(':')[co - 1]);
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            resultdata2 list = Serializer.Deserialize<resultdata2>(t);


            reply = ToSimplified(list.newslist[0].reply);

            reply = reply.Replace("{robotname}", "逗比机器人").Replace("{appellation}", "小主人").Replace("天行", "逗比");
                if(message.Contains("wxid_8v5n5vgs8n6z22"))
                { _api.SendMsgWithWIXD("wxid_8v5n5vgs8n6z22", (reply)); }               
                else if (message.Contains("wxid_b2ohru7xc37z21"))
                { _api.SendMsgWithWIXD("wxid_b2ohru7xc37z21", (reply)); }
                else { 
                 _api.SendMsgWithWIXD("24670846564@chatroom", (reply));
                }
                //_api.SendMsgWithWIXD("wxid_bed02nrzbbkd22", (reply));
            }

            else
            {
                ifreback2(message);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            gushi = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            gushi = false;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);
            // 设置引发时间的时间间隔 此处设置为１秒
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

        }
        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // 得到 hour minute second  如果等于某个值就开始执行
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            // 定制时间,在00：00：00 的时候执行
            int iHour = 22;
            int iMinute = 30;
            int iSecond = 00;
            // 设置 每天的00：00：00开始执行程序
            if (intHour == iHour && intMinute == iMinute && intSecond == iSecond)
            {
                _api.SendMsgWithWIXD(textBox4.Text, ("现在是："+DateTime.Now+ " 该准备睡觉了，睡觉使人年轻，不想老这么快的话就睡觉吧！"));
            }
            if (intHour == 23 && intMinute == 00 && intSecond == 00)
            {
                _api.SendMsgWithWIXD(textBox4.Text, ("现在是：" + DateTime.Now + " 睡着了么，你是睡着了呢，还是睡着了呢，还是睡着了呢？"));
            }
            if (intHour == 23 && intMinute == 30 && intSecond == 00)
            {

                _api.SendMsgWithWIXD(textBox4.Text, ("现在是：" + DateTime.Now + " 再不睡觉就要长皱纹了，睡觉咯，明天见！"));
            }
            if (intHour == 00 && intMinute == 00 && intSecond == 00)
            {
                _api.SendMsgWithWIXD(textBox4.Text, ("现在是：" + DateTime.Now + " 既睡之，则安之，我就不打扰了。"));
                //_api.SendMsgWithWIXD(textBox4.Text, ("现在是：" + DateTime.Now + " 贞子要出来了"));
            }
        }
    }
}
