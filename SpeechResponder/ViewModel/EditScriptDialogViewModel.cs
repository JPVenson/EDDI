using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eddi.Common.WPF.Services.Dialog;
using EddiEvents;
using EddiSpeechResponder.Service;
using EddiSpeechService;
using JPB.WPFToolsAwesome.Error.ValidationRules;
using JPB.WPFToolsAwesome.MVVM.DelegateCommand;
using JPB.WPFToolsAwesome.MVVM.ViewModel;
using Utilities.Unity;

namespace EddiSpeechResponder.ViewModel
{
	public class EditScriptDialogViewModel : DialogViewModelBase<NoErrors>
	{
		public EditScriptDialogViewModel(IDictionary<string, Script> scripts,
			string name)
		{
			Title = "Edit Script";

			_scripts = scripts;
			_oldName = ScriptName = name;
			if (scripts.TryGetValue(name, out var script))
			{
				ScriptName = script.Name;
				ScriptDescription = script.Description;
				ScriptValue = script.Value;
				ScriptDefaultValue = script.defaultValue;
				IsResponder = script.Responder;
				Priority = script.Priority;
			}
			else
			{
				ScriptName = "New script";
				ScriptDescription = null;
				ScriptValue = null;
				IsResponder = false;
				Priority = 3;
			}

			ScriptRecoveryService = IoC.Resolve<ScriptRecoveryService>();
			ScriptRecoveryService.BeginScriptRecovery(this);

			CloseViewCommand = new DelegateCommand(CloseViewExecute, CanCloseViewExecute);
			SaveCommand = new DelegateCommand(SaveExecute, CanSaveExecute);
			ResetCommand = new DelegateCommand(ResetExecute, CanResetExecute);
			TestScriptCommand = new DelegateCommand(TestScriptExecute, CanTestScriptExecute);
			CompareScriptContentCommand = new DelegateCommand(CompareScriptContentExecute, CanCompareScriptContentExecute);
			OpenScriptDocumentationCommand = new DelegateCommand(OpenScriptDocumentationExecute, CanOpenScriptDocumentationExecute);
			OpenScriptVariablesDocumentationCommand = new DelegateCommand(OpenScriptVariablesDocumentationExecute, CanOpenScriptVariablesDocumentationExecute);

			Commands.Add(new DialogCommand(SaveCommand)
			{
				Content = Properties.SpeechResponder.button_ok,
			});
			Commands.Add(new DialogCommand(TestScriptCommand)
			{
				Content = Properties.SpeechResponder.test_script_button,
			});
			Commands.Add(new DialogCommand(ResetCommand)
			{
				Content = Properties.SpeechResponder.reset_script_button,
			});
			Commands.Add(new DialogCommand(CompareScriptContentCommand)
			{
				Content = Properties.SpeechResponder.compare_script_button,
			});
			Commands.Add(new DialogCommand(OpenScriptDocumentationCommand)
			{
				Content = Properties.SpeechResponder.help_button,
			});
			Commands.Add(new DialogCommand(OpenScriptVariablesDocumentationCommand)
			{
				Content = Properties.SpeechResponder.script_variables_button,
			});
			Commands.Add(new DialogCommand(base.CloseDialogCommand)
			{
				Content = Properties.SpeechResponder.button_cancel,
			});
		}

		public DelegateCommand CloseViewCommand { get; private set; }
		public DelegateCommand SaveCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }
		public DelegateCommand TestScriptCommand { get; private set; }
		public DelegateCommand CompareScriptContentCommand { get; private set; }
		public DelegateCommand OpenScriptDocumentationCommand { get; private set; }
		public DelegateCommand OpenScriptVariablesDocumentationCommand { get; private set; }

		private void OpenScriptVariablesDocumentationExecute(object sender)
		{
			IoC.Resolve<IDialogService>()
				.ShowDialog(new DisplayMarkdownDialogViewModel("Help.md"));
		}

		private bool CanOpenScriptVariablesDocumentationExecute(object sender)
		{
			return true;
		}

		private void OpenScriptDocumentationExecute(object sender)
		{
			IoC.Resolve<IDialogService>()
				.ShowDialog(new DisplayMarkdownDialogViewModel("Variables.md", markdown =>
				{
					if (Events.DESCRIPTIONS.TryGetValue(this.ScriptName, out string description))
					{
						// The user is editing an event, add event-specific information
						markdown += "\n\n## " + this.ScriptName + " event\n\n" + description + ".\n\n";
						if (Events.VARIABLES.TryGetValue(this.ScriptName, out IDictionary<string, string> variables))
						{
							if (variables.Count == 0)
							{
								markdown += "This event has no variables.";
							}
							else
							{
								markdown += "Information about this event is available under the `event` object.  Note that these variables are only valid for this particular script; other scripts triggered by different events will have different variables available to them.\n\n";
								foreach (KeyValuePair<string, string> variable in Events.VARIABLES[this.ScriptName])
								{
									markdown += "    - " + variable.Key + " " + variable.Value + "\n";
								}
							}
						}
					}

					return markdown;
				}));
		}

