using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseUtility;
using ConsoleUtility;

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
            string command;
            do
            {
                command = Console.ReadLine();
                string[] tokens = command.Split(' ');
                switch (tokens[0])
                {
                    case "find":
                        DataObject dataObj = DBClient.Find(tokens[1], tokens[2]);
                        if (dataObj != null)
                        {
                            if (tokens[1] == "stat")
                            {
                                StatObject statObj = new StatObject(dataObj);
                                MyConsole.Log(string.Format("username:{0} score:{1}", statObj.username, statObj.score), MyConsole.LogType.Debug);
                            }
                            else if (tokens[1] == "user")
                            {
                                UserObject userObj = new UserObject(dataObj);
                                MyConsole.Log(string.Format("username:{0} password:{1}", userObj.username, userObj.password), MyConsole.LogType.Debug);
                            }
                        }
                        else
                        {
                            MyConsole.Log(string.Format("表{0}不存在用户{1}", tokens[1], tokens[2]));
                        }
                        break;
                    case "insert":
                        if (tokens[1] == "stat")
                        {
                            StatObject statObj = new StatObject();
                            statObj.username = tokens[2];
                            statObj.score = int.Parse(tokens[3]);
                            DBClient.Insert(tokens[1], statObj);
                        }
                        else if (tokens[1] == "user")
                        {
                            UserObject userObj = new UserObject();
                            userObj.username = tokens[2];
                            userObj.password = tokens[3];
                            DBClient.Insert(tokens[1], userObj);
                        }
                        break;
                    case "isexist":
                        bool isExist = DBClient.IsExist(tokens[1], tokens[2]);
                        if (!isExist)
                        {
                            MyConsole.Log(string.Format("表{0}不存在用户{1}", tokens[1], tokens[2]));
                        }
                        else
                        {
                            MyConsole.Log(string.Format("表{0}存在用户{1}", tokens[1], tokens[2]));
                        }
                        break;
                    case "update":
                        if (tokens[1] == "stat")
                        {
                            StatObject statObj = new StatObject();
                            statObj.username = tokens[2];
                            statObj.score = int.Parse(tokens[3]);
                            DBClient.Update(tokens[1], statObj);
                        }
                        else if (tokens[1] == "user")
                        {
                            UserObject userObj = new UserObject();
                            userObj.username = tokens[2];
                            userObj.password = tokens[3];
                            DBClient.Update(tokens[1], userObj);
                        }
                        break;
                    default:
                        break;
                }


            } while (command != "quit");

            DBClient.Disconnect();
        }
    }
}
