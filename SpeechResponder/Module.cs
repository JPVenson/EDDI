using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EddiSpeechResponder.Service;
using Utilities.Unity;

namespace EddiSpeechResponder
{
	public class SpeechResponderModule : UiModule
	{
		public override void Start()
		{
			IoC.Register<ScriptRecoveryService>();
			IoC.Register<SpeechResponder>();
		}

		public override IEnumerable<Uri> Resources()
		{
			yield return CreateAssemblyDictionary("/Resources/EditScriptResources.xaml");
			yield return CreateAssemblyDictionary("/Resources/MarkdownDisplayResources.xaml");
			yield return CreateAssemblyDictionary("/Resources/ScriptCompareResources.xaml");
		}
	}
}