		private bool CanOpenScriptDocumentationExecute(object sender)
		{
			return true;
		}

		private void CompareScriptContentExecute(object sender)
		{
			var diffViewModel = new ScriptCompareViewModel(ScriptDefaultValue, () =>
			{
				return this.ScriptValue;
			});

			void OnPropertyChanged(object o, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == nameof(ScriptValue))
				{
					diffViewModel.ShowScriptDiff();
				}
			}

			PropertyChanged += OnPropertyChanged;
			IoC.Resolve<IDialogService>().ShowDialog(diffViewModel);
			PropertyChanged -= OnPropertyChanged;
		}

		private bool CanCompareScriptContentExecute(object sender)
		{
			return !string.IsNullOrWhiteSpace(ScriptDefaultValue);
		}

		private void TestScriptExecute(object sender)
		{
			var speechService = IoC.Resolve<SpeechService>();
			if (!speechService.eddiSpeaking)
			{
				ScriptRecoveryService.InvokeSave(this);
				Dictionary<string, Script> newScripts = new Dictionary<string, Script>(_scripts);
				Script testScript = new Script(ScriptName, ScriptDescription, false, ScriptValue);
				newScripts.Remove(ScriptName);
				newScripts.Add(ScriptName, testScript);

				var speechResponder = IoC.Resolve<SpeechResponder>();
				speechResponder.Start();
				speechResponder.TestScript(ScriptName, newScripts);
			}
		}

		private bool CanTestScriptExecute(object sender)
		{
			return true;
		}

		private void ResetExecute(object sender)
		{
			ScriptValue = ScriptDefaultValue;
		}

		private bool CanResetExecute(object sender)
		{
			return ScriptValue != ScriptDefaultValue;
		}

		private void SaveExecute(object sender)
		{
			var script = new Script(ScriptName, ScriptDescription, IsResponder, ScriptValue, Priority, ScriptDefaultValue);

			Script defaultScript = null;
			if (Personality.Default().Scripts?.TryGetValue(script.Name, out defaultScript) ?? false)
			{
				script = Personality.UpgradeScript(script, defaultScript);
			}
			// Might be updating an existing script so remove it from the list before adding
			_scripts.Remove(_oldName);
			_scripts.Add(script.Name, script);
			Result = true;

			CloseDialogExecute();
		}

		private bool CanSaveExecute(object sender)
		{
			return !string.IsNullOrWhiteSpace(ScriptValue);
		}

		private void CloseViewExecute(object sender)
		{
			ScriptRecoveryService.StopScriptRecovery();
		}

		private bool CanCloseViewExecute(object sender)
		{
			return true;
		}

		private IDictionary<string, Script> _scripts;
		private string _oldName;
		private string _scriptName;
		private string _scriptDescription;
		private string _scriptValue;
		private bool _isResponder;
		private int _priority;
		private string _scriptDefaultValue;
		public ScriptRecoveryService ScriptRecoveryService { get; set; }

		public string ScriptDefaultValue
		{
			get { return _scriptDefaultValue; }
			set
			{
				SendPropertyChanging(() => ScriptDefaultValue);
				_scriptDefaultValue = value;
				SendPropertyChanged(() => ScriptDefaultValue);
			}
		}

		public int Priority
		{
			get { return _priority; }
			set
			{
				SendPropertyChanging(() => Priority);
				_priority = value;
				SendPropertyChanged(() => Priority);
			}
		}

		public bool IsResponder
		{
			get { return _isResponder; }
			set
			{
				SendPropertyChanging(() => IsResponder);
				_isResponder = value;
				SendPropertyChanged(() => IsResponder);
			}
		}

		public string ScriptValue
		{
			get { return _scriptValue; }
			set
			{
				SendPropertyChanging(() => ScriptValue);
				_scriptValue = value;
				SendPropertyChanged(() => ScriptValue);
			}
		}


		public string ScriptDescription
		{
			get { return _scriptDescription; }
			set
			{
				SendPropertyChanging(() => ScriptDescription);
				_scriptDescription = value;
				SendPropertyChanged(() => ScriptDescription);
			}
		}

		public string ScriptName
		{
			get { return _scriptName; }
			set
			{
				SendPropertyChanging(() => ScriptName);
				_scriptName = value;
				SendPropertyChanged(() => ScriptName);
			}
		}
	}
}
