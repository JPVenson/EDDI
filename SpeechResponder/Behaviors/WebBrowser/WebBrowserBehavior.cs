using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EddiSpeechResponder.Behaviors.WebBrowser
{
	public class BrowserBehavior
	{
		public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
			"Html",
			typeof(string),
			typeof(BrowserBehavior),
			new FrameworkPropertyMetadata(OnHtmlChanged));

		[AttachedPropertyBrowsableForType(typeof(System.Windows.Controls.WebBrowser))]
		public static string GetHtml(System.Windows.Controls.WebBrowser d)
		{
			return (string)d.GetValue(HtmlProperty);
		}

		public static void SetHtml(System.Windows.Controls.WebBrowser d, string value)
		{
			d.SetValue(HtmlProperty, value);
		}

		static void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			System.Windows.Controls.WebBrowser webBrowser = dependencyObject as System.Windows.Controls.WebBrowser;
			if (webBrowser != null)
			{
				webBrowser.NavigateToString(e.NewValue as string ?? "&nbsp;");
			}
		}
	}
}
