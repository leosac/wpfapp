﻿using Leosac.WpfApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leosac.WpfApp
{
    public class UpdateVersion : ViewModelBase
    {
        public UpdateVersion()
        {
            _versionString = "0.0.0";
            _uri = LeosacAppInfo.Instance?.ApplicationUrl;
        }

        private string _versionString;
        private string _uri;

        public string VersionString
        {
            get => _versionString;
            set => SetProperty(ref _versionString, value);
        }

        public string Uri
        {
            get => _uri;
            set => SetProperty(ref _uri, value);
        }
    }
}
