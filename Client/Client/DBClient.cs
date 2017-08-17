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
using DBNetworking;
using DatabaseUtility;

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

        // 获取本机地址
        static IPAddress GetLocalAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("没有找到 IP 地址");
        }

        public static void Initialize()
        {
            // 获取服务器 IP
            //m_ip = IPAddress.Parse(m_serverIP);

            // 测试：自动获取本地 IP
            m_ip = GetLocalAddress();

            // 新建 socket
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }

        public static bool Connect()
        {
            MyConsole.Log("准备连接服务器", MyConsole.LogType.Debug);
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

        // 向服务器发送请求
        public static object Request(Command command/*, object requestData*/)
        {
            Serializer.Send(m_socket, command);

            return Serializer.Receive(m_socket);
        }

        // 数据服务（操作）接口

        // 增
        static public bool Insert(string tableName, DataObject dataObj)
        {
            // 新建命令
            Command command = new Command(tableName, Command.Operation.Insert, new DataObject( dataObj));
            Result result =(Result)Request(command);
            return (bool)result.data;
        }

        // 改
        static public bool Update(string tableName, DataObject dataObj)
        {
            // 新建命令
            Command command = new Command(tableName, Command.Operation.Update, new DataObject(dataObj));
            Result result = (Result)Request(command);
            return (bool)result.data;
        }

        // 查
        static public bool IsExist(string tableName, string userName)
        {
            // 新建命令
            Command command = new Command(tableName, Command.Operation.IsExist, userName);
            Result result = (Result)Request(command);
            return (bool)result.data;
        }

        // 查
        static public DataObject Find(string tableName, string userName)
        {
            // 新建命令
            Command command = new Command(tableName, Command.Operation.Find, userName);
            Result result = (Result)Request(command);

            if(result.code== Result.Code.Fail)
            {
                return null;
            }
            else
            {
                return (DataObject)result.data;
            }
        }
    }
}
