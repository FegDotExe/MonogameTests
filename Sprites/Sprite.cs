using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace FCSG{
    ///<summary>
    ///A class which rapresents a sprite
    ///</summary>
    public class Sprite : SpriteBase{
        //TODO: add ability to give int instead of delegate
        public Sprite(
                SpriteBatch spriteBatch, 
                Texture2D texture,
                Wrapper wrapper=null,
                float? depth=null, 
                IntSpriteObjDelegate xDelegate=null, 
                IntSpriteObjDelegate yDelegate=null,
                IntSpriteObjDelegate widthDelegate=null, 
                IntSpriteObjDelegate heightDelegate=null,
                float? rotation=null, 
                Vector2? origin=null, 
                Color? color=null,
                ObjectGroup<SpriteObject> group=null,
                List<ObjectGroup<SpriteObject>> groups=null
        ) : base(
            spriteBatch:spriteBatch,
            wrapper:wrapper,
            depth:depth,
            xDelegate:xDelegate,
            yDelegate:yDelegate,
            widthDelegate:widthDelegate,
            heightDelegate:heightDelegate,
            rotation:rotation,
            origin:origin,
            color:color,
            group:group,
            groups:groups
        ){
            this.texture = texture;

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
}