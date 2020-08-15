using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Map;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map
{
    public abstract class GameObjectConverter<T> : MapNodeConverter<T>
        where T : Node
    {
        public override void Convert(Node unrealNode, SceneBuilder builder, Node parentUnrealNode, GameObject parentGameObject)
        {
            var nodeAsT = unrealNode as T;

            if(nodeAsT == null) {
                // FIXME:
                throw new System.Exception("Unexpected node type");
            }

            var gameObject = CreateGameObject(builder, nodeAsT);

            if(null == gameObject) {
                return;
            }

            foreach (var childNode in unrealNode.Children) {
                var converter = builder.FindConverterForUnrealNode(childNode);
                converter?.Convert(childNode, builder, unrealNode, gameObject);
            }
        }

        protected virtual GameObject CreateGameObject(SceneBuilder builder, T unrealNode)
        {
            return new GameObject(unrealNode.Name);
        }
    }
}
