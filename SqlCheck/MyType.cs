using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlCheck
{
    internal class MyType
    {
        public int Length { get; internal set; }
        public LiteralType Type { get; internal set; }
    }
}