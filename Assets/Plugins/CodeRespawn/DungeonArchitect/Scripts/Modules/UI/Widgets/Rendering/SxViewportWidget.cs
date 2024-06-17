//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using System.Collections.Generic;
using DungeonArchitect.SxEngine;
using UnityEngine;

namespace DungeonArchitect.UI.Widgets
{
    public class SxViewportWidget : WidgetBase
    {
        public SxWorld World { get; }
        public SxRenderer Renderer => renderer;
        public float MoveSpeed = 4.0f;
        
        protected SxRenderer renderer;
        protected FrameTime frameTime = new FrameTime();
        protected float pitch = 0;
        protected float yaw = 0;
        protected float OrbitAnglePerPixel = 0.5f;
        protected float ZoomPerScrollUnit = 0.5f;
        
        protected Vector3 targetCamLocation;
            

        private Matrix4x4 lastRenderViewMatrix;
        protected bool renderStateInvalidated = false;
        
        public bool RenderEveryFrame { get; set; } = false;

        
        public SxCamera Camera
        {
            get => renderer != null ? renderer.Camera : null;
        }

        public void LookAt(Vector3 target)
        {
            // Calculate the yaw
            {
                var cameraDirection = Vector3.forward;

                var targetDirection = (targetCamLocation - target).normalized;
                targetDirection.y = 0;
                targetDirection.Normalize();

                yaw = Mathf.Acos(Vector3.Dot(cameraDirection, targetDirection)) * Mathf.Rad2Deg;
                var cross = Vector3.Cross(cameraDirection, targetDirection);
                if (cross.y < 0)
                {
                    yaw = -yaw;
                }
            }
            
            // Calculate the pitch
            {
                var v1 = (target - targetCamLocation).normalized;
                var v2 = v1;
                v2.y = 0;
                v2.Normalize();
                
                pitch = -Mathf.Acos(Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
                if (v1.y > 0)
                {
                    pitch = -pitch;
                }
            }
            
            renderStateInvalidated = true;
        } 
        
        
        protected class FrameTime
        {
            private double lastUpdateTimestamp = 0;

            public float DeltaTime
            {
                get
                {
                    consumed = true;
                    return deltaTime;
                }
                set
                {
                    Debug.Assert(consumed);
                    consumed = false;
                    deltaTime = value;
                }
            }
            private float deltaTime = 0;
            public bool SkipNextFrameTime { get; set; }= false;
            private bool consumed = true;

            public void Tick(double timeSinceStartup)
            {
                double currentTime = timeSinceStartup;

                if (lastUpdateTimestamp > 0)
                {
                    deltaTime = (float)(currentTime - lastUpdateTimestamp);
                    deltaTime = Mathf.Min(0.066f, deltaTime);
                }

                if (SkipNextFrameTime)
                {
                    SkipNextFrameTime = false;
                    deltaTime = 0;
                }

                lastUpdateTimestamp = currentTime;
            }
        }

        public void ResetFrameTimer()
        {
            frameTime.SkipNextFrameTime = true;
        }

        public float AnglePerPixelX { get; set; } = 0.4f;
        public float AnglePerPixelY { get; set; } = 0.4f;
        public float PivotDistance = 10; 

        public float FOV
        {
            get => renderer != null ? renderer.Camera.FOV : 0;
        }

        public float AspectRatio
        {
            get => renderer != null ? renderer.Camera.AspectRatio : 1;
        }

        public SxViewportWidget()
        {
            ShowFocusHighlight = true;

            renderer = new SxRenderer();
            renderer.SetClearState(true, true, Color.white);

            ResetCamera(true);
            
            PivotDistance = targetCamLocation.magnitude;
            
            UpdateCamera();

            World = new SxWorld();
        }
        
        public void Release()
        {
            if (renderer != null)
            {
                renderer.Release();
            }
        }

        public void ResetCamera(bool immediate)
        {
            SetCameraLocation(new Vector3(6, 6, -6), immediate);
            LookAt(Vector3.zero);
            UpdateCamera();
        }
        
        public void SetCameraLocation(Vector3 location, bool immediate)
        {
            targetCamLocation = location;
            if (immediate)
            {
                renderer.Camera.Location = location;
            }

            renderStateInvalidated = true;
        }

        public void SetClearState(bool clearDepth, bool clearColor, Color color)
        {
            renderer.SetClearState(clearDepth, clearColor, color);
        }

        public override bool CanAcquireFocus()
        {
            return true;
        }

        public override bool RequiresInputEveryFrame() { return true; }

        protected override void DrawImpl(UISystem uiSystem, UIRenderer uiRenderer)
        {
            var guiState = new GUIState(uiRenderer);
            var bounds = new Rect(Vector2.zero, WidgetBounds.size);

            if (IsPaintEvent(uiSystem) || RenderEveryFrame)
            {
                uiRenderer.DrawTexture(bounds, renderer.Texture);
            }
            
            guiState.Restore();
        }
        
        public override void UpdateWidget(UISystem uiSystem, Rect bounds)
        {
            base.UpdateWidget(uiSystem, bounds);
            frameTime.Tick(uiSystem.Platform.timeSinceStartup);
            UpdateCamera();
            World.Tick(renderer.CreateRenderContext(), frameTime.DeltaTime);

            if (renderStateInvalidated || IsCameraMoving() || RenderEveryFrame || renderer.Texture == null)
            {
                RenderTexture();
                renderStateInvalidated = false;
            }
        }

        public void Invalidate()
        {
            renderStateInvalidated = true;
        }

        public void RenderTexture()
        {
            renderer.Render(WidgetBounds.size, World);
            lastRenderViewMatrix = Camera.ViewMatrix;
        }

        protected virtual void UpdateCamera()
        {
            var rotPitch = Quaternion.AngleAxis(pitch, Vector3.right);
            var rotYaw = Quaternion.AngleAxis(yaw, Vector3.up);
            renderer.Camera.Rotation = rotYaw * rotPitch;

            float elasticPower = 12.0f;
            float t = frameTime.DeltaTime * elasticPower;
            var currentCamLocation = renderer.Camera.Location;
            currentCamLocation = Vector3.Lerp(currentCamLocation, targetCamLocation, t);
            renderer.Camera.Location = currentCamLocation;
        }
        
        public void FocusCameraOnPoints(Vector3[] points, float radius)
        {
            var rotation = Quaternion.Inverse(Camera.Rotation);
            var sum = Vector3.zero;
            foreach (var point in points)
            {
                sum += point;
            }

            var center = sum / points.Length;

            var bounds = new Bounds();
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var p = rotation * (point - center);
                if (i == 0)
                {
                    bounds.SetMinMax(p, p);
                }
                else
                {
                    bounds.Encapsulate(p);
                }
            }

            float distanceV, distanceH;
            {
                var frustumHeight = bounds.extents.y * 2 + radius * 4;
                distanceV = frustumHeight * 0.5f / Mathf.Tan(FOV * 0.5f * Mathf.Deg2Rad) + bounds.extents.z;
            }
            {
                var frustumWidth = bounds.extents.x * 2 + radius * 4;
                var frustumHeight = frustumWidth / AspectRatio;
                distanceH = frustumHeight * 0.5f / Mathf.Tan(FOV * 0.5f * Mathf.Deg2Rad) + bounds.extents.z;
            }
            var distance = Mathf.Max(distanceV, distanceH);
            var offset = Camera.Rotation * (Vector3.forward * distance * 1.1f);
            var target = center + offset;
            SetCameraLocation(target, false);
            PivotDistance = (center - target).magnitude;
        }

