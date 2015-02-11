using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    //Copied from my own OpenGL Framework (by Robot9706)
    class FreeLookCamera
    {
        private Vector3 _baseForward = new Vector3(0, 0, -1);
        private Vector3 _baseUp = new Vector3(0, 1, 0);
        private Vector3 _baseRight = new Vector3(1, 0, 0);

        private Vector3 _rotation = new Vector3(0, 0, 0);
        private Vector3 _position;

        private float _moveSpeed = 3;
        private float _rotationSpeed = 2;

        private Matrix4 _view;
        private Matrix4 _proj;

        public Matrix4 View
        {
            get { return _view; }
        }

        public Matrix4 Projection
        {
            get { return _proj; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public FreeLookCamera(int width, int height)
        {
            float asp = (float)width / (float)height;

            _proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2 - 0.001f, asp, 0.1f, 100f);
        }

        public void Update(float time)
        {
            Matrix4 rotMatrix = Matrix4.CreateRotationX(_rotation.X) * Matrix4.CreateRotationY(_rotation.Y) * Matrix4.CreateRotationZ(_rotation.Z);

            Vector3 target = Vector3.TransformNormal(_baseForward, rotMatrix);
            Vector3 up = Vector3.TransformNormal(_baseUp, rotMatrix);
            Vector3 right = Vector3.TransformNormal(_baseRight, rotMatrix);

            KeyboardState ks = Keyboard.GetState();
            float moveSpeed = _moveSpeed;
            if (ks.IsKeyDown(Key.ShiftLeft))
                moveSpeed = _moveSpeed * 10;

            if (ks.IsKeyDown(Key.W))
                _position += target * time * moveSpeed;
            if (ks.IsKeyDown(Key.S))
                _position -= target * time * moveSpeed;
            if (ks.IsKeyDown(Key.D))
                _position += right * time * moveSpeed;
            if (ks.IsKeyDown(Key.A))
                _position -= right * time * moveSpeed;

            if (ks.IsKeyDown(Key.Right))
                _rotation.Y -= time * _rotationSpeed;
            if (ks.IsKeyDown(Key.Left))
                _rotation.Y += time * _rotationSpeed;
            if (ks.IsKeyDown(Key.Up))
                _rotation.X += time * _rotationSpeed;
            if (ks.IsKeyDown(Key.Down))
                _rotation.X -= time * _rotationSpeed;

            _view = Matrix4.LookAt(_position, _position + target, up);
        }
    }
}
