/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.Windows.Input;

namespace WpfServices
{
    //Code taken from https://stackoverflow.com/questions/12422945/how-to-bind-wpf-button-to-a-command-in-viewmodelbase and
    //https://stackoverflow.com/questions/32064308/pass-command-parameter-to-method-in-viewmodel-in-wpf
    //Modified so CanExecute guards Execute, because that just makes more sense.
    public class CommandHandler : ICommand
    {
        public CommandHandler(Action<object> action, Func<object, bool> canExecute)
        {
            m_action = action ?? throw new ArgumentNullException(nameof(action));
            m_canExecute = canExecute ?? (x => true);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            bool canExecute = m_canExecute.Invoke(parameter);
            return canExecute;
        }

        public void Execute(object parameter)
        {
            bool canExecute = CanExecute(parameter);
            if(canExecute)
            {
                m_action(parameter);
            }
        }

        private readonly Action<object> m_action;
        private readonly Func<object,bool> m_canExecute;
    }
}
