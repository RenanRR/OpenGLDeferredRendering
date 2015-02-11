using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeferredLightRendering
{
    //Copied from my own OpenGL Framework (by Robot9706)
    public class Shader
    {
        private int _shader;

        private Dictionary<string, int> _uniformCache;
        private Dictionary<string, int> _samplers;
        private Dictionary<string, int> _arraySamplers;

        public class ShaderInputLayout
        {
            private Dictionary<int, string> _layout;

            public ShaderInputLayout()
            {
                _layout = new Dictionary<int, string>();
            }

            public ShaderInputLayout(Dictionary<int, string> data)
            {
                _layout = data;
            }

            public void Bind(int layout, string name)
            {
                _layout.Add(layout, name);
            }

            public void BindLayouts(int shader)
            {
                foreach (KeyValuePair<int, string> pair in _layout)
                {
                    GL.BindAttribLocation(shader, pair.Key, pair.Value);
                }
            }
        }

        public Shader(string vertexFile, string fragmentFile, ShaderInputLayout layout)
        {
            _uniformCache = new Dictionary<string, int>();
            _samplers = new Dictionary<string, int>();
            _arraySamplers = new Dictionary<string, int>();

            string vertexCode = File.ReadAllText(vertexFile);
            string fragmentCode = File.ReadAllText(fragmentFile);

            //Find samplers
            {
                int cSampler = 0;

                string findCode = fragmentCode;

                int i = findCode.IndexOf("uniform sampler2D");
                while (i != -1)
                {
                    findCode = findCode.Substring(i + 18);
                    int nx = findCode.IndexOf(';');

                    string var = findCode.Substring(0, nx);
                    if (var.EndsWith("]")) //Array sampler
                    {
                        nx = findCode.IndexOf('[');

                        string name = var.Substring(0, nx);

                        var = var.Substring(nx, var.Length - nx);
                        var = var.Substring(1, var.Length - 1).Substring(0, var.Length - 2);
                        int num = Convert.ToInt32(var);

                        _arraySamplers.Add(name, num);
                        cSampler += num;
                    }
                    else
                    {
                        _samplers.Add(var, cSampler);
                        cSampler++;
                    }

                    i = findCode.IndexOf("uniform sampler2D");
                }
            }

            int vs = GL.CreateShader(ShaderType.VertexShader);
            int ps = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vs, vertexCode);
            GL.ShaderSource(ps, fragmentCode);

            GL.CompileShader(vs);
            GL.CompileShader(ps);

            string vertexLog = GL.GetShaderInfoLog(vs);
            string fragmentLog = GL.GetShaderInfoLog(ps);

            if (!string.IsNullOrEmpty(vertexLog) || !string.IsNullOrEmpty(fragmentLog))
            {
                MessageBox.Show("Failed to compile shader:\nVertex shader log: " + vertexLog + "\nFragment shader log: " + fragmentLog);
                return;
            }

            _shader = GL.CreateProgram();

            GL.AttachShader(_shader, vs);
            GL.AttachShader(_shader, ps);

            if (layout != null)
                layout.BindLayouts(_shader);

            GL.LinkProgram(_shader);

            GL.DeleteShader(vs);
            GL.DeleteShader(ps);

            string programLog = GL.GetShaderInfoLog(vs);

            if (!string.IsNullOrEmpty(programLog))
            {
                MessageBox.Show("Failed to compile shader: " + programLog);
                return;
            }
        }

        public void Use()
        {
            GL.UseProgram(_shader);
        }

        private int GetUniformLocation(string name)
        {
            if (_uniformCache.ContainsKey(name))
            {
                return _uniformCache[name];
            }
            else
            {
                int pos = GL.GetUniformLocation(_shader, name);
                _uniformCache.Add(name, pos);

                return pos;
            }
        }

        public void SetUniform(string name, Matrix4 matrix)
        {
            GL.UniformMatrix4(GetUniformLocation(name), false, ref matrix);
        }

        public void SetUniform(string name, int i)
        {
            GL.Uniform1(GetUniformLocation(name), i);
        }

        public void SetColorAsVec3(string name, Color4 color)
        {
            GL.Uniform3(GetUniformLocation(name), color.R, color.G, color.B);
        }

        public void SetUniform(string name, Color4 color)
        {
            GL.Uniform4(GetUniformLocation(name), color.R, color.G, color.B, color.A);
        }

        public void SetUniform(string name, float f)
        {
            GL.Uniform1(GetUniformLocation(name), f);
        }

        public void SetUniform(string name, Vector3 v3)
        {
            GL.Uniform3(GetUniformLocation(name), v3.X, v3.Y, v3.Z);
        }

        public void SetUniform(string name, Vector2 v2)
        {
            GL.Uniform2(GetUniformLocation(name), v2);
        }

        public void SetUniform(string name, Vector2[] array)
        {
            float[] floatArray = new float[array.Length * 2];
            for (int x = 0; x < array.Length; x++)
            {
                floatArray[(x * 2)] = array[x].X;
                floatArray[(x * 2) + 1] = array[x].Y;
            }
            GL.Uniform2(GetUniformLocation(name), array.Length, floatArray);
        }

        public void SetTextureUniform(string name, int tex)
        {
            if (_samplers.ContainsKey(name))
            {
                int sampler = _samplers[name];
                GL.ActiveTexture(TextureUnit.Texture0 + sampler);
                GL.BindTexture(TextureTarget.Texture2D, tex);

                GL.Uniform1(GetUniformLocation(name), sampler);
            }
        }

        public void SetTextureUniform(string name, Texture2D tex)
        {
            if (_samplers.ContainsKey(name))
            {
                int sampler = _samplers[name];
                GL.ActiveTexture(TextureUnit.Texture0 + sampler);
                tex.BindAsTexture2D();

                GL.Uniform1(GetUniformLocation(name), sampler);
            }
        }

        public void Destroy(bool destructor)
        {
            if (_shader != 0 && !destructor)
            {
                GL.DeleteProgram(_shader);
                _shader = 0;
            }
        }
    }
}
