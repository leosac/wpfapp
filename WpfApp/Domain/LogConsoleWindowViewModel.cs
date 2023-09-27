using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace Leosac.WpfApp.Domain
{
    public class LogConsoleWindowViewModel : ObservableObject
    {
        public LogConsoleWindowViewModel()
        {
            _log = NotifyAppender.Appender;
            FontSizes = new ObservableCollection<int>(new[]
            {
                8, 9, 10, 12, 14, 16
            });
            _selectedFontSize = 10;
            MaxLines = new ObservableCollection<int>(new[]
            {
                10, 50, 100, 250, 500, 1000
            });
            ClearCommand = new RelayCommand(
                () =>
                {
                    if (Log != null)
                    {
                        Log.ClearNotifications();
                    }
                });
            SaveCommand = new RelayCommand(
                () =>
                {
                    if (Log != null)
                    {
                        var sfd = new SaveFileDialog();
                        sfd.AddExtension = true;
                        sfd.Filter = "Text Files|*.txt";
                        if (sfd.ShowDialog() == true)
                        {
                            System.IO.File.WriteAllText(sfd.FileName, Log.Notification);
                        }
                    }
                });
            CutCommand = new RelayCommand(
                () =>
                {
                    if (Log != null)
                    {
                        Clipboard.SetText(Log.Notification);
                        Log.ClearNotifications();
                    }
                });
            CopyCommand = new RelayCommand(
                () =>
                {
                    if (Log != null)
                    {
                        Clipboard.SetText(Log.Notification);
                    }
                });
        }

        private NotifyAppender? _log;
        
        public NotifyAppender? Log
        {
            get => _log;
            set => SetProperty(ref _log, value);
        }

        private int _selectedFontSize;

        public int SelectedFontSize
        {
            get => _selectedFontSize;
            set => SetProperty(ref _selectedFontSize, value);
        }

        public ObservableCollection<int> FontSizes { get; set; }

        public ObservableCollection<int> MaxLines { get; set; }

        public RelayCommand ClearCommand { get; }

        public RelayCommand SaveCommand { get; }

        public RelayCommand CutCommand { get; }

        public RelayCommand CopyCommand { get; }
    }
}
