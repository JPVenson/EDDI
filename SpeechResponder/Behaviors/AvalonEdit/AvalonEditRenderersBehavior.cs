using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Xaml.Behaviors;

namespace EddiSpeechResponder.Behaviors.AvalonEdit
{
	public class AvalonEditRenderersBehavior : Behavior<TextEditor>
	{
		public AvalonEditRenderersBehavior()
		{
			BackgroundRenderes = new ObservableCollection<IBackgroundRenderer>();
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			if (BackgroundRenderes != null)
			{
				foreach (var backgroundRenderer in BackgroundRenderes)
				{
					AssociatedObject.TextArea.TextView.BackgroundRenderers.Add(backgroundRenderer);
				}
			}
		}

		private void BackgroundRenderes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (var item in e.NewItems)
				{
					AssociatedObject.TextArea.TextView.BackgroundRenderers.Add(item as IBackgroundRenderer);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (var item in e.OldItems)
				{
					AssociatedObject.TextArea.TextView.BackgroundRenderers.Remove(item as IBackgroundRenderer);
				}
			}
		}

		public static readonly DependencyProperty BackgroundRenderesProperty = DependencyProperty.Register(
			nameof(BackgroundRenderes), typeof(ObservableCollection<IBackgroundRenderer>), typeof(AvalonEditRenderersBehavior),
			new PropertyMetadata(default(ObservableCollection<IBackgroundRenderer>), RenderesChanged));

		private static void RenderesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as AvalonEditRenderersBehavior).RenderesCollectionChanged(
				e.OldValue as ObservableCollection<IBackgroundRenderer>, e.NewValue as ObservableCollection<IBackgroundRenderer>);
		}

		private void RenderesCollectionChanged(ObservableCollection<IBackgroundRenderer> eOldValue, ObservableCollection<IBackgroundRenderer> eNewValue)
		{
			if (AssociatedObject == null)
			{
				return;
			}

			if (eOldValue != null)
			{
				eOldValue.CollectionChanged -= BackgroundRenderes_CollectionChanged;
			}

			if (eNewValue == null)
			{
				return;
			}
			eNewValue.CollectionChanged -= BackgroundRenderes_CollectionChanged;
			eNewValue.CollectionChanged += BackgroundRenderes_CollectionChanged;
			foreach (var backgroundRenderer in eNewValue)
			{
				AssociatedObject.TextArea.TextView.BackgroundRenderers.Add(backgroundRenderer);
			}
		}

		public ObservableCollection<IBackgroundRenderer> BackgroundRenderes
		{
			get { return (ObservableCollection<IBackgroundRenderer>) GetValue(BackgroundRenderesProperty); }
			set { SetValue(BackgroundRenderesProperty, value); }
		}
	}
}
