using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCheck
{
    public class SelectRegular
    {
        public List<MyColumn> ListMyColumns = new List<MyColumn>();
        public List<MyTable> ListMyTables = new List<MyTable>();
        public List<BooleanExpression> listConditions = new List<BooleanExpression>();
        public SelectRegular InnerQuery { get; set; }

        public SelectRegular(QuerySpecification query)
        {
            if (query.FromClause != null)
            {
                var from = query.FromClause as FromClause;
                foreach (TableReference tableReference in from.TableReferences)
                {
                    GetTableReference(tableReference);
                }
            }
        }
        void GetTableReference(TableReference tableReference)
        {
            if (tableReference is JoinTableReference)
            {
                var join = tableReference as JoinTableReference;
                GetTableReference(join.FirstTableReference);
                GetTableReference(join.SecondTableReference);
                if (tableReference is QualifiedJoin)
                    GetBooleanComparison((join as QualifiedJoin).SearchCondition);
            }
            else
            if (tableReference is NamedTableReference)
            {
                if ((tableReference as NamedTableReference).Alias != null)
                {
                    var alias = (tableReference as NamedTableReference).Alias?.Value;
                    if (alias != null)
                    {
                        //for (int i = 0; i < listColumns.Count; i++)
                        //{
                        //    if (listColumns[i].Alias != null && string.Compare(listColumns[i].Alias, alias, IsIgnoreCase) == 0)
                        //    {
                        //        listColumns.RemoveAt(i--);
                        //    }
                        //}
                    }
                }
                //проверить результат
                //if (IsTempTable((tableReference as NamedTableReference).SchemaObject.BaseIdentifier.Value))
                //{
                    //var table = GetTempTable((tableReference as NamedTableReference).SchemaObject);
                //}
                //else
                //{
                    ////
                //}
                //if (isAdd)
                //{
                    //if (myTableList != null)
                    //{
                        //string key = getAliasOrNameTable(tableReference);
                        /*myTableList.Add(new MyTable()
                        {
                            Name = key,
                            TableReference = tableReference
                        });*/
                    //}
                    //else
                    //{
                        //AddTable(tableReference);
                    //}
                //}
            }
            else
            if (tableReference is VariableTableReference)
            {
                //проверить результат
                //getDeclareTableVariable(tableReference as VariableTableReference);

                //if (isAdd)
                  //  AddTable(tableReference);
            }
            else
            if (tableReference is QueryDerivedTable)
            {
                var query = tableReference as QueryDerivedTable;
                //lastderivedTable = query.Alias.Value;
                //derivedTables.Push(new List<MyTable>() { new MyTable(lastderivedTable) });
                ///пока не понятно когда очищать этот список

                if (query.QueryExpression is BinaryQueryExpression)
                {
#warning нужна рекурсия
                }
                if (query.QueryExpression is QuerySpecification)
                {
                    InnerQuery = new SelectRegular(query.QueryExpression as QuerySpecification);
                }
            }
            else
            if (tableReference is SchemaObjectFunctionTableReference)
            {
                var t = tableReference as SchemaObjectFunctionTableReference;                

                foreach (var param in t.Parameters)
                {
                    if (param is VariableReference)
                    {
                        //var r = getDeclare(param as VariableReference);
                    }
                }
            }
        }

        private void GetBooleanComparison(BooleanExpression searchCondition)
        {
            //throw new NotImplementedException();
        }
    }
}
