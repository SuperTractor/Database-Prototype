using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBNetworking
{
    /// <summary>
    /// 客户端向服务器发送命令
    /// </summary>
    /// 
    [Serializable]
    public class Command
    {
        // 要操作的表名
        string m_tableName;
        public string tableName
        {
            get { return m_tableName; }
        }
        // 操作
        public enum Operation
        {
            Insert,     // 增
            Find,       // 查
            //Delete,   // 删
            Update      // 改
        }
        Operation m_operation;
        public Operation operation
        {
            get { return m_operation; }
        }

        // 构造函数
        public Command(string tableName,Operation operation)
        {
            m_tableName = tableName;
            m_operation = operation;
        }
    }
}