        private bool keyStrafeLeft = false;
        private bool keyStrafeRight = false;
        private bool keyMoveForward = false;
        private bool keyMoveBackward = false;
        private bool keyMoveUp = false;
        private bool keyMoveDown = false;
        private bool keyOrbit = false;

        public bool IsCameraMoving()
        {
            var moveKeyPressed = keyStrafeLeft || keyStrafeRight || keyMoveForward || keyMoveBackward || keyMoveUp || keyMoveDown || keyOrbit;
            if (moveKeyPressed) return true;
            
            // Check if the cam is lagging behind the target location
            var distance = (targetCamLocation - renderer.Camera.Location).magnitude;
            return distance > 0.01f;
        }

        public bool RequiresRepaint()
        {
            return IsCameraMoving();
        }
        
        public override void OnFocus()
        {

        }

        public override void LostFocus()
        {
            keyStrafeLeft = false;
            keyStrafeRight = false;
            keyMoveForward = false;
            keyMoveBackward = false;
            keyMoveUp = false;
            keyMoveDown = false;
            keyOrbit = false;
        }

        public override void HandleInput(Event e, UISystem uiSystem)
        {
            base.HandleInput(e, uiSystem);

            bool isFocused = (uiSystem != null) ? uiSystem.FocusedWidget == this as IWidget : false;
            if (e.isKey)
            {
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.A) keyStrafeLeft = true;
                    if (e.keyCode == KeyCode.D) keyStrafeRight = true;
                    if (e.keyCode == KeyCode.W) keyMoveForward = true;
                    if (e.keyCode == KeyCode.S) keyMoveBackward = true;
                    if (e.keyCode == KeyCode.E) keyMoveUp = true;
                    if (e.keyCode == KeyCode.Q) keyMoveDown = true;
                }
                else if (e.type == EventType.KeyUp)
                {
                    if (e.keyCode == KeyCode.A) keyStrafeLeft = false;
                    if (e.keyCode == KeyCode.D) keyStrafeRight = false;
                    if (e.keyCode == KeyCode.W) keyMoveForward = false;
                    if (e.keyCode == KeyCode.S) keyMoveBackward = false;
                    if (e.keyCode == KeyCode.E) keyMoveUp = false;
                    if (e.keyCode == KeyCode.Q) keyMoveDown = false;
                }
            }

