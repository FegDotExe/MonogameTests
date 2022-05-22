using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FCSG{
    public static class Texture2DExtension{
        /// <summary>
        /// Gets the color of the pixel at the specified position.
        /// </summary>
        public static Color GetPixel(this Texture2D texture, int x, int y){
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(pixels);
            return pixels[x + (y * texture.Width)];
        }
    }
}