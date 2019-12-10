using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Internal;

namespace JollySamurai.UnrealEngine4.Import.ShaderGraph.Converters.Expressions
{
    public class MaterialExpressionAppendVectorConverter : GenericConverter<MaterialExpressionAppendVector>
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is MaterialExpressionAppendVector;
        }

        protected override AbstractMaterialNode CreateNode(ShaderGraphBuilder builder, MaterialExpressionAppendVector unrealNode)
        {
            return new CombineNode() {
                previewExpanded = false,
            };
        }

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ExpressionReference expressionReference)
        {
            var toSlot = to.FindInputSlot<MaterialSlot>(toSlotId);

            if(toSlot.concreteValueType == ConcreteSlotValueType.Vector1 || toSlot.concreteValueType == ConcreteSlotValueType.Vector2) {
                return 6;
            } else if(toSlot.concreteValueType == ConcreteSlotValueType.Vector3) {
                return 5;
            } else if(toSlot.concreteValueType == ConcreteSlotValueType.Vector4) {
                return 4;
            }

            // FIXME:
            throw new System.Exception("unhandled vector type");
        }

        public override void CreateConnections(MaterialExpressionAppendVector unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            if(unrealNode.A != null) {
                var a = unrealMaterial.ResolveExpressionReference(unrealNode.A);
                var aSlot = builder.FindSlot<MaterialSlot>(a?.Name, unrealNode.Name, 0, unrealNode.A);

                if(aSlot.concreteValueType == ConcreteSlotValueType.Vector4 || aSlot.concreteValueType == ConcreteSlotValueType.Vector3) {
                    builder.Connect(a?.Name, unrealNode.Name, 0, unrealNode.A);
                    builder.Connect(a?.Name, unrealNode.Name, 1, unrealNode.A);
                    builder.Connect(a?.Name, unrealNode.Name, 2, unrealNode.A);
                } else if(aSlot.concreteValueType == ConcreteSlotValueType.Vector2) {
                    builder.Connect(a?.Name, unrealNode.Name, 0, unrealNode.A);
                    builder.Connect(a?.Name, unrealNode.Name, 1, unrealNode.A);
                } else if(aSlot.concreteValueType == ConcreteSlotValueType.Vector1) {
                    builder.Connect(a?.Name, unrealNode.Name, 0, unrealNode.A);
                } else {
                    // FIXME:
                    throw new System.Exception("unhandled vector type");
                }
            }

            if(unrealNode.B != null) {
                var a = unrealMaterial.ResolveExpressionReference(unrealNode.B);
                var aSlot = builder.FindSlot<MaterialSlot>(a?.Name, unrealNode.Name, 0, unrealNode.B);

                if(aSlot.concreteValueType == ConcreteSlotValueType.Vector4 || aSlot.concreteValueType == ConcreteSlotValueType.Vector3) {
                    builder.Connect(a?.Name, unrealNode.Name, 0, unrealNode.B);
                    builder.Connect(a?.Name, unrealNode.Name, 1, unrealNode.B);
                    builder.Connect(a?.Name, unrealNode.Name, 2, unrealNode.B);
                } else if(aSlot.concreteValueType == ConcreteSlotValueType.Vector2) {
                    builder.Connect(a?.Name, unrealNode.Name, 1, unrealNode.B);
                    builder.Connect(a?.Name, unrealNode.Name, 2, unrealNode.B);
                } else if(aSlot.concreteValueType == ConcreteSlotValueType.Vector1) {
                    builder.Connect(a?.Name, unrealNode.Name, 1, unrealNode.B);
                } else {
                    // FIXME:
                    throw new System.Exception("unhandled vector type");
                }
            }
        }
    }
}
