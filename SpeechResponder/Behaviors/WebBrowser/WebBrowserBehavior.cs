using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace EddiSpeechResponder.Behaviors.WebBrowser
{
	public class BrowserBehavior : Behavior<System.Windows.Controls.WebBrowser>
	{
		public static readonly DependencyProperty HtmlCodeProperty = DependencyProperty.Register(
			nameof(HtmlCode), typeof(string), typeof(BrowserBehavior), new FrameworkPropertyMetadata(default(string), PropertyChangedCallback) { BindsTwoWayByDefault = true });

		private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as BrowserBehavior).SetCode(e.NewValue as string);
		}

		private void SetCode(string eNewValue)
		{
			if (AssociatedObject != null)
			{
				AssociatedObject.NavigateToString(eNewValue as string ?? "&nbsp;");
			}
		}

		public string HtmlCode
		{
			get { return (string)GetValue(HtmlCodeProperty); }
			set { SetValue(HtmlCodeProperty, value); }
		}

		protected override void OnAttached()
		{
			SetCode(HtmlCode);
		}
	}
}