            keyOrbit = e.alt;

            if (e.button == 0 && keyOrbit)
            {
                if (e.type == EventType.MouseDrag)
                {
                    var direction = Camera.Rotation * -Vector3.forward;
                    var pivot = targetCamLocation + direction * PivotDistance;
                    var pivotToCam = targetCamLocation - pivot;
                    var newPivotToCam = Quaternion.AngleAxis(e.delta.x * OrbitAnglePerPixel, Vector3.up) * pivotToCam;

                    {
                        var pitchRotAxis = Vector3.Cross(newPivotToCam.normalized, Vector3.up);
                        var newYawPivotToCam = Quaternion.AngleAxis(e.delta.y * OrbitAnglePerPixel, pitchRotAxis) * newPivotToCam;
                        var dot = Vector3.Dot(newYawPivotToCam.normalized, Vector3.up);
                        if (1 - Mathf.Abs(dot) > 1e-3f)
                        {
                            newPivotToCam = newYawPivotToCam;
                        }
                    }

                    var newCamLocation = pivot + newPivotToCam;
                    SetCameraLocation(newCamLocation, true);
                    LookAt(pivot);
                    UpdateCamera();

                    renderStateInvalidated = true;
                }
            }

            if (e.type == EventType.ScrollWheel)
            {
                var direction = Camera.Rotation * -Vector3.forward;
                var distance = ZoomPerScrollUnit * e.delta.y * -1;
                var newCamLocation = targetCamLocation + direction * distance;
                PivotDistance -= distance;
                PivotDistance = Mathf.Max(1, PivotDistance);
                SetCameraLocation(newCamLocation, false);

                renderStateInvalidated = true;
            }
            
            
            if (e.button == 1)
            {
                if (e.type == EventType.MouseDrag)
                {
                    pitch = Mathf.Clamp(pitch - AnglePerPixelX * e.delta.y, -90, 90);
                    yaw = yaw + AnglePerPixelY * e.delta.x;
                    UpdateCamera();
                    
                    renderStateInvalidated = true;
                }

                float strafeDirection = 0;
                float forwardDirection = 0;
                float verticalDirection = 0;
                if (keyStrafeLeft) strafeDirection = 1;
                if (keyStrafeRight) strafeDirection = -1;
                if (keyMoveForward) forwardDirection = 1;
                if (keyMoveBackward) forwardDirection = -1;
                if (keyMoveUp) verticalDirection = 1;
                if (keyMoveDown) verticalDirection = -1;

                var accumulatedMoveDist = Vector3.zero;
                
                float deltaTime = frameTime.DeltaTime;
                var directionalMoveDist = deltaTime * MoveSpeed;
                if (strafeDirection != 0)
                {
                    var right = Quaternion.AngleAxis(yaw, Vector3.up) * Vector3.right;
                    accumulatedMoveDist += right * (strafeDirection * directionalMoveDist);
                }

                if (forwardDirection != 0)
                {
                    var forward = renderer.Camera.Rotation * -Vector3.forward;
                    accumulatedMoveDist += forward * (forwardDirection * directionalMoveDist);
                }

                if (verticalDirection != 0)
                {
                    accumulatedMoveDist += Vector3.up * (verticalDirection * directionalMoveDist);
                }

                var moveDistance = accumulatedMoveDist.magnitude;
                if (moveDistance > 0)
                {
                    if (moveDistance > directionalMoveDist)
                    {
                        accumulatedMoveDist = accumulatedMoveDist.normalized * directionalMoveDist;
                    }

                    targetCamLocation += accumulatedMoveDist;
                }
            }
        }
    }
}
