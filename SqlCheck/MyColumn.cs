using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlCheck
{
    public class MyColumn : MyObjectFromServer
    {
        private ColumnDefinition col;

        private DataTypeReference DataType { get; set; }

        public MyColumn(Column column)
        {
            this.IsNullable = column != null ? column.Nullable : true;
            this.Column = column;
        }

        public MyColumn(string name, DataTypeReference dataType)
        {
            this.IsNullable = true;
            this.Name = name;
            this.DataType = dataType;
        }

        public MyColumn(ColumnDefinition col)
        {
            this.Name = col.ColumnIdentifier.Value;
            foreach (var con in col.Constraints)
            {
                if (con is NullableConstraintDefinition)
                {
                    var nul = con as NullableConstraintDefinition;
                    this.IsNullable = nul.Nullable;
                }
            }
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
        public bool IsNullable { get; internal set; }
    }
}
