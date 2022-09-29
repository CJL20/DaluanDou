using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaLuanDou_Program
{
    public partial class MsgHandler
    {
        public static void MsgEnter(ClientState state, string msgBody)
        {
            Console.WriteLine("MsgEnter:OK");

            //参数解析
            string[] split = msgBody.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            float euly = float.Parse(split[4]);
            //赋值
            state.x = x;
            state.y = y;
            state.z = z;
            state.euly = euly;
            state.hp = 100;
            //广播
            string sendStr = "Enter|" + msgBody;
            foreach (var v in Program.clients.Values)
            {
                Program.Send(v, sendStr);
            }
        }

        public static void MsgList(ClientState state,string msgBody)
        {
            Console.WriteLine("MsgList:" + msgBody);

            string sendStr = "List|";
            foreach (var v in Program .clients .Values )
            {
                sendStr += v.Socket.RemoteEndPoint.ToString() + ",";
                sendStr += v.x.ToString() + ",";
                sendStr += v.y.ToString() + ",";
                sendStr += v.z.ToString() + ",";
                sendStr += v.euly.ToString() + ",";
                sendStr += v.hp.ToString() + ",";
            }
            Program.Send(state, sendStr);
        }

        public static void MsgMove(ClientState state ,string msgBody)
        {
            //参数解析
            string[] split = msgBody.Split(',');
            string desc = split[0];
            float x = float.Parse(split[1]);
            float y = float.Parse(split[2]);
            float z = float.Parse(split[3]);
            //赋值
            state.x = x;
            state.y = y;
            state.z = z;
            //广播
            string sendStr = "Move|" + msgBody;
            foreach (var v in Program .clients .Values)
            {
                Program.Send(v, sendStr);
            }
        }

        public static void MsgAudio(ClientState state ,string msgBody)
        {
            string sendStr = "Audio|" + msgBody;
            foreach (var v in Program.clients.Values)
            {
                Program.Send(v, sendStr);
            }
        }

        public static void MsgAttack(ClientState state ,string msgBody)
        {
            //广播
            string sendStr = "Attack|" + msgBody;
            foreach (var v in Program .clients .Values)
            {
                Program.Send(v, sendStr);
            }
        }

        public static void MsgHit(ClientState state ,string msgBody)
        {
          
            //解析参数
            string[] split = msgBody.Split(',');
            string attackerDesc = split[0];
            string hitedDesc = split[1];
            //找出被攻击的角色
            ClientState hitjs = null;
            foreach (var v in Program .clients .Values)
            {
                if(v.Socket .RemoteEndPoint .ToString ()==hitedDesc)
                {
                    hitjs = v;
                }
            }
            if (hitjs == null) return;

            //扣血
            hitjs.hp -= 20;
            #region 处理掉血
            string sendDamageStr = "Damage|";
            sendDamageStr += hitjs.Socket.RemoteEndPoint.ToString();
            foreach (var value in Program.clients.Values)
            {
                Program.Send(value, sendDamageStr);
            }
            //受击
            //string sendHit = "Hit|";
            //sendHit += hitjs.Socket.RemoteEndPoint.ToString();
            //foreach (var v in Program .clients .Values )
            //{
            //    Program.Send(v, sendHit);
            //}
            #endregion
            

            //死亡
            if (hitjs .hp <=0)
            {
                string sendStr = "Die|" + hitjs.Socket.RemoteEndPoint.ToString();
                foreach (ClientState v in Program .clients .Values)
                {
                    Program.Send(v, sendStr);
                }
            }
        }

        public static void MsgJump(ClientState state ,string msgBody)
        {
            string sendStr = "Jump|" + msgBody;
            foreach (var v in Program .clients .Values)
            {
                Program.Send(v, sendStr);
            }
        }
    }
}
