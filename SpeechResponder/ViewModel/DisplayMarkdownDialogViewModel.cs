using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eddi.Common.WPF.Services.Dialog;
using JPB.WPFToolsAwesome.Error.ValidationRules;
using Utilities;

namespace EddiSpeechResponder.ViewModel
{
	public class DisplayMarkdownDialogViewModel : DialogViewModelBase<NoErrors>
	{
		private readonly string _markdownFile;
		private readonly Func<string, string> _transformation;

		public DisplayMarkdownDialogViewModel(string markdownFile, Func<string, string> transformation = null)
		{
			_markdownFile = markdownFile;
			_transformation = transformation;
		}

		public override void OnDisplay()
		{
			SimpleWork(() =>
			{
				// Read Markdown and convert it to HTML
				string markdown;
				try
				{
					DirectoryInfo dir = new DirectoryInfo(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
					markdown = Files.Read(dir.FullName + @"\" + _markdownFile);
				}
				catch (Exception ex)
				{
					Logging.Error("Failed to find " + _markdownFile, ex);
					markdown = "";
				}

				if (_transformation != null)
				{
					markdown = _transformation(markdown);
				}

				string html = CommonMark.CommonMarkConverter.Convert(markdown);
				html = "<head>  <meta charset=\"UTF-8\"> </head> " + html;

				// Insert the HTML
				HtmlCode = html;
			});
		}

		private string _htmlCode;

		public string HtmlCode
		{
			get { return _htmlCode; }
			set
			{
				SendPropertyChanging(() => HtmlCode);
				_htmlCode = value;
				SendPropertyChanged(() => HtmlCode);
			}
		}
	}
}
