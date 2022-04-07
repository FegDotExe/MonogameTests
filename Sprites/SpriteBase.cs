using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace FCSG{
    /// <summary>
    /// The base sprite class, from which other classes inherit.
    /// </summary>
    public class SpriteBase : SpriteObject{
        #region Fields
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
        public Color color{get;set;} //Used when the sprite is drawn
        public float rotation{get;set;}
        public Vector2 origin{get;set;}
        public float depth{get;set;}
        public SpriteEffects effects=SpriteEffects.None;
        public bool draw{get;set;} //Wether the sprite will be drawn or not
        protected Wrapper wrapper; //The wrapper which contains the sprite
        protected List<ObjectGroup<SpriteObject>> groups{get;set;}
        //Click delegates
            public ClickDelegate leftClickDelegate;
            public ClickDelegate middleClickDelegate;
            public ClickDelegate rightClickDelegate;
            public ClickDelegate wheelHoverDelegate;
            public ClickDelegate hoverDelegate;
        #endregion Fields
        #region Constructors
        public SpriteBase(
            SpriteBatch spriteBatch,
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
            ClickDelegate hoverDelegate=null
        ){
            this.spriteBatch = spriteBatch;

            this.wrapper = wrapper;

            if(depth != null) //Sets the depth of the sprite
                this.depth = (float)depth;
            else
                this.depth = 0;

            //Position delegates
                if(x!=null){
                    this.xDelegate=(SpriteObject sprite)=>(int)x;
                }
                else if(xDelegate != null)
                    this.xDelegate = xDelegate;
                else
                    this.xDelegate = (SpriteObject sprite) => 0;
                if(y!=null){
                    this.yDelegate=(SpriteObject sprite)=>(int)y;
                }
                else if(yDelegate != null)
                    this.yDelegate = yDelegate;
                else
                    this.yDelegate = (SpriteObject sprite) => 0;

            //Size delegates
                if(width!=null){
                    this.widthDelegate=(SpriteObject sprite)=>(int)width;
                }
                else if(widthDelegate!=null)
                    this.widthDelegate = widthDelegate;
                else
                    this.widthDelegate = (SpriteObject sprite) => 100;
                midWidth = -1;
                
                if(height!=null){
                    this.heightDelegate=(SpriteObject sprite)=>(int)height;
                }
                else if(heightDelegate!=null)
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
                this.origin = new Vector2(0,0);
            }

            if(color!=null){
                this.color = (Color)color;
            }else{
                this.color = Color.White;
            }
            this.draw=true;

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

            //Click delegates
                this.leftClickDelegate = leftClickDelegate;
                this.middleClickDelegate = middleClickDelegate;
                this.rightClickDelegate = rightClickDelegate;
                this.wheelHoverDelegate = wheelHoverDelegate;
                this.hoverDelegate = hoverDelegate;
            if(wrapper!=null){
                wrapper.Add(this);
            }
        }
        #endregion Constructors
        public virtual void Draw(bool drawMiddle=true){
            if(drawMiddle==true){
                DrawMiddleTexture();
            }
            //spriteBatch.Draw(texture,new Vector2(x,y),null,color,rotation,origin,1,effects,depth);
        }
        ///<summary>
        ///Resizes the texture to the size of the sprite, so that there is no need to resize it every frame; should improve performance.
        ///</summary>
        public virtual void DrawMiddleTexture(){
            if(midWidth!=width || midHeight!=height){
                // Console.WriteLine("Called a sprite resize: w="+midWidth+"/"+width+" h="+midHeight+"/"+height);

                midWidth=width;
                midHeight=height;

                // Console.WriteLine("After var update: w="+midWidth+"/"+width+" h="+midHeight+"/"+height);

                RenderTarget2D renderTarget = new RenderTarget2D(spriteBatch.GraphicsDevice,width,height);
                Utilities.DrawOntoTarget(renderTarget,this,spriteBatch);
                middleTexture = renderTarget;
            }
        }
    
        public void Remove(){
            if(wrapper!=null){
                wrapper.Remove(this);
            }
        }

        //TODO: make a method which takes a rectangle class to check collisions, so that it is more linear and compatible with actual game objects
        public bool CollidesWith(int x, int y){
            if(x>=this.x && x<=this.x+width && y>=this.y && y<=this.y+height){
                return true;
            }
            return false;
        }

        ///<summary>
        ///Checks if the sprite is colliding with another sprite and triggers the right click delegate.
        ///</summary>
        public void Clicked(int x, int y, Clicks clickType){
            if(this.CollidesWith(x,y)){
                switch(clickType){
                    case Clicks.Left:
                        if(leftClickDelegate!=null){
                            leftClickDelegate(this,x,y);
                        }
                        break;
                    case Clicks.Middle:
                        if(middleClickDelegate!=null){
                            middleClickDelegate(this,x,y);
                        }
                        break;
                    case Clicks.Right:
                        if(rightClickDelegate!=null){
                            rightClickDelegate(this,x,y);
                        }
                        break;
                    case Clicks.WheelHover:
                        if(wheelHoverDelegate!=null){
                            wheelHoverDelegate(this,x,y);
                        }
                        break;
                    case Clicks.Hover:
                        if(hoverDelegate!=null){
                            hoverDelegate(this,x,y);
                        }
                        break;
                }
            }
        }
    }
}