using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlCheck.Modele;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlCheck
{
    public class Message
    {
        public Message()
        {
            Messages = new List<Message>();
        }
        public void addMessage(Code code, TSqlFragment format, params string[] data)
        {
            Messages.Add(new Message(code, data, format));
        }
        public void addMessage(MyTyps text, TSqlFragment format, params string[] data)
        {
            Messages.Add(new Message(text, data, format));
        }
        public List<Message> Messages { get; set; }
        private MyTyps text;

        public Message(Code code, string[] data, TSqlFragment format)
        {
            this.Code = code;
            this.Data = data;
            this.Format = format;
        }
        public Message(MyTyps text, string[] data, TSqlFragment format)
        {
            this.text = text;
            this.Data = data;
            this.Format = format;
        }
        public string MessageInformation
        {
            get
            {
                string template = string.Format("({0}) {1} Line: {2}", Code.Value, Text.Message, Format?.StartLine);
                return string.Format(template, Data);
            }
        }
        public Code? Code { get; set; }
        public string[] Data { get; set; }
        public MyTyps Text
        {
            get
            {
                return Code.HasValue ? DictionaryMessage.GetMessage(Code.Value) : text;
            }
            private set
            {
                text = value;
            }
        }
        public TSqlFragment Format { get; set; }
        public int? StartLine { get { return Format?.StartLine; } }
    }
}
