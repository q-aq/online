using System.Net.Sockets;

namespace Client
{
    public partial class Form1 : Form
    {
        public User? s;
        private bool IsConnect = false;
        TcpClient Client = new TcpClient();
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Clear();
            comboBox1.Items.Add("ALL");
            comboBox1.SelectedItem = "ALL";
            button2.Enabled = false;
            button3.Enabled = false;
        }
        private void button1_Click(object sender, EventArgs e)//连接按钮
        {
            if (textBox3.Text == "") MessageBox.Show("用户名为空");
            else Connect();

        }
        private void Connect()//建立客户端到服务端的连接
        {
            string ip = "127.0.0.2";
            int port = 55545;
            Client = new TcpClient();
            try
            {
                Client.Connect(ip, port);//尝试建立连接
                s = new User(textBox3.Text, Client, this);//创建对象连接到服务端
                s.Send("lgin", s.UserName);//发送自身的用户名到服务器
                IsConnect = true;
                button1.Enabled = false;
                textBox3.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"error:{ex.Message}");
            }
        }
        private void button2_Click(object sender, EventArgs e)//发送按钮
        {
            if (IsConnect)
            {
                try
                {
                    if (s != null)
                    {
                        string? name = comboBox1.SelectedItem.ToString();//下拉框当前选项
                        if (name == "ALL")
                        {
                            if(textBox1.Text != "") 
                                s.Send("sdal", textBox1.Text);//向所有用户发送
                        }
                        else
                        {
                            if (textBox1.Text == "") return;
                            s.Send("sdon", textBox1.Text, name);//向name字段指定用户发送
                            s.Sendtobox(s.UserName, textBox1.Text);//发送到自己的屏幕
                        }
                        textBox1.Clear();//清空输入框
                    }
                    else
                    {
                        MessageBox.Show("未建立连接");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"error:{ex.Message}");
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)//退出按钮
        {
            if (s != null)
            {
                s.IsAlive = false;
                s.Send("lgot", "发送退出请求");//发送退出请求
                this.Close();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//当窗口关闭
        {
            if (s != null)
            {
                s.IsAlive = false;
                s.Send("erro", $"主动退出");//发送退出请求
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (s != null)
            {
                if(s.IsAlive)
                    s.t.Join();
            }
        }
    }
    public class User
    {
        public string UserName;
        public string select = "ALL";
        public BinaryReader Reader;
        public BinaryWriter Writer;
        public TcpClient Client;
        public Form1 form;
        public Thread t;
        public bool IsAlive;
        public User(string name, TcpClient client, Form1 form)
        {
            this.form = form;
            IsAlive = true;
            this.Client = client;
            this.UserName = name;
            this.Reader = new BinaryReader(client.GetStream());
            this.Writer = new BinaryWriter(client.GetStream());
            t = new Thread(Recv);
            t.Start();
        }
        public void Recv()//接收处理消息
        {
            while (IsAlive)
            {
                try
                {
                    string? information;//记录传递过来的消息
                    information = this.Reader.ReadString();
                    if (information != null)
                    {
                        string Head = information[..4];//消息头，用于确定消息类型，做出不同操作
                        string Oper = information.Substring(4, 10).TrimEnd('*');//紧跟在消息头后面，用来传递参数之类的
                        string Mainfo = information[14..];//消息主体
                        if (Head == "lgin")//login in表示有新用户登入
                        {
                            update(Mainfo);
                            SendInformation(Mainfo);
                        }
                        else if (Head == "info")//information表示文本信息
                        {
                            Sendtobox(Oper, Mainfo);
                        }
                        else if (Head == "upda")//update表示需要更新用户列表
                        {
                            update(Mainfo);
                        }
                        else if (Head == "cler")//clear表示需要清空用户列表
                        {
                            Clear();
                        }
                        else if(Head == "lgot")//login out表示有用户登出
                        {
                            SendText("用户"+Mainfo+"退出");
                        }
                        else if(Head == "chge")//change提示用户修改登录名称
                        {
                            Change(Mainfo);
                        }
                    }
                }
                catch (Exception e)
                {
                    IsAlive = false;
                    Send("erro", e.Message);
                    break;
                }
            }
        }
        public void Change(string Mainfo)
        {
            form.Invoke(() =>
            {
                MessageBox.Show(Mainfo);
                form.textBox3.Enabled = true;
                form.button1.Enabled = true;
                form.textBox3.Clear();
            });
        }
        public void Clear()//清除用户列表，同时初始化下拉框
        {
            form.Invoke(() =>
            {
                select = form.comboBox1.SelectedItem.ToString();
                form.listBox1.Items.Clear();
                form.comboBox1.Items.Clear();
                form.comboBox1.Items.Add("ALL");
                form.comboBox1.SelectedItem = "ALL";
            });
        }
        public void update(string name)//向用户列表和下拉框中添加新的用户
        {
            form.Invoke(() =>
            {
                form.listBox1.Items.Add(name);
                if(name !=this.UserName)
                    form.comboBox1.Items.Add(name);
                if(name == select)
                    form.comboBox1.SelectedItem = select;
            });
        }
        public void Sendtobox(string user, string text)//user发送text
        {
            form.Invoke(() =>
            {
                form.textBox2.Text += $"用户{user}发送:{text}\r\n";
            });
        }
        public void SendInformation(string text)//发送text建立连接
        {
            form.Invoke(() =>
            {
                form.textBox2.Text += $"用户{text}已成功建立连接\r\n";
            });
        }
        public void SendText(string text)//完全发送text
        {
            form.Invoke(() =>
            {
                form.textBox2.Text += $"{text}\r\n";
            });
        }
        //将信息发送到服务器，head表示消息类型，infomation表示消息主题，Oper表示可选参数
        public void Send(string head, string information,string Oper = "")
        {
            try
            {
                Oper = Oper.PadRight(10, '*');
                this.Writer.Write(head + Oper + information);
            }
            catch
            {

            }
        }
    }
}
