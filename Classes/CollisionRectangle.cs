using Microsoft.Xna.Framework;

namespace FCSG{
    /// <summary>
    /// A class which represents a sprite's collision box. It was created so that delegates can be used to handle collisions. It can cast implicitly to a rectangle.
    /// </summary>
    public class CollisionRectangle{
        private LinkedVariable xVariable;
        private LinkedVariable yVariable;
        private LinkedVariable widthVariable;
        private LinkedVariable heightVariable;
        public SpriteBase sprite; //The sprite which this collision rectangle is associated with

        public int x{
            get{
                return xVariable;
            }
        }
        public int y{
            get{
                return yVariable;
            }
        }
        public int width{
            get{
                return widthVariable;
            }
        }
        public int height{
            get{
                return heightVariable;
            }
        }

        #region Constructors
        /// <summary>
        /// Construct a new CollisionRectangle with the given settings. It will need to be activated with CollisionRectangle.Activate() in order for it to work.
        /// </summary>
        public CollisionRectangle(LinkedVariableParams xVariable=null, LinkedVariableParams yVariable=null, LinkedVariableParams widthVariable=null, LinkedVariableParams heightVariable=null){
            if(xVariable!=null){
                this.xVariable=new LinkedVariable(this.sprite,xVariable);
            }else
                this.xVariable=new LinkedVariable(new LinkedVariableParams((SpriteBase sb)=>(int)sb.x, sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.xVariable})); //TODO: add a way to choose wether the coords are relative or not to the sprite.
            if(yVariable!=null){
                this.yVariable=new LinkedVariable(this.sprite,yVariable);
            }else
                this.yVariable=new LinkedVariable(new LinkedVariableParams((SpriteBase sb)=>(int)sb.y, sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.yVariable}));
            if(widthVariable!=null){
                this.widthVariable=new LinkedVariable(widthVariable);
            }else
                this.widthVariable=new LinkedVariable(new LinkedVariableParams((SpriteBase sb)=>(int)sb.width, sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.widthVariable}));
            if(heightVariable!=null){
                this.heightVariable=new LinkedVariable(heightVariable);
            }else
                this.heightVariable=new LinkedVariable(new LinkedVariableParams((SpriteBase sb)=>(int)sb.height, sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.heightVariable}));
        }

        public CollisionRectangle(LinkedVariable xVariable, LinkedVariable yVariable, LinkedVariable widthVariable, LinkedVariable heightVariable){
            this.xVariable=xVariable;
            this.yVariable=yVariable;
            this.widthVariable=widthVariable;
            this.heightVariable=heightVariable;
        }
        /// <summary>
        /// Construct a collision rectangle with values corresponding to the ones of the sprite.
        /// </summary>
        public CollisionRectangle(SpriteBase sprite){
            this.sprite=sprite;
            this.xVariable=new LinkedVariable(this.sprite,(SpriteBase sb)=>this.sprite.xVariable,new LinkedVariable[] {this.sprite.xVariable});
            this.yVariable=sprite.yVariable;
            this.widthVariable=sprite.widthVariable;
            this.heightVariable=sprite.heightVariable;
        }
        public CollisionRectangle(SpriteBase sprite, LinkedVariable xVariable=null, LinkedVariable yVariable=null, LinkedVariable widthVariable=null, LinkedVariable heightVariable=null){
            if(sprite!=null){
                this.sprite=sprite;
            }
            if(xVariable!=null){
                this.xVariable=xVariable;
            }else
                this.xVariable=new LinkedVariable(this.sprite,(SpriteBase sprite)=>(int)sprite.x, new LinkedVariable[] {sprite.xVariable}); //TODO: add a way to choose wether the coords are relative or not to the sprite.
            if(yVariable!=null){
                this.yVariable=yVariable;
            }else
                this.yVariable=new LinkedVariable(this.sprite,(SpriteBase sprite)=>(int)sprite.y, new LinkedVariable[] {sprite.yVariable});
            if(widthVariable!=null){
                this.widthVariable=widthVariable;
            }else
                this.widthVariable=new LinkedVariable(this.sprite,(SpriteBase sprite)=>(int)sprite.width, new LinkedVariable[] {sprite.widthVariable});
            if(heightVariable!=null){
                this.heightVariable=heightVariable;
            }else
                this.heightVariable=new LinkedVariable(this.sprite,(SpriteBase sprite)=>(int)sprite.height, new LinkedVariable[] {sprite.heightVariable});
        }
        #endregion Constructors

        public void Activate(SpriteBase sb){
            this.sprite=sb;
            this.xVariable.Activate(sb);
            this.yVariable.Activate(sb);
            this.widthVariable.Activate(sb);
            this.heightVariable.Activate(sb);
        }

        public void SetSprite(SpriteBase sprite){
            this.sprite=sprite;
        }

        /// <summary>
        /// Returns true if the given point is inside the collision rectangle. It is not relative to the rectangle.
        /// </summary>
        public bool CollidesWith(int x,int y){
            return x>=this.x && x<=this.x+this.width && y>=this.y && y<=this.y+this.height;
        }

        public static implicit operator Rectangle(CollisionRectangle collisionRectangle){
            return new Rectangle(collisionRectangle.x, collisionRectangle.y, collisionRectangle.width, collisionRectangle.height);
        }
    }
}