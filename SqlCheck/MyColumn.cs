using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCheck
{
    public class MyColumn : MyObjectFromServer
    {
        public MyColumn(Column column)
        {
            this.Column = column;
        }
        public string FullName
        {
            get
            {
                return !string.IsNullOrEmpty(Alias) ? Alias + "." + Name : Name;
            }
        }
        public Column Column { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public bool IsValid { get; internal set; }
    }
}
