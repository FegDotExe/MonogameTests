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
            #region ClickDelegates
            protected ClickDelegate _leftClickDelegate;
            public ClickDelegate leftClickDelegate{
                get{
                    return _leftClickDelegate;
                }
                set{
                    _leftClickDelegate = value;
                    if(value!=null){
                        wrapper.leftClick.Add(this);
                    }else{
                        wrapper.leftClick.Remove(this);
                    }
                }
            }
            protected ClickDelegate _middleClickDelegate;
            public ClickDelegate middleClickDelegate{
                get{
                    return _middleClickDelegate;
                }
                set{
                    _middleClickDelegate = value;
                    if(value!=null){
                        wrapper.middleClick.Add(this);
                    }else{
                        wrapper.middleClick.Remove(this);
                    }
                }
            }
            protected ClickDelegate _rightClickDelegate;
            public ClickDelegate rightClickDelegate{
                get{
                    return _rightClickDelegate;
                }
                set{
                    _rightClickDelegate = value;
                    if(value!=null){
                        wrapper.rightClick.Add(this);
                    }else{
                        wrapper.rightClick.Remove(this);
                    }
                }
            }
            protected ClickDelegate _wheelHoverDelegate;
            public ClickDelegate wheelHoverDelegate{
                get{
                    return _wheelHoverDelegate;
                }
                set{
                    _wheelHoverDelegate = value;
                    if(value!=null){
                        wrapper.wheelHover.Add(this);
                    }else{
                        wrapper.wheelHover.Remove(this);
                    }
                }
            }
            protected ClickDelegate _hoverDelegate;
            public ClickDelegate hoverDelegate{
                get{
                    return _hoverDelegate;
                }
                set{
                    _hoverDelegate = value;
                    if(value!=null){
                        wrapper.hover.Add(this);
                    }else{
                        wrapper.hover.Remove(this);
                    }
                }
            }
            #endregion ClickDelegates
        protected bool precise;
        public Dictionary<string,object> variables{get; protected set;}

        protected CollisionRectangle collisionRectangle; //A rectangle used for collision detection. its coordinates are relative to the sprite's position, and so is the size.
        #endregion Fields
        #region Constructors
        public SpriteBase(
            SpriteParameters spriteParameters
        ){
            this.spriteBatch = spriteParameters.spriteBatch;

            this.wrapper = spriteParameters.wrapper;
            //Add to wrapper
            if(this.wrapper!=null){
                this.wrapper.Add(this);
            }

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
                this._leftClickDelegate = spriteParameters.leftClickDelegate;
                this._middleClickDelegate = spriteParameters.middleClickDelegate;
                this._rightClickDelegate = spriteParameters.rightClickDelegate;
                this._wheelHoverDelegate = spriteParameters.wheelHoverDelegate;
                this._hoverDelegate = spriteParameters.hoverDelegate;
                
                this.precise=spriteParameters.precise;

                bool enableCollisionRectangle=true; //Controls wether the collision rectangle is enabled or not
                if(this.leftClickDelegate==null && this.middleClickDelegate==null && this.rightClickDelegate==null && this.wheelHoverDelegate==null && this.hoverDelegate==null){
                    enableCollisionRectangle=false;
                }

            //Collision rectangle
            if(spriteParameters.collisionRectangle!=null || enableCollisionRectangle){
                if(spriteParameters.collisionRectangle!=null){
                    this.collisionRectangle=spriteParameters.collisionRectangle; //Use the collision rectangle provided by the parameters
                }else{
                    this.collisionRectangle=new CollisionRectangle(this); //Create a new collision rectangle, just in case there are click delegates but the collision rectangle was not defined in the parameters
                }
            }

            this.xVariable.Activate();
            this.yVariable.Activate();
            this.widthVariable.Activate();
            this.heightVariable.Activate();
            if(this.collisionRectangle!=null && this.collisionRectangle.sprite==null){
                this.collisionRectangle.Activate(this);
            }

            if(spriteParameters.variables!=null){
                this.variables=spriteParameters.variables;
            }else{
                this.variables=new Dictionary<string,object>();
            }

            this.draw=true;
        }
        #endregion Constructors
        public virtual void Draw(bool drawMiddle=true){
            BasicDraw(this.spriteBatch,drawMiddle);
        }

        /// <summary>
        /// The bare skeleton of the draw method. Should be used in every context in which a custom spriteBatch is used (== everywhere except in Wrapper)
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
            if(collisionRectangle==null){
                return false;
            }
            return collisionRectangle.CollidesWith(x,y); //This uses the collision rectangle to check collisions, so that delegates can be used.
        }

        ///<summary>
        ///Checks if the sprite is colliding with another sprite and triggers the right click delegate.
        ///</summary>
        public bool Clicked(int x, int y, Clicks clickType){
            if((!precise && this.CollidesWith(x,y))||(precise && this.CollidesWith(x,y) && this.IsolateTexture().GetPixel(x,y).A!=0)){ //TODO: a complete resize is needed: there should be a rendertarget to test for precise click.
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

        ///<summary>
        ///Draws this and only this texture to a new texture of the size of the screen.
        ///</summary>
        private Texture2D IsolateTexture(){
            RenderTarget2D renderTarget=new RenderTarget2D(spriteBatch.GraphicsDevice,spriteBatch.GraphicsDevice.Viewport.Width,spriteBatch.GraphicsDevice.Viewport.Width);
            Utilities.DrawOntoTarget(renderTarget,this,spriteBatch);
            return renderTarget;
        }
    }
}