using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FCSG{
    public delegate void TypeAction<Type>(Type type);
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

    public class Utilities{
        public static void DrawOntoTarget(RenderTarget2D renderTarget, ObjectGroup<Object2D> group, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch){
            graphicsDevice.SetRenderTarget(renderTarget);
            graphicsDevice.Clear(Color.Transparent);// TODO: make this optional

            spriteBatch.Begin();//TODO: add spriteBatch settings
            group.PerformOnAll((Object2D obj)=>{
                obj.Draw();
            });
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }
        public static void DrawOntoTarget(RenderTarget2D renderTarget, SpriteObject spriteObject, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin();
            spriteObject.Draw(drawMiddle:false);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }
    }
}