﻿using Leosac.WpfApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Leosac.WpfApp
{
    /// <summary>
    /// Interaction logic for LogConsoleWindow.xaml
    /// </summary>
    public partial class LogConsoleWindow : Window
    {
        public LogConsoleWindow()
        {
            InitializeComponent();

            DataContext = new LogConsoleWindowViewModel();
        }

        private void tbxLogOutput_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbxLogOutput.ScrollToEnd();
        }
    }
}
