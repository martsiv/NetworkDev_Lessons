using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailServiceNamespace
{
    [AddINotifyPropertyChangedInterface]
    public class MailServiceSMTP_IMAP
    {
        public string NameOfService { get; init; }
        private string SMTP_Host { get; init; }
        private int SMTP_Port { get; init; }
        private string IMAP_Host { get; init; }
        private int IMAP_Port { get; init; }
        public string NameFrom { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        private SmtpClient smtpClient;
        private ImapClient imapClient;
        public MailServiceSMTP_IMAP(string NameOfService, string SMTP_Host, int SMTP_Port, string IMAP_Host, int IMAP_Port)
        {
            smtpClient = new SmtpClient();
            imapClient = new ImapClient();
            this.NameOfService = NameOfService;
            this.SMTP_Host = SMTP_Host;
            this.SMTP_Port = SMTP_Port;
            this.IMAP_Host = IMAP_Host;
            this.IMAP_Port = IMAP_Port;
        }
        public async Task<bool> IsAuthentificateSuccessAsync()
        {
            try
            {
                await smtpClient.ConnectAsync(SMTP_Host, SMTP_Port);
                await smtpClient.AuthenticateAsync(Login, Password);
                await smtpClient.DisconnectAsync(true);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine("Problem with SMTP Authentication: " + ex);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            try
            {
                await imapClient.ConnectAsync(IMAP_Host, IMAP_Port);
                await imapClient.AuthenticateAsync(Login, Password);
                await imapClient.DisconnectAsync(true);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine("Problem with IMAP Authentication: " + ex);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }
        public async Task<IList<IMailFolder>> GetFoldersAsync()
        {
            await imapClient.ConnectAsync(IMAP_Host, IMAP_Port);
            await imapClient.AuthenticateAsync(Login, Password);
            var folders = new List<IMailFolder>();
            folders = imapClient.GetFolders(imapClient.PersonalNamespaces[0]).ToList();

            await imapClient.DisconnectAsync(true);
            return folders;
        }
        public async Task<List<EmailListData>> GetMessagesAsync(SpecialFolder specialFolder)
        {
            await imapClient.ConnectAsync(IMAP_Host, IMAP_Port);
            await imapClient.AuthenticateAsync(Login, Password);
            var folder = imapClient.GetFolder(specialFolder);
            await folder.OpenAsync(FolderAccess.ReadWrite);
            List<EmailListData> emailListDatas = new List<EmailListData>();
            foreach (var email in folder)
                emailListDatas.Add(new EmailListData() { Id = email.MessageId, Subject = email.Subject });
            await imapClient.DisconnectAsync(true);
            return emailListDatas;
        }
        public async Task<MimeMessage?> GetMessageAsync(SpecialFolder specialFolder, string Id)
        {
            await imapClient.ConnectAsync(IMAP_Host, IMAP_Port);
            await imapClient.AuthenticateAsync(Login, Password);
            var folder = imapClient.GetFolder(specialFolder);
            await folder.OpenAsync(FolderAccess.ReadWrite);
            var message = folder.FirstOrDefault(m => m.MessageId == Id);
            await imapClient.DisconnectAsync(true);
            return message;
        }
        public async Task DeleteMessageAsync(SpecialFolder specialFolder, string Id)
        {
            await imapClient.ConnectAsync(IMAP_Host, IMAP_Port);
            await imapClient.AuthenticateAsync(Login, Password);
            var folder = imapClient.GetFolder(specialFolder);
            await folder.OpenAsync(FolderAccess.ReadWrite);
            UniqueId messageId = folder.Search(SearchQuery.HeaderContains("Message-ID", Id)).FirstOrDefault();
            if (messageId != null)
            {
                //folder.Open(FolderAccess.ReadWrite);
                //folder.AddFlags(messageId, MessageFlags.Deleted, true); // Встановіть прапорець Deleted, вказавши true для видалення
                //folder.Expunge(); // Видаліть видалені повідомлення
                folder.MoveTo(messageId, imapClient.GetFolder(SpecialFolder.Trash)); 
            }

            await imapClient.DisconnectAsync(true);
        }
        public async Task<IList<UniqueId>> GetAllInboxMessagesAsync()
        {
            await imapClient.ConnectAsync(IMAP_Host, IMAP_Port);
            await imapClient.AuthenticateAsync(Login, Password);
            await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite);
            IList<UniqueId> uids = imapClient.Inbox.Search(SearchQuery.All);
            await imapClient.DisconnectAsync(true);
            return uids;
        }
        public async Task<string> SendNewMailAsync(string subjectMail, string textBody, string htmlBody, string emailTo, int priority, List<string>? attachments = null, string? nameTo = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(NameFrom, Login));
            message.To.Add(new MailboxAddress(nameTo == null ? "" : nameTo, emailTo));
            message.Subject = subjectMail;
            message.Importance = (MessageImportance)priority;
            var bodyBuilder = new BodyBuilder();
            if (!string.IsNullOrEmpty(textBody))
                bodyBuilder.TextBody = textBody;
            if (!string.IsNullOrEmpty(htmlBody))
                bodyBuilder.HtmlBody = htmlBody;

            // Adding atachments
            if (attachments != null && 0 < attachments.Count)
                foreach (var attachment in attachments)
                    bodyBuilder.Attachments.Add(attachment); // Path to each attachment

            message.Body = bodyBuilder.ToMessageBody();

            await smtpClient.ConnectAsync(SMTP_Host, SMTP_Port);
            await smtpClient.AuthenticateAsync(Login, Password);
            string answer = await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
            return answer;
        }
    }
    public class EmailListData
    {
        public string Id { get; set; }
        public string Subject { get; set; }
    }
}
