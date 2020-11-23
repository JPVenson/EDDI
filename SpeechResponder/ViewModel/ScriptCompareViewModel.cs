using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Eddi.Common.WPF.Services.Dialog;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using JPB.WPFToolsAwesome.Error.ValidationRules;
using Utilities;

namespace EddiSpeechResponder.ViewModel
{
	public class ScriptCompareViewModel : DialogViewModelBase<NoErrors>
	{
		private readonly string _referenceScript;
		private readonly Func<string> _getCurrentScript;

		public ScriptCompareViewModel(string referenceScript, Func<string> getCurrentScript)
		{
			_referenceScript = referenceScript;
			_getCurrentScript = getCurrentScript;
			base.Title = Properties.SpeechResponder.compare_script_title;
			_diffHighlighter = new DiffHighlighter();
			Renderers = new ObservableCollection<IBackgroundRenderer>();
			Renderers.Add(_diffHighlighter);
			base.IsModalDialog = false;
		}

		public ObservableCollection<IBackgroundRenderer> Renderers { get; set; }

		private string _text;

		public string Text
		{
			get { return _text; }
			set
			{
				SendPropertyChanging(() => Text);
				_text = value;
				SendPropertyChanged(() => Text);
			}
		}

		public override void OnDisplay()
		{
			ShowScriptDiff();
		}

		public void ShowScriptDiff()
		{
			SimpleWork(() =>
			{
				ParseDiffs(_referenceScript, _getCurrentScript());
			});
		}

		private readonly DiffHighlighter _diffHighlighter;

		private void ParseDiffs(string oldScript, string newScript)
		{
			var textBuilder = new StringBuilder();
			var diffItems = Diff.DiffTexts(oldScript, newScript);

			foreach (var diffItem in diffItems)
			{
				switch (diffItem.type)
				{
					case DiffItem.DiffType.Deleted:
					case DiffItem.DiffType.Inserted:
						ViewModelAction(() =>
						{
							var segment = new DiffSegment()
							{
								StartOffset = textBuilder.Length,
								Length = diffItem.data.Length,
								type = diffItem.type
							};
							_diffHighlighter.AddSegment(segment);
						});
						break;
				}
				textBuilder.AppendLine(diffItem.data);
			}

			try
			{
				var newline = Environment.NewLine;
				textBuilder.Remove(textBuilder.Length - newline.Length, newline.Length);
			}
			catch (ArgumentOutOfRangeException) { } // pass

			Text = textBuilder.ToString();
		}
	}


	internal class DiffSegment : TextSegment
	{
		public DiffItem.DiffType type;
	}

	internal class DiffHighlighter : IBackgroundRenderer
	{

		private readonly Brush deletedBrush = Brushes.LightCoral;
		private readonly Brush addedBrush = Brushes.LightGreen;
		private readonly TextSegmentCollection<DiffSegment> diffSegments = new TextSegmentCollection<DiffSegment>();

		public void AddSegment(DiffSegment segment)
		{
			diffSegments.Add(segment);
		}

		// this defines which layer we draw in
		public KnownLayer Layer => KnownLayer.Selection;

		public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (textView == null) { throw new ArgumentNullException(nameof(textView)); }
			if (drawingContext == null) { throw new ArgumentNullException(nameof(drawingContext)); }
			if (diffSegments == null || !textView.VisualLinesValid) { return; }

			var visualLines = textView.VisualLines;
			if (visualLines.Count == 0) { return; }

			int viewStart = visualLines.First().FirstDocumentLine.Offset;
			int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
			var segmentsOnScreen = diffSegments.FindOverlappingSegments(viewStart, viewEnd - viewStart);
			foreach (DiffSegment segment in segmentsOnScreen)
			{
				DrawSegment(segment, textView, drawingContext);
			}
		}

		private void DrawSegment(DiffSegment segment, TextView textView, DrawingContext drawingContext)
		{
			BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder
			{
				AlignToWholePixels = true,
				BorderThickness = 0,
				CornerRadius = 3,
			};

			Brush markerBrush;
			switch (segment.type)
			{
				case DiffItem.DiffType.Deleted:
					markerBrush = deletedBrush;
					geoBuilder.AddSegment(textView, segment);
					break;
				case DiffItem.DiffType.Inserted:
					markerBrush = addedBrush;
					geoBuilder.AddSegment(textView, segment);
					break;
				default:
					markerBrush = null;
					break;
			}

			Geometry geometry = geoBuilder.CreateGeometry();
			if (geometry != null)
			{
				drawingContext.DrawGeometry(markerBrush, null, geometry);
			}
		}
	}

}
