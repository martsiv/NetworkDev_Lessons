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

namespace _04._1_IMAP_Client.ViewModel
{
    [AddINotifyPropertyChangedInterface]

    public class ViewModelWork
    {
        private MailServiceSMTP_IMAP mailService;
        private ObservableCollection<IMailFolder> mailFolders = new ObservableCollection<IMailFolder>();
        public IEnumerable<IMailFolder> MailFolders => mailFolders;
        public ViewModelWork(MailServiceSMTP_IMAP mailService)
        {
            refreshCmd = new((o) => RefreshMail());

            this.mailService = mailService;
        }
        private readonly RelayCommand refreshCmd;
        public ICommand RefreshCmd => refreshCmd;
        private async void RefreshMail()
        {
            mailFolders.Clear();
            var folders = await mailService.GetFoldersAsync();
            foreach (var folder in folders)
                mailFolders.Add(folder);
        }
    }
}
