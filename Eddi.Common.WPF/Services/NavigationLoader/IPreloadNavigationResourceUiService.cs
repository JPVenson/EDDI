using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using JPB.WPFToolsAwesome.MVVM.ViewModel;
using Utilities.Unity;

namespace Eddi.Common.WPF.Services.NavigationLoader
{
	public interface IPreloadNavigationResourceUiService
	{
		IDictionary<string, ResourceDictionary> DataTemplatesSource { get; }
	}

	public class PreloadNavigationResourceUiServiceService : IPreloadNavigationResourceUiService
	{
		public PreloadNavigationResourceUiServiceService()
		{
			DataTemplatesSource = new Dictionary<string, ResourceDictionary>();
		}
		
		public IDictionary<string, ResourceDictionary> DataTemplatesSource { get; set; }

		public void Load()
		{
			var uiResourceAssemblies = IoC.ResolveMany<IUiModule>()
				.SelectMany(f => f.Resources())
				.ToArray();
			var result = new ConcurrentDictionary<string, ResourceDictionary>();
			var index = 0;
			var vmActor = new ViewModelBase(Application.Current.Dispatcher);
			Parallel.ForEach(uiResourceAssemblies, (module) =>
			{
				try
				{
					var resourceDictionary = new ResourceDictionary();
					resourceDictionary.BeginInit();
					vmActor.ViewModelAction(() =>
					{
						resourceDictionary.Source = module;
					});
					resourceDictionary.EndInit();
					result.TryAdd(module.ToString(), resourceDictionary);
				}
				catch (Exception e)
				{
					throw;
				}
			});
			DataTemplatesSource = result;
		}
	}
}
