using System.Collections.Generic;
using System.IO;
using System.Text;
using JollySamurai.UnrealEngine4.Import.Exception;
using JollySamurai.UnrealEngine4.Import.ShaderGraph;
using JollySamurai.UnrealEngine4.T3D.Exception;
using JollySamurai.UnrealEngine4.T3D.Map;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map
{
    public class WorkingSet
    {
        public int MapCount {
            get { return _mapList.Count; }
        }

        private readonly string[] _selection;
        private readonly string _inputDirectory;
        private readonly string _outputDirectory;

        public event BaseConverterWindow.StepDelegate Step;


        private readonly List<ParsedDocument> _mapList = new List<ParsedDocument>();

        public WorkingSet(string[] selection, string inputDirectory, string outputDirectory)
        {
            _selection = selection;
            _inputDirectory = inputDirectory;
            _outputDirectory = outputDirectory;
        }

        public ResultSet Parse()
        {
            var resultSetBuilder = new ResultSetBuilder();

            _mapList.Clear();

            for (var i = 0; i < _selection.Length; i++) {
                var file = _selection[i];

                RaiseStepEvent(file, i);

                try {
                    var content = File.ReadAllText(file);
                    var document = ParsedDocument.From(file, content);

                    if(document.RootNode.SectionType != "Map") {
                        continue;
                    }

                    _mapList.Add(document);
                } catch (ParserException ex) {
                    resultSetBuilder.AddProblem(ProblemSeverity.Fatal, file, ex.Message);
                }
            }

            return resultSetBuilder.ToResultSet();
        }

        public ResultSet CreateScenes()
        {
            var resultSetBuilder = new ResultSetBuilder();

            for (var i = 0; i < _mapList.Count; i++) {
                var document = _mapList[i];
                var processor = new MapDocumentProcessor();

                RaiseStepEvent(document.FileName, i);

                var result = processor.Convert(document);
                var map = result.RootNode;

                resultSetBuilder.AddProcessorProblems(result.Problems, document.FileName);

                if(map == null) {
                    continue;
                }

                SceneBuilder.OutputFromMap(map, MakeOutputPathForScene(document));
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            return resultSetBuilder.ToResultSet();
        }

        private string MakeRelativePath(string input, string path)
        {
            if(File.Exists(input)) {
                return Path.GetFileName(input);
            }

            var relativePath = path.Substring(input.Length);
            relativePath = relativePath.Replace('\\', '/');

            if(relativePath.StartsWith("/")) {
                relativePath = relativePath.Substring(1);
            }

            return relativePath;
        }

        private string MakeOutputPathForScene(ParsedDocument document)
        {
            var relativePath = MakeRelativePath(_inputDirectory, document.FileName);
            relativePath = Path.ChangeExtension(relativePath, "unity");

            return Path.Combine(_outputDirectory, relativePath);
        }

        private void RaiseStepEvent(string fileName, int index)
        {
            var args = new BaseConverterWindow.StepEventArgs(fileName, index);

            Step?.Invoke(args);

            if(args.Cancel) {
                throw new UserCancelledOperation();
            }
        }

    }
}
