using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    class Sphere
    {
        int _vbo, _ibo;
        int _vao;

        int _il;

        public Sphere(float radius, int slices, int stacks)
        {
            List<float> vdata = new List<float>();

            float phi, theta;
            float dphi = MathHelper.Pi / stacks;
            float dtheta = MathHelper.TwoPi / slices;
            float x, y, z, sc;
            int index = 0;

            for (int stack = 0; stack <= stacks; stack++)
            {
                phi = MathHelper.PiOver2 - stack * dphi;
                y = radius * (float)Math.Sin(phi);
                sc = -radius * (float)Math.Cos(phi);

                for (int slice = 0; slice <= slices; slice++)
                {
                    theta = slice * dtheta;
                    x = sc * (float)Math.Sin(theta);
                    z = sc * (float)Math.Cos(theta);
                    vdata.Add(x);
                    vdata.Add(y);
                    vdata.Add(z);
                }
            }

            int[] indices = new int[slices * stacks * 6];
            index = 0;
            int k = slices + 1;

            for (int stack = 0; stack < stacks; stack++)
            {
                for (int slice = 0; slice < slices; slice++)
                {
                    indices[index++] = (stack + 0) * k + slice;
                    indices[index++] = (stack + 1) * k + slice;
                    indices[index++] = (stack + 0) * k + slice + 1;

                    indices[index++] = (stack + 0) * k + slice + 1;
                    indices[index++] = (stack + 1) * k + slice;
                    indices[index++] = (stack + 1) * k + slice + 1;
                }
            }

            float[] vData = vdata.ToArray();

            _il = indices.Length;

            _vbo = GL.GenBuffer();
            _ibo = GL.GenBuffer();

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vData.Length * sizeof(float)), vData, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0); //Position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(Int32)), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, _il, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}
