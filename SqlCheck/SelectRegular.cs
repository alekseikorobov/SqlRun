using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlCheck.Modele;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlCheck
{
    public class MySelectElement
    {
        public MySelectElement()
        {
            Expressions = new List<TSqlFragment>();
            ListSelectVariable = new List<VariableReference>();
        }
        public string ColumnName { get; set; }
        public List<TSqlFragment> Expressions { get; set; }
        public SelectScalarExpression SelectScalarExpression { get; set; }
        public List<VariableReference> ListSelectVariable { get; set; }
    }
    public class SelectRegular : MyObjectFromServer
    {
        public List<MySelectElement> MySelectElements = new List<MySelectElement>();
        public List<MyColumn> ListMyColumns = new List<MyColumn>();

        //public List<MyTable> ListMyTables = new List<MyTable>();
        public List<TableReference> ListTables = new List<TableReference>();
        public List<BooleanExpression> listConditions = new List<BooleanExpression>();

        public List<MyColumn> OrderByColumns = new List<MyColumn>();
        public List<MyColumn> OrderByVaribles = new List<MyColumn>();

        public string AliasTable { get; set; }
        public List<SelectRegular> InnerQuery { get; set; }
        public bool IsIgnoreCase { get; private set; }

        /// <summary>
        /// Варианты объектов
        /// select:
        /// @name
        /// name
        /// t.name
        /// dbo.name(@name1)
        /// dbo.name(t.name1)
        /// dbo.name(name1)
        /// n = name
        /// n = t.name
        /// n = (select @var)
        /// n = (select var from table)
        /// n = (select var from #table)
        /// n = case when 1=1 then 1 else 0 end
        /// 
        /// </summary>
        /// <param name="query"></param>
        public SelectRegular(QuerySpecification query)
        {
            InnerQuery = new List<SelectRegular>();

            if (query.OrderByClause != null)
            {
                foreach (var element in query.OrderByClause.OrderByElements)
                {
                    if (element.Expression is Literal)
                    {
                        //messages.addMessage(query.T0000051, element.Expression);
                    }
                }
            }
            if (query.FromClause != null)
            {
                var from = query.FromClause as FromClause;
                foreach (TableReference tableReference in from.TableReferences)
                {
                    GetTableReference(tableReference);
                }
            }

            foreach (var element in query.SelectElements)
            {
                if (element is SelectScalarExpression)
                {
                    var expression = element as SelectScalarExpression;
                    var selel = new MySelectElement()
                    {
                        ColumnName = GetColumnNameFromSelectScalarExpression(expression),
                        SelectScalarExpression = expression
                    };
                    SelectExpression(expression.Expression, selel);

                    MySelectElements.Add(selel);
                }
            }

            if (query.WhereClause != null)
            {
                GetWhereClause(query.WhereClause.SearchCondition);
                //checkedBooleanComparison(Query.WhereClause.SearchCondition);
            }
        }

        public SelectRegular(QuerySpecification query, string aliasTable) : this(query)
        {
            AliasTable = aliasTable;
        }

        private string GetColumnNameFromSelectScalarExpression(SelectScalarExpression expression)
        {
            string name = null;
            if (expression.ColumnName != null)
            {
                name = expression.ColumnName.Value;
            }
            if (expression.Expression is ColumnReferenceExpression)
            {
                var column = expression.Expression as ColumnReferenceExpression;
                name = column.MultiPartIdentifier.Identifiers[column.MultiPartIdentifier.Identifiers.Count - 1].Value;
            }
            return name;
        }

        private void SelectExpression(ScalarExpression expression, MySelectElement mySelectElement)
        {
            if (expression is VariableReference)
            {
                //ListSelectVariable.Add(expression as VariableReference);
                mySelectElement.ListSelectVariable.Add(expression as VariableReference);
                //mySelectElement.Expressions.Add()
            }
            else
            if (expression is ColumnReferenceExpression)
            {
                var myColumn = GetMyColumn(expression as ColumnReferenceExpression);

                mySelectElement.Expressions.Add(myColumn);

                //var myColumn = checkedColumnReference(expression.Expression as ColumnReferenceExpression);

                //if (myTableList != null)
                //{
                //    var lastTable = myTableList.SingleOrDefault(c => string.Compare(c.Name, lastderivedTable, IsIgnoreCase) == 0);
                //    if (lastTable != null)
                //    {
                //        if (expression.ColumnName != null)
                //        {
                //            myColumn.Name = expression.ColumnName.Value;
                //        }
                //        lastTable.AddColumns(myColumn);
                //    }
                //}
            }
            else
            if (expression is FunctionCall)
            {
                mySelectElement.Expressions.Add(expression);
                foreach (var param in (expression as FunctionCall).Parameters)
                {
                    if (param is VariableReference)
                    {
                        SelectExpression(param, mySelectElement);
                    }
                    else if (param is Literal)
                    {

                    }

                }
                //getResultFunctionCall(expression.Expression as FunctionCall);
            }
            else if (expression is SearchedCaseExpression)
            {
                //CheckedSearchedCase(expression.Expression as SearchedCaseExpression);
            }
            else if (expression is BinaryExpression) { mySelectElement.Expressions.Add(expression); }
            else if (expression is ExtractFromExpression) { mySelectElement.Expressions.Add(expression); }
            else if (expression is IdentityFunctionCall) { mySelectElement.Expressions.Add(expression); }
            else if (expression is OdbcConvertSpecification) { mySelectElement.Expressions.Add(expression); }
            else if (expression is CaseExpression) { mySelectElement.Expressions.Add(expression); }
            else if (expression is SimpleCaseExpression) { mySelectElement.Expressions.Add(expression); }
            else if (expression is CastCall) { mySelectElement.Expressions.Add(expression); }
            else if (expression is ConvertCall) { mySelectElement.Expressions.Add(expression); }
            else if (expression is IIfCall) { mySelectElement.Expressions.Add(expression); }
            else if (expression is LeftFunctionCall) { mySelectElement.Expressions.Add(expression); }
            else if (expression is NextValueForExpression) { mySelectElement.Expressions.Add(expression); }
            else if (expression is NullIfExpression) { mySelectElement.Expressions.Add(expression); }
            else if (expression is ParenthesisExpression) { mySelectElement.Expressions.Add(expression); }
            else if (expression is ParseCall) { mySelectElement.Expressions.Add(expression); }
            else if (expression is RightFunctionCall) { mySelectElement.Expressions.Add(expression); }
            else if (expression is ScalarSubquery) { mySelectElement.Expressions.Add(expression); }
            else if (expression is ValueExpression) { mySelectElement.Expressions.Add(expression); }
            else if (expression is GlobalVariableExpression) { mySelectElement.Expressions.Add(expression); }

        }


        void GetLiteral(Literal literal)
        {
            if (literal is BinaryLiteral) { }
            else if (literal is DefaultLiteral) { }
            else if (literal is IdentifierLiteral) { }
            else if (literal is IntegerLiteral) { }
            else if (literal is MaxLiteral) { }
            else if (literal is MoneyLiteral) { }
            else if (literal is NullLiteral) { }
            else if (literal is NumericLiteral) { }
            else if (literal is OdbcLiteral) { }
            else if (literal is RealLiteral) { }
            else if (literal is StringLiteral) { }
        }


        bool isFromSelect = true;
        private string value;

        private void GetWhereClause(BooleanExpression searchCondition)
        {
            if (searchCondition is BooleanBinaryExpression)
            {
                GetWhereClause((searchCondition as BooleanBinaryExpression).FirstExpression);
                GetWhereClause((searchCondition as BooleanBinaryExpression).SecondExpression);
            }
            else
            if (searchCondition is BooleanTernaryExpression)
            {
                #region BooleanTernaryExpression


                var ter = searchCondition as BooleanTernaryExpression;

                if (ter.FirstExpression is ColumnReferenceExpression)
                {

                    GetMyColumn(ter.FirstExpression as ColumnReferenceExpression);

                }
                if (ter.FirstExpression is VariableReference)
                {
                    //ListSelectVariable.Add(ter.FirstExpression as VariableReference);
                    //getDeclare(ter.FirstExpression as VariableReference);
                }
                if (ter.SecondExpression is ColumnReferenceExpression)
                {
                    GetMyColumn(ter.SecondExpression as ColumnReferenceExpression);
                }
                if (ter.SecondExpression is VariableReference)
                {
                    //getDeclare(ter.SecondExpression as VariableReference);
                    //ListSelectVariable.Add(ter.SecondExpression as VariableReference);
                }
                if (ter.ThirdExpression is BinaryExpression)
                {
                    var scal = ter.ThirdExpression as BinaryExpression;
                    if (scal.FirstExpression is VariableReference)
                    {
                        //getDeclare(scal.FirstExpression as VariableReference);
                        //ListSelectVariable.Add(scal.FirstExpression as VariableReference);
                    }
                    if (scal.SecondExpression is VariableReference)
                    {
                        //getDeclare(scal.SecondExpression as VariableReference);
                        //ListSelectVariable.Add(scal.SecondExpression as VariableReference);
                    }
                    if (scal.FirstExpression is ColumnReferenceExpression)
                    {
                        GetMyColumn(scal.FirstExpression as ColumnReferenceExpression);
                    }
                    if (scal.SecondExpression is ColumnReferenceExpression)
                    {
                        GetMyColumn(ter.SecondExpression as ColumnReferenceExpression);
                    }
                }
                #endregion
                //bool Th = checkedBooleanComparison(ter.ThirdExpression);

                //if (ter.ThirdExpression is BooleanBinaryExpression)
                //{
                //    bool IsFirst1 = checkedBooleanComparison((ter.ThirdExpression as BinaryExpression).FirstExpression);
                //    bool Second2 = checkedBooleanComparison( (ter.ThirdExpression as BooleanBinaryExpression).SecondExpression);
                //}

                //    bool IsFirst = checkedBooleanComparison(ter.FirstExpression);
                //bool Second = checkedBooleanComparison(ter.SecondExpression);
            }
            else
            if (searchCondition is BooleanComparisonExpression)
            {
                GetDataFromCondition(searchCondition as BooleanComparisonExpression);
                //checkedBooleanComparisonExpression(searchCondition as BooleanComparisonExpression, isFromSelect);
            }
            else
            if (searchCondition is BooleanIsNullExpression)
            {
                //checkedBooleanIsNullExpression(searchCondition as BooleanIsNullExpression);
            }
            else
            if (searchCondition is ExistsPredicate)
            {
            }
            else
            if (searchCondition is InPredicate)
            {
                if ((searchCondition as InPredicate).Expression is ColumnReferenceExpression)
                {
                    GetMyColumn((searchCondition as InPredicate).Expression as ColumnReferenceExpression);
                }
                if ((searchCondition as InPredicate).Subquery is ScalarSubquery)
                {
                    //GetScalarSubquery((searchCondition as InPredicate).Subquery as ScalarSubquery);
                }
            }
            else
            if (searchCondition is BooleanParenthesisExpression)
            {
                var par = searchCondition as BooleanParenthesisExpression;
                if (par.Expression is BooleanBinaryExpression)
                {
                    GetWhereClause(par.Expression);
                }
            }

            else if (searchCondition is BooleanNotExpression) { }
            else if (searchCondition is EventDeclarationCompareFunctionParameter) { }
            else if (searchCondition is ExistsPredicate) { }
            else if (searchCondition is FullTextPredicate) { }
            else if (searchCondition is GraphMatchExpression) { }
            else if (searchCondition is GraphMatchPredicate) { }
            else if (searchCondition is InPredicate) { }
            else if (searchCondition is LikePredicate) { }
            else if (searchCondition is SubqueryComparisonPredicate) { }

            else if (searchCondition is BooleanExpressionSnippet) { }
            else if (searchCondition is TSEqualCall) { }
            else if (searchCondition is UpdateCall) { }

        }
        private void GetDataFromCondition(BooleanComparisonExpression booleanComparisonExpression)
        {
            if (booleanComparisonExpression.FirstExpression is ColumnReferenceExpression)
            {
                var myColumn = GetMyColumn(booleanComparisonExpression.FirstExpression as ColumnReferenceExpression);
            }
            if (booleanComparisonExpression.SecondExpression is ColumnReferenceExpression)
            {
                var myColumn = GetMyColumn(booleanComparisonExpression.SecondExpression as ColumnReferenceExpression);
            }
        }

        MyColumn getColumnReference(ColumnReferenceExpression Expression)
        {
            var column = new MyColumn();

            var Identifiers = Expression.MultiPartIdentifier.Identifiers;
            column.Alias = Identifiers.Count > 1 ? Identifiers[Identifiers.Count - 2].Value : null;
            column.Name = Identifiers[Identifiers.Count - 1].Value;

            column.Expression = Expression;

            column.IsValid = !(Identifiers.Count > 2);
            if (!column.IsValid)
            {
                column.ErrorCode = Code.T0000027;
            }
            return column;
        }
        private SelectRegular getInnerQuery(string alias)
        {
            return InnerQuery.SingleOrDefault(c => c.AliasTable.Eq(alias, IsIgnoreCase));
        }
        private MyTable getTableFromAlias(string alias)
        {
            foreach (var table in ListTables)
            {
                if ((table is TableReferenceWithAlias) &&
                    (table as TableReferenceWithAlias).Alias.Value.Eq(alias, IsIgnoreCase))
                {
                    return new MyTable(table);
                }
            }
            return null;
        }
        MyColumn GetMyColumn(ColumnReferenceExpression columnReferenceExpression)//string alias, string columnName, TSqlFragment fragment)
        {
            MyColumn column = getColumnReference(columnReferenceExpression);

            if (!column.IsValid) return column;
            //column = getColumnFromDerivedTable(alias, columnName, fragment);
            //if (column != null) return column;
            if (column.Alias != null)
            {
                var table = getTableFromAlias(column.Alias);
                if (table != null)
                {
                    column.MyTable = table;
                }
                else
                {
                    var inner = getInnerQuery(column.Alias);
                    if (inner != null)
                    {
                        column.InnerQuery = inner;
                    }
                }
                //if (table is NamedTableReference) {
                //var myTable = GetMyTable(table as NamedTableReference, true);
                //if (myTable != null && myTable.IsExists)
                //{
                //    column = myTable.getColumn(column.Name);
                //    if (column != null)
                //    {
                //        //messages.addMessage(Code.T0000029, table, (table as NamedTableReference).SchemaObject.BaseIdentifier.Value, columnName);
                //    }
                //}
                //} if (table is VariableTableReference) {
                //DeclareTableVariableBody myTable = getDeclareTableVariable(table as VariableTableReference);
                //if (myTable != null)
                //{
                //    foreach (var col in myTable.Definition.ColumnDefinitions)
                //    {
                //        if (col is ColumnDefinition)
                //        {
                //            var c = col as ColumnDefinition;
                //            if (string.Compare(c.ColumnIdentifier.Value, column.Name, IsIgnoreCase) == 0)
                //            {
                //                column = new MyColumn(c);
                //                break;
                //            }
                //        }
                //    }
                //    if (column == null)
                //    {
                //        //messages.addMessage(Code.T0000029, table, (table as VariableTableReference).Variable.Name, columnName);
                //    }
                //}
                //}
            }
            else
            {
                //List<MyColumn> cols = new List<MyColumn>();
                //bool IsSendMessage = false;
                //foreach (var table in tables)
                //{
                //    var myTable = GetMyTable(table.Value.Obj, true);

                //    if (myTable != null && myTable.IsExists)
                //    {
                //        IsSendMessage = true;
                //        MyColumn myColumn = myTable.getColumn(columnName);
                //        if (myColumn != null)
                //            cols.Add(myColumn);

                //        if (cols.Count > 1)
                //            break;
                //    }
                //}
                //if (cols.Count == 1)
                //{
                //    column = cols[0];
                //}
                //if (cols.Count > 1)
                //{
                //    messages.addMessage(Code.T0000033, fragment, columnName);
                //}
                //if (IsSendMessage && cols.Count == 0)
                //    messages.addMessage(Code.T0000030, fragment, columnName);
            }
            return column;
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
                ListTables.Add(tableReference);
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
                ListTables.Add(tableReference);
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
                    InnerQuery.Add(new SelectRegular(query.QueryExpression as QuerySpecification, query.Alias.Value));
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
