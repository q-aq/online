using System.Net;
using System.Net.Sockets;

namespace server
{
    internal class Program
    {
        public static List<User> users = new();
        public static void Main(string[] args)
        {
            Program s = new Program();
            s.Listener();
        }
        public void Listener()//启动监听函数
        {
            IPAddress ip = IPAddress.Parse("127.0.0.2");  
            int port = 55545;//端口号
            TcpListener server = new TcpListener(ip,port);
            server.Start(4);
            Console.WriteLine("等待连接中");
            try
            {
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();//等待用户连接
                    User s = new User("NULL",client);
                }
            }catch (Exception ex)
            {
                Console.WriteLine("连接失败"+ex.Message);
            }
        } 
    }
    public class User
    {
        public string UserName;
        public BinaryReader Reader;
        public BinaryWriter Writer;
        public TcpClient Client;
        public Thread t;
        public bool once;
        public User(string name, TcpClient client)
        {
            this.Client = client;
            this.UserName = name;
            this.once = false;
            this.Reader = new BinaryReader(client.GetStream());
            this.Writer = new BinaryWriter(client.GetStream());
            t = new Thread(Recv);
            t.Start();
        }
        public void Recv()//接收消息
        {
            while (true)
            {
                try
                {
                    string? information;
                    information = this.Reader.ReadString();
                    if (information != null)
                    {
                        string Head = information[..4];//消息头，用于确定消息类型，做出不同操作
                        string Oper = information.Substring(4, 10).TrimEnd('*');//紧跟在消息头后面，用来传递参数之类的，需要去除尾部的填充符
                        string Mainfo = information[14..];//消息主体
                        if (Head == "lgin")//接收登录信息，向全体在线用户发送该信息，提示有人登录
                        {
                            this.UserName = Mainfo;
                            Login(Mainfo);
                        }
                        else if (Head == "info")//接收文本信息，显示在服务器上，不进行任何操作
                        {
                            Console.WriteLine($"client{this.UserName}向server发送{Head}信息:{Mainfo}");
                        }
                        else if (Head == "sdal")//SendAll,接收到信息后，向全体在线人员转发该信息
                        {
                            Console.WriteLine($"client{this.UserName}向server发送{Head}信息:{Mainfo}");
                            SendAll("info",Mainfo,this.UserName);//name字段用来表示是谁发送了消息
                        }
                        else if (Head == "sdon")//SendOne,接收信息后向oper字段指定的用户发送该信息
                        {
                            Console.WriteLine($"client{this.UserName}向server发送{Head}信息:{Mainfo}");
                            SendOne("info",Mainfo,Oper,this.UserName);
                        }
                        else if (Head == "lgot")//login out,接收信息后，将该发送者移除users，并向全体在线用户发送用户登出信息
                        {
                            Console.WriteLine($"client{this.UserName}向server发送{Head}信息:{Mainfo}");
                            ReMove();
                        }
                        else if(Head == "erro")//error 客户端发生异常后关闭该客户端
                        {
                            Console.WriteLine($"client{this.UserName}发生异常{Mainfo}");
                            ReMove();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"client{this.UserName}断开连接{e.Message}");
                    t.Join();
                    break;
                }
            }
        }
        public void Login(string Mainfo)//登录
        {
            foreach (User s in Program.users)//遍历在线用户表
            {
                if (s.UserName == Mainfo)
                {
                    once = true;
                    break;
                }
            }
            if (once)//如果重名则发送消息
                Send("chge", "请修改您的名称后重新建立连接");
            else//如果没有重名则登入
                UpDate();
        }
        public void UpDate()
        {
            Program.users.Add(this);
            SendAll("lgin", this.UserName);//向全体用户发送登陆消息
            SendOne("cler", "清除用户列表指令", this.UserName);
            foreach (User s in Program.users)
                SendOne("upda", s.UserName, this.UserName);
            Console.WriteLine($"client:{this.UserName}连接成功");
        }
        public void ReMove()//将自身从在线用户清除
        {
            Program.users.Remove(this);//将自己从用户列表中移除
            SendAll("lgot", this.UserName);//向所有用户发送退出信息
            SendAll("cler", "清除用户列表指令");//发送信号清楚用户列表
            foreach (User s in Program.users)//逐个发送在线用户信息
                SendAll("upda", s.UserName);
            Client.Close();
            Writer.Close();
            Reader.Close();
            t.Join();
        }
        public void Send(string head, string information ,string Oper = "server")
        {
            try
            {
                string mainfo = information;//消息主体
                Oper = Oper.PadRight(10, '*');
                this.Writer.Write(head + Oper + mainfo);
                Console.WriteLine($"server向client{this.UserName}发送{head}信息:{mainfo}");
            }
            catch(Exception e)
            {
                Console.WriteLine("error:"+e.Message);
            }
        }
        public void SendAll(string head,string info,string name = "server")//sendall函数的name字段用来表示是谁发送了消息
        {
            foreach (User s in Program.users)
            {
                s.Send(head,info,name);
            }
        }
        public void SendOne(string head,string info,string name,string Oper = "server")//sendone函数的name字段用来表示给谁发消息
        {
            foreach (User s in Program.users)
            {
                if (s.UserName == name)
                {
                    s.Send(head,info,Oper);
                    break;
                }
            }
        }
    }
}
