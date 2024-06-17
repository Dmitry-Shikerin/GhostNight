using System;
using System.Collections.Generic;
using DungeonArchitect.UI.Windows;
using UnityEditor;
using UnityEngine;

namespace DungeonArchitect.UI.Impl.UnityEditor
{
    public class UnityEditorUIWindow<TImpl> : EditorWindow, IUIWindow<TImpl> where TImpl : IUIWindowImpl, new()
    {
        public TImpl Impl { get; private set; }

        public UnityEditorUIWindow()
        {
            Impl = new TImpl();
        }
        
        private void OnEnable()
        {
            if (_instance != null)
            {
                _instance.OnEnable();
            }
        }

        private void OnDisable()
        {
            if (_instance != null)
            {
                _instance.OnDisable();
            }
        }

        private void OnDestroy()
        {
            if (_instance != null)
            {
                _instance.OnDestroy();
            }
        }
        
        private void CreateGUI()
        {
            if (_instance != null)
            {
                _instance.CreateGUI();
            }
        }

        private void OnGUI()
        {
            if (_instance != null)
            {
                _instance.OnGUI();
            }
        }

        private void Update()
        {
            if (_instance != null)
            {
                _instance.Update();
            }
        }

        public static UnityEditorUIWindow<TImpl> Get()
        {
            _instance = EditorWindow.GetWindow<UnityEditorUIWindow<TImpl>>();
            return _instance;
        }

        private static UnityEditorUIWindow<TImpl> _instance;
    }

}