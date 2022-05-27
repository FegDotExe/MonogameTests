using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FCSG{
    public partial class Utilities{
        public static Texture2D CombineTextures(List<Texture2D> textures, SpriteBatch spriteBatch){
            if(textures.Count==0){
                throw new System.ArgumentException("The list of textures is empty");
            }
            RenderTarget2D renderTarget=new RenderTarget2D(spriteBatch.GraphicsDevice, textures[0].Width, textures[0].Height);
            
            Utilities.DrawOntoTarget(renderTarget, textures, spriteBatch);
            
            return renderTarget;
        }
    }
}