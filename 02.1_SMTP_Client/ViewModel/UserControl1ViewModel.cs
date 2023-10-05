using _02._1_SMTP_Client.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _02._1_SMTP_Client.ViewModel
{
    public class UserControl1ViewModel : IPageViewModel
    {
        private ICommand? _goTo2;

        public event EventHandler<EventArgs<string>>? ViewChanged;
        public string PageId { get; set; }
        public string Title { get; set; }

        public UserControl1ViewModel(string pageIndex = "1")
        {
            PageId = pageIndex;
            Title = "View 1";
        }
        public ICommand GoTo2
        {
            get
            {
                return _goTo2 ??= new RelayCommand(x =>
                {
                    ViewChanged?.Raise(this, "2");


                });
            }
        }
    }
}
