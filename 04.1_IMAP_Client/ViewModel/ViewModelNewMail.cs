using _04._1_IMAP_Client.Help;
using MailKit;
using MailServiceNamespace;
using Microsoft.Win32;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace _04._1_IMAP_Client.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class ViewModelNewMail
    {
        private MailServiceSMTP_IMAP mailService;
        public int SelectedPriority { get; set; } = 0;
        private ObservableCollection<string> priority = new ObservableCollection<string>();
        public IEnumerable<string> Priority => priority;
        private ObservableCollection<string> attachments = new ObservableCollection<string>();
        public IEnumerable<string> Attachments => attachments;
        public string To { get; set; }
        public string YourNameToShow { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public ViewModelNewMail(MailServiceSMTP_IMAP mailService)
        {
            this.mailService = mailService;
            priority.Add("Low");
            priority.Add("Normal");
            priority.Add("High");
            To = string.Empty;
            Subject = string.Empty;
            Text = string.Empty;
            sendMailCmd = new((o) => SendMail(), (o) => !string.IsNullOrEmpty(To));
            addAttachmentCmd = new((o) => AddAttachment());
        }
        private readonly RelayCommand sendMailCmd;
        public ICommand SendMailCmd => sendMailCmd;
        public async void SendMail()
        {
            // Прив'язати ліст з вкладаннями до attachments і передавати його в метод далі
            mailService.NameFrom = YourNameToShow;
            string answer = await mailService.SendNewMailAsync(Subject, Text, string.Empty, To, SelectedPriority, attachments.ToList<string>());
            MessageBox.Show(answer);
        }
        private readonly RelayCommand addAttachmentCmd;
        public ICommand AddAttachmentCmd => addAttachmentCmd;
        public async void AddAttachment()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
                attachments.Add(dialog.FileName);
        }
    }
}
