using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleUtility;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Client
{
    /// <summary>
    /// 数据库客户端
    /// </summary>
    public class DBClient
    {
        static Socket m_socket;
        // 服务器 IP 地址
        static string m_serverIP = "192.168.56.1";
        static IPAddress m_ip;
        // 数据服务的端口
        static int m_port = 8884;

        public static void Initialize()
        {
            // 获取服务器 IP
            m_ip = IPAddress.Parse(m_serverIP);
            // 新建 socket
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }


        public static bool Connect()
        {
            MyConsole.Log("准备连接服务器",MyConsole.LogType.Debug);
            // 连接服务器
            m_socket.Connect(new IPEndPoint(m_ip, m_port));
            // 检查是否连接上服务器
            if (!m_socket.Connected)
            {
                throw new Exception("无法连接" + m_serverIP);
            }
            else
            {
                MyConsole.Log("成功连接服务器", MyConsole.LogType.Debug);
            }
            return m_socket.Connected;
        }
    }
}
