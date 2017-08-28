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
        Socket m_socket;
        // 服务器 IP 地址
#if (ALI)
         string m_serverIP = "39.108.178.24";
        // string m_serverIP = "0.0.0.0";
#else
        string m_serverIP = "192.168.56.1";

#endif
        IPAddress m_ip;
        // 数据服务的端口
        int m_port = 8884;

        // 连接服务器返回信息
        string m_connectMessage;
        public string connectMessage
        {
            get { return m_connectMessage; }
        }

        // 控制台输出函数指针
        public delegate void LogHandler(string message);

        LogHandler m_logHandler = null;

        // 读写锁，用来处理多个进程同时使用 socket 的冲突
        ReaderWriterLockSlim m_lock = new ReaderWriterLockSlim();

        // 获取本机地址
        IPAddress GetLocalAddress()
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



        public void Initialize()
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
        public void RegisterLogger(LogHandler logHandler)
        {
            m_logHandler = logHandler;
        }

        // 调试输出信息封装
        void Log(string message)
        {
            // 当有注册调试信息输出接口时，才会输出调试信息
            m_logHandler?.Invoke(message);
        }

        /// <summary>
        /// 连接函数；超过一定时长连接不上，就认为是网络异常；其实也可能是服务器很繁忙，但先不考虑
        /// </summary>
        /// <param name="timeoutMilliseconds">超时限制；默认为 0，即等待服务器 0 毫秒，第一次连接不行就异常</param>
        /// <returns></returns>
        public bool Connect(int timeoutMilliseconds = 0)
        {
            //MyConsole.Log("准备连接服务器", MyConsole.LogType.Debug);
            Log(string.Format("准备连接数据库服务器 {0}:{1}", m_ip, m_port));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // 不断尝试连接
            do
            {
                try
                {
                    // 连接服务器
                    m_socket.Connect(new IPEndPoint(m_ip, m_port));
                }
                // 如果连接不上
                catch
                {
                    // 如果已经超时了
                    if (stopwatch.ElapsedMilliseconds > timeoutMilliseconds)
                    {
                        // 抛出超时异常
                        throw new Exception(string.Format("连接数据库服务器 {0}:{1} 超时", m_ip, m_port));
                    }
                    // 继续连
                    continue;
                }
                // 如果连接上了
                if (m_socket.Connected)
                {
                    // 跳出连接尝试循环
                    break;
                }
             
            }
            while (true);


            // 检查是否连接上服务器
            if (!m_socket.Connected)
            {
                m_connectMessage = "无法连接服务器";
                throw new Exception("无法连接" + m_serverIP);
            }
            // 如果连接上服务器
            else
            {
                // 从服务器获取超载信息
                bool isOverload = (bool)Serializer.Receive(m_socket);
                // 如果服务器已经超载了
                if (isOverload)
                {
                    m_connectMessage = "服务器已经过载，请稍后连接";
                    Log(string.Format("数据库服务器 {0}:{1} 过载", m_ip, m_port));
                    return false;
                }
                // 如果服务器没有超载
                else
                {
                    m_connectMessage = "成功连接服务器";
                    //MyConsole.Log("成功连接服务器", MyConsole.LogType.Debug);
                    Log(string.Format("成功连接数据库服务器 {0}:{1}", m_ip, m_port));
                    //Log("成功连接服务器");
                }
            }
            return m_socket.Connected;
        }

        // 向服务器发送请求
        public object Request(Command command/*, object requestData*/)
        {
            m_lock.EnterWriteLock();
            object obj;
            try
            {
                Serializer.Send(m_socket, command);

                obj = Serializer.Receive(m_socket);
            }
            finally
            {
                m_lock.ExitWriteLock();
            }
            return obj;
        }

        // 数据服务（操作）接口

        // 增
        public bool Insert(string tableName, DataObject dataObj)
        {
            // 新建命令
            Command command = new Command(tableName, Command.Operation.Insert, new DataObject(dataObj));
            Result result = (Result)Request(command);
            return (bool)result.data;
        }

        // 改
        public bool Update(string tableName, DataObject dataObj)
        {
            // 新建命令
            Command command = new Command(tableName, Command.Operation.Update, new DataObject(dataObj));
            Result result = (Result)Request(command);
            return (bool)result.data;
        }

        // 改
        public bool Update(string tableName, string username, string itemName, object value)
        {
            // 新建命令
            Command command = new Command(tableName, username, itemName, value);
            Result result = (Result)Request(command);
            return (bool)result.data;
        }


        // 查
        public bool IsExist(string tableName, string userName)
        {
            // 新建命令
            Command command = new Command(tableName, Command.Operation.IsExist, userName);
            Result result = (Result)Request(command);
            return (bool)result.data;
        }

        // 查
        public DataObject Find(string tableName, string userName)
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

        public void Disconnect()
        {
            m_socket.Close();
        }

        // 询问数据库，以检查连接状况
        public bool CheckConnection()
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

        public void SummitUsername(string username)
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
