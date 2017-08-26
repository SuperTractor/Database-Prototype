//#define ALI
#undef ALI

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using ConsoleUtility;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO;
using DatabaseUtility;

namespace DBNetworking
{
    /// <summary>
    /// 数据库客户端
    /// </summary>
    public class DBClient
    {
        static Socket m_socket;
        // 服务器 IP 地址
#if (ALI)
        static string m_serverIP = "39.108.178.24";
        //static string m_serverIP = "0.0.0.0";
#else
        static string m_serverIP = "192.168.56.1";

#endif
        static IPAddress m_ip;
        // 数据服务的端口
        static int m_port = 8884;

        // 控制台输出函数指针
        public delegate void LogHandler(string message);

        static LogHandler m_logHandler = null;

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

        public static void Initialize()
        {
            // 获取服务器 IP
            //m_ip = IPAddress.Parse(m_serverIP);
#if (ALI)
            m_ip = IPAddress.Parse(m_serverIP);
#else
             // 测试：自动获取本地 IP
            m_ip = GetLocalAddress();
#endif


            // 新建 socket
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        // 注册调试信息输出接口
        public static void RegisterLogger(LogHandler logHandler)
        {
            m_logHandler = logHandler;
        }

        // 调试输出信息封装
        static void Log(string message)
        {
            // 当有注册调试信息输出接口时，才会输出调试信息
            m_logHandler?.Invoke(message);
        }

        public static bool Connect()
        {
            //MyConsole.Log("准备连接服务器", MyConsole.LogType.Debug);
            Log(string.Format("准备连接数据库服务器{0}:{1}", m_ip, m_port));

            // 连接服务器
            m_socket.Connect(new IPEndPoint(m_ip, m_port));
            // 检查是否连接上服务器
            if (!m_socket.Connected)
            {
                throw new Exception("无法连接" + m_serverIP);
            }
            else
            {
                //MyConsole.Log("成功连接服务器", MyConsole.LogType.Debug);
                Log(string.Format("成功连接数据库服务器{0}:{1}", m_ip, m_port));

                //Log("成功连接服务器");
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
            Command command = new Command(tableName, Command.Operation.Insert, new DataObject(dataObj));
            Result result = (Result)Request(command);
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

        // 改
        static public bool Update(string tableName, string username, string itemName, object value)
        {
            // 新建命令
            Command command = new Command(tableName, username, itemName, value);
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

            if (result.code == Result.Code.Fail)
            {
                return null;
            }
            else
            {
                return (DataObject)result.data;
            }
        }

        static public void Disconnect()
        {
            m_socket.Close();
        }

        // 询问数据库，以检查连接状况
        static public bool CheckConnection()
        {
            // 新建命令
            Command command = new Command();
            try
            {
                Result result = (Result)Request(command);
                return true;
            }
            catch
            {
                // 有可能是断线了
                return false;
            }
        }

        static public void SummitUsername(string username)
        {
            if (username != null && username != "")
            {
                // 新建命令
                Command command = new Command(username);
                Result result = (Result)Request(command);
            }
        }
    }
}
