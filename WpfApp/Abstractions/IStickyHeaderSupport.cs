namespace Leosac.WpfApp.Abstractions
{
    public interface IStickyHeaderSupport
    {
        bool SupportsStickyHeader { get; }
        bool IsStickyHeaderVisible { get; }
    }
}