using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Metadata
    {
        public int ClassVersion { get; set; }
    }

    public abstract class BaseMessage
    {
        public DateTime Date { get; set; }

        protected BaseMessage()
        {
            Date = DateTime.Now;
        }
    }

    public class CompletedMessage: BaseMessage
    {
        
    }

    public class TextMessageSent:BaseMessage
    {
        public string Sender { get; set; }
        public string Message { get; set; }
    }
}
