using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Unity;

namespace EddiSpeechService
{
	public class SpeechModule : IModule
	{
		public void Start()
		{
			IoC.RegisterInstance(SpeechService.Instance);
		}
	}
}
