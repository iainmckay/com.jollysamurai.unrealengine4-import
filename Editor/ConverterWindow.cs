using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using JollySamurai.UnrealEngine4.Import.Exception;
using JollySamurai.UnrealEngine4.Import.ShaderGraph;
using JollySamurai.UnrealEngine4.Import.UI;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using Material = UnityEngine.Material;

namespace JollySamurai.UnrealEngine4.Import
{
    public class ConverterWindow : EditorWindow
    {
        private const int StageCount = 4;

        private WorkingSet _currentWorkingSet;

        private string _stageName;
        private int _stageStepCount;
        private int _stageNumber;

        [MenuItem("UnrealEngine4/Material Converter")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConverterWindow>();
            window.titleContent = new GUIContent("UnrealEngine4/Material Converter");
        }

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
        }

        private TextField AddInput(VisualElement container, string labelText, bool enableMultiSelect, string viewKeyPrefix)
        {
            var row = new VisualElement();

            var input = new TextField();
            input.label = labelText;

            var inputBrowserButton = new Button();
            inputBrowserButton.AddToClassList("unity-object-field__selector");
            inputBrowserButton.style.marginLeft = 0;
            inputBrowserButton.viewDataKey = viewKeyPrefix + labelText;

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

            input.value = EditorPrefs.GetString(viewKeyPrefix + labelText);
            input.RegisterValueChangedCallback(evt => EditorPrefs.SetString(viewKeyPrefix + labelText, evt.newValue));

            input.contentContainer.Add(inputBrowserButton);
            row.Add(input);
            container.Add(row);

            return input;
        }

        private TextField AddOutput(VisualElement container, string labelText, string viewKeyPrefix)
        {
            var row = new VisualElement();

            var input = new TextField();
            input.label = labelText;

            var inputBrowserButton = new Button();
            inputBrowserButton.AddToClassList("unity-object-field__selector");
            inputBrowserButton.style.marginLeft = 0;
            inputBrowserButton.viewDataKey = viewKeyPrefix + labelText;

            inputBrowserButton.clicked += () => {
                string selectedPath = EditorUtility.OpenFolderPanel("Destination", null, null);

                if(! string.IsNullOrEmpty(selectedPath)) {
                    input.value = selectedPath;
                }
            };

            input.value = EditorPrefs.GetString(viewKeyPrefix + labelText);
            input.RegisterValueChangedCallback(evt => EditorPrefs.SetString(viewKeyPrefix + labelText, evt.newValue));

            input.contentContainer.Add(inputBrowserButton);
            row.Add(input);
            container.Add(row);

            return input;
        }

        private void UpdateButtonState(Button button, TextField input, TextField output)
        {
            button.SetEnabled(! string.IsNullOrEmpty(input.value) && ! string.IsNullOrEmpty(output.value));
        }

        private void ProcessSelection(string[] selection, string inputDirectory, string outputDirectory)
        {
            _stageName = "Parsing and Sorting";
            _stageStepCount = selection.Length;
            _stageNumber = 1;

            UpdateProgressBar(_stageName, "Initializing...", _stageNumber, 0, _stageStepCount);

            try {
                _currentWorkingSet = new WorkingSet(selection, inputDirectory, outputDirectory);
                _currentWorkingSet.Step += HandleStep;

                if(! ProcessResult(_currentWorkingSet.Parse())) {
                    return;
                }

                _stageName = "Validating dependencies";
                _stageStepCount = _currentWorkingSet.MaterialInstanceCount;
                _stageNumber = 2;

                if(! ProcessResult(_currentWorkingSet.ValidateDependencies())) {
                    return;
                }

                _stageName = "Creating ShaderGraphs";
                _stageStepCount = _currentWorkingSet.MaterialCount;
                _stageNumber = 3;

                AssetDatabase.StartAssetEditing();
                ProcessResult(_currentWorkingSet.CreateShaderGraphs(), false);
                AssetDatabase.StopAssetEditing();

                _stageName = "Creating Materials";
                _stageStepCount = _currentWorkingSet.MaterialCount + _currentWorkingSet.MaterialInstanceCount;
                _stageNumber = 4;

                AssetDatabase.StartAssetEditing();
                ProcessResult(_currentWorkingSet.CreateMaterials(), false);
            } catch (UserCancelledOperation) {
                Debug.Log("Conversion cancelled by user");
            } finally {
                EditorUtility.ClearProgressBar();
                AssetDatabase.StopAssetEditing();
            }
        }

        private void HandleStep(WorkingSet.StepEventArgs args)
        {
            if(UpdateProgressBar(_stageName, args.FileName, _stageNumber, args.Index, _stageStepCount)) {
                args.Cancel = true;
            }
        }

        private bool UpdateProgressBar(string title, string info, int stage, int step, int stepCount)
        {
            float progress = (float) step / (float) stepCount;

            return EditorUtility.DisplayCancelableProgressBar(title + $" (Pass {stage} of {StageCount})", info, progress);
        }

        private bool ProcessResult(WorkingSet.ResultSet resultSet, bool showProblemDialog = true)
        {
            foreach (var problem in resultSet.Problems) {
                if(problem.Severity == WorkingSet.ProblemSeverity.Fatal) {
                    Debug.LogErrorFormat("{0}\nin {1}", problem.Message, problem.FileName);
                } else if(problem.Severity == WorkingSet.ProblemSeverity.Warning) {
                    Debug.LogWarningFormat("{0}\nin {1}", problem.Message, problem.FileName);
                }
            }

            if(showProblemDialog && resultSet.HasErrors) {
                return EditorUtility.DisplayDialog("There were problems", "There were one or more errors, do you want to continue?", "Yes", "No");
            }

            return true;
        }
    }
}
