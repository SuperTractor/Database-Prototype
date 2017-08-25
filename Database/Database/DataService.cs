using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using DBNetworking;
using DatabaseUtility;
using ConsoleUtility;
using System.Diagnostics;

namespace Database
{
    /// <summary>
    /// 提供数据服务的类；包含所有和提供数据服务必需的信息，给出服务接口
    /// </summary>
    public class DataService
    {
        // 客户端 socket
        Socket m_socket;

        // 可选的用户名；当为 "" 或者 null 表示这不是客户端，而是服务器；否则就是客户端
        // 用来断线后，标记离线的；所有客户端连线之后，登录用户之后，一定要发送用户名
        string m_username = null;



        // 构造函数
        public DataService(Socket socket)
        {
            m_socket = socket;
        }

        // 数据服务接口
        public void Serve()
        {
            try
            {
                while (true)
                {
                    // 接收客户端发来的命令
                    Command command = (Command)Serializer.Receive(m_socket);

                    // 处理请求，并返回结果
                    Result result;

                    switch (command.operation)
                    {
                        case Command.Operation.Find:
                            DataObject dataObj = DBManager.Find(command.tableName, (string)command.data);
                            if (dataObj != null)
                            {
                                Serializer.Send(m_socket, new Result(dataObj));
                            }
                            else
                            {
                                Serializer.Send(m_socket, new Result(dataObj, Result.Code.Fail));
                            }
                            MyConsole.Log(string.Format("客户端{0}查找 - 表单{1}；用户名{2}", m_socket.RemoteEndPoint, command.tableName, (string)command.data));
                            break;
                        case Command.Operation.Insert:
                            result = new Result(DBManager.Insert(command.tableName, (DataObject)command.data));
                            Serializer.Send(m_socket, result);
                            MyConsole.Log(string.Format("客户端{0}插入 - 表单{1}", m_socket.RemoteEndPoint, command.tableName));
                            break;
                        case Command.Operation.IsExist:
                            result = new Result(DBManager.IsExist(command.tableName, (string)command.data));
                            Serializer.Send(m_socket, result);
                            MyConsole.Log(string.Format("客户端{0}查存 - 表单{1}；用户名{2}", m_socket.RemoteEndPoint, command.tableName, (string)command.data));
                            break;
                        case Command.Operation.Update:
                            result = new Result(DBManager.Update(command.tableName, (DataObject)command.data));
                            Serializer.Send(m_socket,result);
                            MyConsole.Log(string.Format("客户端{0}更新 - 表单{1}", m_socket.RemoteEndPoint, command.tableName));
                            break;
                        case Command.Operation.UpdateItem:
                            result = new Result(DBManager.Update(command.tableName, command.username, command.itemName, command.data));
                            Serializer.Send(m_socket, result);
                            MyConsole.Log(string.Format("客户端{0}更新 - 表单{1}；用户名{2}；数据项{3}", m_socket.RemoteEndPoint, command.tableName, command.username, command.itemName));
                            break;
                        case Command.Operation.CheckConnection:
                            result = new Result(true,Result.Code.Success);
                            Serializer.Send(m_socket, result);
                            break;
                        case Command.Operation.SummitUsername:
                            m_username = (string)command.data;
                            result = new Result("收到用户名");
                            Serializer.Send(m_socket, result);
                            break;
                        default:
                            break;
                    }

                }
            }
            // 可能是断线了
            catch(Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);

                StackFrame[] frames = st.GetFrames();

                for (int i = 0; i < frames.Length; i++)
                {
                    MyConsole.Log(frames[i].ToString());
                }

                MyConsole.Log(string.Format("客户端{0}抛出异常：{1}", m_socket.RemoteEndPoint, ex.Message));

                MyConsole.Log(string.Format("断开客户端{0}的连接", m_socket.RemoteEndPoint), MyConsole.LogType.Debug);

                // 如果有存用户名
                if (m_username != null && m_username != "")
                {
                    // 将指定用户标记为离线
                    DBManager.Update("user", m_username, "isOnline", false);
                }


                // 断开连接
                m_socket.Close();
                //throw;

            }

        }
    }
}
