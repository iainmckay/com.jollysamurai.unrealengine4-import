using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JollySamurai.UnrealEngine4.Import.ShaderGraph;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;
using TextureType = UnityEditor.ShaderGraph.TextureType;

namespace DefaultNamespace
{
    public class TestEditorWindow : EditorWindow
    {
        [MenuItem("Unreal/Test")]
        public static void ShowTest()
        {
            TestEditorWindow wnd = GetWindow<TestEditorWindow>();
            wnd.titleContent = new GUIContent("Converter");
        }

        public void OnEnable()
        {
        
            var button = new Button();
            button.text = "Run";
            button.clicked += () => {
                var document = ParsedDocument.From(File.ReadAllText("M_Base_Trim.T3D"));
                var rootNode = document.RootNode;

                MaterialDocumentProcessor processor = new MaterialDocumentProcessor();
                var material = processor.Convert(document);

                if(material == null) {
                    // FIXME:
                    Debug.Log("unhandled material");

                    return;
                }

                var shaderGraph = ShaderGraphBuilder.FromMaterial(material);
                var y = 1;

//
////                var content = JsonUtility.ToJson(x);
////                File.WriteAllText(@"D:\ConverterProject\Assets\ModSci_Engineer\New Shader Graph2.shadergraph", content, Encoding.UTF8);
////                    
////                var shaderAsset = AssetDatabase.LoadAssetAtPath<Shader>(@"Assets\ModSci_Engineer\New Shader Graph2.shadergraph");
////                var abc = 1;
////                var unityMaterial = new UnityEngine.Material(shaderAsset);
//
                var shaderGraphPath = @"Assets\ModSci_Engineer\New Shader Graph2.shadergraph";
                shaderGraph.assetGuid = AssetDatabase.AssetPathToGUID(shaderGraphPath);
                shaderGraph.OnEnable();
                shaderGraph.ValidateGraph();
//
                var content = JsonUtility.ToJson(shaderGraph);
//                //var shader = ShaderUtil.CreateShaderAsset(content, false);
                File.WriteAllText(@"D:\ConverterProject\Assets\ModSci_Engineer\New Shader Graph2.shadergraph", content, Encoding.UTF8);
//
                AssetDatabase.ImportAsset(shaderGraphPath, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
//
//                AssetDatabase.StartAssetEditing();
//                try {
//
//
//                    var shaderAsset = AssetDatabase.LoadAssetAtPath<Shader>(@"Assets\ModSci_Engineer\New Shader Graph2.shadergraph");
//                    var unityMaterial = new UnityEngine.Material(shaderAsset);
//                    
//                    foreach (var keyValuePair in propertyValueList) {
//                        if (keyValuePair.Value is Texture2D) {
//                            unityMaterial.SetTexture(keyValuePair.Key, (Texture2D) keyValuePair.Value);
//                        }
//                    }
//
//                    AssetDatabase.CreateAsset(unityMaterial, @"Assets\ModSci_Engineer\New Shader Graph2.mat");
//                    var aaa = 1;
//                } catch (Exception ex) {
//                    Debug.Log(x.ToString());
//                } finally {
//                    AssetDatabase.StopAssetEditing();
//                }
//
////                content = JsonUtility.ToJson(unityMaterial);
////                File.WriteAllText(@"D:\ConverterProject\Assets\ModSci_Engineer\New Shader Graph2.mat", content, Encoding.UTF8);
            };

            var root = this.rootVisualElement;
            root.Add(button);
        }

        /*private int FindSlotReferenceId(AbstractMaterialNode destination, AbstractMaterialNode source, UnresolvedExpressionReference expressionReference, int destinationSlotId)
        {
            





















            // FIXME:
            Debug.Log("unhandled slot reference");

            throw new Exception("unhandled slot reference");
        }
        
        private SlotReference FindSlotReference(AbstractMaterialNode destination, AbstractMaterialNode source, UnresolvedExpressionReference expressionReference, int destinationSlotId)
        {
            return source.GetSlotReference(FindSlotReferenceId(destination, source, expressionReference, destinationSlotId));
        }
*/
    }
}
