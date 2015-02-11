
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GDIPixelFormat = System.Drawing.Imaging.PixelFormat;
using BitmapData = System.Drawing.Imaging.BitmapData;
using ImageLockMode = System.Drawing.Imaging.ImageLockMode;
using OpenTK.Graphics.OpenGL4;

namespace DeferredLightRendering
{
    //Copied from my own OpenGL Framework (by Robot9706)
    public class Texture2D
    {
        private int _texture;
        public int GLId
        {
            get { return _texture; }
        }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public Texture2D(Stream imageStream)
        {
            using (Bitmap bitmap = new Bitmap(imageStream))
            {
                CreateTexture(bitmap);
            }
        }

        public Texture2D(string path)
        {
            using (Bitmap bitmap = new Bitmap(path))
            {
                CreateTexture(bitmap);
            }
        }

        private void CreateTexture(Bitmap bitmap)
        {
            bool alpha = true;
            PixelInternalFormat iFormat = (alpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb);
            PixelFormat pixelFormat = (alpha ? PixelFormat.Bgra : PixelFormat.Bgr);
            GDIPixelFormat lockFormat = (alpha ? GDIPixelFormat.Format32bppArgb : GDIPixelFormat.Format24bppRgb);

            _texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _texture);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, lockFormat);
            Width = data.Width;
            Height = data.Height;
            GL.TexImage2D(TextureTarget.Texture2D, 0, iFormat, data.Width, data.Height, 0, pixelFormat, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void BindAsTexture2D()
        {
            GL.BindTexture(TextureTarget.Texture2D, _texture);
        }

        public void Delete()
        {
            GL.DeleteTexture(_texture);
        }
    }
}
