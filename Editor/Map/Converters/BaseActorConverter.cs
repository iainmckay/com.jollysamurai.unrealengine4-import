using JollySamurai.UnrealEngine4.T3D.Map;
using UnityEditor;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map.Converters
{
    public abstract class BaseActorConverter<T> : GameObjectConverter<T> where T : BaseActorNode
    {
        protected override GameObject CreateGameObject(SceneBuilder builder, T unrealNode)
        {
            return new GameObject(unrealNode.ActorLabel);
        }
    }
}
