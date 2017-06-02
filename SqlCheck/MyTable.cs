using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.TransactSql.ScriptDom;

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

        public MyTable(TableReference table)
        {
            this.TableReference = table;
        }

        public bool IsExists { get; set; }
        string name;
        public string Name { get { return name; } set { name = value; IsExists = true; } }
        public List<MyColumn> Columns { get; set; }
        public TableReference TableReference { get; set; }
        public string Alias { get; set; }


        internal void AddColumns(ColumnDefinition column)
        {
            Columns.Add(new MyColumn(column));
        }

        internal void AddColumns(Column column)
        {
            Columns.Add(new MyColumn(column));
        }

        public bool ExistsColumn(ColumnReferenceExpression column)
        {
            var dif = column.MultiPartIdentifier.Identifiers;
            return ExistsColumn(dif[dif.Count - 1]?.Value);
        }

        public bool ExistsColumn(string columnName)
        {
            var column = getColumn(columnName);
            return column != null;
        }

        public MyColumn getColumn(string columnName)
        {
            return this.Columns.SingleOrDefault(c => string.Compare(c.Name, columnName, true) == 0);
        }

        public object getColumn(ColumnReferenceExpression column)
        {
            var dif = column.MultiPartIdentifier.Identifiers;
            return getColumn(dif[dif.Count - 1]?.Value);
        }
        public void AddColumns(MyColumn myColumn)
        {
            Columns.Add(myColumn);
        }
    }
}
