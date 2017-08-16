using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Initialize()
        {
            DBClient.Initialize();
        }

        static void Main(string[] args)
        {
            Initialize();
            DBClient.Connect();


        }
    }
}
