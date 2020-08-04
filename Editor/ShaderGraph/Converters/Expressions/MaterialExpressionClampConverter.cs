using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionClampConverter : GenericConverter<MaterialExpressionClamp>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionClamp;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionClamp unrealNode)
        {
            return new ClampNode() {
                previewExpanded = false,
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            return 3;
        }

        public override void CreateConnections(MaterialExpressionClamp unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            builder.Connect(unrealNode.Input, unrealNode.Name, 0);

            if(unrealNode.Min != null) {
                builder.Connect(unrealNode.Min, unrealNode.Name, 1);
            }

            if(unrealNode.Max != null) {
                builder.Connect(unrealNode.Max, unrealNode.Name, 2);
            }
        }
    }
}
