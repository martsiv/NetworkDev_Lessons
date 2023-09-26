using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;
using System.IO;
using System.Windows;

namespace _01._1_UDP_Chat
{
    [AddINotifyPropertyChangedInterface]

    public class ViewModel
    {
        private ObservableCollection<MessageLogger> messages { get; set; }
        public IEnumerable<MessageLogger> Messages => messages;
        public string IPProp { get; set; }
        public int? PortProp { get; set; }
        public string MessageField { get; set; }

        public ViewModel()
        {
            messages = new ObservableCollection<MessageLogger>();
            sendMessageCmd = new((o) => SendMessage(), (с) => !string.IsNullOrEmpty(IPProp) && PortProp != null && PortProp.Value != 0);
            saveMessagesCmd = new((o) => SaveMessages(), (с) => 0 < messages.Count);
        }

        private readonly RelayCommand sendMessageCmd;
        public ICommand SendMessageCmd => sendMessageCmd;
        private readonly RelayCommand saveMessagesCmd;
        public ICommand SaveMessagesCmd => saveMessagesCmd;
        private void SendMessage()
        {
            try
            {
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPProp), PortProp.Value);
                IPEndPoint remoteIpPoint = new IPEndPoint(IPAddress.Any, 0);
                UdpClient client = new UdpClient();


                MessageLogger messageTo = new MessageLogger()
                {
                    IPData = ipPoint,
                    MessageTime = DateTime.Now,
                    MessageText = MessageField,
                    IsRequestOrAnswer = MessageLogger.MessageType.Request
                };
                messages.Add(messageTo);

                byte[] data = Encoding.Unicode.GetBytes(MessageField);
                client.Send(data, data.Length, ipPoint);
                data = client.Receive(ref remoteIpPoint);
                string response = Encoding.Unicode.GetString(data);

                MessageLogger messageFrom = new MessageLogger()
                {
                    IPData = remoteIpPoint,
                    MessageTime = DateTime.Now,
                    MessageText = response,
                    IsRequestOrAnswer = MessageLogger.MessageType.Answer
                };
                messages.Add(messageFrom);
                
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void SaveMessages()
        {
            string fileName = "Messages.txt";
            foreach (var message in messages)
                SaveResult(fileName, message.GetToString);
        }
        private void SaveResult(string filename, string message)
        {
            string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);

            using (StreamWriter writer = File.AppendText(logFilePath))
            {
                writer.WriteLine(message);
            }
        }
    }
}
