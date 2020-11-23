using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JPB.WPFToolsAwesome.MVVM.ViewModel;

namespace Eddi.Common.WPF.Services.Dialog
{
	public class DialogService : IDialogService
	{
		public DialogService()
		{
			WpfActorHelper = new ThreadSaveObservableCollection<object>();
			_dialogs = new List<Window>();
		}

		public ThreadSaveViewModelActor WpfActorHelper { get; private set; }

		private IList<Window> _dialogs;

		public TDialog ShowDialog<TDialog>(TDialog dialogViewModel) where TDialog : IDialogViewModel
		{
			return Show(dialogViewModel, dialogViewModel.IsModalDialog);
		}

		public TDialog Show<TDialog>(TDialog dialogViewModel) where TDialog : IDialogViewModel
		{
			return Show(dialogViewModel, false);
		}

		public TDialog Show<TDialog>(TDialog dialogViewModel, bool modal) where TDialog : IDialogViewModel
		{
			WpfActorHelper.ViewModelAction(() =>
			{
				var title = dialogViewModel.Title;

				var currentDialog = new Window();
				if (title != null)
				{
					currentDialog.Title = title.ToString();
				}

				currentDialog.WindowStyle = dialogViewModel.ExternalyClosed ? WindowStyle.None : WindowStyle.ToolWindow;

				if (Application.Current.MainWindow != null && Application.Current.MainWindow != currentDialog)
				{
					currentDialog.Owner = Application.Current.MainWindow;
				}

				currentDialog.InputBindings.Add(new KeyBinding(dialogViewModel.CloseDialogCommand, Key.Escape,
					ModifierKeys.None));

				foreach (var dialogCommand in dialogViewModel.Commands.Where(e => e.KeyBinding.HasValue))
				{
					currentDialog.InputBindings.Add(new KeyBinding(dialogCommand, dialogCommand.KeyBinding.Value,
						dialogCommand.ModifierKeys));
				}

				currentDialog.SizeToContent = SizeToContent.WidthAndHeight;
				currentDialog.Width = 500;
				currentDialog.Height = 600;
				currentDialog.ContentTemplate = Application.Current
					.FindResource("DialogDefaultDataTemplate") as DataTemplate;
				currentDialog.Content = dialogViewModel;
				currentDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
				dialogViewModel.OnDisplay();
				_dialogs.Add(currentDialog);
				if (modal)
				{
					currentDialog.ShowDialog();
				}
				else
				{
					currentDialog.Show();
				}
			});

			return dialogViewModel;
		}

		public void CloseDialog(bool? result, IDialogViewModel dialogViewModel = null)
		{
			WpfActorHelper.BeginViewModelAction(() =>
			{
				if (_dialogs.Count != 0)
				{
					var currentDialog = dialogViewModel != null ? (_dialogs.First(e => e.Content == dialogViewModel)) : (_dialogs.Last());
					if (currentDialog != null)
					{
						if (dialogViewModel.IsModalDialog)
						{
							currentDialog.DialogResult = result;
						}

						(currentDialog.Content as IDialogViewModel).OnClosed();
						currentDialog.Close();
					}

					currentDialog = null;
				}
			});
		}
	}
}