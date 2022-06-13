using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FCSG{
    /// <summary>
    /// A class which stores useful static methods
    /// </summary>
    public partial class Utilities{
        /// <summary>
        /// Draws the given sprite on the target.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, SpriteBase sprite, SpriteBatch spriteBatch,SpriteBatchParameters spriteBatchParameters=null){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            if(spriteBatchParameters==null){
                spriteBatchParameters=new SpriteBatchParameters(samplerState:SamplerState.PointClamp);
            }

            spriteBatch.Begin(spriteBatchParameters);
            sprite.BasicDraw(spriteBatch,drawMiddle:false);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given texture on the target. In order to draw, it uses the simplest Draw function possible (position at 0,0 and white as color)
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, Texture2D texture, SpriteBatch spriteBatch,SpriteBatchParameters spriteBatchParameters=null){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            if(spriteBatchParameters==null){
                spriteBatchParameters=new SpriteBatchParameters(samplerState:SamplerState.PointClamp);
            }

            spriteBatch.Begin(spriteBatchParameters);
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given textures on the target. In order to draw, it uses the simplest Draw function possible (position at 0,0 and white as color), and goes from first to last texture in list
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, List<Texture2D> textures, SpriteBatch spriteBatch,SpriteBatchParameters spriteBatchParameters=null){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            if(spriteBatchParameters==null){
                spriteBatchParameters=new SpriteBatchParameters(samplerState:SamplerState.PointClamp);
            }

            spriteBatch.Begin(spriteBatchParameters);
            foreach(Texture2D texture in textures){
                spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            }
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given sprites on the target. In order to draw, it uses each sprite's BasicDraw function and it calls the sprites in the same order they have inside of the LayerGroup, leaving the rest to the batch settings.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, LayerGroup sprites, SpriteBatch spriteBatch,SpriteBatchParameters spriteBatchParameters=null){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional

            if(spriteBatchParameters==null){
                spriteBatchParameters=new SpriteBatchParameters(sortMode:SpriteSortMode.FrontToBack, samplerState:SamplerState.PointClamp);
            }

            spriteBatch.Begin(spriteBatchParameters);
            foreach(SpriteBase sprite in sprites.objects){
                sprite.BasicDraw(spriteBatch, drawMiddle:false);
            }
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given sprites on the target. In order to draw, it uses each sprite's BasicDraw function, leaving the rest to the batch settings.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, List<SpriteBase> sprites, SpriteBatch spriteBatch,SpriteBatchParameters spriteBatchParameters=null){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional

            if(spriteBatchParameters==null){
                spriteBatchParameters=new SpriteBatchParameters(sortMode:SpriteSortMode.FrontToBack, samplerState:SamplerState.PointClamp);
            }

            spriteBatch.Begin(spriteBatchParameters);
            foreach(SpriteBase sprite in sprites){
                sprite.BasicDraw(spriteBatch, drawMiddle:false);
            }
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }
    }
}