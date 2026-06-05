using Leosac.WpfApp.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Leosac.WpfApp.Domain
{
    public abstract class StickyHeaderSupportBase : ObservableObject, IStickyHeaderSupport
    {
        public virtual bool SupportsStickyHeader => false;
        public virtual bool IsStickyHeaderVisible => false;
    }
}