using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtility
{
    // 表单记录的基类
    [Serializable]
    public class DataObject : MetaObject
    {
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
