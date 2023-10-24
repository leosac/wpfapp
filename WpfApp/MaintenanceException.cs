using System;

namespace Leosac.WpfApp
{
    public class MaintenanceException : Exception
    {
        public MaintenanceException() { }

        public MaintenanceException(string message) : base(message) { }

        public MaintenanceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
