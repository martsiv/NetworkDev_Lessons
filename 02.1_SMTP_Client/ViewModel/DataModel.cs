﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02._1_SMTP_Client.ViewModel
{
    public class DataModel : IDataModel
    {
        public string Data { get; set; }
        public string? Reverse()
        {
            char[] charArray = Data.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public DataModel()
        {
            Data = "";
        }

        public DataModel(string data)
        {
            Data = data;
        }
    }
}
