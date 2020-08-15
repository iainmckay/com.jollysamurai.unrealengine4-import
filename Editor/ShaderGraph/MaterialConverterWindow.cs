using JollySamurai.UnrealEngine4.Import.Exception;
using UnityEditor;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph
{
    public class MaterialConverterWindow : BaseConverterWindow
    {
        public override int TotalStageCount => 5;
        public override string ViewKeyPrefix => "MaterialConverter";

        private WorkingSet _currentWorkingSet;

        [MenuItem("UnrealEngine4/Material Converter")]
        public static void ShowWindow()
        {
            var window = GetWindow<MaterialConverterWindow>();
            window.titleContent = new GUIContent("UnrealEngine4/Material Converter");
        }

        protected override void ProcessSelection(string[] selection, string inputDirectory, string outputDirectory)
        {
            UpdateStageInfo("Parsing and Sorting", selection.Length, 1);

            try {
                _currentWorkingSet = new WorkingSet(selection, inputDirectory, outputDirectory);
                _currentWorkingSet.Step += IncrementStep;

                if(! ProcessResult(_currentWorkingSet.Parse())) {
                    return;
                }

                UpdateStageInfo("Validating dependencies", _currentWorkingSet.MaterialInstanceCount, 2);

                if(! ProcessResult(_currentWorkingSet.ValidateDependencies())) {
                    return;
                }

                UpdateStageInfo("Creating ShaderGraphs", _currentWorkingSet.MaterialCount, 3);
                ProcessResult(_currentWorkingSet.CreateShaderGraphs(), false);

                UpdateStageInfo("Creating Materials", _currentWorkingSet.MaterialCount, 4);
                ProcessResult(_currentWorkingSet.CreateMaterials(), false);

                UpdateStageInfo("Creating Material Instances", _currentWorkingSet.MaterialInstanceCount, 5);
                ProcessResult(_currentWorkingSet.CreateMaterialInstances(), false);
            } catch (UserCancelledOperation) {
                Debug.Log("Conversion cancelled by user");
            } finally {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
