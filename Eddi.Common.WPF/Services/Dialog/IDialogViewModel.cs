using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using JPB.WPFToolsAwesome.Error;
using JPB.WPFToolsAwesome.Error.ValidationRules;
using JPB.WPFToolsAwesome.Error.ViewModelProvider.Base;
using JPB.WPFToolsAwesome.MVVM.DelegateCommand;
using Utilities.Unity;

namespace Eddi.Common.WPF.Services.Dialog
{
	public interface IDialogViewModel
	{
		object Title { get; set; }
		List<DialogCommand> Commands { get; set; }
		object Result { get; }
		bool ExternalyClosed { get; }
		DelegateCommand CloseDialogCommand { get; }

		void OnDisplay();
	}

	
	public class DialogViewModelBase : DialogViewModelBase<NoErrors>
	{
		
	}

	public class DialogViewModelBase<TErrors> : AsyncErrorProviderBase<TErrors>, 
		IDialogViewModel 
		where TErrors : IErrorCollectionBase, new()
	{
		public DialogViewModelBase()
		{
			Commands = new List<DialogCommand>();
			CloseDialogCommand = new DelegateCommand(CloseDialogExecute, CanCloseDialogExecute);

			var abortCommand = new DelegateCommand(AbortExecute, CanAbortExecute);
			Commands.Add(DefaultAbortCommand = new DialogCommand(abortCommand)
			{
				Content = "Abort",//TODO Needs Localization
				KeyBinding = Key.Escape,    
				Position = Dock.Right
			});
		}

		public DialogCommand DefaultAbortCommand { get; set; }

		public object Title { get; set; }
		public List<DialogCommand> Commands { get; set; }
		public object Result { get; set; }
		public bool ExternalyClosed { get; set; }

		public DelegateCommand CloseDialogCommand { get; private set; }
		public virtual void OnDisplay()
		{
			
		}

		protected virtual void AbortExecute()
		{
			Result = null;
			IoC.Resolve<IDialogService>().CloseDialog(false, this);
		}

		private bool CanAbortExecute()
		{
			return true;
		}

		protected virtual void CloseDialogExecute()
		{
			IoC.Resolve<IDialogService>().CloseDialog(null, this);
		}

		protected virtual bool CanCloseDialogExecute()
		{
			return true;
		}
	}
}