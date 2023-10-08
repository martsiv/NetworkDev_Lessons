using _04._1_IMAP_Client.Help;
using _04._1_IMAP_Client.View;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MailServiceNamespace;
using MailKit;
using Org.BouncyCastle.Crypto;

namespace _04._1_IMAP_Client.ViewModel
{
    [AddINotifyPropertyChangedInterface]

    public class ViewModelMainWindow
    {
        private ObservableCollection<MailServiceSMTP_IMAP> mailServices;
        public IEnumerable<MailServiceSMTP_IMAP> MailServices => mailServices;
        public MailServiceSMTP_IMAP? currentService { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }

        private WindowWork windowWork = null;

        private readonly RelayCommand openWindowWorkCmd;
        public ICommand OpenWindowWorkCmd => openWindowWorkCmd;
        public async void OpenAdminWindow()
        {
            if (currentService == null)
                return;
            currentService.Login = Login;
            currentService.Password = Password;
            if (await currentService.IsAuthentificateSuccessAsync())
            {
                windowWork = new WindowWork();
                windowWork.Closed += WindowWork_Closed;
                windowWork.DataContext = new ViewModelWork(currentService);
                windowWork.Show();
            }
            else
            {
                currentService.Login = string.Empty;
                currentService.Password = string.Empty;
            }
        }
        private void WindowWork_Closed(object sender, EventArgs e)
        {
            windowWork = null;
        }
        private bool CanOpenWindowWork(object obj)
        {
            // Check if window is openes. If yes, this will returned false.
            return windowWork == null || !windowWork.IsVisible;
        }
        
        public ViewModelMainWindow()
        {
            openWindowWorkCmd = new((o) => OpenAdminWindow(), CanOpenWindowWork);
            mailServices = new ObservableCollection<MailServiceSMTP_IMAP>();
            mailServices.Add(new MailServiceSMTP_IMAP(NameOfService: "GMAIL", SMTP_Host: "smtp.gmail.com", SMTP_Port: 587, IMAP_Host: "imap.gmail.com", IMAP_Port: 993));
            mailServices.Add(new MailServiceSMTP_IMAP(NameOfService: "Ukr.net", SMTP_Host: "smtp.ukr.net", SMTP_Port: 465, IMAP_Host: "imap.ukr.net", IMAP_Port: 993));
        }
    }
}
