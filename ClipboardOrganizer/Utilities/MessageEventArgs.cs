using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardOrganizer.Utilities
{
    public enum MessageTypeEnum { Good, Info, Warning, Error }

    public class MessageEventArgs
    {
        public MessageTypeEnum MessageType { get; private set; }

        public string Message { get; private set; }

        public MessageEventArgs(MessageTypeEnum messageType, string message)
        {
            MessageType = messageType;
            Message = message;
        }
    }
}
