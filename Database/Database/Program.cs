using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBNetworking;
using System.Threading;

namespace Database
{
    class Program
    {
        // 启动前台接待线程
        static void StartWaitClient()
        {
            // 新建前台接待线程
            Thread waitThread = new Thread(new ThreadStart(DBServer.WaitClient));
            waitThread.Name = "接待处";
            // 启动前台接待线程
            waitThread.Start();
            // Spin for a while waiting for the started thread to become
            // alive:
            while (!waitThread.IsAlive) ;
        }

        // 初始化函数
        static void Initialize()
        {
            // 这是主线程
            Thread.CurrentThread.Name = "主线程";
            // 初始化数据库服务器
            DBServer.Initialize();
        }

        static void Main(string[] args)
        {
            // 初始化服务器
            Initialize();

            // 启动前台接待线程
            StartWaitClient();

        }
    }
}
