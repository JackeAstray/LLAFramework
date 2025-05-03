using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLAFramework
{
    /// <summary>
    /// 消息
    /// </summary>
    public class Message
    {
        public MessageTipType Type { get; private set; }
        public string Content { get; private set; }

        public Message(MessageTipType type, string content)
        {
            Type = type;
            Content = content;
        }
    }
}
