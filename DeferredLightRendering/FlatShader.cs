using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    class FlatShader : Shader
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

        public Texture2D Texture
        {
            set { SetTextureUniform("Texture", value); }
        }

        static Dictionary<int,string> _data = new Dictionary<int,string>(){
            {0,"Position"},
            {1, "Tex"},
        };

        public FlatShader(string data)
            : base(data + "FlatShader.vs", data + "FlatShader.fs", new ShaderInputLayout(_data))
        { 
        }
    }
}
