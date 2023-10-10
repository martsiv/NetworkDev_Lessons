using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailServiceNamespace;
using System.Collections.ObjectModel;
using MailKit;
using _04._1_IMAP_Client.Help;
using System.Windows.Input;
using _04._1_IMAP_Client.View;
using MimeKit;
using System.Globalization;
using System.Windows.Data;

namespace _04._1_IMAP_Client.ViewModel
{
    [AddINotifyPropertyChangedInterface]

    public class ViewModelWork
    {
        private MailServiceSMTP_IMAP mailService;
        private ObservableCollection<MainFoledr> mailFolders = new ObservableCollection<MainFoledr>();
        public IEnumerable<MainFoledr> MailFolders => mailFolders;
        public MainFoledr SelectedFolder { get; set; }
        private ObservableCollection<EmailListData> emailList = new ObservableCollection<EmailListData>();
        public IEnumerable<EmailListData> EmailList => emailList;
        public EmailListData? SelectedEmail { get; set; }
        public MimeMessage? FullMessage { get; set; }
        private ObservableCollection<string> attachments = new ObservableCollection<string>();
        public IEnumerable<string> Attachments => attachments;
        private WindowNewMail windowNewMail = null;
        private readonly RelayCommand openWindowNewMailCmd;
        public ICommand OpenWindowNewMailCmd => openWindowNewMailCmd;
        public async void OpenWindowNewMail()
        {

            windowNewMail = new WindowNewMail();
            windowNewMail.Closed += WindowNewMail_Closed;
            windowNewMail.DataContext = new ViewModelNewMail(mailService);
            windowNewMail.Show();
            
        }
        private void WindowNewMail_Closed(object sender, EventArgs e)
        {
            windowNewMail = null;
        }
        private bool CanOpenWindowNewMail(object obj)
        {
            // Check if window is openes. If yes, this will returned false.
            return windowNewMail == null || !windowNewMail.IsVisible;
        }

        public ViewModelWork(MailServiceSMTP_IMAP mailService)
        {
            openWindowNewMailCmd = new((o) => OpenWindowNewMail(), CanOpenWindowNewMail);
            showMailsInFolderCmd = new((o) => ShomMailsInFolder());
            showMailCmd = new((o) => ShomMail());
            deleteMessageCmd = new((o) => DeleteMessage(), (o) => SelectedEmail != null);
            replyMessageCmd = new((o) => ReplyMessage(), (o) => FullMessage != null);
            mailFolders.Add(new MainFoledr() { Name = "Уся пошта", ServiceName = SpecialFolder.All });
            mailFolders.Add(new MainFoledr() { Name = "Відправлені", ServiceName = SpecialFolder.Sent });
            mailFolders.Add(new MainFoledr() { Name = "Чернетки", ServiceName = SpecialFolder.Drafts });
            mailFolders.Add(new MainFoledr() { Name = "Спам", ServiceName = SpecialFolder.Junk });
            mailFolders.Add(new MainFoledr() { Name = "Видалені", ServiceName = SpecialFolder.Trash });
            mailFolders.Add(new MainFoledr() { Name = "Архів", ServiceName = SpecialFolder.Archive });
            this.mailService = mailService;
        }

        private readonly RelayCommand deleteMessageCmd;
        public ICommand DeleteMessageCmd => deleteMessageCmd;
        private async void DeleteMessage()
        {
            await mailService.DeleteMessageAsync(SelectedFolder.ServiceName, SelectedEmail.Id);
        }
        private readonly RelayCommand replyMessageCmd;
        public ICommand ReplyMessageCmd => replyMessageCmd;
        private async void ReplyMessage()
        {
            windowNewMail = new WindowNewMail();
            windowNewMail.Closed += WindowNewMail_Closed;
            windowNewMail.DataContext = new ViewModelNewMail(mailService) { To = FullMessage.From.First().Name, Subject = "Re:" + FullMessage.Subject };
            windowNewMail.Show();
        }
        private readonly RelayCommand showMailsInFolderCmd;
        public ICommand ShowMailsInFolderCmd => showMailsInFolderCmd;
        private async void ShomMailsInFolder()
        {
            emailList.Clear();
            var messages = await mailService.GetMessagesAsync(SelectedFolder.ServiceName);
            messages.Reverse();
            foreach (var message in messages)
            {
                if (20 < emailList.Count)
                    break;
                emailList.Add(message);
            }
        }
        private readonly RelayCommand showMailCmd;
        public ICommand ShowMailCmd => showMailCmd;
        private async void ShomMail()
        {
            var message = await mailService.GetMessageAsync(SelectedFolder.ServiceName, SelectedEmail.Id);
            FullMessage = message;
            
            var att = FullMessage.Attachments;
            foreach (var attachment in att)
                attachments.Add(attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name);
        }
    }
    public class MainFoledr
    {
        public string Name { get; set; }
        public SpecialFolder ServiceName { get; set; }
    }
    public class MessageContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MimeMessage message)
            {
                if (!string.IsNullOrEmpty(message.HtmlBody))
                {
                    return message.HtmlBody;
                }
                else if (!string.IsNullOrEmpty(message.TextBody))
                {
                    return message.TextBody;
                }
                else
                {
                    return message.Body;
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
