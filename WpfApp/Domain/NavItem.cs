using CommunityToolkit.Mvvm.ComponentModel;
using Leosac.WpfApp.Abstractions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Leosac.WpfApp.Domain
{
    public class NavItem : ObservableObject
    {
        private readonly Type _contentType;
        private readonly object? _dataContext;

        private object? _content;
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto;
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement = ScrollBarVisibility.Auto;
        private Thickness _marginRequirement = new(16);

        public NavItem(string name, Type contentType, string icon) : this(name, contentType, icon, null) { }

        public NavItem(string name, Type contentType, string icon, object? dataContext)
        {
            Name = name;
            _contentType = contentType;
            Icon = icon;
            _dataContext = dataContext;
            _stickyHeaderSupport = dataContext as IStickyHeaderSupport;
        }

        public string Name { get; }

        public string Icon { get; }

        public object? Content => _content ??= CreateContent();

        public object? DataContext => _dataContext;

        private readonly IStickyHeaderSupport? _stickyHeaderSupport;
        public IStickyHeaderSupport? StickyHeaderSupport => _stickyHeaderSupport;

        public object? StickyHeader { get; set; }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => _horizontalScrollBarVisibilityRequirement;
            set => SetProperty(ref _horizontalScrollBarVisibilityRequirement, value);
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => _verticalScrollBarVisibilityRequirement;
            set => SetProperty(ref _verticalScrollBarVisibilityRequirement, value);
        }

        public Thickness MarginRequirement
        {
            get => _marginRequirement;
            set => SetProperty(ref _marginRequirement, value);
        }

        private object? CreateContent()
        {
            var content = Activator.CreateInstance(_contentType);
            if (_dataContext != null && content is FrameworkElement element)
            {
                element.DataContext = _dataContext;
            }

            return content;
        }
    }
}
