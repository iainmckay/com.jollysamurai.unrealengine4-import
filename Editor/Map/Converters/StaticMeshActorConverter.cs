using JollySamurai.UnrealEngine4.T3D.Map;
using UnityEditor;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map.Converters
{
    public class StaticMeshActorConverter : GameObjectConverter<StaticMeshActor>
    {
        protected override GameObject CreateGameObject(SceneBuilder builder, StaticMeshActor unrealNode)
        {
            var gameObject = CreateGameObjectImpl(builder, unrealNode.StaticMeshComponent.StaticMesh.FileName);
            gameObject.name = unrealNode.ActorLabel;

            return gameObject;
        }

        public static GameObject CreateGameObjectImpl(SceneBuilder builder, string unrealMeshFile)
        {
            var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(Helper.GetMeshPath(unrealMeshFile));
            gameObject = Object.Instantiate(gameObject);

            return gameObject;
        }
    }
}
