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
                await imapClient.ConnectAsync (IMAP_Host, IMAP_Port);
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
            //var personal = imapClient.GetFolderAsync(imapClient.PersonalNamespaces[0]);

            //await personal.OpenAsync(FolderAccess.ReadOnly);
            //var subfolders = personal.GetSubfolders();

            //foreach (var folder in subfolders)
            //{
            //    folders.Add(folder);
            //}
            folders = imapClient.GetFolders(imapClient.PersonalNamespaces[0]).ToList();
            await imapClient.DisconnectAsync(true);
            return folders;
        }
        public async Task<IList<UniqueId>> GetAllSendMessagesAsync()
        {
            await imapClient.ConnectAsync(IMAP_Host, IMAP_Port);
            await imapClient.AuthenticateAsync(Login, Password);
            var folder = imapClient.GetFolder(SpecialFolder.Sent);
            folder.Open(FolderAccess.ReadWrite);
            IList<UniqueId> uids = folder.Search(SearchQuery.All);
            await imapClient.DisconnectAsync(true);
            return uids;
        }
        public async Task<IList<UniqueId>> GetAllInbaxMessagesAsync()
        {
            await imapClient.ConnectAsync(IMAP_Host, IMAP_Port);
            await imapClient.AuthenticateAsync(Login, Password);
            await imapClient.Inbox.OpenAsync(FolderAccess.ReadWrite);
            IList<UniqueId> uids = imapClient.Inbox.Search(SearchQuery.All);
            await imapClient.DisconnectAsync(true);
            return uids;
        }
    }
}
