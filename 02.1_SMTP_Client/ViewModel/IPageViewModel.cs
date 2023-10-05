using _02._1_SMTP_Client.Help;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02._1_SMTP_Client.ViewModel
{
    public interface IPageViewModel
    {
        event EventHandler<EventArgs<string>>? ViewChanged;
        string PageId { get; set; }
        string Title { get; set; }
    }
}
