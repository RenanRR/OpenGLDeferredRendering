
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace DeferredLightRendering
{
    public class Cube
    {
        public const int PositionAttrib = 0;
        
        public const int TexAttrib = 1;
        public const int TexOffset = sizeof(float) * 3;

        public const int NormAttrib = 2;
        public const int NormOffset = sizeof(float) * 5;

        public const int Stride = sizeof(float) * 8;

        int _vbo, _ibo;
        int _vao;
        int _li;

        public Cube()
        {
            float[] vData = new float[]
            {
                //front
                -1.0f, -1.0f,  1.0f, 0, 0, 0, 0, 1, //0 (0)
                 1.0f, -1.0f,  1.0f, 1, 0, 0, 0, 1, //1 (1)
                 1.0f,  1.0f,  1.0f, 1, 1, 0, 0, 1, //2 (2)
                -1.0f,  1.0f,  1.0f, 0, 1, 0, 0, 1, //3 (3)

                //top
                -1.0f,  1.0f,  1.0f, 0, 1, 0, 1, 0, //0 -> 4 (3)
                 1.0f,  1.0f,  1.0f, 1, 1, 0, 1, 0, //1 -> 5 (2)
                 1.0f,  1.0f, -1.0f, 1, 0, 0, 1, 0, //2 -> 6 (6)
                -1.0f,  1.0f, -1.0f, 0, 0, 0, 1, 0, //3 -> 7 (7)

                //back
                -1.0f,  1.0f, -1.0f, 0, 1, 0, 0, -1, //0 -> 8 (7)
                 1.0f,  1.0f, -1.0f, 1, 1, 0, 0, -1, //1 -> 9 (6)
                 1.0f, -1.0f, -1.0f, 1, 0, 0, 0, -1, //2 -> 10 (5)
                -1.0f, -1.0f, -1.0f, 0, 0, 0, 0, -1, //3 -> 11 (4)
                
                //left
                -1.0f, -1.0f, -1.0f, 0, 0, -1, 0, 0, //0 -> 12 (4)
                -1.0f, -1.0f,  1.0f, 0, 1, -1, 0, 0, //1 -> 13 (0)
                -1.0f,  1.0f,  1.0f, 1, 1, -1, 0, 0, //2 -> 14 (3)
                -1.0f,  1.0f, -1.0f, 1, 0, -1, 0, 0, //3 -> 15 (7)

                //bottom
                -1.0f, -1.0f,  1.0f, 0, 1, 0, -1, 0, //0 -> 16 (0)
                 1.0f, -1.0f,  1.0f, 1, 1, 0, -1, 0, //1 -> 17 (1)
                 1.0f, -1.0f, -1.0f, 1, 0, 0, -1, 0, //2 -> 18 (5)
                -1.0f, -1.0f, -1.0f, 0, 0, 0, -1, 0, //3 -> 19 (4)

                //right
                 1.0f, -1.0f,  1.0f, 0, 1, 1, 0, 0, //0 -> 20 (1)
                 1.0f, -1.0f, -1.0f, 0, 0, 1, 0, 0, //1 -> 21 (5)
                 1.0f,  1.0f, -1.0f, 1, 0, 1, 0, 0, //2 -> 22 (6)
                 1.0f,  1.0f,  1.0f, 1, 1, 1, 0, 0, //3 -> 23 (2)
            };

            int[] iData = new int[]
            {
                //front
                0, 1, 2, 2, 3, 0,
                //top
                4, 5, 6, 6, 7, 4,
                //back
                8, 9, 10, 10, 11, 8,
                // left face
                12, 13, 14, 14, 15, 12,
                // bottom face
                18, 17, 16, 16, 19, 18,
                // right face
                20, 21, 22, 22, 23, 20,
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

            GL.EnableVertexAttribArray(TexAttrib); //Tex
            GL.VertexAttribPointer(TexAttrib, 2, VertexAttribPointerType.Float, false, Stride, TexOffset);

            GL.EnableVertexAttribArray(NormAttrib); //Normal
            GL.VertexAttribPointer(NormAttrib, 3, VertexAttribPointerType.Float, false, Stride, NormOffset);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(iData.Length * sizeof(Int32)), iData, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        public static int ColorToRgba32(Color c)
        {
            return (int)((c.A << 24) | (c.B << 16) | (c.G << 8) | c.R);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _li, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}
