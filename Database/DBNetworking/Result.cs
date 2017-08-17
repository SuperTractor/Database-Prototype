using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DBNetworking
{
    [Serializable]
    public class Result
    {
        object m_data;
        public object data
        {
            get { return m_data; }
        }

        public enum Code
        {
            Success,    // 请求处理成功
            Fail        // 请求失败
        }
        Code m_code;
        public Code code
        {
            get { return m_code; }
        }

        // 构造函数
        public Result(object data, Code code = Code.Success)
        {
            m_data = data;
            m_code = code;
        }
    }
}
