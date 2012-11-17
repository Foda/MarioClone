using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//http://www.david-amador.com/2009/10/xna-camera-2d-with-zoom-and-rotation/
namespace SFML_Test
{
    class Camera
    {
        protected float         _zoom; // Camera Zoom
        public Matrix           _transform; // Matrix Transform
        public Vector2          _pos; // Camera Position
        protected float         _rotation; // Camera Rotation

        public Vector2 plyPos;
        public bool isLeft = true;
        public bool XStick = false;
        public bool YStick = true;

        private bool isPanning = false;
        private int panDir = 0;

        public Vector2 moveOffset;

        public Camera()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        // Sets and gets zoom
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }
        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public void Update(GameTime time)
        {
            if (XStick == false)
            {
                _pos.X = plyPos.X + moveOffset.X;
            }
            if (YStick == false)
            {
                _pos.Y = plyPos.Y + moveOffset.Y;
            }
           
            if (isPanning)
            {
                if (isLeft)
                {
                    moveOffset.X -= 1;
                    panDir++;
                }
                else
                {
                    moveOffset.X += 1;
                    panDir--;
                }

                if (panDir == 0)
                    isPanning = false;
            }

            if (_pos.X < 350)
                _pos.X = 350;
        }

        public void PanLeft()
        {
            if (isLeft == false)
            {
                isLeft = true;
                isPanning = true;
                panDir = -32;
            }
        }

        public void PanRight()
        {
            if (isLeft == true)
            {
                isLeft = false;
                isPanning = true;
                panDir = 32;
            }
        }

        public Matrix GetTransformation(GraphicsDevice graphicsDevice)
        {
            _transform =       // Thanks to o KB o for this solution
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(640 * 0.5f, 480 * 0.5f, 0));
            return _transform;
        }
    }
}
