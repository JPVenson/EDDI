using Eddi.Common.WPF.Services.Dialog;
using Eddi.Common.WPF.Services.NavigationLoader;
using Utilities.Unity;

namespace Eddi.Common.WPF
{
	public class Module : IModule
	{
		public void Start()
		{
			IoC.RegisterInstance<IDialogService>(new DialogService());
			IoC.RegisterInstance<IPreloadNavigationResourceUiService>(new PreloadNavigationResourceUiServiceService());
		}
	}
}