using System.Collections.Generic;
using JollySamurai.UnrealEngine4.Import.Map.Converters;
using JollySamurai.UnrealEngine4.T3D;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JollySamurai.UnrealEngine4.Import.Map
{
    public class SceneBuilder
    {
        private readonly T3D.Map.Map _unrealMap;

        private readonly Dictionary<string, GameObject> _nodeLookupByUnrealNodeName;
        private readonly List<MapNodeConverter> _converters;

        private SceneBuilder(T3D.Map.Map unrealMap)
        {
            _unrealMap = unrealMap;
            _nodeLookupByUnrealNodeName = new Dictionary<string, GameObject>();
            _converters = new List<MapNodeConverter>();

            AddBuiltinConverters();
        }

        private void AddBuiltinConverters()
        {
            AddConverter(new PointLightActorConverter());
            AddConverter(new SpotLightActorConverter());
            AddConverter(new StaticMeshActorConverter());

            AddConverter(new PointLightComponentConverter());
            AddConverter(new SpotLightComponentConverter());
            AddConverter(new StaticMeshComponentConverter());
        }

        private void AddConverter(MapNodeConverter converter)
        {
            _converters.Add(converter);
        }

        private Scene OutputScene(string destination)
        {
            if(_unrealMap.LevelCount > 1) {
                throw new System.Exception("Maps with more than one level is not supported");
            }

            var level = _unrealMap.Levels[0];

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            foreach (var node in level.Children) {
                ConvertUnrealNode(node);
            }

            EditorSceneManager.SaveScene(scene, destination);
            EditorSceneManager.CloseScene(scene, true);

            return scene;
        }

        public void ConvertUnrealNode(Node unrealNode)
        {
            var converter = FindConverterForUnrealNode(unrealNode);
            converter?.Convert(unrealNode, this, null, null);
        }

        public MapNodeConverter FindConverterForUnrealNode(Node unrealNode)
        {
            foreach (var converter in _converters) {
                if(converter.CanConvert(unrealNode)) {
                    return converter;
                }
            }

            return null;
        }

        public GameObject FindGameObjectByUnrealName(string unrealName)
        {
            if(string.IsNullOrEmpty(unrealName)) {
                return null;
            }

            if(_nodeLookupByUnrealNodeName.ContainsKey(unrealName)) {
                return _nodeLookupByUnrealNodeName[unrealName];
            }

            return null;
        }

        public static void OutputFromMap(T3D.Map.Map map, string sceneDestination)
        {
            var converter = new SceneBuilder(map);
            converter.OutputScene(sceneDestination);
        }
    }
}
