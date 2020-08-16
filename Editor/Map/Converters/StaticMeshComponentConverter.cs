using System.Linq;
using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Map;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map.Converters
{
    public class StaticMeshComponentConverter : ComponentConverter<StaticMeshComponent>
    {
        protected override void CreateComponent(SceneBuilder builder, StaticMeshComponent unrealNode, Node parentUnrealNode, GameObject parentGameObject)
        {
            var actorNode = parentUnrealNode as BaseActorNode;
            var containingGameObject = parentGameObject;

            if(actorNode.RootComponentName != unrealNode.Name) {
                containingGameObject = StaticMeshActorConverter.CreateGameObjectImpl(builder, unrealNode.StaticMesh.FileName);
                containingGameObject.name = unrealNode.Name;
                containingGameObject.transform.parent = parentGameObject.transform;
            }

            containingGameObject.transform.localPosition = Helper.ConvertUnrealVector3(unrealNode.Location, true);
            containingGameObject.transform.localRotation = Helper.ConvertUnrealRotator(unrealNode.Rotation);

            containingGameObject.transform.localScale = Helper.ConvertUnrealVector3(unrealNode.Scale3D, false);

            var meshRenderer = containingGameObject.GetComponent<MeshRenderer>();

            var overrideMaterials = unrealNode.OverrideMaterials
                .Where(reference => "None" != reference.FileName)
                .Select(reference => Helper.LoadMaterial(reference.FileName))
                .ToArray();

            if(overrideMaterials.Length > 0) {
                meshRenderer.materials = overrideMaterials;
            }
        }
    }
}
