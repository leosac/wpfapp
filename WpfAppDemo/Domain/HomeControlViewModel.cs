using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Leosac.WpfApp;
using MaterialDesignThemes.Wpf;

namespace WpfAppDemo.Domain
{
    public class HomeControlViewModel : ObservableValidator
    {
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;

        public HomeControlViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackbarMessageQueue = snackbarMessageQueue;

            SnackbarInfoCommand = new RelayCommand(() =>
            {
                SnackbarHelper.EnqueueMessage(_snackbarMessageQueue, "Simple information message.");
            });

            SnackbarErrorCommand = new RelayCommand(() =>
            {
                SnackbarHelper.EnqueueError(_snackbarMessageQueue, "An error message. This message is usually longer than other messages as some details may be included.");
            });
        }

        public RelayCommand SnackbarInfoCommand { get; }

        public RelayCommand SnackbarErrorCommand { get; }
    }
}
