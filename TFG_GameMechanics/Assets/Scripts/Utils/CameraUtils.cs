using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Utils
{
    public class CameraUtils
    {
        public static Texture2D TakeScreenShot(Camera camera)
        {
            RenderTexture renderTexture = camera.targetTexture;

            if (renderTexture == null)
            {
                renderTexture =
                    new RenderTexture(512, 512, 24, RenderTextureFormat.R16);
                camera.targetTexture = renderTexture;
                camera.Render();
            }

            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, GraphicsFormat.R16G16B16A16_UNorm, TextureCreationFlags.None);
            Graphics.CopyTexture(renderTexture, texture);
            //camera.targetTexture = null;
            camera.Render();
            return texture;
        }
    }
}