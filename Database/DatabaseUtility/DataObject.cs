using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtility
{
    // 表单记录的基类
    [Serializable]
    public class DataObject
    {
        // 一系列命名变量
        public List<NamedVariable> variables;
        //public List<NamedVariable> variables
        //{
        //    get { return m_variables; }
        //}
        // 存取接口
        public object Get(string name)
        {
            NamedVariable variable = variables.Find(namedVar => namedVar.name == name);
            if (variable != null)
            {
                return variable.data;
            }
            else
            {
                return null;
            }
            //return variables.Find(namedVar => namedVar.name == name).data;
        }

        public void Set(string name, object value)
        {
            NamedVariable variable = variables.Find(namedVar => namedVar.name == name);
            if (variable != null)
            {
                variable.data = value;
            }
        }

        public string username
        {
            get
            {
                return (string)Get("username");
            }
            set
            {
                Set("username", value);
            }
        }

        //public int id { get; set; }
        public DateTime lastUpdatedTime
        {
            get
            {
                return (DateTime)Get("lastUpdatedTime");
            }
            set
            {
                Set("lastUpdatedTime", value);
            }
        }

        // 构造函数
        public DataObject()
        {
            // 创建数据模板
            variables = new List<NamedVariable>();
            variables.Add(new NamedVariable("username", "string"));
            variables.Add(new NamedVariable("lastUpdatedTime", "DateTime"));

            // 盖个时间戳
            lastUpdatedTime = DateTime.Now;
        }


        // 复制构造函数
        public DataObject(DataObject dataObj)
        {
            variables = new List<NamedVariable>(dataObj.variables);
        }




    }
}
