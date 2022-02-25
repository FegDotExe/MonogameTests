using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FCSG{
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
            int? x=null,
            IntSpriteObjDelegate yDelegate=null,
            int? y=null,
            IntSpriteObjDelegate widthDelegate=null, 
            int? width=null,
            IntSpriteObjDelegate heightDelegate=null,
            int? height=null,
            float? rotation=null, 
            Vector2? origin=null, 
            Color? color=null
        ){
            Sprite newSprite=new Sprite(spriteBatch, texture:texture, wrapper:this, group:group, groups:groups, depth:depth, xDelegate:xDelegate, yDelegate:yDelegate, widthDelegate:widthDelegate, heightDelegate:heightDelegate, rotation:rotation, origin:origin, color:color, x:x, y:y, width:width, height:height);
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
            spriteBatch.Begin(sortMode:SpriteSortMode.FrontToBack,samplerState:SamplerState.PointClamp); //TODO: Should add options
            foreach(SpriteObject sprite in sprites){
                sprite.Draw();
            }
            spriteBatch.End();
        }
    }
}