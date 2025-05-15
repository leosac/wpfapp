using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace Leosac.WpfApp.Domain
{
    /// <summary>
    /// From https://www.codeproject.com/Articles/25808/Aggregating-WPF-Commands-with-CommandGroup
    /// </summary>
    public class CommandGroup : FreezableCollection<CommandReference>, ICommand
    {
        public CommandGroup()
        {
            var me_colchange = this as INotifyCollectionChanged;
            me_colchange.CollectionChanged += OnCommandsCollectionChanged;
        }

        private IList<CommandReference> Commands
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// Returns the collection of child commands. They are executed
        /// in the order that they exist in this collection.
        /// </summary>

        void OnCommandsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // We have a new child command so our ability to execute may have changed.
            OnCanExecuteChanged();

            if (e.NewItems != null && 0 < e.NewItems.Count)
            {
                foreach (ICommand cmd in e.NewItems)
                {
                    cmd.CanExecuteChanged += OnChildCommandCanExecuteChanged;
                }
            }

            if (e.OldItems != null && 0 < e.OldItems.Count)
            {
                foreach (ICommand cmd in e.OldItems)
                {
                    cmd.CanExecuteChanged -= OnChildCommandCanExecuteChanged;
                }
            }
            OnCanExecuteChanged();
        }

        void OnChildCommandCanExecuteChanged(object? sender, EventArgs e)
        {
            // Bubble up the child commands CanExecuteChanged event so that
            // it will be observed by WPF.
            OnCanExecuteChanged();
        }

        public bool CanExecute(object? parameter)
        {
            foreach (CommandReference cmd in Commands)
            {
                if (!cmd.CanExecute(parameter))
                {
                    return false;
                }
            }

            return true;
        }

        public event EventHandler? CanExecuteChanged;
        protected virtual void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Execute(object? parameter)
        {
            foreach (ICommand cmd in Commands)
            {
                cmd.Execute(parameter);
            }
        }
    }
}
