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
//                if (rootMaterialNode is PBRMasterNode) {
//                    var baseColor = material.ResolveExpressionReference(material.BaseColor);
//
//                    if (baseColor != null && nodeList.ContainsKey(baseColor.Name)) {
//                        var baseColorSlot = FindSlotReference(rootMaterialNode, nodeList[baseColor.Name], material.BaseColor, PBRMasterNode.AlbedoSlotId);
//
//                        x.Connect(baseColorSlot, rootMaterialNode.GetSlotReference(PBRMasterNode.AlbedoSlotId));
//                    }
//
//                    var metallic = material.ResolveExpressionReference(material.Metallic);
//
//                    if (metallic != null && nodeList.ContainsKey(metallic.Name)) {
//                        var metallicSlot = FindSlotReference(rootMaterialNode, nodeList[metallic.Name], material.Metallic, PBRMasterNode.MetallicSlotId);
//
//                        x.Connect(metallicSlot, rootMaterialNode.GetSlotReference(PBRMasterNode.MetallicSlotId));
//                    }
//
//                    var specular = material.ResolveExpressionReference(material.Specular);
//
//                    if (specular != null && nodeList.ContainsKey(specular.Name)) {
//                        var specularSlot = FindSlotReference(rootMaterialNode, nodeList[specular.Name], material.Specular, PBRMasterNode.SpecularSlotId);
//
//                        // FIXME:
//                        Debug.Log("unhandled specular");
//
//                        //x.Connect(specularSlot, rootMaterialNode.GetSlotReference(PBRMasterNode.SpecularSlotId));
//                    }
//
//                    var roughness = material.ResolveExpressionReference(material.Roughness);
//
//                    if (roughness != null && nodeList.ContainsKey(roughness.Name)) {
//                        var roughnessSlot = FindSlotReference(rootMaterialNode, nodeList[roughness.Name], material.Roughness, PBRMasterNode.SmoothnessSlotId);
//
//                        x.Connect(roughnessSlot, rootMaterialNode.GetSlotReference(PBRMasterNode.SmoothnessSlotId));
//                    }
//
//                    var normal = material.ResolveExpressionReference(material.Normal);
//
//                    if (normal != null && nodeList.ContainsKey(normal.Name)) {
//                        var normalSlot = FindSlotReference(rootMaterialNode, nodeList[normal.Name], material.Normal, PBRMasterNode.NormalSlotId);
//
//                        x.Connect(normalSlot, rootMaterialNode.GetSlotReference(PBRMasterNode.NormalSlotId));
//                    }
//                }
//                
//                foreach (var unresolvedExpression in material.Expressions) {
//                    var materialChild = material.ResolveExpressionReference(unresolvedExpression);
//
//                    if (null == materialChild) {
//                        // FIXME:
//                        Debug.Log("unresolved expression");
//                        continue;
//                    }
//
//                    if (materialChild is MaterialExpressionTextureSampleParameter2D) {
//                        var expression = materialChild as MaterialExpressionTextureSampleParameter2D;
//
//                        if (expression.Coordinates != null) {
//                            var coordinates = material.ResolveExpressionReference(expression.Coordinates);
//
//                            if (coordinates != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(coordinates.Name)) {
//                                var coordinatesSlot = FindSlotReference(nodeList[expression.Name], nodeList[coordinates.Name], expression.Coordinates, SampleTexture2DNode.UVInput);
//
//                                x.Connect(coordinatesSlot, nodeList[expression.Name].GetSlotReference(SampleTexture2DNode.UVInput));
//                            }
//                        }
//                    }
//
//                    if (materialChild is MaterialExpressionClamp) {
//                        var expression = materialChild as MaterialExpressionClamp;
//
//                        if (expression.Input != null) {
//                            var input = material.ResolveExpressionReference(expression.Input);
//
//                            if (input != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(input.Name)) {
//                                var inputSlot = FindSlotReference(nodeList[expression.Name], nodeList[input.Name], expression.Input, 0);
//
//                                x.Connect(inputSlot, nodeList[expression.Name].GetSlotReference(0));
//                            }
//                        }
//
//                        if (expression.Min != null) {
//                            var min = material.ResolveExpressionReference(expression.Min);
//
//                            if (min != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(min.Name)) {
//                                var minSlot = FindSlotReference(nodeList[expression.Name], nodeList[min.Name], expression.Min, 1);
//
//                                x.Connect(minSlot, nodeList[expression.Name].GetSlotReference(1));
//                            }
//                        }
//
//                        if (expression.Max != null) {
//                            var max = material.ResolveExpressionReference(expression.Max);
//
//                            if (max != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(max.Name)) {
//                                var maxSlot = FindSlotReference(nodeList[expression.Name], nodeList[max.Name], expression.Max, 2);
//
//                                x.Connect(maxSlot, nodeList[expression.Name].GetSlotReference(2));
//                            }
//                        }
//                    }
//
//                    if (materialChild is MaterialExpressionDesaturation) {
//                        var expression = materialChild as MaterialExpressionDesaturation;
//
//                        if (expression.Input != null) {
//                            var input = material.ResolveExpressionReference(expression.Input);
//
//                            if (input != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(input.Name)) {
//                                var inputSlot = FindSlotReference(nodeList[expression.Name], nodeList[input.Name], expression.Input, 0);
//
//                                x.Connect(inputSlot, nodeList[expression.Name].GetSlotReference(0));
//                            }
//                        }
//
//                        if (expression.Fraction != null) {
//                            var saturation = material.ResolveExpressionReference(expression.Fraction);
//
//                            if (saturation != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(saturation.Name)) {
//                                var saturationSlot = FindSlotReference(nodeList[expression.Name], nodeList[saturation.Name], expression.Fraction, 1);
//
//                                x.Connect(saturationSlot, nodeList[expression.Name].GetSlotReference(1));
//                            }
//                        }
//                    }
//
//                    if (materialChild is MaterialExpressionStaticSwitchParameter) {
//                        var expression = materialChild as MaterialExpressionStaticSwitchParameter;
//
//                        var a = material.ResolveExpressionReference(expression.A);
//
//                        if (a != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(a.Name)) {
//                            var aSlot = FindSlotReference(nodeList[expression.Name], nodeList[a.Name], expression.A, 1);
//
//                            x.Connect(aSlot, nodeList[expression.Name].GetSlotReference(1));
//                        }
//                        
//                        var b = material.ResolveExpressionReference(expression.B);
//
//                        if (b != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(b.Name)) {
//                            var bSlot = FindSlotReference(nodeList[expression.Name], nodeList[b.Name], expression.B, 2);
//
//                            x.Connect(bSlot, nodeList[expression.Name].GetSlotReference(2));
//                        }
//                    }
//
//                    if (materialChild is MaterialExpressionAdd) {
//                        var expression = materialChild as MaterialExpressionAdd;
//
//                        if (expression.A != null) {
//                            var a = material.ResolveExpressionReference(expression.A);
//
//                            if (a != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(a.Name)) {
//                                var aSlot = FindSlotReference(nodeList[expression.Name], nodeList[a.Name], expression.A, 0);
//
//                                x.Connect(aSlot, nodeList[expression.Name].GetSlotReference(0));
//                            }
//                        }
//
//                        if (expression.B != null) {
//                            var b = material.ResolveExpressionReference(expression.B);
//
//                            if (b != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(b.Name)) {
//                                // FIXME: append nodes are missing
//                                if (! nodeList.ContainsKey(b.Name)) {
//                                    continue;
//                                }
//
//                                var bSlot = FindSlotReference(nodeList[expression.Name], nodeList[b.Name], expression.B, 1);
//
//                                x.Connect(bSlot, nodeList[expression.Name].GetSlotReference(1));
//                            }
//                        }
//                    }
//
//                    if (materialChild is MaterialExpressionMultiply) {
//                        var expression = materialChild as MaterialExpressionMultiply;
//
//                        if (expression.A != null) {
//                            var a = material.ResolveExpressionReference(expression.A);
//
//                            if (a != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(a.Name)) {
//                                var aSlot = FindSlotReference(nodeList[expression.Name], nodeList[a.Name], expression.A, 0);
//
//                                x.Connect(aSlot, nodeList[expression.Name].GetSlotReference(0));
//                            }
//                        }
//
//                        if (expression.B != null) {
//                            var b = material.ResolveExpressionReference(expression.B);
//
//                            if (b != null) {
//                                // FIXME: append nodes are missing
//                                if (! nodeList.ContainsKey(b.Name) || ! nodeList.ContainsKey(expression.Name)) {
//                                    continue;
//                                }
//
//                                var bSlot = FindSlotReference(nodeList[expression.Name], nodeList[b.Name], expression.B, 1);
//
//                                x.Connect(bSlot, nodeList[expression.Name].GetSlotReference(1));
//                            }
//                        }
//                    }
//
//                    if (materialChild is MaterialExpressionLinearInterpolate) {
//                        var expression = materialChild as MaterialExpressionLinearInterpolate;
//
//                        if (expression.A != null) {
//                            var a = material.ResolveExpressionReference(expression.A);
//
//                            if (a != null && nodeList.ContainsKey(a.Name) && nodeList.ContainsKey(expression.Name)) {
//                                var aSlot = FindSlotReference(nodeList[expression.Name], nodeList[a.Name], expression.A, 0);
//
//                                x.Connect(aSlot, nodeList[expression.Name].GetSlotReference(0));
//                            }
//                        }
//
//                        if (expression.B != null) {
//                            var b = material.ResolveExpressionReference(expression.B);
//
//                            // FIXME: append nodes are missing
//                            if (b != null && nodeList.ContainsKey(expression.Name) && nodeList.ContainsKey(b.Name)) {
//                                var bSlot = FindSlotReference(nodeList[expression.Name], nodeList[b.Name], expression.B, 1);
//
//                                x.Connect(bSlot, nodeList[expression.Name].GetSlotReference(1));
//                            }
//                        }
//
//                        if (expression.Alpha != null) {
//                            var alpha = material.ResolveExpressionReference(expression.Alpha);
//
//                            // FIXME: append nodes are missing
//                            if (alpha == null || ! nodeList.ContainsKey(alpha.Name)) {
//                                continue;
//                            }
//
//                            var alphaSlot = FindSlotReference(nodeList[expression.Name], nodeList[alpha.Name], expression.Alpha, 2);
//
//                            x.Connect(alphaSlot, nodeList[expression.Name].GetSlotReference(2));
//                        }
//                    }
//
//                    if (materialChild is MaterialExpressionAppendVector) {
//                        var expression = materialChild as MaterialExpressionAppendVector;
//
//                        if (expression.A != null) {
//                            var a = material.ResolveExpressionReference(expression.A);
//
//                            if (a != null && nodeList.ContainsKey(a.Name) && nodeList.ContainsKey(expression.Name)) {
//                                var aSlot = nodeList[a.Name].FindSlot<MaterialSlot>(FindSlotReferenceId(nodeList[expression.Name], nodeList[a.Name], expression.A, 0));
//                                var aSlotRef = FindSlotReference(nodeList[expression.Name], nodeList[a.Name], expression.A, 0);
//
//                                if (aSlot.concreteValueType == ConcreteSlotValueType.Vector4 || aSlot.concreteValueType == ConcreteSlotValueType.Vector3) {
//                                    x.Connect(aSlotRef, nodeList[expression.Name].GetSlotReference(0));
//                                    x.Connect(aSlotRef, nodeList[expression.Name].GetSlotReference(1));
//                                    x.Connect(aSlotRef, nodeList[expression.Name].GetSlotReference(2));
//                                } else if (aSlot.concreteValueType == ConcreteSlotValueType.Vector2) {
//                                    x.Connect(aSlotRef, nodeList[expression.Name].GetSlotReference(0));
//                                    x.Connect(aSlotRef, nodeList[expression.Name].GetSlotReference(1));
//                                } else if (aSlot.concreteValueType == ConcreteSlotValueType.Vector1) {
//                                    x.Connect(aSlotRef, nodeList[expression.Name].GetSlotReference(0));
//                                } else {
//                                    throw new Exception("unhandled vector type");
//                                }
//                            }
//                        }
//
//                        if (expression.B != null) {
//                            var b = material.ResolveExpressionReference(expression.B);
//
//                            if (b != null && nodeList.ContainsKey(b.Name) && nodeList.ContainsKey(expression.Name)) {
//                                var bSlot = nodeList[b.Name].FindSlot<MaterialSlot>(FindSlotReferenceId(nodeList[expression.Name], nodeList[b.Name], expression.A, 0));
//                                var bSlotRef = FindSlotReference(nodeList[expression.Name], nodeList[b.Name], expression.A, 0);
//
//                                if (bSlot.concreteValueType == ConcreteSlotValueType.Vector4 || bSlot.concreteValueType == ConcreteSlotValueType.Vector3) {
//                                    x.Connect(bSlotRef, nodeList[expression.Name].GetSlotReference(0));
//                                    x.Connect(bSlotRef, nodeList[expression.Name].GetSlotReference(1));
//                                    x.Connect(bSlotRef, nodeList[expression.Name].GetSlotReference(2));
//                                } else if (bSlot.concreteValueType == ConcreteSlotValueType.Vector2) {
//                                    x.Connect(bSlotRef, nodeList[expression.Name].GetSlotReference(1));
//                                    x.Connect(bSlotRef, nodeList[expression.Name].GetSlotReference(2));
//                                } else if (bSlot.concreteValueType == ConcreteSlotValueType.Vector1) {
//                                    x.Connect(bSlotRef, nodeList[expression.Name].GetSlotReference(1));
//                                } else {
//                                    throw new Exception("unhandled vector type");
//                                }
//                            }
//                        }
//                    }
//
//                    if (materialChild is MaterialExpressionMaterialFunctionCall) {
//                        var expression = materialChild as MaterialExpressionMaterialFunctionCall;
//
//                        if (expression.MaterialFunction.NodeName == "/Engine/Functions/Engine_MaterialFunctions01/ImageAdjustment/CheapContrast_RGB.CheapContrast_RGB") {
//                            foreach (var functionInput in expression.FunctionInputs) {
//                                var resolvedFunctionInput = material.ResolveExpressionReference(functionInput);
//
//                                if (resolvedFunctionInput != null) {
//                                    var slotId = -1;
//                                    
//                                    if (functionInput.Properties.FindPropertyValue("InputName") == "In") {
//                                        slotId = 0;
//                                    } else if (functionInput.Properties.FindPropertyValue("InputName") == "Contrast") {
//                                        slotId = 1;
//                                    } else {
//                                        throw new Exception("unhandled function call input");
//                                    }
//
//                                    var inputSlot = nodeList[resolvedFunctionInput.Name].FindSlot<MaterialSlot>(FindSlotReferenceId(nodeList[expression.Name], nodeList[resolvedFunctionInput.Name], functionInput, slotId));
//                                    var inputSlotRef = FindSlotReference(nodeList[expression.Name], nodeList[resolvedFunctionInput.Name], functionInput, slotId);
//                                    
//                                    x.Connect(inputSlotRef, nodeList[expression.Name].GetSlotReference(slotId));
//                                }
//                            }
//                        }
//                        
//                        if (expression.MaterialFunction.NodeName == "/Engine/Functions/Engine_MaterialFunctions02/Utility/BlendAngleCorrectedNormals.BlendAngleCorrectedNormals") {
//                            foreach (var functionInput in expression.FunctionInputs) {
//                                var resolvedFunctionInput = material.ResolveExpressionReference(functionInput);
//
//                                if (resolvedFunctionInput != null) {
//                                    var slotId = -1;
//                                    
//                                    if (functionInput.Properties.FindPropertyValue("InputName") == "BaseNormal") {
//                                        slotId = 0;
//                                    } else if (functionInput.Properties.FindPropertyValue("InputName") == "AdditionalNormal") {
//                                        slotId = 1;
//                                    } else {
//                                        throw new Exception("unhandled function call input");
//                                    }
//
//                                    var inputSlot = nodeList[resolvedFunctionInput.Name].FindSlot<MaterialSlot>(FindSlotReferenceId(nodeList[expression.Name], nodeList[resolvedFunctionInput.Name], functionInput, slotId));
//                                    var inputSlotRef = FindSlotReference(nodeList[expression.Name], nodeList[resolvedFunctionInput.Name], functionInput, slotId);
//                                    
//                                    x.Connect(inputSlotRef, nodeList[expression.Name].GetSlotReference(slotId));
//                                }
//                            }
//                        }
//                    }
//                }
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
            if (expressionReference.ClassName == "MaterialExpressionTextureSampleParameter2D") {
                var propertyBag = expressionReference.Properties;

                var hasR = propertyBag.HasProperty("MaskR");
                var hasG = propertyBag.HasProperty("MaskG");
                var hasB = propertyBag.HasProperty("MaskB");
                var hasA = propertyBag.HasProperty("MaskA");

                if (hasR && hasG && hasB && hasA) {
                    return SampleTexture2DNode.OutputSlotRGBAId;
                }  else if (hasR && hasG && hasB) {
                    // FIXME: should this be only RGB instead?
                    return SampleTexture2DNode.OutputSlotRGBAId;
                } else if (hasR) {
                    return SampleTexture2DNode.OutputSlotRId;
                } else if (hasG) {
                    return SampleTexture2DNode.OutputSlotGId;
                } else if (hasB) {
                    return SampleTexture2DNode.OutputSlotBId;
                }
            }

            if (expressionReference.ClassName == "MaterialExpressionMultiply") {
                return 2;
            }

            if (expressionReference.ClassName == "MaterialExpressionLinearInterpolate") {
                return 3;
            }

            if (expressionReference.ClassName == "MaterialExpressionClamp") {
                return 3;
            }

            if (expressionReference.ClassName == "MaterialExpressionVectorParameter") {
                var propertyBag = expressionReference.Properties;

                var hasR = propertyBag.HasProperty("MaskR");
                var hasG = propertyBag.HasProperty("MaskG");
                var hasB = propertyBag.HasProperty("MaskB");
                var hasA = propertyBag.HasProperty("MaskA");

                if (hasA || ! hasR || ! hasG || ! hasB) {
                    Debug.Log("unhandled vector parameter mask");
                    return default;
                }

                return PropertyNode.OutputSlotId;
            }

            if (expressionReference.ClassName == "MaterialExpressionScalarParameter") {
                return PropertyNode.OutputSlotId;
            }

            if (expressionReference.ClassName == "MaterialExpressionTextureCoordinate") {
                return UVNode.OutputSlotId;
            }

            if (expressionReference.ClassName == "MaterialExpressionDesaturation") {
                return 2;
            }

            if (expressionReference.ClassName == "MaterialExpressionAdd") {
                return 2;
            }

            if (expressionReference.ClassName == "MaterialExpressionStaticSwitchParameter") {
                return 3;
            }

            if (expressionReference.ClassName == "MaterialExpressionAppendVector") {
                var destinationSlot = destination.FindInputSlot<MaterialSlot>(destinationSlotId);

                if (destinationSlot.concreteValueType == ConcreteSlotValueType.Vector2) {
                    return 6;
                } else if (destinationSlot.concreteValueType == ConcreteSlotValueType.Vector3) {
                    return 5;
                } else if (destinationSlot.concreteValueType == ConcreteSlotValueType.Vector4) {
                    return 4;
                }

                throw new Exception("unhandled vector type");
            }

            if (expressionReference.ClassName == "MaterialExpressionMaterialFunctionCall") {
                if (source is ContrastNode) {
                    return 2;
                }

                if (source is NormalBlendNode) {
                    return 2;
                }
            }

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
