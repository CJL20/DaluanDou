using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 聊天室
/// Socket服务器
/// </summary>

#region 异步实现
//public class ClientState
//{
//    public Socket Socket;
//    public byte[] readBuffer = new byte[1024];
//}
//namespace QQ_Program
//{
//    class Program
//    {
//        private static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

//        static Socket listenfd;


//        static void Main(string[] args)
//        {

//            Console.WriteLine("欢迎来到我的世界！");

//            //声明socket
//            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            //绑定
//            IPAddress iPAddress = IPAddress.Parse("192.168.44.113");
//            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 8888);
//            listenfd.Bind(iPEndPoint);
//            //监听
//            listenfd.Listen(0);
//            Console.WriteLine("[服务器]启动成功！");

//            //accept
//            listenfd.BeginAccept(AcceptCallback, listenfd);
//            //等待
//            Console.ReadLine();

//        }
//        //异步响应回调
//        public static void AcceptCallback(IAsyncResult async)
//        {
//            try
//            {
//                Console.WriteLine("[服务器]Accept");
//                Socket listenfd = (Socket)async.AsyncState;
//                Socket clientfd = listenfd.EndAccept(async);
//                //clients列表
//                ClientState state = new ClientState();
//                state.Socket = clientfd;
//                clients.Add(state.Socket, state);
//                //接收数据BeginReceive
//                clientfd.BeginReceive(state.readBuffer, 0, 1024, 0, ReceiveCallback, state);
//                //继续响应
//                listenfd.BeginAccept(AcceptCallback, listenfd);
//            }
//            catch (SocketException e)
//            {
//                Console.WriteLine("Socket Accept fail" + e.ToString());
//            }
//        }

//        //异步接收实现
//        public static void ReceiveCallback(IAsyncResult async)
//        {
//            try
//            {
//                ClientState state = (ClientState)async.AsyncState;
//                Socket clientfd = state.Socket;
//                int count = clientfd.EndReceive(async);
//                //客户端关闭
//                if (count == 0)
//                {
//                    clientfd.Close();
//                    clients.Remove(clientfd);
//                    Console.WriteLine("Socket Close");
//                    return;
//                }
//                string receiveStr = System.Text.Encoding.UTF8.GetString(state.readBuffer, 0, count);
//                string sendStr = clientfd.RemoteEndPoint.ToString() + ":" + receiveStr;
//                byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
//                foreach (ClientState client in clients.Values)
//                {
//                    //client.Socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, state);
//                    //clientfd.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, state);
//                    client.Socket.Send(sendBytes);
//                }
//                clientfd.BeginReceive(state.readBuffer, 0, 1024, 0, ReceiveCallback, state);
//            }
//            catch (SocketException e)
//            {
//                Console.WriteLine("Socket Receive fail" + e.ToString());
//            }
//        }

//        //异步发送回调
//        public static void SendCallback(IAsyncResult async)
//        {
//            try
//            {
//                ClientState state = (ClientState)async.AsyncState;
//                Socket socket = state.Socket;
//                int count = socket.EndSend(async);
//                Console.WriteLine("Socket Send Succ" + count);
//            }
//            catch (SocketException e)
//            {
//                Console.WriteLine(e.ToString());
//            }
//        }
//    }
//}
#endregion


#region Poll(状态检测)实现
//public class ClientState
//{
//    public Socket Socket;
//    public byte[] readBuffer = new byte[1024];
//}
//namespace QQ_Program
//{
//    class Program
//    {
//        private static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

//        static Socket listenfd;


//        static void Main(string[] args)
//        {

//            Console.WriteLine("欢迎来到我的世界！");

//            //声明socket
//            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//            //绑定
//            IPAddress iPAddress = IPAddress.Parse("192.168.44.113");
//            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 8888);
//            listenfd.Bind(iPEndPoint);
//            //监听
//            listenfd.Listen(0);
//            Console.WriteLine("[服务器]启动成功！");

//            //循环监听并且响应
//            while (true)
//            {
//                //检查listenfd
//                if (listenfd.Poll(0, SelectMode.SelectRead))
//                {
//                    ReadListenfd(listenfd);
//                }
//                //检查clientfd
//                foreach (ClientState s in clients.Values)
//                {
//                    Socket clientfd = s.Socket;
//                    if (clientfd.Poll(0, SelectMode.SelectRead))
//                    {
//                        if (!ReadClientfd(clientfd))
//                        {
//                            break;
//                        }
//                    }
//                }
//                System.Threading.Thread.Sleep(1);
//            }
//        }

//        //读取Listenfd
//        public static void ReadListenfd(Socket socket)
//        {
//            Console.WriteLine("Accept");
//            Socket clientfd = socket.Accept();
//            ClientState state = new ClientState();
//            state.Socket = clientfd;
//            clients.Add(state.Socket, state);
//        }

//        //读取Clientfd
//        public static bool ReadClientfd(Socket socket)
//        {
//            ClientState state = clients[socket];
//            //接收
//            int count = 0;
//            try
//            {
//                count = socket.Receive(state.readBuffer);
//            }
//            catch (SocketException e)
//            {
//                socket.Close();
//                clients.Remove(socket);
//                Console.WriteLine("Receive SocketException:" + e.ToString());
//                return false;
//            }
//            //客户端关闭
//            if(count ==0)
//            {
//                socket.Close();
//                clients.Remove(socket);
//                Console.WriteLine("Socket Close");
//                return false;
//            }
//            //广播
//            string receiveStr = System.Text.Encoding.UTF8.GetString(state.readBuffer, 0, count);
//            Console.WriteLine("Receive:" + receiveStr);
//            string sendStr = socket.RemoteEndPoint.ToString() + ":" + receiveStr;
//            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
//            foreach (ClientState v in clients .Values)
//            {
//                v.Socket.Send(sendBytes);
//            }
//            return true;
//        }
//    }
//}
#endregion


#region Selcet(多路复用)实现
public class ClientState
{
    public Socket Socket;
    public byte[] readBuffer = new byte[1024];
}
namespace QQ_Program
{
    class Program
    {
        private static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        static Socket listenfd;


        static void Main(string[] args)
        {

            Console.WriteLine("欢迎来到我的世界！");

            //声明socket
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定
            IPAddress iPAddress = IPAddress.Parse("192.168.44.113");
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
                foreach (var v in clients .Values)
                {
                    checkRead.Add(v.Socket);
                }
                //select
                Socket.Select(checkRead, null, null, 10000);
                //检查可读对象
                foreach (var v in checkRead)
                {
                    if(v==listenfd)
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
                socket.Close();
                clients.Remove(socket);
                Console.WriteLine("Receive SocketException:" + e.ToString());
                return false;
            }
            //客户端关闭
            if (count == 0)
            {
                socket.Close();
                clients.Remove(socket);
                Console.WriteLine("Socket Close");
                return false;
            }
            //广播
            string receiveStr = System.Text.Encoding.UTF8.GetString(state.readBuffer, 0, count);
            Console.WriteLine("Receive:" + receiveStr);
            string sendStr = socket.RemoteEndPoint.ToString() + ":" + receiveStr;
            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendStr);
            foreach (ClientState v in clients.Values)
            {
                v.Socket.Send(sendBytes);
            }
            return true;
        }
    }
}
#endregion


