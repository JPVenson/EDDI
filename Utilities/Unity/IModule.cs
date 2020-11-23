using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Utilities.Unity
{
	public interface IModule
	{
		void Start();
	}

	public interface IUiModule : IModule
	{
		IEnumerable<Uri> Resources();
	}

	public abstract class UiModule : IUiModule
	{
		//protected virtual ResourceDictionary CreateDictionary(Uri source)
		//{
		//	var resourceDictionary = new ResourceDictionary();
		//	resourceDictionary.BeginInit();
		//	resourceDictionary.Source = source;
		//	resourceDictionary.EndInit();
		//	return resourceDictionary;
		//}

		protected virtual Uri CreateAssemblyDictionary(string path)
		{
			return new Uri($"pack://application:,,,/{GetType().Assembly.GetName().Name};component/{path}");
		}

		protected virtual Uri CreateModuleResourcesDictionary()
		{
			return new Uri($"pack://application:,,,/{GetType().Assembly.GetName().Name};component/Resources/ModuleResources.xaml");
		}

		public virtual IEnumerable<Uri> Resources()
		{
			yield break;
		}

		public abstract void Start();
	}
}
