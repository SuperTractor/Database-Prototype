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

namespace Database
{
    /// <summary>
    /// 提供数据服务的类；包含所有和提供数据服务必需的信息，给出服务接口
    /// </summary>
    public class DataService
    {
        // 客户端 socket
        Socket m_socket;

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
                            break;
                        case Command.Operation.Insert:
                            result = new Result(DBManager.Insert(command.tableName, (DataObject)command.data));
                            Serializer.Send(m_socket, result);
                            break;
                        case Command.Operation.IsExist:
                            result = new Result(DBManager.IsExist(command.tableName, (string)command.data));

                            Serializer.Send(m_socket, result);
                            break;
                        case Command.Operation.Update:
                            result = new Result(DBManager.Update(command.tableName, (DataObject)command.data));
                            Serializer.Send(m_socket,result);
                            break;
                        default:
                            break;
                    }
                }
            }
            // 可能是断线了
            catch(Exception e)
            {
                MyConsole.Log(string.Format("客户端{0}抛出异常：{1}", m_socket.RemoteEndPoint,e.Message));

                MyConsole.Log(string.Format("断开客户端{0}的连接", m_socket.RemoteEndPoint), MyConsole.LogType.Debug);
                // 断开连接
                m_socket.Close();
                //throw;

            }

        }
    }
}
