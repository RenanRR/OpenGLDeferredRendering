using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    //INFO:
    //Use WSAD to move camera
    //Use arrows to look
    //F1 to see gbuffer
    class Program
    {
        const int Width = 1280;
        const int Height = 720;

        //Textures & Buffer
        GBuffer _gbuffer;

        Texture2D _crate;
        Texture2D _blub;

        //Geometry stuff
        Cube _cube;
        Sphere _sphere;
        Quad _quad;

        //Shaders
        GeomShader _geomShader;
        PointLightShader _pointShader;
        FlatShader _flatShader;

        //Scene data
        List<Box> _boxes;
        List<PointLight> _pointLights;

        //Other
        Color4 _transparentBlack = new Color4(0, 0, 0, 0);
        FreeLookCamera _camera;
        float lightRot = 0;
        Random _rnd = new Random();

        #region Entry and startup
        [STAThread]
        static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            GraphicsMode gmode = new GraphicsMode(GraphicsMode.Default.ColorFormat, GraphicsMode.Default.Depth, GraphicsMode.Default.Stencil, 0);

            DisplayDevice dd = DisplayDevice.Default;

            NativeWindow nw = new NativeWindow(Width, Height, "Test", GameWindowFlags.Default, gmode, dd);

            GraphicsContext glContext = new GraphicsContext(gmode, nw.WindowInfo, 3, 0, GraphicsContextFlags.Default);
            glContext.LoadAll();

            glContext.MakeCurrent(nw.WindowInfo);

            if (Load())
            {
                nw.Visible = true;
                while (nw.Visible)
                {
                    Frame();

                    glContext.SwapBuffers();

                    Thread.Sleep(1000 / 60); //"60" fps (not really...)

                    nw.ProcessEvents();
                }

                nw.Visible = false;
            }

            Unload();
        }
        #endregion

        #region Load & Unload
        //Load resources
        private bool Load()
        {
            _camera = new FreeLookCamera(Width, Height);
            _camera.Position = new Vector3(0, 1, 0);

            string data = Path.Combine(System.Windows.Forms.Application.StartupPath, "Data") + @"\";

            GL.Enable(EnableCap.CullFace);

            _crate = new Texture2D(data + "crate001.jpg");
            _blub = new Texture2D(data + "blub.png");

            _gbuffer = new GBuffer();
            _gbuffer.Init(Width, Height);

            _cube = new Cube();
            _sphere = new Sphere(1f, 25, 25);
            _quad = new Quad();

            _geomShader = new GeomShader(data);
            _pointShader = new PointLightShader(data);
            _flatShader = new FlatShader(data);

            _boxes = new List<Box>();
            //Setup scene boxes
            _boxes.Add(new Box(new Vector3(0, 1f, 0), new Vector3(0, 0, 0), new Vector3(1, 1f, 1)));
            _boxes.Add(new Box(new Vector3(0, -1f, 0), new Vector3(0, 0, 0), new Vector3(10, 1f, 10)));

            //Setup lights
            _pointLights = new List<PointLight>();
            _pointLights.Add(new PointLight(new Vector3(0, 2.5f, 0), 10, new Vector3(1, 1, 1), 1));

            return true;
        }

        private void Unload()
        {
            _gbuffer.Delete();
            _crate.Delete();
            _blub.Delete();
        }
        #endregion

        #region Helpers
        private Vector3 GetColor(Color4 c)
        {
            return new Vector3(c.R, c.G, c.B);
        }

        private Vector3 RandomVector3()
        {
            return new Vector3((float)_rnd.NextDouble(), (float)_rnd.NextDouble(), (float)_rnd.NextDouble());
        }

        private float RandomFloat()
        {
            return ((float)_rnd.NextDouble() - 0.5f) * 2.0f;
        }
        #endregion

        #region Rendering
        private void Frame()
        {
            lightRot += 0.01f;

            _camera.Update(1f / 60f);

            //Rotate lights around
            //float one = (MathHelper.Pi * 2) / _pointLights.Count;
            //for (int x = 0; x < _pointLights.Count; x++)
            //{
            //    PointLight pl = _pointLights[x];

            //    pl.Center.X = (float)Math.Sin(lightRot + one * x) * 4;
            //    pl.Center.Z = (float)Math.Cos(lightRot + one * x) * 4;
            //}

            //Actual rendering:
            //#1 pass:
            GeomPass();

            //#2 pass:
            LightPass();

            //Blit result to screen
            if (Keyboard.GetState().IsKeyDown(Key.F1))
            {
                _gbuffer.DumpToScreen(Width, Height);
            }
            else
            {
                _gbuffer.BlitResult(Width, Height);
            }
        }

        private void GeomPass()
        {
            _gbuffer.BindForGeometryPass();
            GL.Enable(EnableCap.DepthTest);
            {
                GL.CullFace(CullFaceMode.Back);

                GL.ClearColor(_transparentBlack);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

                _geomShader.Use();

                _geomShader.View = _camera.View;
                _geomShader.Projection = _camera.Projection;
                _geomShader.Texture = _crate;

                foreach (Box b in _boxes)
                {
                    _geomShader.World = Matrix4.CreateScale(b.Scale) * Matrix4.CreateRotationX(b.Rotation.X) * Matrix4.CreateRotationY(b.Rotation.Y) * Matrix4.CreateRotationZ(b.Rotation.Z) * Matrix4.CreateTranslation(b.Position);

                    _cube.Draw();
                }
            }
            GL.Disable(EnableCap.DepthTest);
        }

        private void LightPass()
        {
            _gbuffer.BindForLightPass();
            {
                //Light rendering
                GL.ClearColor(_transparentBlack);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.Enable(EnableCap.Blend);
                GL.BlendEquation(BlendEquationMode.FuncAdd);
                GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.One);

                GL.CullFace(CullFaceMode.Front);

                _pointShader.Use();
                _pointShader.View = _camera.View;
                _pointShader.Projection = _camera.Projection;
                _pointShader.ScreenSize = new Vector2(Width, Height);
                _pointShader.DiffuseBuffer = _gbuffer.DiffuseTexture;
                _pointShader.PositionBuffer = _gbuffer.PositionTexture;
                _pointShader.NormalBuffer = _gbuffer.NormalTexture;

                foreach (PointLight pl in _pointLights)
                {
                    if (pl.Enabled)
                    {
                        _pointShader.World = Matrix4.CreateScale(pl.Radius) * Matrix4.CreateTranslation(pl.Center);
                        _pointShader.Light = pl;

                        _sphere.Draw();
                    }
                }

                //Light blub icon rendering
                GL.Enable(EnableCap.DepthTest);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                _flatShader.Use();
                _flatShader.View = _camera.View;
                _flatShader.Projection = _camera.Projection;
                _flatShader.Texture = _blub;

                foreach (PointLight pl in _pointLights)
                {
                    if (pl.Enabled)
                    {
                        _flatShader.World = Matrix4.CreateScale(0.2f) * CreateBillboard(pl.Center, _camera.Position, new Vector3(0, 1, 0), null);

                        _quad.Draw();
                    }
                }

                GL.Disable(EnableCap.DepthTest);
                GL.Disable(EnableCap.Blend);
            }
            _gbuffer.Restore();
        }

        //From monogame: https://github.com/mono/MonoGame/blob/develop/MonoGame.Framework/Matrix.cs
        private Matrix4 CreateBillboard(Vector3 objectPosition, Vector3 cameraPosition,
           Vector3 cameraUpVector, Vector3? cameraForwardVector)
        {
            Matrix4 result = Matrix4.Identity;

            Vector3 vector;
            Vector3 vector2;
            Vector3 vector3;
            vector.X = objectPosition.X - cameraPosition.X;
            vector.Y = objectPosition.Y - cameraPosition.Y;
            vector.Z = objectPosition.Z - cameraPosition.Z;
            float num = vector.LengthSquared;
            if (num < 0.0001f)
            {
                vector = cameraForwardVector.HasValue ? -cameraForwardVector.Value : new Vector3(0,0,-1);
            }
            else
            {
                Vector3.Multiply(ref vector, (float)(1f / ((float)Math.Sqrt((double)num))), out vector);
            }
            Vector3.Cross(ref cameraUpVector, ref vector, out vector3);
            vector3.Normalize();
            Vector3.Cross(ref vector, ref vector3, out vector2);
            result.M11 = vector3.X;
            result.M12 = vector3.Y;
            result.M13 = vector3.Z;
            result.M14 = 0;
            result.M21 = vector2.X;
            result.M22 = vector2.Y;
            result.M23 = vector2.Z;
            result.M24 = 0;
            result.M31 = vector.X;
            result.M32 = vector.Y;
            result.M33 = vector.Z;
            result.M34 = 0;
            result.M41 = objectPosition.X;
            result.M42 = objectPosition.Y;
            result.M43 = objectPosition.Z;
            result.M44 = 1;

            return result;
        }
        #endregion
    }
}
