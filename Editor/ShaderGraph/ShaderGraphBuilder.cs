using System;
using System.Collections.Generic;
using JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions;
using JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Functions;
using JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Roots;
using UnityEditor.Graphing;
using UnityEditor.Graphs;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Material = JollySamurai.UnrealEngine4.T3D.Material.Material;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph
{
    public class ShaderGraphBuilder
    {
        private readonly Material _unrealMaterial;
        private readonly GraphData _graph;

        private readonly Dictionary<string, ShaderInputLookup> _propertyLookup;
        private readonly Dictionary<string, AbstractMaterialNode> _nodeLookupByUnrealNodeName;
        private readonly List<UnrealNodeConverter> _nodeConverters;

        private ShaderGraphBuilder(Material unrealMaterial)
        {
            _unrealMaterial = unrealMaterial;
            _graph = new GraphData();
            _propertyLookup = new Dictionary<string, ShaderInputLookup>();
            _nodeLookupByUnrealNodeName = new Dictionary<string, AbstractMaterialNode>();
            _nodeConverters = new List<UnrealNodeConverter>();

            AddBuiltinConverters();
        }

        private void AddBuiltinConverters()
        {
            AddNodeConverter(new MaterialExpressionAddConverter());
            AddNodeConverter(new MaterialExpressionAppendVectorConverter());
            AddNodeConverter(new MaterialExpressionClampConverter());
            AddNodeConverter(new MaterialExpressionDesaturationConverter());
            AddNodeConverter(new MaterialExpressionLinearInterpolateConverter());
            AddNodeConverter(new MaterialExpressionMultiplyConverter());
            AddNodeConverter(new MaterialExpressionScalarParameterConverter());
            AddNodeConverter(new MaterialExpressionStaticSwitchParameterConverter());
            AddNodeConverter(new MaterialExpressionTextureCoordinateConverter());
            AddNodeConverter(new MaterialExpressionTextureSampleParameter2DConverter());
            AddNodeConverter(new MaterialExpressionVectorParameterConverter());

            AddNodeConverter(new MaterialFunctionBlendAngleCorrectedNormalsConverter());
            AddNodeConverter(new MaterialFunctionCheapContrastConverter());

            AddNodeConverter(new LitRootConverter());
        }

        private void AddNodeConverter(UnrealNodeConverter converter)
        {
            _nodeConverters.Add(converter);
        }

        public GraphData ToGraphData()
        {
            ConvertUnrealNode(_unrealMaterial);

            foreach (var unresolvedExpression in _unrealMaterial.Expressions) {
                var childNode = _unrealMaterial.ResolveExpressionReference(unresolvedExpression);

                if(null == childNode) {
                    // FIXME:
                    Debug.Log("unresolved expression");

                    continue;
                }

                ConvertUnrealNode(childNode);
            }

            return _graph;
        }

        public void ConvertUnrealNode(T3D.Node unrealNode)
        {
            var converter = FindConverterForUnrealNode(unrealNode);
            converter?.Convert(unrealNode, this);
        }

        public UnrealNodeConverter FindConverterForUnrealNode(T3D.Node unrealNode)
        {
            foreach (var converter in _nodeConverters) {
                if(converter.CanConvert(unrealNode)) {
                    return converter;
                }
            }

            return null;
        }

        public void RegisterNodeAndPositionOnGraph(AbstractMaterialNode graphNode, T3D.Node unrealNode)
        {
            AddNodeToGraph(graphNode);
            PositionNodeOnGraph(graphNode, unrealNode);

            _nodeLookupByUnrealNodeName.Add(unrealNode.Name, graphNode);
        }

        public T CreateNode<T>()
            where T : AbstractMaterialNode, new()
        {
            var node = new T();
            AddNodeToGraph(node);

            return node;
        }

        public void AddNodeToGraph(AbstractMaterialNode node)
        {
            if(! _graph.ContainsNodeGuid(node.guid)) {
                _graph.AddNode(node);
            }
        }

        public void AddInputToGraph(ShaderInput input)
        {
            _graph.AddGraphInput(input);
        }

        public void PositionNodeOnGraph(AbstractMaterialNode graphNode, T3D.Node unrealNode)
        {
            var drawState = graphNode.drawState;
            drawState.position = new Rect(new Vector2(unrealNode.EditorX, unrealNode.EditorY), Vector2.zero);
            graphNode.drawState = drawState;
        }

        public void Connect(SlotReference from, SlotReference to)
        {
            _graph.Connect(from, to);
        }

        public delegate void ConfigureShaderPropertyDelegate<T>(T shaderProperty)
            where T : AbstractShaderProperty;

        public T FindOrCreateProperty<T>(string name, ConfigureShaderPropertyDelegate<T> configureShaderProperty)
            where T : AbstractShaderProperty, new()
        {
            T property = null;

            if(! _propertyLookup.ContainsKey(name)) {
                property = new T {
                    displayName = name
                };

                _graph.AddGraphInput(property);

                configureShaderProperty(property);

                var propertyAsGenericType = property as AbstractShaderProperty<object>;
                var lookupEntry = new ShaderInputLookup(name, property, propertyAsGenericType?.value);

                _propertyLookup.Add(name, lookupEntry);
            } else {
                property = (T) _propertyLookup[name].ShaderInput;
            }

            return property;
        }

        public static GraphData FromMaterial(Material material)
        {
            var converter = new ShaderGraphBuilder(material);

            return converter.ToGraphData();
        }

        private class ShaderInputLookup
        {
            public string Name { get; }
            public ShaderInput ShaderInput { get; }
            public object Value { get; }

            public ShaderInputLookup(string name, ShaderInput shaderInput, object value)
            {
                Name = name;
                ShaderInput = shaderInput;
                Value = value;
            }
        }
    }
}
