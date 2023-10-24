using log4net.Appender;
using log4net.Core;
using log4net;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Collections.ObjectModel;

namespace Leosac.WpfApp
{
    public class NotifyAppender : AppenderSkeleton, INotifyPropertyChanged
    {
        #region Members and events
        private static readonly object objlock = new();
        private static int _maxlines = 100;
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        /// <summary>
        /// Get the joined notification message.
        /// </summary>
        public string Notification
        {
            get => string.Join(Environment.NewLine, NotificationLines);
        }

        /// <summary>
        /// Get or set the notification message lines.
        /// </summary>
        public ObservableCollection<string> NotificationLines = new();

        /// <summary>
        /// Get or set the maximum number of lines.
        /// </summary>
        public int MaxLines
        {
            get => _maxlines;
            set
            {
                if (_maxlines != value)
                {
                    _maxlines = value;
                    OnChange();
                }
            }
        }

        /// <summary>
        /// Raise the change notification.
        /// </summary>
        private void OnChange(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Get a reference to the log instance.
        /// </summary>
        public static NotifyAppender? Appender
        {
            get
            {
                foreach (ILog log in LogManager.GetCurrentLoggers())
                {
                    foreach (IAppender appender in log.Logger.Repository.GetAppenders())
                    {
                        if (appender is NotifyAppender notifyAppender)
                        {
                            return notifyAppender;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Append the log information to the notification.
        /// </summary>
        /// <param name="loggingEvent">The log event.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            var writer = new StringWriter(CultureInfo.InvariantCulture);
            Layout.Format(writer, loggingEvent);
            lock (objlock)
            {
                NotificationLines.Add(writer.ToString());
                var count = NotificationLines.Count - MaxLines;
                if (loggingEvent.ExceptionObject != null)
                {
                    NotificationLines.Add(loggingEvent.ExceptionObject.ToString());
                }
                if (count > 0)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        NotificationLines.RemoveAt(0);
                    }
                }
                OnChange(nameof(Notification));
            }
        }

        /// <summary>
        /// Clear all the notifications
        /// </summary>
        public void ClearNotifications()
        {
            lock (objlock)
            {
                NotificationLines.Clear();
                OnChange(nameof(Notification));
            }
        }
    }
}
