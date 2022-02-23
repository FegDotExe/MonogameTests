using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FCSG{
    public delegate int IntSpriteDelegate(Sprite sprite);
    public delegate int IntSpriteObjDelegate(SpriteObject sprite);

    ///<summary>
    ///A class which rapresents a sprite
    ///</summary>
    public class Sprite : SpriteBase{
        private List<ObjectGroup<SpriteObject>> groups{get;set;} //A list of all the groups this sprite is in

        //TODO: add ability to give int instead of delegate
        public Sprite(
                SpriteBatch spriteBatch, 
                Texture2D texture,
                Wrapper wrapper=null,
                ObjectGroup<SpriteObject> group=null,
                List<ObjectGroup<SpriteObject>> groups=null,
                float? depth=null, 
                IntSpriteObjDelegate xDelegate=null, 
                IntSpriteObjDelegate yDelegate=null,
                IntSpriteObjDelegate widthDelegate=null, 
                IntSpriteObjDelegate heightDelegate=null,
                float? rotation=null, 
                Vector2? origin=null, 
                Color? color=null
        ) : base(
            spriteBatch,
            wrapper,
            depth,
            xDelegate,
            yDelegate,
            widthDelegate,
            heightDelegate,
            rotation,
            origin,
            color
        ){
            this.texture = texture;

            this.groups=new List<ObjectGroup<SpriteObject>>();

            if(group!=null){ //Adds the sprite to the group
                this.groups.Add(group);
                group.Add(this);
            }
            if (groups!=null){
                foreach(ObjectGroup<SpriteObject> spriteGroup in groups){
                    this.groups.Add(spriteGroup);
                    spriteGroup.Add(this);
                }
            }

            this.draw=true;
        }
        public override void Draw(bool drawMiddle=true){
            if(draw){
                if(drawMiddle==true){
                    DrawMiddleTexture();
                }
                spriteBatch.Draw(texture, new Rectangle(this.x,this.y,this.width,this.height),null,color,rotation,origin,effects,depth);
            }
        }
    }
    
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