using System;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Material;
using JollySamurai.UnrealEngine4.T3D.Parser;
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

        public override int GetConnectionSlotId(AbstractMaterialNode from, AbstractMaterialNode to, int toSlotId, ParsedPropertyBag propertyBag)
        {
            var toSlot = to.FindInputSlot<MaterialSlot>(toSlotId);

            if(toSlot.concreteValueType == ConcreteSlotValueType.Vector1 || toSlot.concreteValueType == ConcreteSlotValueType.Vector2) {
                return 6;
            } else if(toSlot.concreteValueType == ConcreteSlotValueType.Vector3) {
                return 5;
            } else if(toSlot.concreteValueType == ConcreteSlotValueType.Vector4) {
                return 4;
            }

            throw new System.Exception("FIXME: unhandled vector type");
        }

        public override void CreateConnections(MaterialExpressionAppendVector unrealNode, Material unrealMaterial, ShaderGraphBuilder builder)
        {
            if(unrealNode.A != null) {
                var a = unrealMaterial.ResolveExpressionReference(
                    ValueUtil.ParseExpressionReference(unrealNode.A.FindPropertyValue("Expression"))
                );
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
                    throw new System.Exception("FIXME: unhandled vector type");
                }
            }

            if(unrealNode.B != null) {
                var b = unrealMaterial.ResolveExpressionReference(
                    ValueUtil.ParseExpressionReference(unrealNode.B.FindPropertyValue("Expression"))
                );
                var bSlot = builder.FindSlot<MaterialSlot>(b?.Name, unrealNode.Name, 0, unrealNode.B);

                if(bSlot.concreteValueType == ConcreteSlotValueType.Vector4 || bSlot.concreteValueType == ConcreteSlotValueType.Vector3) {
                    builder.Connect(b?.Name, unrealNode.Name, 0, unrealNode.B);
                    builder.Connect(b?.Name, unrealNode.Name, 1, unrealNode.B);
                    builder.Connect(b?.Name, unrealNode.Name, 2, unrealNode.B);
                } else if(bSlot.concreteValueType == ConcreteSlotValueType.Vector2) {
                    builder.Connect(b?.Name, unrealNode.Name, 1, unrealNode.B);
                    builder.Connect(b?.Name, unrealNode.Name, 2, unrealNode.B);
                } else if(bSlot.concreteValueType == ConcreteSlotValueType.Vector1) {
                    builder.Connect(b?.Name, unrealNode.Name, 1, unrealNode.B);
                } else {
                    throw new System.Exception("FIXME: unhandled vector type");
                }
            }
        }
    }
}
