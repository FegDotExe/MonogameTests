//Where inheritable classes are defined
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FCSG{
    ///<summary>
    ///An interface which rapresents a 2d object with a position and a size
    ///</summary>
    public interface Object2D{
        int x{get;set;}
        int y{get;set;}
        int width{get;}
        int height{get;}
        void Draw(bool drawMiddle=true);
    }

    ///<summary>
    ///An interface which rapresents an object with a boolean value which indicates if it is visible
    ///</summary>
    public interface BoolDrawable{
        bool draw{get;set;}
    }

    ///<summary>
    ///An interface which rapresents a sprite object
    ///</summary>
    public interface SpriteObject : Object2D, BoolDrawable{
        
    }

    public class SpriteBase : SpriteObject{
        protected SpriteBatch spriteBatch;
        //Position values
            public int x{
                get{return xDelegate(this);}
                set{xDelegate=(SpriteObject sprite)=>value;}
            }
            public int y{
                get{return yDelegate(this);}
                set{yDelegate=(SpriteObject sprite)=>value;}
            }
            protected IntSpriteObjDelegate xDelegate;
            protected IntSpriteObjDelegate yDelegate;
        //Size values
            protected IntSpriteObjDelegate widthDelegate;
            protected IntSpriteObjDelegate heightDelegate;
            public int width{
                get{return widthDelegate(this);}
                set{widthDelegate=(SpriteObject sprite)=>value;}
            }
            protected int midWidth{get;set;}
            public int height{
                get{return heightDelegate(this);}
                set{heightDelegate=(SpriteObject sprite)=>value;}
            }
            protected int midHeight{get;set;}
        public Texture2D texture{get;set;}
        protected Texture2D middleTexture;
        public Color color{get;set;}
        public float rotation{get;set;}
        public Vector2 origin{get;set;}
        public float depth{get;set;}
        public SpriteEffects effects=SpriteEffects.None;
        public bool draw{get;set;}
        public SpriteBase(
                SpriteBatch spriteBatch,
                float? depth=null, 
                IntSpriteObjDelegate xDelegate=null, 
                IntSpriteObjDelegate yDelegate=null,
                IntSpriteObjDelegate widthDelegate=null, 
                IntSpriteObjDelegate heightDelegate=null,
                float? rotation=null, 
                Vector2? origin=null, 
                Color? color=null
        ){
            this.spriteBatch = spriteBatch;

            if(depth != null) //Sets the depth of the sprite
                this.depth = (float)depth;
            else
                this.depth = 0;

            //Position delegates
                if(xDelegate != null)
                    this.xDelegate = xDelegate;
                else
                    this.xDelegate = (SpriteObject sprite) => 0;
                if(yDelegate != null)
                    this.yDelegate = yDelegate;
                else
                    this.yDelegate = (SpriteObject sprite) => 0;

            //Size delegates
                if(widthDelegate!=null)
                    this.widthDelegate = widthDelegate;
                else
                    this.widthDelegate = (SpriteObject sprite) => 100;
                midWidth = -1;
                if(heightDelegate!=null)
                    this.heightDelegate = heightDelegate;
                else
                    this.heightDelegate = (SpriteObject sprite) => 100;
                midHeight = -1;

            if(rotation!=null){ //Sets the rotation of the sprite
                this.rotation = (float)rotation;
            }else{
                this.rotation = 0;
            }

            if(origin!=null){
                this.origin = (Vector2)origin;
            }else{
                this.origin = new Vector2(0.5f,0.5f);
            }

            if(color!=null){
                this.color = (Color)color;
            }else{
                this.color = Color.White;
            }
            this.draw=true;
        }
        public virtual void Draw(bool drawMiddle=true){
            //spriteBatch.Draw(texture,new Vector2(x,y),null,color,rotation,origin,1,effects,depth);
        }
        public virtual void DrawMiddleTexture(){
            if(midWidth!=width || midHeight!=height){
                Console.WriteLine("Called a sprite resize: w="+midWidth+"/"+width+" h="+midHeight+"/"+height);

                midWidth=width;
                midHeight=height;

                Console.WriteLine("After var update: w="+midWidth+"/"+width+" h="+midHeight+"/"+height);

                RenderTarget2D renderTarget = new RenderTarget2D(spriteBatch.GraphicsDevice,width,height);
                Utilities.DrawOntoTarget(renderTarget,this,spriteBatch);
                middleTexture = renderTarget;
            }
        }
    }
}