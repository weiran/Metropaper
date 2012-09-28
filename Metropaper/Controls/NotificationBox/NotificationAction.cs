using System;
using System.Windows.Input;

namespace WeiranZhang.Metropaper.Controls
{
    public class NotificationAction
    {
        #region Properties

        public object Content { get; private set; }
        public object ContentTemplateKey { get; private set; }

        internal ICommand Command
        {
            get;
            private set;
        }

        #endregion

        #region NotificationCommand

        private class NotificationCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public NotificationCommand(Action execute, Func<bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            bool ICommand.CanExecute(object parameter)
            {
                return _canExecute();
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                add { }
                remove { }
            }

            void ICommand.Execute(object parameter)
            {
                _execute();

                NotificationTool.Close();
            }
        }

        #endregion

        #region Ctor

        public NotificationAction(object content, Action execute)
            : this(content, null, execute, () => true)
        {
        }

        public NotificationAction(object content, Action execute, Func<bool> canExecute)
            : this(content, null, execute, canExecute)
        {
        }

        public NotificationAction(object content, object contentTemplateKey, Action execute)
            : this(content, contentTemplateKey, execute, () => true)
        {
        }

        public NotificationAction(object content, object contentTemplateKey, Action execute, Func<bool> canExecute)
        {
            Content = content;
            ContentTemplateKey = contentTemplateKey;
            Command = new NotificationCommand(execute, canExecute);
        }

        #endregion
    }
}
