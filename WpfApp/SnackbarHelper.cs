using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System;

namespace Leosac.WpfApp
{
    public static class SnackbarHelper
    {
        public static void EnqueueError(ISnackbarMessageQueue? queue, string message)
        {
            EnqueueError(queue, null, message);
        }

        public static void EnqueueError(ISnackbarMessageQueue? queue, Exception? ex)
        {
            EnqueueError(queue, ex, null);
        }

        public static void EnqueueError(ISnackbarMessageQueue? queue, Exception? ex, string? message)
        {
            if (ex != null)
            {
                if (message == null)
                {
                    message = ex.Message;
                }
                else
                {
                    message += ": " + ex.Message;
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                message = "An error occured.";
            }

            var panel = new DockPanel();
            var errorIcon = new PackIcon() { Kind = PackIconKind.ExclamationThick };
            DockPanel.SetDock(errorIcon, Dock.Left);
            panel.Children.Add(errorIcon);
            panel.Children.Add(new TextBlock() { Text = message, Margin = new Thickness(5, 0, 0 ,0), TextWrapping = TextWrapping.Wrap });

            EnqueueMessage(queue, panel);
        }

        public static void EnqueueMessage(ISnackbarMessageQueue? queue, PackIconKind icon, object message)
        {
            queue?.Enqueue(message, new PackIcon { Kind = icon }, (object? _) => { }, null, false, true, TimeSpan.FromSeconds(5));
        }

        public static void EnqueueMessage(ISnackbarMessageQueue? queue, object message)
        {
            EnqueueMessage(queue, PackIconKind.CloseBold, message);
        }

        public static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };
                var parent = ((Control)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
        }
    }
}
