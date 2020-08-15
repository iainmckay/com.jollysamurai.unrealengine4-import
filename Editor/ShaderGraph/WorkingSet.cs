using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JollySamurai.UnrealEngine4.Import.Exception;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Exception;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.MaterialInstance;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Material = UnityEngine.Material;
using Vector4 = UnityEngine.Vector4;

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

        public event BaseConverterWindow.StepDelegate Step;

        public WorkingSet(string[] selection, string inputDirectory, string outputDirectory)
        {
            _selection = selection;
            _inputDirectory = inputDirectory;
            _outputDirectory = outputDirectory;
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

                    if(document.RootNode.SectionType != "Object") {
                        continue;
                    }

                    if(classValue == "/Script/Engine.Material") {
                        _materialList.Add(document);
                    } else if(classValue == "/Script/Engine.MaterialInstanceConstant") {
                        _materialInstanceList.Add(document);
                    }
                } catch (ParserException ex) {
                    resultSetBuilder.AddProblem(ProblemSeverity.Fatal, file, ex.Message);
                }
            }

            return resultSetBuilder.ToResultSet();
        }

        public ResultSet ValidateDependencies()
        {
            var resultSetBuilder = new ResultSetBuilder();

            for (var i = 0; i < _materialInstanceList.Count; i++) {
                var document = _materialInstanceList[i];

                RaiseStepEvent(document.FileName, i);

                var material = FindUnrealMaterialByInstanceDocument(document);

                if(material == null) {
                    resultSetBuilder.AddProblem(ProblemSeverity.Fatal, document.FileName, "Parent material not found");
                }
            }

            return resultSetBuilder.ToResultSet();
        }

        private ParsedDocument FindUnrealMaterialByInstanceDocument(ParsedDocument document)
        {
            var parentValue = document.RootNode.FindPropertyValue("Parent");
            var parentExpr = ValueUtil.ParseExpressionReference(parentValue);

            // TODO: check on filesystem to see if the parent has already been converted

            return FindUnrealMaterialByFileName(parentExpr.NodeName);
        }

        private ParsedDocument FindUnrealMaterialByFileName(string fileName)
        {
            fileName = Path.ChangeExtension(fileName, "T3D");

            return _materialList.Find(d => "/" + MakeRelativePath(_inputDirectory, d.FileName) == fileName);
        }

        public ResultSet CreateShaderGraphs()
        {
            var resultSetBuilder = new ResultSetBuilder();

            AssetDatabase.StartAssetEditing();

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

                var shaderGraph = ShaderGraphBuilder.FromMaterial(material);
                shaderGraph.ValidateGraph();

                var content = JsonUtility.ToJson(shaderGraph);
                var outputPath = MakeOutputPathForShaderGraph(document);

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                File.WriteAllText(outputPath, content, Encoding.UTF8);
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            return resultSetBuilder.ToResultSet();
        }

        public ResultSet CreateMaterials()
        {
            var resultSetBuilder = new ResultSetBuilder();

            AssetDatabase.StartAssetEditing();
            CreateMaterialsForShaderGraphs(resultSetBuilder);
            AssetDatabase.StopAssetEditing();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            return resultSetBuilder.ToResultSet();
        }

        public ResultSet CreateMaterialInstances()
        {
            var resultSetBuilder = new ResultSetBuilder();

            AssetDatabase.StartAssetEditing();
            CreateMaterialsForInstances(resultSetBuilder);
            AssetDatabase.StopAssetEditing();

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);

            return resultSetBuilder.ToResultSet();
        }

        private void CreateMaterialsForShaderGraphs(ResultSetBuilder resultSetBuilder)
        {
            for (var i = 0; i < _materialList.Count; i++) {
                var document = _materialList[i];

                RaiseStepEvent(document.FileName, i);

                var shaderPath = "Assets/" + MakeRelativePath(_outputDirectory, MakeOutputPathForShaderGraph(document));
                var shaderAsset = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);

                if(null == shaderAsset) {
                    continue;
                }

                var graphData = JsonUtility.FromJson<GraphData>(File.ReadAllText(shaderPath));
                var textureProperties = graphData.properties.Where(p => p.propertyType == PropertyType.Texture2D || p.propertyType == PropertyType.Texture3D);

                var materialAsset = new Material(shaderAsset);
                var outputPath = MakeOutputPathForMaterial(document, true);

                foreach (var shaderProperty in textureProperties) {
                    if(shaderProperty is Texture2DShaderProperty) {
                        // materialAsset.SetTexture(shaderProperty.referenceName, ((Texture2DShaderProperty) shaderProperty).value.texture);
                    } else if(shaderProperty is Texture3DShaderProperty) {
                        // materialAsset.SetTexture(shaderProperty.referenceName, ((Texture3DShaderProperty) shaderProperty).value.texture);
                    }
                }

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                AssetDatabase.CreateAsset(materialAsset, outputPath);
            }
        }

        private string FindNameFromParameterInfo(ParsedPropertyBag parameter)
        {
            // FIXME: this should be handled by the parser
            var tmpBag = ValueUtil.ParseAttributeList(parameter.FindPropertyValue("ParameterInfo"));

            return tmpBag.FindPropertyValue("Name");
        }

        private void CreateMaterialsForInstances(ResultSetBuilder resultSetBuilder)
        {
            for (var i = 0; i < _materialInstanceList.Count; i++) {
                var instanceDocument = _materialInstanceList[i];
                var parentDocument = FindUnrealMaterialByInstanceDocument(instanceDocument);

                RaiseStepEvent(instanceDocument.FileName, MaterialCount + i);

                if(null == parentDocument) {
                    continue;
                }

                var processor = new MaterialInstanceDocumentProcessor();
                var result = processor.Convert(instanceDocument);
                var materialInstance = result.RootNode;

                var shaderPath = "Assets/" + MakeRelativePath(_outputDirectory, MakeOutputPathForShaderGraph(parentDocument));
                var shaderAsset = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);

                if(null == shaderAsset) {
                    continue;
                }

                var graphData = JsonUtility.FromJson<GraphData>(File.ReadAllText(shaderPath));
                var materialAsset = new Material(shaderAsset);

                foreach (var parameter in materialInstance.ScalarParameters) {
                    var parameterName = parameter.FindPropertyValue("ParameterName") ?? FindNameFromParameterInfo(parameter);
                    var parameterValue = ValueUtil.ParseFloat(parameter.FindPropertyValue("ParameterValue") ?? "1.0");

                    if(graphData.properties.Any(p => p.displayName == parameterName)) {
                        var parameterReference = graphData.properties.First(p => p.displayName == parameterName).referenceName;

                        materialAsset.SetFloat(parameterReference, parameterValue);
                    }
                }

                foreach (var parameter in materialInstance.VectorParameters) {
                    var parameterName = parameter.FindPropertyValue("ParameterName") ?? FindNameFromParameterInfo(parameter);
                    var parameterValue = ValueUtil.ParseVector4(parameter.FindPropertyValue("ParameterValue") ?? "(R=0.0,G=0.0,B=0.0,A=1.0)");

                    if(graphData.properties.Any(p => p.displayName == parameterName)) {
                        var parameterReference = graphData.properties.First(p => p.displayName == parameterName).referenceName;

                        materialAsset.SetVector(parameterReference, new Vector4(parameterValue.X, parameterValue.Y, parameterValue.Z, parameterValue.A));
                    }
                }

                foreach (var parameter in materialInstance.TextureParameters) {
                    var parameterName = parameter.FindPropertyValue("ParameterName") ?? FindNameFromParameterInfo(parameter);
                    var parameterValue = ValueUtil.ParseResourceReference(parameter.FindPropertyValue("ParameterValue"));

                    if(graphData.properties.Any(p => p.displayName == parameterName)) {
                        var parameterReference = graphData.properties.First(p => p.displayName == parameterName).referenceName;

                        var textureAssetPath = "Assets" + Path.ChangeExtension(parameterValue.FileName, "TGA");
                        var textureAsset = AssetDatabase.LoadAssetAtPath<Texture>(textureAssetPath);

                        materialAsset.SetTexture(parameterReference, textureAsset);
                    }
                }

                var outputPath = MakeOutputPathForMaterial(instanceDocument, true);

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                AssetDatabase.CreateAsset(materialAsset, outputPath);
            }
        }

        private string MakeRelativePath(string input, string path)
        {
            var relativePath = path.Substring(input.Length);
            relativePath = relativePath.Replace('\\', '/');

            if(relativePath.StartsWith("/")) {
                relativePath = relativePath.Substring(1);
            }

            return relativePath;
        }

        private string MakeOutputPathForShaderGraph(ParsedDocument document)
        {
            var relativePath = MakeRelativePath(_inputDirectory, document.FileName);
            relativePath = Path.ChangeExtension(relativePath, "shadergraph");

            return Path.Combine(_outputDirectory, relativePath);
        }

        private string MakeOutputPathForMaterial(ParsedDocument document, bool relative = false)
        {
            var relativePath = MakeRelativePath(_inputDirectory, document.FileName);
            relativePath = Path.ChangeExtension(relativePath, "mat");

            if(relative) {
                return "Assets/" + relativePath;
            }

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
