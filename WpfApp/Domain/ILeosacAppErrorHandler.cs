using System;

namespace Leosac.WpfApp.Domain
{
    public interface ILeosacAppErrorHandler
    {
        void HandleError(Exception ex);
    }
}
