using System;
using AB_Utility.FromSceneToEntityConverter;
using Leopotam.EcsLite;
using UnityEngine;

namespace Sources.App.Ecs
{
    public class EcsStartUp : MonoBehaviour
    {
        private IEcsSystems _systems;
        private IEcsSystems _editorSystems;

        private void Awake() 
        {
            _systems = new EcsSystems(new EcsWorld());
            AddEditorSystems();
            AddRunSystems();
            AddOneFrame();
            Inject();
            AddComponentsConverterWorld();
            _systems.Init ();
        }

        private void Update () 
        {
            _systems?.Run ();
            UpdateEditorSystems();
        }

        private void OnDestroy () 
        {
            if (_systems != null) 
            {
                _systems.Destroy ();
                _systems.GetWorld().Destroy();
                _systems = null;
            }
            
            DestroyEditorSystems();
        }

        private void AddRunSystems()
        {
            
        }

        private void AddOneFrame()
        {
        }

        private void Inject()
        {
        }

        private void AddComponentsConverterWorld()
        {
            _systems
                .AddWorld(new EcsWorld(), "events")
                .ConvertScene();
        }

        private void AddEditorSystems()
        {
#if UNITY_EDITOR
            // Создаем отдельную группу для отладочных систем.
            _editorSystems = new EcsSystems (_systems.GetWorld ());
            _editorSystems
                .Add(new Mitfart.LeoECSLite.UnityIntegration.EcsWorldDebugSystem()) 
                .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
                .Init ();
#endif
            //_systems.Add ()
#if UNITY_EDITOR
            _systems.Add(new Leopotam.EcsLite.UnityEditor.EcsSystemsDebugSystem());
#endif
        }

        private void UpdateEditorSystems()
        {
#if UNITY_EDITOR
            _editorSystems?.Run ();
#endif
        }

        private void DestroyEditorSystems()
        {
#if UNITY_EDITOR
            if (_editorSystems != null) 
            {
                _editorSystems.Destroy ();
                _editorSystems = null;
            }
#endif
        }
    }
}