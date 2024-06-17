using UnityEngine;

namespace DungeonArchitect.UI
{
    public class MouseDeltaTracker
    {
        private bool mouseButtonDown = false;
        private bool dragging = false;
        private Vector2 trackingStart = Vector2.zero;
        private Vector2 trackingEnd = Vector2.zero;


        public delegate void MouseClickEvent(Event e, UISystem uiSystem);

        public event MouseClickEvent OnLeftClick;
        public event MouseClickEvent OnRightClick;
        public event MouseClickEvent OnDragStart;
        public event MouseClickEvent OnDragEnd;
        public event MouseClickEvent OnDrag;
            
        // Max number of pixels the mouse can move before invoking a click 
        public float ClickMoveDeltaTolerance { get; set; } = 4;

        public void HandleInput(Event e, UISystem uiSystem)
        {
            if (e == null)
            {
                return;
            }

            if (e.type == EventType.MouseDown)
            {
                StartTracking(e);
            }
            else if (e.type == EventType.MouseUp)
            {
                EndTracking(e, uiSystem);
            }
            else if (e.type == EventType.MouseDrag)
            {
                HandleDrag(e, uiSystem);
            }
        }

        private void HandleDrag(Event e, UISystem uiSystem)
        {
            if (!mouseButtonDown || e.button != 0) return;
            if (!dragging)
            {
                var moveDistance = (e.mousePosition - trackingStart).magnitude;
                if (moveDistance > ClickMoveDeltaTolerance)
                {
                    dragging = true;
                    OnDragStart?.Invoke(e, uiSystem);
                }
            }
            else
            {
                OnDrag?.Invoke(e, uiSystem);
            }
        }
        
        private void StartTracking(Event e)
        {
            mouseButtonDown = true;
            trackingStart = e.mousePosition;
        }

        private void EndTracking(Event e, UISystem uiSystem)
        {
            if (!mouseButtonDown)
            {
                // called end tracking without calling start first
                return;
            }
            
            mouseButtonDown = false;
            trackingEnd = e.mousePosition;
            var moveDistance = (trackingEnd - trackingStart).magnitude;
            if (moveDistance <= ClickMoveDeltaTolerance)
            {
                if (e.button == 0)
                {
                    // Invoke left click
                    OnLeftClick?.Invoke(e, uiSystem);
                }
                else if (e.button == 1)
                {
                    // Invoke right click
                    OnRightClick?.Invoke(e, uiSystem);
                }
            }

            if (dragging)
            {
                if (e.button == 0)
                {
                    dragging = false;
                    OnDragEnd?.Invoke(e, uiSystem);
                }
            }
        }
    }
}