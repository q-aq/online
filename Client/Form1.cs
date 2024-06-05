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
        private void button1_Click(object sender, EventArgs e)//���Ӱ�ť
        {
            if (textBox3.Text == "") MessageBox.Show("�û���Ϊ��");
            else Connect();

        }
        private void Connect()//�����ͻ��˵�����˵�����
        {
            string ip = "127.0.0.2";
            int port = 55545;
            Client = new TcpClient();
            try
            {
                Client.Connect(ip, port);//���Խ�������
                s = new User(textBox3.Text, Client, this);//�����������ӵ������
                s.Send("lgin", s.UserName);//����������û�����������
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
        private void button2_Click(object sender, EventArgs e)//���Ͱ�ť
        {
            if (IsConnect)
            {
                try
                {
                    if (s != null)
                    {
                        string? name = comboBox1.SelectedItem.ToString();//������ǰѡ��
                        if (name == "ALL")
                        {
                            if(textBox1.Text != "") 
                                s.Send("sdal", textBox1.Text);//�������û�����
                        }
                        else
                        {
                            if (textBox1.Text == "") return;
                            s.Send("sdon", textBox1.Text, name);//��name�ֶ�ָ���û�����
                            s.Sendtobox(s.UserName, textBox1.Text);//���͵��Լ�����Ļ
                        }
                        textBox1.Clear();//��������
                    }
                    else
                    {
                        MessageBox.Show("δ��������");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"error:{ex.Message}");
                }
            }
        }
        private void button3_Click(object sender, EventArgs e)//�˳���ť
        {
            if (s != null)
            {
                s.IsAlive = false;
                s.Send("lgot", "�����˳�����");//�����˳�����
                this.Close();
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)//�����ڹر�
        {
            if (s != null)
            {
                s.IsAlive = false;
                s.Send("erro", $"�����˳�");//�����˳�����
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
        public void Recv()//���մ�����Ϣ
        {
            while (IsAlive)
            {
                try
                {
                    string? information;//��¼���ݹ�������Ϣ
                    information = this.Reader.ReadString();
                    if (information != null)
                    {
                        string Head = information[..4];//��Ϣͷ������ȷ����Ϣ���ͣ�������ͬ����
                        string Oper = information.Substring(4, 10).TrimEnd('*');//��������Ϣͷ���棬�������ݲ���֮���
                        string Mainfo = information[14..];//��Ϣ����
                        if (Head == "lgin")//login in��ʾ�����û�����
                        {
                            update(Mainfo);
                            SendInformation(Mainfo);
                        }
                        else if (Head == "info")//information��ʾ�ı���Ϣ
                        {
                            Sendtobox(Oper, Mainfo);
                        }
                        else if (Head == "upda")//update��ʾ��Ҫ�����û��б�
                        {
                            update(Mainfo);
                        }
                        else if (Head == "cler")//clear��ʾ��Ҫ����û��б�
                        {
                            Clear();
                        }
                        else if(Head == "lgot")//login out��ʾ���û��ǳ�
                        {
                            SendText("�û�"+Mainfo+"�˳�");
                        }
                        else if(Head == "chge")//change��ʾ�û��޸ĵ�¼����
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
        public void Clear()//����û��б�ͬʱ��ʼ��������
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
        public void update(string name)//���û��б��������������µ��û�
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
        public void Sendtobox(string user, string text)//user����text
        {
            form.Invoke(() =>
            {
                form.textBox2.Text += $"�û�{user}����:{text}\r\n";
            });
        }
        public void SendInformation(string text)//����text��������
        {
            form.Invoke(() =>
            {
                form.textBox2.Text += $"�û�{text}�ѳɹ���������\r\n";
            });
        }
        public void SendText(string text)//��ȫ����text
        {
            form.Invoke(() =>
            {
                form.textBox2.Text += $"{text}\r\n";
            });
        }
        //����Ϣ���͵���������head��ʾ��Ϣ���ͣ�infomation��ʾ��Ϣ���⣬Oper��ʾ��ѡ����
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
