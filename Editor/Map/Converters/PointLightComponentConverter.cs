using JollySamurai.UnrealEngine4.T3D;
using JollySamurai.UnrealEngine4.T3D.Map;
using UnityEngine;

namespace JollySamurai.UnrealEngine4.Import.Map.Converters
{
    public class PointLightComponentConverter : ComponentConverter<PointLightComponent>
    {
        protected override void CreateComponent(SceneBuilder builder, PointLightComponent unrealNode, Node parentUnrealNode, GameObject parentGameObject)
        {
            var actorNode = parentUnrealNode as BaseActorNode;
            var containingGameObject = parentGameObject;

            if(actorNode.RootComponentName != unrealNode.Name) {
                containingGameObject = new GameObject(unrealNode.Name);
                containingGameObject.transform.parent = parentGameObject.transform;
            }

            containingGameObject.transform.localPosition = Helper.ConvertUnrealVector3(unrealNode.Location, true);
            containingGameObject.transform.localRotation = Helper.ConvertUnrealRotator(unrealNode.Rotation);
            containingGameObject.transform.localScale = Helper.ConvertUnrealVector3(unrealNode.Scale3D, false);
            containingGameObject.isStatic = unrealNode.Mobility == Mobility.Static;

            var lightComponent = containingGameObject.AddComponent<Light>();
            lightComponent.range = Helper.ScaleUnit(unrealNode.AttenuationRadius);
            lightComponent.intensity = unrealNode.Intensity / 100.0f;
            lightComponent.color = Helper.ConvertUnrealColor(unrealNode.LightColor);

            if(! unrealNode.CastShadows) {
                lightComponent.shadows = LightShadows.None;
            }
        }
    }
}
