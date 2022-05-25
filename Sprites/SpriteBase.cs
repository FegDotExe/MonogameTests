using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;


namespace FCSG{
    /// <summary>
    /// The base sprite class, from which other classes inherit.
    /// </summary>
    public class SpriteBase : SpriteObject{
        #region Fields
        protected SpriteBatch spriteBatch;
        public string name="";
        //Position values
            public int x{
                get{return xVariable;}
                set{xVariable.Set(value);}
            }
            public int y{
                get{return yVariable;}
                set{yVariable.Set(value);}
            }
            public LinkedVariable xVariable;
            public LinkedVariable yVariable;
        //Size values
            public LinkedVariable widthVariable;
            public LinkedVariable heightVariable;
            public int width{
                get{return widthVariable;}
                set{widthVariable.Set(value);}
            }
            protected int midWidth{get;set;} //Used to know wether the middle texture should be redrawn or not
            public int height{
                get{return heightVariable;}
                set{heightVariable.Set(value);}
            }
            protected int midHeight{get;set;} //Used to know wether the middle texture should be redrawn or not
        public Texture2D texture{get;set;}
        protected Texture2D middleTexture;
        public Color color; //Used when the sprite is drawn
        public float rotation{get;set;}
        public Vector2 origin{get;set;}
        public float depth{get;set;}
        public SpriteEffects effects=SpriteEffects.None;
        public bool draw{get;set;} //Wether the sprite will be drawn or not
        public Wrapper wrapper; //The wrapper which contains the sprite
        protected List<ObjectGroup<SpriteObject>> groups{get;set;}
        //Click delegates
            public ClickDelegate leftClickDelegate;
            public ClickDelegate middleClickDelegate;
            public ClickDelegate rightClickDelegate;
            public ClickDelegate wheelHoverDelegate;
            public ClickDelegate hoverDelegate;

