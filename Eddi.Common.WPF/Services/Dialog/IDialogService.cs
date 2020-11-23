using System;
using System.Text;
using System.Threading.Tasks;
using JPB.WPFToolsAwesome.MVVM.DelegateCommand;

namespace Eddi.Common.WPF.Services.Dialog
{
	public interface IDialogService
	{
		TDialog ShowDialog<TDialog>(TDialog dialogViewModel) where TDialog : IDialogViewModel;
		void CloseDialog(bool? result, IDialogViewModel dialogViewModel);
	}
}
