using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    class FinalCombineShader : Shader
    {
        static Dictionary<int, string> _data = new Dictionary<int, string>(){
            {0,"Position"},
            {1, "Tex"},
        };

        public int ColorBuffer
        {
            set { SetTextureUniform("ColorBuffer", value); }
        }

        public int LightBuffer
        {
            set { SetTextureUniform("LightBuffer", value); }
        }

        public Color4 AmbientColor
        {
            set { SetUniform("AmbientColor", new Vector3(value.R, value.G, value.B)); }
        }

        public float AmbientPower
        {
            set { SetUniform("AmbientPower", value); }
        }

        public FinalCombineShader(string data)
            : base(data + "FinalCombine.vs", data + "FinalCombine.fs", new ShaderInputLayout(_data))
        { 
        }
    }
}
