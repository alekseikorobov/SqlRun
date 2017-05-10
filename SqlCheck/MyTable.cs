using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCheck
{
    public class MyTable : MyObjectFromServer
    {
        public MyTable()
        {
            IsExists = false;
            Columns = new List<MyColumn>();
        }
        public MyTable(string name) : this()
        {
            Name = name;
        }
        public bool IsExists { get; set; }
        string name;
        public string Name { get { return name; } set { name = value; IsExists = true; } }
        public List<MyColumn> Columns { get; set; }
    }
}
