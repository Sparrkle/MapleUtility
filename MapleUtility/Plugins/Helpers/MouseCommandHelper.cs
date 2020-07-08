using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MapleUtility.Plugins.Helpers
{
    public static class MouseCommandHelper
    {
        private static void LeftClickChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = (Control)sender;

            if (args.NewValue != null && args.NewValue is ICommand)
            {
                var newBinding = new MouseBinding(args.NewValue as ICommand, new MouseGesture(MouseAction.LeftClick));
                control.InputBindings.Add(newBinding);
            }
            else
            {
                var oldBinding = control.InputBindings.Cast<InputBinding>().First(b => b.Command.Equals(args.OldValue));
                control.InputBindings.Remove(oldBinding);
            }
        }

        public static readonly DependencyProperty LeftClickProperty =
            DependencyProperty.RegisterAttached("LeftClick",
                typeof(ICommand),
                typeof(MouseCommandHelper),
                new UIPropertyMetadata(LeftClickChanged));

        public static void SetLeftClick(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LeftClickProperty, value);
        }

        public static ICommand GetLeftClick(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LeftClickProperty);
        }
    }
}
