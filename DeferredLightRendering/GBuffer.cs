using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeferredLightRendering
{
    enum GBufferTexture : int
    { 
        Position = 0,
        Diffuse = 1,
        Normal = 2,
        Light = 3
    }

    class GBuffer
    {
        int _fbo;
        int[] _textures;
        int _depth;

        PixelInternalFormat[] _iFormats = new PixelInternalFormat[]{
            PixelInternalFormat.Rgb32f, //Position -> we need float values
            PixelInternalFormat.Rgba, //Diffuse -> we need color values [0-1]
            PixelInternalFormat.Rgb32f, //Normal -> Same as position
            PixelInternalFormat.Rgba, //Light -> Same as diffuse

        };

        public int PositionTexture
        {
            get { return _textures[(int)GBufferTexture.Position]; }
        }

        public int DiffuseTexture
        {
            get { return _textures[(int)GBufferTexture.Diffuse]; }
        }

        public int NormalTexture
        {
            get { return _textures[(int)GBufferTexture.Normal]; }
        }

        public int LightTexture
        {
            get { return _textures[(int)GBufferTexture.Light]; }
        }

        public bool Init(int width, int height)
        {
            //FBO
            _fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

            //Textures
            _textures = new int[(int)GBufferTexture.Light + 1];
            GL.GenTextures(4, _textures);
            
            _depth = GL.GenTexture();

             for (int i = 0; i < _textures.Length; i++) 
             {
                 GL.BindTexture(TextureTarget.Texture2D, _textures[i]);
                 GL.TexImage2D(TextureTarget.Texture2D, 0, _iFormats[i], width, height, 0, PixelFormat.Rgb, PixelType.Float, (IntPtr)null);
                 GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                 GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                 GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0 + i, TextureTarget.Texture2D, _textures[i], 0);
            }

            //Depth
            GL.BindTexture(TextureTarget.Texture2D, _depth);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth32fStencil8, width, height, 0, PixelFormat.DepthComponent, PixelType.Float, (IntPtr)null);
            GL.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, _depth, 0);

            FramebufferErrorCode code = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if(code != FramebufferErrorCode.FramebufferComplete)
            {
                Console.WriteLine("GBuffer ERROR: " + code.ToString());
                return false;
            }

            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);

            return true;
        }

        public void BindForGeometryPass()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);

            GL.DrawBuffers(3, new DrawBuffersEnum[]{
                DrawBuffersEnum.ColorAttachment0, //Pos
                DrawBuffersEnum.ColorAttachment1, //Diff
                DrawBuffersEnum.ColorAttachment2  //Normal
            });
        }

        public void BindForLightPass()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment3);
        }

        public void BlitResult(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _fbo);

            GL.ReadBuffer(ReadBufferMode.ColorAttachment3);
            GL.BlitFramebuffer(0, 0, width, height, 0, 0, width, height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
        }

        public void Restore()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
        }

        public void Delete()
        {
            GL.DeleteFramebuffer(_fbo);
            GL.DeleteTextures(4, _textures);
            GL.DeleteTexture(_depth);
        }

        public void DumpToScreen(int width, int height)
        {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _fbo);

            GL.ReadBuffer(ReadBufferMode.ColorAttachment0);
            GL.BlitFramebuffer(0, 0, width, height, 0, 0, width / 2, height / 2, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

            GL.ReadBuffer(ReadBufferMode.ColorAttachment1);
            GL.BlitFramebuffer(0, 0, width, height, width / 2, 0, width, height / 2, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

            GL.ReadBuffer(ReadBufferMode.ColorAttachment2);
            GL.BlitFramebuffer(0, 0, width, height, 0, height / 2, width / 2, height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

            GL.ReadBuffer(ReadBufferMode.ColorAttachment3);
            GL.BlitFramebuffer(0, 0, width, height, width / 2, height / 2, width, height, ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Linear);

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
        }
    }
}
