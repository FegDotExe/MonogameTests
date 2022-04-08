using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System;

namespace FCSG{
    public delegate void TypeAction<Type>(Type type);
    /// <summary>
    /// A class used to group together objects and perform actions on them
    /// </summary>
    public class ObjectGroup<Type>{
        public List<Type> objects;
        public ObjectGroup(){
            objects=new List<Type>();
        }
        public ObjectGroup(List<Type> list){
            objects=list;
        }
        public void Add(Type obj){
            objects.Add(obj);
        }
        public void Remove(Type obj){
            objects.Remove(obj);
        }
        public void PerformOnAll(TypeAction<Type> action){
            foreach(Type obj in objects){
                action(obj);
            }
        }
    }

    /// <summary>
    /// A class which stores useful static methods
    /// </summary>
    public class Utilities{

        /// <summary>
        /// Draws the given Object2Ds on the target.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, ObjectGroup<Object2D> group, SpriteBatch spriteBatch){
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);//TODO: add spriteBatch settings
            group.PerformOnAll((Object2D obj)=>{
                obj.Draw(drawMiddle:false);
            });
            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }
        /// <summary>
        /// Draws the given sprite on the target.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, SpriteObject spriteObject, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            spriteObject.Draw(drawMiddle:false);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given texture on the target. In order to draw, it uses the simplest Draw function possible (position at 0,0 and white as color)
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, Texture2D texture, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given textures on the target. In order to draw, it uses the simplest Draw function possible (position at 0,0 and white as color), and goes from first to last texture in list
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, List<Texture2D> textures, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            foreach(Texture2D texture in textures){
                spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            }
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given sprites on the target. In order to draw, it uses each sprite's BasicDraw function, leaving the rest to the batch settings.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, LayerGroup sprites, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(sortMode:SpriteSortMode.FrontToBack,samplerState:SamplerState.PointClamp);
            foreach(SpriteBase sprite in sprites.objects){
                sprite.BasicDraw(spriteBatch, drawMiddle:false);
            }
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        public static Texture2D CombineTextures(List<Texture2D> textures, SpriteBatch spriteBatch){
            if(textures.Count==0){
                throw new System.ArgumentException("The list of textures is empty");
            }
            RenderTarget2D renderTarget=new RenderTarget2D(spriteBatch.GraphicsDevice, textures[0].Width, textures[0].Height);
            
            Utilities.DrawOntoTarget(renderTarget, textures, spriteBatch);
            
            return renderTarget;
        }
    
        public static void UpdateMouse(MouseState mouseState, MouseHandler mouseHandler){
            if((mouseState.LeftButton==ButtonState.Pressed)){
                if(mouseHandler.IsNewDown(Clicks.Left)){
                    mouseHandler.Held(Clicks.Left,mouseState.X,mouseState.Y);
                }
                mouseHandler.Down(Clicks.Left, mouseState.X, mouseState.Y);
            }else{
                mouseHandler.Up(Clicks.Left,mouseState.X,mouseState.Y);
            }
            //TODO: finish this and reevaluate life choices (is a mouse handler really needed?)
        }
    }

    /// <summary>
    /// A class which stores sprites in a list, keeping them ordered by their depth, with the ones with higher values in front
    /// </summary>
    public class LayerGroup{
        public List<SpriteBase> objects; //TODO: decide if this should be private or not->I'd say it should not
        public LayerGroup(){
            objects=new List<SpriteBase>();
        }
        public void Add(SpriteBase sprite){
            this.Add(sprite,this.objects);
        }
        private void Add(SpriteBase sprite, List<SpriteBase> objects){
            if(objects.Count==0){
                objects.Add(sprite);
            }else{
                int i=0;
                for(i=objects.Count; i>0 && objects[i-1].depth<sprite.depth; i--){
                    objects.Insert(i,objects[i-1]);
                }
                if(objects.Count==i){ //This covers the edge case in which the index of the object which should be added is out of bounds.
                    objects.Add(sprite);
                }
                else{
                    objects[i]=sprite;
                }
            }
        }

        public void Remove(SpriteBase sprite){
            objects.Remove(sprite);
        }

        /// <summary>
        /// Reorders all the objects in the group
        /// </summary>
        public void Update(){
            List<SpriteBase> newObjects=new List<SpriteBase>();
            foreach(SpriteBase sprite in objects){
                this.Add(sprite,newObjects);
            }
            objects=newObjects;
        }
    }

    /// <summary>
    /// A class used to simply generate textures composed of many different sprites.
    /// </summary>
    public class TextureGenerator{
        public LayerGroup sprites;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        public int x;
        public int y;
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
        public TextureGenerator(GraphicsDevice graphicsDevice, LayerGroup sprites, int x, int y, SpriteBatch spriteBatch){
            this.graphicsDevice=graphicsDevice;
            this.sprites=sprites;
            this.x=x;
            this.y=y;
            this.spriteBatch=spriteBatch;
        }
        public Texture2D Generate(){
            SpriteBatch spriteBatch=this.spriteBatch;
            if(spriteBatch==null){
                spriteBatch=new SpriteBatch(graphicsDevice);
            }
            RenderTarget2D renderTarget=new RenderTarget2D(graphicsDevice, x, y);
            Utilities.DrawOntoTarget(renderTarget, sprites, spriteBatch);
            return renderTarget;
        }
    }
}