using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

/// <summary>
/// Echo程序
/// Socket服务端
/// </summary>
namespace DaLuanDou_Program
{
    #region 异步实现
    public class ClientState
    {
        public Socket Socket;
        public byte[] readBuffer = new byte[1024];
    }
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

            //accept
            listenfd.BeginAccept(AcceptCallback, listenfd);
            //等待
            Console.ReadLine();

        }
        //异步响应回调
        public static void AcceptCallback(IAsyncResult async)
        {
            try
            {
                Console.WriteLine("[服务器]Accept");
                Socket listenfd = (Socket)async.AsyncState;
                Socket clientfd = listenfd.EndAccept(async);
                //clients列表
                ClientState state = new ClientState();
                state.Socket = clientfd;
                clients.Add(state.Socket, state);
                //接收数据BeginReceive
                clientfd.BeginReceive(state.readBuffer, 0, 1024, 0, ReceiveCallback, state);
                //继续响应
                listenfd.BeginAccept(AcceptCallback, listenfd);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Accept fail" + e.ToString());
            }
        }

        //异步接收实现
        public static void ReceiveCallback(IAsyncResult async)
        {
            try
            {
                ClientState state = (ClientState)async.AsyncState;
                Socket clientfd = state.Socket;
                int count = clientfd.EndReceive(async);
                //客户端关闭
                if (count == 0)
                {
                    clientfd.Close();
                    clients.Remove(clientfd);
                    Console.WriteLine("Socket Close");
                    return;
                }
                string receiveStr = System.Text.Encoding.UTF8.GetString(state.readBuffer, 0, count);
                byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes("Echo" + receiveStr);
                //clientfd.Send(sendBytes);
                clientfd.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, state);

                clientfd.BeginReceive(state.readBuffer, 0, 1024, 0, ReceiveCallback, state);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Receive fail" + e.ToString());
            }
        }

        //异步发送回调
        public static void SendCallback(IAsyncResult async)
        {
            try
            {
                ClientState state = (ClientState)async.AsyncState;
                Socket socket = state.Socket;
                int count = socket.EndSend(async);
                Console.WriteLine("Socket Send Succ" + count);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
    #endregion


    #region 同步实现
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("欢迎来到我的世界！");

            //声明socket
            Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //绑定
            IPAddress iPAddress = IPAddress.Parse("192.168.44.113");
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 8888);
            listenfd.Bind(iPEndPoint);
            //监听
            listenfd.Listen(0);
            Console.WriteLine("[服务器]启动成功！");

            while (true)
            {
                //Accept
                Socket connfd = listenfd.Accept();
                Console.WriteLine("[服务器]Accept");
                //Receive
                byte[] readBuffer = new byte[1024];
                int count = connfd.Receive(readBuffer);
                string receiveStr = System.Text.Encoding.UTF8.GetString(readBuffer, 0, count);
                Console.WriteLine("[服务器接收]" + receiveStr);

                //send
                byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(receiveStr);
                connfd.Send(sendBytes);
            }
        }
    }
    #endregion
}
