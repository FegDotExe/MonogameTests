using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FCSG{
    public class Wrapper{
        private List<SpriteObject> sprites;
        private SpriteBatch spriteBatch;
        public LayerGroup leftClick;
        public LayerGroup middleClick;
        public LayerGroup rightClick;
        public LayerGroup wheelHover;
        public LayerGroup hover;

        public Wrapper(SpriteBatch spriteBatch){
            this.spriteBatch = spriteBatch;
            sprites = new List<SpriteObject>();
            leftClick = new LayerGroup();
            middleClick = new LayerGroup();
            rightClick = new LayerGroup();
            wheelHover = new LayerGroup();
            hover = new LayerGroup();
        }

        #region NewSprite
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
            Color? color=null,
            ClickDelegate leftClickDelegate=null,
            ClickDelegate middleClickDelegate=null,
            ClickDelegate rightClickDelegate=null,
            ClickDelegate wheelHoverDelegate=null,
            ClickDelegate hoverDelegate=null
        ){
            Sprite newSprite=new Sprite(spriteBatch, texture:texture, wrapper:this, group:group, groups:groups, depth:depth, xDelegate:xDelegate, yDelegate:yDelegate, widthDelegate:widthDelegate, heightDelegate:heightDelegate, rotation:rotation, origin:origin, color:color, x:x, y:y, width:width, height:height, leftClickDelegate:leftClickDelegate, middleClickDelegate:middleClickDelegate, rightClickDelegate:rightClickDelegate, wheelHoverDelegate:wheelHoverDelegate, hoverDelegate:hoverDelegate);
            sprites.Add(newSprite);
            return newSprite;
        }
        #endregion NewSprite

        /// <summary>
        /// Completely handles sprite addition (Adds to draw and click groups).
        /// </summary>
        public void Add(SpriteBase sprite){
            if(sprite.leftClickDelegate!=null){
                leftClick.Add(sprite);
            }
            if(sprite.middleClickDelegate!=null){
                middleClick.Add(sprite);
            }
            if(sprite.rightClickDelegate!=null){
                rightClick.Add(sprite);
            }
            if(sprite.wheelHoverDelegate!=null){
                wheelHover.Add(sprite);
            }
            if(sprite.hoverDelegate!=null){
                hover.Add(sprite);
            }
            sprite.wrapper=this;
            sprites.Add(sprite);
        }

        /// <summary>
        /// Completely handles sprite removal (Removes from draw and click groups).
        /// </summary>
        public void Remove(SpriteBase sprite){
            leftClick.Remove(sprite);
            middleClick.Remove(sprite);
            rightClick.Remove(sprite);
            wheelHover.Remove(sprite);
            hover.Remove(sprite);
            sprite.wrapper=null;
            sprites.Remove(sprite);
        }

        public void Draw(){
            spriteBatch.Begin(sortMode:SpriteSortMode.FrontToBack,samplerState:SamplerState.PointClamp); //TODO: Should add options
            foreach(SpriteObject sprite in sprites){
                sprite.Draw();
            }
            spriteBatch.End();
        }
    
        public void Click(Clicks click,int x, int y){
            List<SpriteBase> layers=null;
            switch(click){
                case Clicks.Left:
                    layers=leftClick.objects;
                    break;
                case Clicks.Middle:
                    layers=middleClick.objects;
                    break;
                case Clicks.Right:
                    layers=rightClick.objects;
                    break;
                case Clicks.WheelHover:
                    layers=wheelHover.objects;
                    break;
                case Clicks.Hover:
                    layers=hover.objects;
                    break;
            }
            foreach(SpriteBase sprite in layers){
                sprite.Clicked(x,y,click); //Everything should be correctly handled in each sprite
            }
        }
    }
}