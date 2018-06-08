using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 客户端
{
    class Data
    {
       readonly System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
        public List<Person> GetPersonList()
        {
            //读取json数据
            string jsonStr = File.ReadAllText("Data.json");
            
            List<Person> list = js.Deserialize<List<Person>>(jsonStr);
            return list;
        }

        public bool Add(List<Person> list)
        {
            //向数据中覆盖追加
            string strJson = js.Serialize(list);
            try
            {
                File.WriteAllText("Data.json", strJson);
                return true;
            }
            catch
            {

                return false;
            }
           
        }
    }
}
