﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ConsoleUtility;
using DBNetworking;

namespace Database
{
    /// <summary>
    /// 数据库服务器
    /// </summary>
    public class DBServer
    {
        // 服务的端口
        static int m_port = 8884;
        // 服务器用来监听连接的 socket
        static Socket m_socket;
        // 服务器的 IP 地址
        static IPAddress m_ip;
        // 挂起的连接队列的最大长度
        static int m_backlog = 1000;
        // 服务器容量
        static int m_capacity = 400;
 
        // 当前已连接的玩家总数
        //static int m_currentConnectCount = 0;


        //static string m_serverIP = "39.108.178.24";
        //static string m_serverIP = "0.0.0.0";
        static string m_serverIP = "192.168.56.1";

        // 指示正在等待客户端连接服务器的事件
        private static ManualResetEvent m_waitingCustomerEvent = new ManualResetEvent(false);
        public static ManualResetEvent waitingCustomerEvent
        {
            get { return m_waitingCustomerEvent; }
        }

        // 数据服务列表
        static List<DataService> m_dataServices;

        // 获取本机地址
        static IPAddress GetLocalAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            for (int i = host.AddressList.Length - 1; i >= 0; i--)
            {
                if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    return host.AddressList[i];
                }
            }

            //foreach (var ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        return ip;
            //    }
            //}
            throw new Exception("没有找到 IP 地址");

        }

        // 初始化函数
        public static void Initialize()
        {
            //m_ip = IPAddress.Parse(m_serverIP); 
                    // 获取本机地址
            m_ip = GetLocalAddress();

            // 侦听连接
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Bind(new IPEndPoint(m_ip, m_port));
            m_socket.Listen(m_backlog);

            MyConsole.Log("启动监听" + m_socket.LocalEndPoint.ToString() + "成功",/* "ComServer",*/ MyConsole.LogType.Debug);

            m_dataServices = new List<DataService>();
        }

        // 等待客户端连接
        public static void WaitClient()
        {
            MyConsole.Log("准备开始接待线程", MyConsole.LogType.Debug);

            // 数据服务线程 ID
            int id = 0;

            while (true)
            {
                MyConsole.Log("等待客户端接入", MyConsole.LogType.Debug);
                // 指示正在等待客户端连接
                m_waitingCustomerEvent.Set();
                // 接收客户端连接
                Socket socket = m_socket.Accept();
                // 指示已经有客户端连接，准备开始接待事宜
                m_waitingCustomerEvent.Reset();

                MyConsole.Log(string.Format("客户端 {0} 连接到数据库", socket.RemoteEndPoint));

                // 首先清除已经完成的服务
                m_dataServices.RemoveAll(service => service.doneService);

                // 告知客户端是服务器是否已经超载
                Serializer.Send(socket, m_dataServices.Count >= m_capacity);

                // 如果连接数据库的客户端超出了服务器容量
                if (m_dataServices.Count >= m_capacity)
                {
                    // 断开此客户端的连接
                    socket.Close();
                }
                // 如果服务器还没有超载
                else
                {
                    // 创建数据服务 
                    m_dataServices.Add(new DataService(socket));
                    // 创建数据服务线程
                    Thread serviceThread = new Thread(new ThreadStart(m_dataServices.Last().Serve));
                    // 给数据服务线程命名
                    serviceThread.Name = string.Format("数据服务 {0}", m_dataServices.Count);
                    // 启动数据服务
                    serviceThread.Start();
                    // 等待数据服务启动
                    while (!serviceThread.IsAlive) ;
                }
            }
        }
    }
}
