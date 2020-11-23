using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Eddi.Common.WPF.Services.NavigationLoader;
using Utilities.Unity;

namespace Eddi.Common.WPF.DataTemplateSelectors
{
	public class ModuleResourcesDataTemplateSelector : DataTemplateSelector
	{
		public ModuleResourcesDataTemplateSelector()
		{
			PreloadNavigationResourceUiService = IoC.Resolve<IPreloadNavigationResourceUiService>();
		}

		public IPreloadNavigationResourceUiService PreloadNavigationResourceUiService { get; private set; }
		
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var assembly = item.GetType().Assembly;
			var cacheKey = $"pack://application:,,,/{assembly.GetName().Name};component/Resources/ModuleResources.xaml";

			var dataTemplateKey = new DataTemplateKey(item.GetType());
			if (PreloadNavigationResourceUiService.DataTemplatesSource.TryGetValue(cacheKey, out var cachedRes))
			{
				if (container is FrameworkElement fe)
				{
					fe.Resources.MergedDictionaries.Add(cachedRes);
				}

				var dataTemplate = cachedRes[dataTemplateKey] as DataTemplate;
				return dataTemplate;
			}
			var tryFindResource = Application.Current.MainWindow.TryFindResource(dataTemplateKey) as DataTemplate;

			if (tryFindResource == null)
			{
				var resourceDictionary = new ResourceDictionary();
				resourceDictionary.Source = new Uri(cacheKey, UriKind.RelativeOrAbsolute);
				var dataTemplate = resourceDictionary[dataTemplateKey] as DataTemplate;
				PreloadNavigationResourceUiService.DataTemplatesSource.Add(cacheKey, resourceDictionary);
				return dataTemplate;
			}

			return tryFindResource;
		}
	}
}
