using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    class FullscreenQuad
    { 
        public const int PositionAttrib = 0;

        public const int TexAttrib = 1;
        public const int TexOffset = sizeof(float) * 3;

        public const int Stride = sizeof(float) * 5;

        int _vbo, _ibo;
        int _vao;

        int _li;

        public FullscreenQuad()
        {
            float[] vData = new float[]
            {
                //front
                -1.0f, -1.0f,  0.0f, 0, 0,
                 1.0f, -1.0f,  0.0f, 1, 0,
                 1.0f,  1.0f,  0.0f, 1, 1,
                -1.0f,  1.0f,  0.0f, 0, 1
            };

            int[] iData = new int[]
            {
                0, 1, 2, 2, 3, 0,
            };
            _li = iData.Length;

            _vbo = GL.GenBuffer();
            _ibo = GL.GenBuffer();

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vData.Length * sizeof(float)), vData, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(PositionAttrib); //Position
            GL.VertexAttribPointer(PositionAttrib, 3, VertexAttribPointerType.Float, false, Stride, 0);

            GL.EnableVertexAttribArray(TexAttrib); //TexCoord
            GL.VertexAttribPointer(TexAttrib, 2, VertexAttribPointerType.Float, false, Stride, TexOffset);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(iData.Length * sizeof(Int32)), iData, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _li, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}
