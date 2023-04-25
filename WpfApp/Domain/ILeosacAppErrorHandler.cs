using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leosac.WpfApp.Domain
{
    public interface ILeosacAppErrorHandler
    {
        void HandleError(Exception ex);
    }
}
