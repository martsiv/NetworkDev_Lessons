using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02._1_SMTP_Client.ViewModel
{
    //[AddINotifyPropertyChangedInterface]

    public class MainWindowViewModel : BaseViewModel
    {
        private IPageViewModel? _pageViewModel;
        private readonly Dictionary<string, IPageViewModel>? _pageViewModels = new();

        public IPageViewModel? CurrentPageViewModel
        {
            get
            {
                return _pageViewModel;
            }
            set
            {
                _pageViewModel = value;
                OnPropertyChanged(nameof(CurrentPageViewModel));
            }
        }


        public MainWindowViewModel(IDataModel pageViews)
        {
            _pageViewModels["1"] = new UserControl1ViewModel("1");
            _pageViewModels["1"].ViewChanged += (o, s) =>
            {
                CurrentPageViewModel = _pageViewModels[s.Value];
                pageViews.Data = "Data: " + s.Value.ToString();
            };

            _pageViewModels["2"] = new UserControl2ViewModel("2");
            _pageViewModels["2"].ViewChanged += (o, s) =>
            {
                CurrentPageViewModel = _pageViewModels[s.Value];
                pageViews.Data = "Data: " + s.Value.ToString();
            };


            CurrentPageViewModel = _pageViewModels["1"];
        }
    }
}
