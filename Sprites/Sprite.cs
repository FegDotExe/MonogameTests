using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace FCSG{
    ///<summary>
    ///A class which rapresents a sprite
    ///</summary>
    public class Sprite : SpriteBase{
        public Sprite(
                SpriteBatch spriteBatch, 
                Texture2D texture,
                Wrapper wrapper=null,
                float? depth=null, 
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
                Color? color=null,
                ObjectGroup<SpriteObject> group=null,
                List<ObjectGroup<SpriteObject>> groups=null,
            ClickDelegate leftClickDelegate=null,
            ClickDelegate middleClickDelegate=null,
            ClickDelegate rightClickDelegate=null,
            ClickDelegate wheelHoverDelegate=null,
            ClickDelegate hoverDelegate=null,
            Dictionary<string, SpriteBase> spritesDict=null,
            string dictKey=null
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
            groups:groups,
            x:x,
            y:y,
            width:width,
            height:height,
            leftClickDelegate:leftClickDelegate,
            middleClickDelegate:middleClickDelegate,
            rightClickDelegate:rightClickDelegate,
            wheelHoverDelegate:wheelHoverDelegate,
            hoverDelegate:hoverDelegate,
            spritesDict:spritesDict,
            dictKey:dictKey
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
            BasicDraw(this.spriteBatch,drawMiddle);
        }

        public override void BasicDraw(SpriteBatch spriteBatch, bool drawMiddle = true)
        {
            if(draw){
                drawMiddle=false;
                if(drawMiddle==true){
                    DrawMiddleTexture();
                }
                spriteBatch.Draw(texture, new Rectangle(this.x,this.y,this.width,this.height),null,color,rotation,origin,effects,depth);
            }
        }
    }
}