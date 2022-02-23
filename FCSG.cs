using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FCSG{
    public delegate int IntSpriteDelegate(Sprite sprite);
    public delegate int IntSpriteObjDelegate(SpriteObject sprite);

    public class Wrapper{
        private List<SpriteObject> sprites;
        private SpriteBatch spriteBatch;

        public Wrapper(SpriteBatch spriteBatch){
            this.spriteBatch = spriteBatch;
            sprites = new List<SpriteObject>();
        }

        public Sprite NewSprite(
            Texture2D texture, 
            float? depth=null, 
            ObjectGroup<SpriteObject> group=null,
            List<ObjectGroup<SpriteObject>> groups=null,
            IntSpriteObjDelegate xDelegate=null, 
            IntSpriteObjDelegate yDelegate=null,
            IntSpriteObjDelegate widthDelegate=null, 
            IntSpriteObjDelegate heightDelegate=null,
            float? rotation=null, 
            Vector2? origin=null, 
            Color? color=null
        ){
            Sprite newSprite=new Sprite(spriteBatch, texture:texture, wrapper:this, group:group, groups:groups, depth:depth, xDelegate:xDelegate, yDelegate:yDelegate, widthDelegate:widthDelegate, heightDelegate:heightDelegate, rotation:rotation, origin:origin, color:color);
            sprites.Add(newSprite);
            return newSprite;
        }

        public void Add(SpriteObject sprite){
            sprites.Add(sprite);
        }

        public void Remove(SpriteObject sprite){
            sprites.Remove(sprite);
        }

        public void Draw(){
            spriteBatch.Begin(sortMode:SpriteSortMode.FrontToBack); //TODO: Should add options
            foreach(SpriteObject sprite in sprites){
                sprite.Draw();
            }
            spriteBatch.End();
        }
    }
}