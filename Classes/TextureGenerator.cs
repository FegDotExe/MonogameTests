using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FCSG{
    /// <summary>
    /// A class used to simply generate textures composed of many different sprites.
    /// </summary>
    public class TextureGenerator{
        public List<SpriteBase> sprites;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        public int x;
        public int y;
        public SpriteBatchParameters spriteBatchParams;

        public TextureGenerator(GraphicsDevice graphicsDevice){
            sprites=new LayerGroup();
            this.graphicsDevice=graphicsDevice;
            this.x=100;
            this.y=100;
        }
        /// <param name="sprites">A layer group containing the sprites which will be drawn on the texture</param>
        /// <param name="x">The width of the result texture</param>
        /// <param name="y">The height of the result texture</param>
        /// <summary>
        /// Construct a texture generator which will automatically generate a new SpriteBatch from its GraphicsDevice
        /// </summary>
        public TextureGenerator(GraphicsDevice graphicsDevice, LayerGroup sprites, int x, int y){
            this.graphicsDevice=graphicsDevice;
            this.sprites=sprites;
            this.x=x;
            this.y=y;
        }

        /// <param name="sprites">A layer group containing the sprites which will be drawn on the texture</param>
        /// <param name="x">The width of the result texture</param>
        /// <param name="y">The height of the result texture</param>
        /// <summary>
        /// Construct a texture generator which will use the given SpriteBatch
        /// </summary>
        public TextureGenerator(GraphicsDevice graphicsDevice, LayerGroup sprites, int x, int y, SpriteBatch spriteBatch, SpriteBatchParameters spriteBatchParams=null){
            this.graphicsDevice=graphicsDevice;
            this.sprites=sprites;
            this.x=x;
            this.y=y;
            this.spriteBatch=spriteBatch;
            this.spriteBatchParams=spriteBatchParams;
        }

        /// <summary>
        /// Generate the texture related to the given generator. If spriteBatchParams are given, they will be added to the default ones.
        /// </summary>
        public Texture2D Generate(SpriteBatchParameters spriteBatchParams=null){
            // SpriteBatch spriteBatch=this.spriteBatch;
            if(spriteBatch==null){
                spriteBatch=new SpriteBatch(graphicsDevice);
            }
            if(spriteBatchParams==null){
                spriteBatchParams=this.spriteBatchParams;
            }else{
                spriteBatchParams=this.spriteBatchParams+spriteBatchParams;
            }
            RenderTarget2D renderTarget=new RenderTarget2D(graphicsDevice, x, y);
            Utilities.DrawOntoTarget(renderTarget, sprites, spriteBatch,spriteBatchParams);
            return renderTarget;
        }

        public void Add(SpriteBase sprite){
            sprites.Add(sprite);
        }
        public void Remove(SpriteBase sprite){
            sprites.Remove(sprite);
        }
    }
}