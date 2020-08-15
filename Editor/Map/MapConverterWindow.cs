using JollySamurai.UnrealEngine4.Import.Exception;
using UnityEditor;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map
{
    public class MapConverterWindow : BaseConverterWindow
    {
        public override int TotalStageCount => 2;
        public override string ViewKeyPrefix => "MapConverter";

        private WorkingSet _currentWorkingSet;

        [MenuItem("UnrealEngine4/Map Converter")]
        public static void ShowWindow()
        {
            var window = GetWindow<MapConverterWindow>();
            window.titleContent = new GUIContent("UnrealEngine4/Map Converter");
        }

        protected override void ProcessSelection(string[] selection, string inputDirectory, string outputDirectory)
        {
            UpdateStageInfo("Parsing", selection.Length, 1);

            try {
                _currentWorkingSet = new WorkingSet(selection, inputDirectory, outputDirectory);
                _currentWorkingSet.Step += IncrementStep;

                if(! ProcessResult(_currentWorkingSet.Parse())) {
                    return;
                }

                UpdateStageInfo("Creating Scenes", _currentWorkingSet.MapCount, 2);

                AssetDatabase.StartAssetEditing();
                ProcessResult(_currentWorkingSet.CreateScenes(), false);
            } catch (UserCancelledOperation) {
                Debug.Log("Conversion cancelled by user");
            } finally {
                EditorUtility.ClearProgressBar();
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}
