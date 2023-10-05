using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace _03_HTTP_WebClient
{
    [AddINotifyPropertyChangedInterface]
    public class ImageInfo
    {
        public string Category { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string TargetDirectory { get; set; }
        public string FileName { get; set; }
        public string Destination { get; set; }
        public DateTime OperationDate { get; set; }
        public string OperationDateString => OperationDate.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
