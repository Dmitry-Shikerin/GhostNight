//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;

namespace DungeonArchitect.SxEngine
{
    public class SxCamera
    {
        public Vector3 Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    viewMatrixDirty = true;
                }
            } 
        }
        public Quaternion Rotation
        {
            get => rotation;
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    viewMatrixDirty = true;
                }
            } 
        }

        public float FOV
        {
            get => fov;
            set
            {
                fov = value;
                projMatrixDirty = true;
            }
        }

        public float AspectRatio { get; private set; } = 1.0f;
        public Vector2 ScreenSize { get; private set; } = Vector2.one;

        public void SetAspectRatio(float width, float height)
        {
            AspectRatio = width / height;
            ScreenSize = new Vector2(width, height);
            projMatrixDirty = true;
        }
        
        public Vector3 GetRightVector()
        {
            var axisZ = Rotation * Vector3.forward;
            return Vector3.Cross(Vector3.up, axisZ);
        }

        public Matrix4x4 ViewMatrix
        {
            get
            {
                if (viewMatrixDirty)
                {
                    BuildViewMatrix();
                }

                return viewMatrix;
            }
        }

        public Matrix4x4 ViewMatrixInverse => viewMatrixInverse;

        public Matrix4x4 ProjectionMatrix
        {
            get
            {
                if (projMatrixDirty)
                {
                    BuildProjectionMatrix();
                }

                return projMatrix;
            }
        }

        public Matrix4x4 ProjectionMatrixInverse => projMatrixInverse;

        private Vector3 location = Vector3.zero;
        private Quaternion rotation = Quaternion.identity;
        private Matrix4x4 viewMatrix = Matrix4x4.identity;
        private Matrix4x4 viewMatrixInverse = Matrix4x4.identity;
        private Matrix4x4 projMatrix = Matrix4x4.identity;
        private Matrix4x4 projMatrixInverse = Matrix4x4.identity;
        
        private float fov = 75;
        private bool viewMatrixDirty = true;
        private bool projMatrixDirty = true;

        public void LookAt(Vector3 target)
        {
            Rotation = Quaternion.LookRotation((location - target).normalized);
        }

        void BuildViewMatrix()
        {
            var baseTransform = new Matrix4x4(
                new Vector4(-1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1));
            var camTransform = Matrix4x4.TRS(location, rotation, Vector3.one) * baseTransform;
            viewMatrix = camTransform.inverse;
            viewMatrixInverse = viewMatrix.inverse;
            viewMatrixDirty = false;
        }

        public Ray ScreenToRay(Vector2 screenPosition)
        {
            // https://antongerdelan.net/opengl/raycasting.html
            screenPosition.y = ScreenSize.y - screenPosition.y;
            var rayNdc = screenPosition / ScreenSize * 2 - Vector2.one;
            var rayClip = new Vector4(rayNdc.x, rayNdc.y, -1, 1);
            var rayEye = projMatrixInverse * rayClip;
            rayEye.z = -1;
            rayEye.w = 0;

            Vector3 rayWorldDir = viewMatrixInverse * rayEye;
            rayWorldDir.Normalize();

            return new Ray(location, rayWorldDir);
        }
        
        void BuildProjectionMatrix()
        {
            projMatrix = Matrix4x4.Perspective(FOV, AspectRatio, 0.1f, 100.0f);
            projMatrixInverse = projMatrix.inverse;
        }
    }
}