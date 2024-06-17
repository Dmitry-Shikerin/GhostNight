

using System;
using UnityEngine;

namespace DungeonArchitect.UI.Windows
{
    public delegate void UIWindowRepaintDelegate();
    
    public interface IUIWindowImpl
    {
        void OnEnable();
        void OnDisable();
        void OnDestroy();
        
        void Awake();
        void Update();
        
        void CreateGUI();
        void OnGUI();
        void OnInspectorUpdate();
        void Repaint();
        
        Vector4 position { get; }
        string titleContent { get; set; }
        bool wantsMouseMove { get; set; }

        UIWindowRepaintDelegate OnRepaint { get; }
    }

    public interface IUIWindow<out TImpl> where TImpl : IUIWindowImpl
    {
        TImpl Impl { get; }

        void Repaint();
    }
}