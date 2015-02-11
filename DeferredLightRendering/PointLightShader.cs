using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    class PointLightShader : Shader
    {
        public Matrix4 View
        {
            set { SetUniform("View", value); }
        }

        public Matrix4 Projection
        {
            set { SetUniform("Proj", value); }
        }

        public Matrix4 World
        {
            set { SetUniform("World", value); }
        }

        public Vector2 ScreenSize
        {
            set { SetUniform("ScreenSize", value); }
        }

        public int DiffuseBuffer
        {
            set { SetTextureUniform("ColorBuffer", value); }
        }

        public int PositionBuffer
        {
            set { SetTextureUniform("PositionBuffer", value); }
        }

        public int NormalBuffer
        {
            set { SetTextureUniform("NormalBuffer", value); }
        }

        public PointLight Light
        {
            set
            {
                SetUniform("LightCenter", value.Center);
                SetUniform("LightRadius", value.Radius);
                SetUniform("LightColor", value.Color);
                SetUniform("LightIntensity", value.Intensity);
            }
        }

        static Dictionary<int, string> _data = new Dictionary<int,string>(){
            {0, "Position"}
        };

        public PointLightShader(string data)
            : base(data + "PointLight.vs", data + "PointLight.fs", new ShaderInputLayout(_data))
        { 
        }
    }
}
