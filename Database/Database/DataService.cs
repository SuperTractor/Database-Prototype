using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

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

        }
    }
}
