using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace JollySamurai.UnrealEngine4.Import
{
    public abstract class BaseConverterWindow : EditorWindow
    {
        public abstract int TotalStageCount { get; }
        public abstract string ViewKeyPrefix { get; }

        private string _stageName;
        private int _stageTotalStepCount;
        private int _currentStageNumber;

        public void OnEnable()
        {
            AddOption("Convert a single file", false);
            AddOption("Convert a folder", true);

            this.minSize = new Vector2(300, 200);

            this.rootVisualElement.style.paddingTop = 5;
            this.rootVisualElement.style.paddingBottom = 5;
            this.rootVisualElement.style.paddingLeft = 5;
            this.rootVisualElement.style.paddingRight = 5;
        }

        protected void UpdateStageInfo(string name, int totalStepCount, int stageNumber)
        {
            _stageName = name;
            _stageTotalStepCount = totalStepCount;
            _currentStageNumber = stageNumber;
        }

        private void AddOption(string name, bool enableMultiSelect)
        {
            var container = new Foldout();
            container.text = name;

            var input = AddInput(container.contentContainer, "Source", enableMultiSelect, name);
            var output = AddOutput(container.contentContainer, "Destination", name);

            var button = new Button();
            button.text = "Convert";
            button.style.flexGrow = 1;
            button.style.marginBottom = 25;
            button.SetEnabled(false);

            container.Add(button);

            this.rootVisualElement.Add(container);

            input.RegisterValueChangedCallback(evt => {
                UpdateButtonState(button, input, output);
            });

            output.RegisterValueChangedCallback(evt => {
                UpdateButtonState(button, input, output);
            });

            button.clicked += () => {
                if(enableMultiSelect) {
                    var selection = Directory.GetFiles(input.value, "*.T3D", SearchOption.AllDirectories);

                    if(selection.Length == 0) {
                        EditorUtility.DisplayDialog("Selection Problem", "Source directory does not contain any T3D files", "OK");
                    } else {
                        ProcessSelection(selection, input.value, output.value);
                    }
                } else {
                    if(! File.Exists(input.value)) {
                        EditorUtility.DisplayDialog("Selection Problem", "Source file does not exist", "OK");
                    } else {
                        ProcessSelection(new string[] {
                            input.value
                        }, input.value, output.value);
                    }
                }
            };

            UpdateButtonState(button, input, output);
        }

        private TextField AddInput(VisualElement container, string labelText, bool enableMultiSelect, string viewKeySection)
        {
            var row = new VisualElement();

            var input = new TextField();
            input.label = labelText;

            var inputBrowserButton = new Button();
            inputBrowserButton.AddToClassList("unity-object-field__selector");
            inputBrowserButton.style.marginLeft = 0;
            inputBrowserButton.viewDataKey = MakeViewKey(viewKeySection + labelText);

            inputBrowserButton.clicked += () => {
                if(enableMultiSelect) {
                    string selectedPath = EditorUtility.OpenFolderPanel("Select folder", null, null);

                    if(! string.IsNullOrEmpty(selectedPath)) {
                        input.value = selectedPath;
                    }
                } else {
                    string selectedFile = EditorUtility.OpenFilePanelWithFilters("Select file", null, new string[] {
                        "T3D files",
                        "T3D"
                    });

                    if(! string.IsNullOrEmpty(selectedFile)) {
                        input.value = selectedFile;
                    }
                }
            };

            input.value = EditorPrefs.GetString(MakeViewKey(viewKeySection + labelText));
            input.RegisterValueChangedCallback(evt => EditorPrefs.SetString(MakeViewKey(viewKeySection + labelText), evt.newValue));

            input.contentContainer.Add(inputBrowserButton);
            row.Add(input);
            container.Add(row);

            return input;
        }

        private TextField AddOutput(VisualElement container, string labelText, string viewKeySection)
        {
            var row = new VisualElement();

            var input = new TextField();
            input.label = labelText;

            var inputBrowserButton = new Button();
            inputBrowserButton.AddToClassList("unity-object-field__selector");
            inputBrowserButton.style.marginLeft = 0;
            inputBrowserButton.viewDataKey = MakeViewKey(viewKeySection + labelText);

            inputBrowserButton.clicked += () => {
                string selectedPath = EditorUtility.OpenFolderPanel("Destination", null, null);

                if(! string.IsNullOrEmpty(selectedPath)) {
                    input.value = selectedPath;
                }
            };

            input.value = EditorPrefs.GetString(MakeViewKey(viewKeySection + labelText));
            input.RegisterValueChangedCallback(evt => EditorPrefs.SetString(MakeViewKey(viewKeySection + labelText), evt.newValue));

            input.contentContainer.Add(inputBrowserButton);
            row.Add(input);
            container.Add(row);

            return input;
        }

        private string MakeViewKey(string name)
        {
            return ViewKeyPrefix + name;
        }

        private void UpdateButtonState(Button button, TextField input, TextField output)
        {
            button.SetEnabled(! string.IsNullOrEmpty(input.value) && ! string.IsNullOrEmpty(output.value));
        }

        protected abstract void ProcessSelection(string[] selection, string inputDirectory, string outputDirectory);

        protected void IncrementStep(StepEventArgs args)
        {
            if(UpdateProgressBar(_stageName, args.FileName, _currentStageNumber, args.Index, _stageTotalStepCount)) {
                args.Cancel = true;
            }
        }

        protected bool UpdateProgressBar(string title, string info, int stage, int step, int stepCount)
        {
            float progress = (float) step / (float) stepCount;

            return EditorUtility.DisplayCancelableProgressBar(title + $" (Pass {stage} of {TotalStageCount})", info, progress);
        }

        protected bool ProcessResult(ResultSet resultSet, bool showProblemDialog = true)
        {
            foreach (var problem in resultSet.Problems) {
                if(problem.Severity == ProblemSeverity.Fatal) {
                    Debug.LogErrorFormat("{0}\nin {1}", problem.Message, problem.FileName);
                } else if(problem.Severity == ProblemSeverity.Warning) {
                    Debug.LogWarningFormat("{0}\nin {1}", problem.Message, problem.FileName);
                }
            }

            if(showProblemDialog && resultSet.HasErrors) {
                return EditorUtility.DisplayDialog("There were problems", "There were one or more errors, do you want to continue?", "Yes", "No");
            }

            return true;
        }

        public delegate void StepDelegate(StepEventArgs args);

        public class StepEventArgs : EventArgs
        {
            public string FileName { get; }
            public int Index { get; }
            public bool Cancel { get; set; }

            public StepEventArgs(string fileName, int index)
            {
                FileName = fileName;
                Index = index;
            }
        }
    }
}
