using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			BackgroundRenderes.CollectionChanged -= BackgroundRenderes_CollectionChanged;
			BackgroundRenderes.CollectionChanged += BackgroundRenderes_CollectionChanged;
			foreach (var backgroundRenderer in BackgroundRenderes)
			{
				AssociatedObject.TextArea.TextView.BackgroundRenderers.Add(backgroundRenderer);
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

		public ObservableCollection<IBackgroundRenderer> BackgroundRenderes { get; set; }
	}
}
