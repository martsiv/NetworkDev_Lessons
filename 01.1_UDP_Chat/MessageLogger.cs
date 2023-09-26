using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _01._1_UDP_Chat
{
    public class MessageLogger
    {
        public enum  MessageType { Request, Answer};
        public MessageType IsRequestOrAnswer { get; set; }
        public string MessageText { get; set; }
        public IPEndPoint IPData { get; set; }
        public DateTime MessageTime { get; set; }
        public string GetToString => ToString();
        public override string ToString()
        {
            return $"{(IsRequestOrAnswer == MessageType.Request ? "Request" : "Answer")} - IP: {IPData.ToString()} - Time: {MessageTime.ToShortTimeString()} - Message: {MessageText}";
        }
    }
}
