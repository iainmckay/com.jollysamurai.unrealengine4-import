using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Map;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map
{
    public abstract class ComponentConverter<T> : MapNodeConverter<T>
        where T : Node
    {
        public override void Convert(Node unrealNode, SceneBuilder builder, Node parentUnrealNode, GameObject parentGameObject)
        {
            var nodeAsT = unrealNode as T;

            if(nodeAsT == null) {
                // FIXME:
                throw new System.Exception("Unexpected node type");
            }

            CreateComponent(builder, nodeAsT, parentUnrealNode, parentGameObject);
        }

        protected abstract void CreateComponent(SceneBuilder builder, T unrealNode, Node parentUnrealNode, GameObject parentGameObject);
    }
}
