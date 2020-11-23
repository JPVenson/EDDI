using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using JPB.WPFToolsAwesome.MVVM.DelegateCommand;

namespace Eddi.Common.WPF.Services.Dialog
{
	public class DialogCommand : DelegateCommand
	{
		public object Content { get; set; }
		public Dock Position { get; set; } = Dock.Left;

		public Key? KeyBinding { get; set; }
		public ModifierKeys ModifierKeys { get; set; } = ModifierKeys.None;

		public DialogCommand(ICommand command) : base(command.Execute, command.CanExecute)
		{
		}

		public DialogCommand(Action execute) : base(execute)
		{
		}

		public DialogCommand(Action<object> execute) : base(execute)
		{
		}

		public DialogCommand(Action<object> execute, Func<object, bool> canExecute) : base(execute, canExecute)
		{
		}

		public DialogCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
		{
		}
	}
}
