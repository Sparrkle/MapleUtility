using System;
using System.Windows;
using System.Windows.Input;

namespace MapleUtility.Plugin.Lib
{
    class RelayCommand : ICommand
    {
        private Action<object> _action;

        public RelayCommand(Action<object> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    _action(parameter);
                }
                else
                {
                    _action("Hello world");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("오류가 발생했습니다. : " + e.Message);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
