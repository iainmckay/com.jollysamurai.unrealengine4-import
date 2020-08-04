using System;
using System.Collections.Generic;
using JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions;
using JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Functions;
using JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Roots;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.Graphing;
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

        private readonly Dictionary<Guid, Rect> _groupDimensions;

        private ShaderGraphBuilder(Material unrealMaterial)
        {
            _unrealMaterial = unrealMaterial;
            _graph = new GraphData();
            _propertyLookup = new Dictionary<string, ShaderInputLookup>();
            _nodeLookupByUnrealNodeName = new Dictionary<string, AbstractMaterialNode>();
            _nodeConverters = new List<UnrealNodeConverter>();
            _groupDimensions = new Dictionary<Guid, Rect>();

            AddBuiltinConverters();
        }

        private void AddBuiltinConverters()
        {
            AddNodeConverter(new MaterialExpressionAddConverter());
            AddNodeConverter(new MaterialExpressionAppendVectorConverter());
            AddNodeConverter(new MaterialExpressionClampConverter());
            AddNodeConverter(new MaterialExpressionCommentConverter());
            AddNodeConverter(new MaterialExpressionConstantConverter());
            AddNodeConverter(new MaterialExpressionConstant2VectorConverter());
            AddNodeConverter(new MaterialExpressionConstant3VectorConverter());
            AddNodeConverter(new MaterialExpressionConstant4VectorConverter());
            AddNodeConverter(new MaterialExpressionDesaturationConverter());
            AddNodeConverter(new MaterialExpressionDivideConverter());
            AddNodeConverter(new MaterialExpressionFresnelConverter());
            AddNodeConverter(new MaterialExpressionLinearInterpolateConverter());
            AddNodeConverter(new MaterialExpressionMultiplyConverter());
            AddNodeConverter(new MaterialExpressionOneMinusConverter());
            AddNodeConverter(new MaterialExpressionScalarParameterConverter());
            AddNodeConverter(new MaterialExpressionStaticSwitchParameterConverter());
            AddNodeConverter(new MaterialExpressionSubtractConverter());
            AddNodeConverter(new MaterialExpressionTextureCoordinateConverter());
            AddNodeConverter(new MaterialExpressionTextureObjectParameterConverter());
            AddNodeConverter(new MaterialExpressionTextureSampleConverter());
            AddNodeConverter(new MaterialExpressionTextureSampleParameter2DConverter());
            AddNodeConverter(new MaterialExpressionVectorParameterConverter());

            AddNodeConverter(new MaterialFunctionBlendAngleCorrectedNormalsConverter());
            AddNodeConverter(new MaterialFunctionCheapContrastConverter());

            AddNodeConverter(new LitRootConverter());
            AddNodeConverter(new UnlitRootConverter());
        }

        private void AddNodeConverter(UnrealNodeConverter converter)
        {
            _nodeConverters.Add(converter);
        }

        public GraphData ToGraphData()
        {
            ConvertUnrealNode(_unrealMaterial);

            IterateOverExpressionReferences(_unrealMaterial.EditorComments, ConvertUnrealNode);
            IterateOverExpressionReferences(_unrealMaterial.Expressions, ConvertUnrealNode);
            IterateOverExpressionReferences(_unrealMaterial.Expressions, ConnectUnrealNode);

            ConnectUnrealNode(_unrealMaterial);

            AttachToGroups(_unrealMaterial);

            return _graph;
        }

        private delegate void ExpressionReferenceCallback(T3D.Node node);

        private void IterateOverExpressionReferences(ExpressionReference[] references, ExpressionReferenceCallback callback)
        {
            if(null == references) {
                return;
            }

            foreach (var unresolvedExpression in references) {
                var childNode = _unrealMaterial.ResolveExpressionReference(unresolvedExpression);

                if(null == childNode) {
                    // FIXME:
                    Debug.Log("FIXME: unresolved expression");

                    continue;
                }

                callback(childNode);
            }
        }

        public void ConvertUnrealNode(T3D.Node unrealNode)
        {
            var converter = FindConverterForUnrealNode(unrealNode);
            converter?.Convert(unrealNode, this);
        }

        public void ConnectUnrealNode(T3D.Node unrealNode)
        {
            var converter = FindConverterForUnrealNode(unrealNode);
            converter?.CreateConnections(unrealNode, _unrealMaterial, this);
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

        public AbstractMaterialNode FindNodeByUnrealName(string unrealName)
        {
            if(string.IsNullOrEmpty(unrealName)) {
                return null;
            }

            if(_nodeLookupByUnrealNodeName.ContainsKey(unrealName)) {
                return _nodeLookupByUnrealNodeName[unrealName];
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

        public void AddGroup(string text, Vector2 position, Vector2 size)
        {
            var groupData = new GroupData(text, position);

            _graph.CreateGroup(groupData);
            _groupDimensions.Add(groupData.guid, Rect.MinMaxRect(position.x, position.y, position.x + size.x, position.y + size.y));
        }

        public void Connect(SlotReference from, SlotReference to)
        {
            _graph.Connect(from, to);
        }

        public void Connect(ParsedPropertyBag propertyBag, string toUnrealNodeName, int toSlotId)
        {
            if(null == propertyBag) {
                return;
            }

            if(! propertyBag.HasProperty("Expression")) {
                return;
            }

            var expressionValue = propertyBag.FindPropertyValue("Expression");
            var expression = ValueUtil.ParseExpressionReference(expressionValue);

            Connect(expression.NodeName, toUnrealNodeName, toSlotId, propertyBag);
        }

        public void Connect(string fromUnrealNodeName, string toUnrealNodeName, int toSlotId, ParsedPropertyBag propertyBag)
        {
            var fromNode = FindNodeByUnrealName(fromUnrealNodeName);
            var toNode = FindNodeByUnrealName(toUnrealNodeName);
            var fromSlotId = FindSlotId(fromUnrealNodeName, toUnrealNodeName, toSlotId, propertyBag);

            if(fromNode != null && toNode != null && fromSlotId != -1) {
                Connect(fromNode.GetSlotReference(fromSlotId), toNode.GetSlotReference(toSlotId));
            }
        }

        public delegate void ConfigureShaderPropertyDelegate<T>(T shaderProperty)
            where T : AbstractShaderProperty;

        public T FindOrCreateProperty<T>(string name, ConfigureShaderPropertyDelegate<T> configureShaderProperty)
            where T : AbstractShaderProperty, new()
        {
            T property = null;

            if(! _propertyLookup.ContainsKey(name)) {
                property = new T {
                    displayName = name,
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

        public int FindSlotId(string fromUnrealName, string toUnrealName, int toSlotId, ParsedPropertyBag propertyBag)
        {
            var fromUnrealNode = _unrealMaterial.FindChildByName(fromUnrealName);
            var fromNode = FindNodeByUnrealName(fromUnrealName);
            var toNode = FindNodeByUnrealName(toUnrealName);

            if(fromUnrealNode == null || fromNode == null || toNode == null) {
                return -1;
            }

            foreach (var converter in _nodeConverters) {
                if(converter.CanConvert(fromUnrealNode)) {
                    return converter.GetConnectionSlotId(fromNode, toNode, toSlotId, propertyBag);
                }
            }

            return -1;
        }

        public T FindSlot<T>(string fromUnrealName, string toUnrealName, int toSlotId, ParsedPropertyBag propertyBag)
            where T : ISlot
        {
            var slotId = FindSlotId(fromUnrealName, toUnrealName, toSlotId, propertyBag);
            var fromNode = FindNodeByUnrealName(fromUnrealName);

            if(slotId == -1) {
                return default;
            }

            return fromNode.FindSlot<T>(slotId);
        }

        public static GraphData FromMaterial(Material material)
        {
            var converter = new ShaderGraphBuilder(material);

            return converter.ToGraphData();
        }

        private void AttachToGroups(Material unrealMaterial)
        {
            foreach (var abstractMaterialNode in _graph.addedNodes) {
                foreach (var kvp in _groupDimensions) {
                    if(abstractMaterialNode.drawState.position.Overlaps(kvp.Value)) {
                        abstractMaterialNode.groupGuid = kvp.Key;

                        break;
                    }
                }
            }
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
