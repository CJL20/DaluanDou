using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Linq;
/// <summary>
///大乱斗程序
/// 服务端
/// </summary>
namespace DaLuanDou_Program
{

    public class ClientState
    {
        public Socket Socket;
        public byte[] readBuffer = new byte[1024];

        public int hp = 100;
        public float x = 0f;
        public float y = 0f;
        public float z = 0f;
        public float euly = 0f;
     

    }

    class Program
    {
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        static Socket listenfd;


        static void Main(string[] args)
        {

            Console.WriteLine("欢迎来到我的世界！");

            //声明socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定
            IPAddress iPAddress = IPAddress.Parse("192.168.44.109");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 8888);
            listenfd.Bind(iPEndPoint);
            //监听
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功！");

            //CheckSocketList
            List<Socket> checkRead = new List<Socket>();

            //循环监听并且响应
            while (true)
            {
                //填充checkRead
                checkRead.Clear();
                checkRead.Add(listenfd);
                foreach (var v in clients.Values)
                {
                    checkRead.Add(v.Socket);
                }
                //select
                Socket.Select(checkRead, null, null, 10000);
                //检查可读对象
                foreach (var v in checkRead)
                {
                    if (v == listenfd)
                    {
                        ReadListenfd(v);
                    }
                    else
                    {
                        ReadClientfd(v);
                    }
                }
            }
        }

        //读取Listenfd
        public static void ReadListenfd(Socket socket)
        {
            Console.WriteLine("Accept");
            Socket clientfd = socket.Accept();
            ClientState state = new ClientState();
            state.Socket = clientfd;
            clients.Add(state.Socket, state);
        }

        //读取Clientfd
        public static bool ReadClientfd(Socket socket)
        {
            ClientState state = clients[socket];
            //接收
            int count = 0;
            try
            {
                count = socket.Receive(state.readBuffer);
            }
            catch (SocketException e)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisConnect");
                object[] ob = { state };
                if(mei !=null)
                {
                    mei.Invoke(null, ob);
                }

                socket.Close();
                clients.Remove(socket);
                Console.WriteLine("Receive SocketException:" + e.ToString());
                return false;
            }
            //客户端关闭
            if (count == 0)
            {
                MethodInfo mei = typeof(EventHandler).GetMethod("OnDisConnect");
                object[] ob = { state };
                if (mei != null)
                {
                    mei.Invoke(null, ob);
                }

                socket.Close();
                clients.Remove(socket);
                Console.WriteLine("Socket Close");
                return false;
            }
            //广播(信息处理)
            string receiveStr = System.Text.Encoding.UTF8.GetString(state.readBuffer, 0, count);
            string[] split = receiveStr.Split('|');
            Console.WriteLine("Receive:" + receiveStr);
            string msgName = split[0];
            string msgBody = split[1];
            string FunName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandler).GetMethod(FunName);
            object[] o = { state, msgBody };
            if (mi != null)
            {
                //第一个参数null代表this指针，由于处理方法都是静态方法，因此此次要填null
                //第二个参数 o代表的是参数列表。 这里定
                //义的消息处理函数都有两个参数， 第一个参数是客户
                //端状态state, 第二个参数是消息的内容msgArgs。
                mi.Invoke(null, o);
            }
            return true;
        }

        public static void Send(ClientState state ,string str)
        {
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(str);
            state.Socket.Send(sendBytes);
        }
    }
}