        protected CollisionRectangle collisionRectangle; //A rectangle used for collision detection. its coordinates are relative to the sprite's position, and so is the size.
        #endregion Fields
        #region Constructors
        /// <param name="depth">The depth of the sprite. The higher the number, the closer to the camera. The value can vary between 1 and 0.</param>
        /// <param name="xDelegate">The delegate which returns the x position of the sprite.</param>
        /// <param name="yDelegate">The delegate which returns the y position of the sprite.</param>
        /// <param name="widthDelegate">The delegate which returns the width of the sprite.</param>
        /// <param name="heightDelegate">The delegate which returns the height of the sprite.</param>
        /// <param name="rotation">The rotation of the sprite. (I should probably not touch this)</param>
        /// <param name="origin">The origin of the sprite. (I should probably not touch this)</param>
        /// <param name="color">The color of the sprite.</param>
        /// <param name="group">A group to which the sprite will be added when constructed.</param>
        /// <param name="groups">A list of groups to which the sprite will be added when constructed.</param>
        /// <param name="leftClickDelegate">The delegate which will be called when the sprite is clicked with the left mouse button.</param>
        /// <param name="middleClickDelegate">The delegate which will be called when the sprite is clicked with the middle mouse button.</param>
        /// <param name="rightClickDelegate">The delegate which will be called when the sprite is clicked with the right mouse button.</param>
        /// <param name="wheelHoverDelegate">The delegate which will be called when the scrolls.</param>
        /// <param name="hoverDelegate">The delegate which will be called when the mouse is over the sprite.</param>
        /// <param name="spritesDict">A dictionary to which the sprite will be added once constructed.</param>
        /// <param name="dictKey">The key the dictionary will use when inserted in the <c>spritesDict</c></param>
        public SpriteBase(
            SpriteParameters spriteParameters
        ){
            this.spriteBatch = spriteParameters.spriteBatch;

            this.wrapper = spriteParameters.wrapper;

            //Adding to groups
            this.groups=new List<ObjectGroup<SpriteObject>>();
            if(spriteParameters.group!=null){ //Adds the sprite to the group
                this.groups.Add(spriteParameters.group);
                spriteParameters.group.Add(this);
            }
            if (spriteParameters.groups!=null){
                foreach(ObjectGroup<SpriteObject> spriteGroup in spriteParameters.groups){
                    this.groups.Add(spriteGroup);
                    spriteGroup.Add(this);
                }
            }
            if(spriteParameters.spritesDict!=null){
                if(spriteParameters.dictKey!=null){
                    spriteParameters.spritesDict.Add(spriteParameters.dictKey,this);
                    this.name=spriteParameters.dictKey; //FIXME: remove this along with names
                }else{
                    Console.WriteLine("Warning: SpriteBase constructor: dictKey is null, thus the object was not added to the dictionary.");
                }
            }

            this.depth = spriteParameters.depth;

            //Position variables
                if(spriteParameters.x!=null){
                    this.xVariable = new LinkedVariable(this,(SpriteBase sb)=>spriteParameters.x);
                }
                else if(spriteParameters.xVariable != null){
                    this.xVariable = new LinkedVariable(this, spriteParameters.xVariable);
                    // xVariable.SetSprite(this);
                }
                else
                    this.xVariable = new LinkedVariable(this,(SpriteBase sb)=>0);
                if(spriteParameters.y!=null){
                    this.yVariable = new LinkedVariable(this,(SpriteBase sb)=>spriteParameters.y);
                }
                else if(spriteParameters.yVariable != null){
                    this.yVariable = new LinkedVariable(this, spriteParameters.yVariable);
                    // yVariable.SetSprite(this);
                }
                else
                    this.yVariable = new LinkedVariable(this,(SpriteBase sb)=>0);

            //Size delegates
                if(spriteParameters.width!=null){
                    this.widthVariable=new LinkedVariable(this,(SpriteBase sprite)=>(int)spriteParameters.width);
                }
                else if(spriteParameters.widthVariable!=null)
                    this.widthVariable = new LinkedVariable(this, spriteParameters.widthVariable);
                else
                    this.widthVariable = new LinkedVariable(this,(SpriteBase sprite) => 100);
                midWidth = -1;
                
                if(spriteParameters.height!=null){
                    this.heightVariable=new LinkedVariable(this,(SpriteBase sprite)=>(int)spriteParameters.height);
                }
                else if(spriteParameters.heightVariable!=null)
                    this.heightVariable = new LinkedVariable(this, spriteParameters.heightVariable);
                else
                    this.heightVariable = new LinkedVariable(this,(SpriteBase sprite) => 100);
                midHeight = -1;

            this.rotation=spriteParameters.rotation;

            this.origin=spriteParameters.origin;

            this.color=spriteParameters.color;

            //Click delegates
                this.leftClickDelegate = spriteParameters.leftClickDelegate;
                this.middleClickDelegate = spriteParameters.middleClickDelegate;
                this.rightClickDelegate = spriteParameters.rightClickDelegate;
                this.wheelHoverDelegate = spriteParameters.wheelHoverDelegate;
                this.hoverDelegate = spriteParameters.hoverDelegate;

            //Collision rectangle
            if(spriteParameters.collisionRectangle==null){
                this.collisionRectangle = new CollisionRectangle(this);
            }else{
                this.collisionRectangle = spriteParameters.collisionRectangle;
            }

            //Add to wrapper
            if(wrapper!=null){
                wrapper.Add(this);
            }

            this.xVariable.Activate();
            this.yVariable.Activate();
            this.widthVariable.Activate();
            this.heightVariable.Activate();
            if(this.collisionRectangle.sprite==null){
                this.collisionRectangle.Activate(this);
            }

            this.draw=true;
        }
        #endregion Constructors
        public virtual void Draw(bool drawMiddle=true){
            BasicDraw(this.spriteBatch,drawMiddle);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void BasicDraw(SpriteBatch spriteBatch, bool drawMiddle=true){
            if(drawMiddle==true){
                DrawMiddleTexture();
            }
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
            return collisionRectangle.CollidesWith(x,y); //This uses the collision rectangle to check collisions, so that delegates can be used.
        }

        ///<summary>
        ///Checks if the sprite is colliding with another sprite and triggers the right click delegate.
        ///</summary>
        public bool Clicked(int x, int y, Clicks clickType){
            if(this.CollidesWith(x,y)){
                switch(clickType){
                    case Clicks.Left:
                        if(leftClickDelegate!=null){
                            return leftClickDelegate(this,x,y);
                        }
                        return true;
                    case Clicks.Middle:
                        if(middleClickDelegate!=null){
                            return middleClickDelegate(this,x,y);
                        }
                        return true;
                    case Clicks.Right:
                        if(rightClickDelegate!=null){
                            return rightClickDelegate(this,x,y);
                        }
                        return true;
                    case Clicks.WheelHover:
                        if(wheelHoverDelegate!=null){
                            return wheelHoverDelegate(this,x,y);
                        }
                        return true;
                    case Clicks.Hover:
                        if(hoverDelegate!=null){
                            return hoverDelegate(this,x,y);
                        }
                        return true;
                }
            }
            return true;
        }
    }
}