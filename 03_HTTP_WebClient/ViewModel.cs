using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using PropertyChanged;
using System.Windows.Input;
using System.IO;
using System.Net.Http;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace _03_HTTP_WebClient
{
    [AddINotifyPropertyChangedInterface]
    public class ViewModel
    {
        private WebClient client;
        ObservableCollection<ImageInfo> operations = new ObservableCollection<ImageInfo>();
        public IEnumerable<ImageInfo> Operations => operations;
        public int Width { get; set; }
        public int Height { get; set; }
        public string Category { get; set; }
        public string TargetDirectory { get; set; }
        public int Progress { get; set; }
        string urlPhotos = "https://source.unsplash.com/random/";
        public ViewModel()
        {
            chooseFolderCmd = new((o) => OpenFolder(), (o) => client.IsBusy == false);
            downloadImageCmd = new((o) => DownloadFileAsync(), IsCorrectFormsToStartDownload);
            stopCmd = new((o) => StopOperation());
            client = new WebClient();

            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
        }

        #region Command Download image
        private readonly RelayCommand downloadImageCmd;
        public ICommand DownloadImageCmd => downloadImageCmd;
        private async void DownloadFileAsync()
        {
            if (client.IsBusy)
            {
                System.Windows.MessageBox.Show("Web Client is busy! Try agin later...");
                return;
            }
            Uri uriRequest = new Uri($"{urlPhotos}{Width}x{Height}/"
                + (string.IsNullOrEmpty(Category) ? "" : $"?{Category}"));

            //Виключно для того, щоб дістати назву і формат картинки перед завантаженням
            Uri uriResponse;
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(uriRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        // Отримати URI з відповіді сервера
                        uriResponse = new Uri(response.RequestMessage.RequestUri.AbsoluteUri);
                        //Витягуємо назву файлу з відповіді
                        string fileName = ExtractFileName(uriResponse);
                        string destination = Path.Combine(TargetDirectory, fileName);
                        // Безпосередньо саме завантаження
                        await client.DownloadFileTaskAsync(uriResponse, destination);
                        //Зберігаємо до історії
                        ImageInfo imageInfo = new ImageInfo()
                        {
                            Category = Category,
                            Width = Width,
                            Height = Height,
                            Destination = destination,
                            FileName = fileName,
                            TargetDirectory = TargetDirectory,
                            OperationDate = DateTime.Now
                        };
                        operations.Add(imageInfo);
                        //Автоматичне відкриття картинки стандартним переглядачем
                        OpenImageInDefaultViewer(destination);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show($"Помилка: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    System.Windows.MessageBox.Show($"Помилка запиту: {ex.Message}");
                }
                catch (ArgumentNullException agrEx)
                {
                    System.Windows.MessageBox.Show(agrEx.Message);
                }
                catch (WebException webEx)
                {
                    System.Windows.MessageBox.Show(webEx.Message);
                }
                catch (InvalidOperationException invalidEx)
                {
                    System.Windows.MessageBox.Show(invalidEx.Message);
                }
            }
        }
        #endregion

        #region Choose folder command
        private readonly RelayCommand chooseFolderCmd;
        public ICommand ChooseFolderCmd => chooseFolderCmd;
        private void OpenFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    TargetDirectory = dialog.SelectedPath;
            }
        }
        #endregion

        #region Stop Command
        private readonly RelayCommand stopCmd;
        public ICommand StopCmd => stopCmd;
        private void StopOperation() => client.CancelAsync();
        #endregion

        #region Download events
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                System.Windows.MessageBox.Show("Operation was canceled!");
            }
            else if (e.Error != null)
            {
                System.Windows.MessageBox.Show("Downloading exeption!");
            }
            else
            {
                System.Windows.MessageBox.Show("Download complete!");
            }
        }
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }
        #endregion

        #region Other methods
        private string ExtractFileName(Uri myUri)
        {
            string path = myUri.LocalPath.TrimStart('/');
            string extension = myUri.Query.Split('&', StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(p => p.StartsWith("fm=",
                                         StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(extension))
            {
                extension = extension.Substring(3); // Відкидаємо "fm="
            }
            else
            {
                extension = "jpg"; // За замовчуванням, якщо параметр "fm" відсутній
            }

            return $"{path}.{extension}";
        }
        static void OpenImageInDefaultViewer(string imagePath)
        {
            try
            {
                // Створити новий процес для запуску стандартного переглядача картинок
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = imagePath;
                    process.StartInfo.UseShellExecute = true;
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
        public bool IsCorrectFormsToStartDownload(object obj)
        {
            if (Width != null && Width <= 0)
                return false;
            if (Height != null && Height <= 0)
                return false;
            if ((Width != null && Height == null) || (Width == null && Height != null))
                return false;
            if (string.IsNullOrEmpty(TargetDirectory))
                return false;
            return true;
        }
        #endregion
    }
}
