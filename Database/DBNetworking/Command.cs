using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            IsExist,    // 是否存在
            //Delete,   // 删
            Update,      // 改；修改整条记录
            UpdateItem, // 改；修改指定记录的某项数据
            CheckConnection, // 检查连接状态
            SummitUsername  // 上传用户名
        }
        Operation m_operation;
        public Operation operation
        {
            get { return m_operation; }
        }

        object m_data;
        public object data
        {
            get { return m_data; }
        }

        // 修改指定记录的某项数据专用
        string m_username;
        public string username
        {
            get { return m_username; }
        }

        string m_itemName;
        public string itemName
        {
            get { return m_itemName; }
        }

        // 默认构造函数，专门用来检查连接状态的
        public Command()
        {
            m_tableName = "";
            m_data = 0;
            m_operation = Operation.CheckConnection;
        }

        // 专门用来上传用户名的构造函数
        public Command(string username)
        {
            m_tableName = "";
            m_data = username;
            m_operation = Operation.SummitUsername;
        }

        // 构造函数
        public Command(string tableName, Operation operation, object data)
        {
            m_tableName = tableName;
            m_operation = operation;
            m_data = data;
        }
        // 修改指定记录的指定项专用构造函数
        public Command(string tableName, string username, string itemName, object data)
        {
            m_tableName = tableName;
            m_operation = Operation.UpdateItem;
            m_data = data;
            m_username = username;
            m_itemName = itemName;
        }

    }
}
