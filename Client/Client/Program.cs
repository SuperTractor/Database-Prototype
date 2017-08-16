using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        // 初始化函数
        static void Initialize()
        {
            DBClient.Initialize();
        }

        // 主函数
        static void Main(string[] args)
        {
            Initialize();
            DBClient.Connect();
        }
    }
}
