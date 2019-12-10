using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using JollySamurai.UnrealEngine4.Import.Exception;
using JollySamurai.UnrealEngine4.T3D.Exception;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph
{
    public class WorkingSet
    {
        public int MaterialCount {
            get { return _materialList.Count; }
        }

        public int MaterialInstanceCount {
            get { return _materialInstanceList.Count; }
        }

        private readonly string[] _selection;
        private readonly string _inputDirectory;
        private readonly string _outputDirectory;
        private readonly List<ParsedDocument> _materialList = new List<ParsedDocument>();
        private readonly List<ParsedDocument> _materialInstanceList = new List<ParsedDocument>();

        public event StepDelegate Step;

        public WorkingSet(string[] selection, string inputDirectory, string outputDirectory)
        {
            _selection = selection;
            _inputDirectory = inputDirectory;
            _outputDirectory = outputDirectory;
        }

        public void Begin()
        {
            AssetDatabase.StartAssetEditing();
        }

        public void End()
        {
            AssetDatabase.StopAssetEditing();
        }

        public ResultSet Parse()
        {
            var resultSetBuilder = new ResultSetBuilder();

            _materialList.Clear();
            _materialInstanceList.Clear();

            for (var i = 0; i < _selection.Length; i++) {
                var file = _selection[i];

                RaiseStepEvent(file, i);

                try {
                    var content = File.ReadAllText(file);
                    var document = ParsedDocument.From(file, content);
                    var classValue = document.RootNode.FindAttributeValue("Class");

                    if(classValue == "/Script/Engine.Material") {
                        _materialList.Add(document);
                    } else {
                        resultSetBuilder.AddProblem(ProblemSeverity.Warning, file, $"Encountered unhandled Unreal type \"{classValue}\" in \"{file}\"");
                    }
                } catch (ParserException ex) {
                    resultSetBuilder.AddProblem(ProblemSeverity.Fatal, file, ex.Message);
                }
            }

            return resultSetBuilder.ToResultSet();
        }

        public void ValidateDependencies()
        {
        }

        public ResultSet CreateShaderGraphs()
        {
            var resultSetBuilder = new ResultSetBuilder();

            for (var i = 0; i < _materialList.Count; i++) {
                var document = _materialList[i];
                var processor = new MaterialDocumentProcessor();

                RaiseStepEvent(document.FileName, i);

                var result = processor.Convert(document);
                var material = result.RootNode;

                resultSetBuilder.AddProcessorProblems(result.Problems, document.FileName);
                
                if(material == null) {
                    continue;
                }

                var relativePath = document.FileName.Substring(_inputDirectory.Length);
                relativePath = Path.ChangeExtension(relativePath, "shadergraph");

                if(relativePath.StartsWith("/") || relativePath.StartsWith("\\")) {
                    relativePath = relativePath.Substring(1);
                }

                var shaderGraph = ShaderGraphBuilder.FromMaterial(material);
                shaderGraph.ValidateGraph();

                var content = JsonUtility.ToJson(shaderGraph);
                var outputPath = Path.Combine(_outputDirectory, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                File.WriteAllText(outputPath, content, Encoding.UTF8);
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            return resultSetBuilder.ToResultSet();
        }

        private void RaiseStepEvent(string fileName, int index)
        {
            var args = new StepEventArgs(fileName, index);

            Step?.Invoke(args);

            if(args.Cancel) {
                throw new UserCancelledOperation();
            }
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

        public class ResultSetBuilder
        {
            public bool HasErrors {
                get { return _hasErrors; }
            }

            public bool HasWarnings {
                get { return _hasWarnings; }
            }

            private List<Problem> _problems = new List<Problem>();
            private bool _hasErrors;
            private bool _hasWarnings;

            public void AddProblem(ProblemSeverity severity, string fileName, string message)
            {
                _problems.Add(new Problem(severity, fileName, message));

                if(severity == ProblemSeverity.Fatal) {
                    _hasErrors = true;
                } else if(severity == ProblemSeverity.Warning) {
                    _hasWarnings = true;
                }
            }

            public void AddProcessorProblems(T3D.Processor.Problem[] resultProblems, string documentFileName)
            {
                foreach (var resultProblem in resultProblems) {
                    AddProblem(TranslateSeverity(resultProblem.Severity), documentFileName, resultProblem.Message);
                }
            }

            public ResultSet ToResultSet()
            {
                return new ResultSet(_problems.ToArray(), _hasErrors, _hasWarnings);
            }

            private ProblemSeverity TranslateSeverity(T3D.Processor.ProblemSeverity severity)
            {
                switch (severity) {
                    case T3D.Processor.ProblemSeverity.Error:
                        return ProblemSeverity.Fatal;
                    case T3D.Processor.ProblemSeverity.Warning:
                        return ProblemSeverity.Warning;
                }

                throw new System.Exception("Unexpected severity");
            }
        }

        public class ResultSet
        {
            public Problem[] Problems { get; }
            public bool HasErrors { get; }
            public bool HasWarnings { get; }

            internal ResultSet(Problem[] problems, bool hasErrors, bool hasWarnings)
            {
                Problems = problems;
                HasErrors = hasErrors;
                HasWarnings = hasWarnings;
            }
        }

        public class Problem
        {
            public ProblemSeverity Severity { get; }
            public string FileName { get; }
            public string Message { get; }

            public Problem(ProblemSeverity severity, string fileName, string message)
            {
                Severity = severity;
                FileName = fileName;
                Message = message;
            }
        }

        public enum ProblemSeverity
        {
            Fatal,
            Warning
        }
    }
}
