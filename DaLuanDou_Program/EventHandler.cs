using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaLuanDou_Program
{
    public class EventHandler
    {
        //掉线处理
        public static void OnDisConnect(ClientState client)
        {
            string desc = client.Socket.RemoteEndPoint.ToString();
            string sendStr = "Leave|" + desc + ",";
            foreach (ClientState v in Program .clients .Values)
            {
                Program.Send(v, sendStr);
            }
        } 
    }
}
