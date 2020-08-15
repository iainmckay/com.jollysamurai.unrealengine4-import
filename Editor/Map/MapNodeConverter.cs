using JollySamurai.UnrealEngine4.T3D;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map
{
    public abstract class MapNodeConverter<T> : MapNodeConverter
        where T : Node
    {
        public override bool CanConvert(Node unrealNode)
        {
            return unrealNode is T;
        }
    }

    public abstract class MapNodeConverter
    {
        public abstract bool CanConvert(Node unrealNode);
        public abstract void Convert(Node unrealNode, SceneBuilder builder, Node parentUnrealNode, GameObject parentGameObject);
    }
}
