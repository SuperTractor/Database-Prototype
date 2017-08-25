using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseUtility
{
    [Serializable]
    public class MetaObject
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

    }
}
