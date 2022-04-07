using Microsoft.Xna.Framework;

namespace FCSG{
    #region Description
    ///<summary>
    ///This class is used to wrap the mouse input system. 
    ///<para>
    ///Here are its GET functions:
    ///<list type="bullet">
    ///<item>
    ///<term>IsDown(Clicks)</term>
    ///<description>Returns true if the mouse button is down.</description>
    ///</item>
    ///<item>
    ///<term>IsNewDown(Clicks)</term>
    ///<description>Returns true if the mouse button is down and not held yet.</description>
    ///</item>
    ///<item>
    ///<term>IsHeld(Clicks)</term>
    ///<description>Returns true if the mouse button is held.</description>
    ///</item>
    ///<item>
    ///<term>IsUp(Clicks)</term>
    ///<description>Returns true if the mouse button is up.</description>
    ///</item>
    ///</list>
    ///</para>
    ///<para>
    ///Here are its SET functions:
    ///<list type="bullet">
    ///<item>
    ///<term>Down(Clicks clickType, int x, int y)</term>
    ///<description>Sets the click to down.</description>
    ///</item>
    ///</list>
    ///</para>
    ///</summary>
    #endregion Description
    //TODO: finish documentation
    public class MouseHandler{
        private BoolClick left;
        private BoolClick middle;
        private BoolClick right;
        private int scrolled;

        private int xHover;
        private int yHover;

        //Get functions
        public bool IsDown(Clicks clickType){
            switch(clickType){
                case Clicks.Left:
                    return left.down;
                case Clicks.Middle:
                    return middle.down;
                case Clicks.Right:
                    return right.down;
                default:
                    return false;
            }
        }
        public bool IsNewDown(Clicks clickType){
            switch(clickType){
                case Clicks.Left:
                    return (left.down && !left.held);
                case Clicks.Middle:
                    return (middle.down && !middle.held);
                case Clicks.Right:
                    return (right.down && !right.held);
                default:
                    return false;
            }
        }
        public bool IsHeld(Clicks clickType){
            switch(clickType){
                case Clicks.Left:
                    return left.held;
                case Clicks.Middle:
                    return middle.held;
                case Clicks.Right:
                    return right.held;
                default:
                    return false;
            }
        }
        public bool IsUp(Clicks clickType){
            switch(clickType){
                case Clicks.Left:
                    return left.up;
                case Clicks.Middle:
                    return middle.up;
                case Clicks.Right:
                    return right.up;
                default:
                    return false;
            }
        }
        public Vector2 GetPosition(Clicks clickType){
            switch(clickType){
                case Clicks.Left:
                    return new Vector2(left.x,left.y);
                case Clicks.Middle:
                    return new Vector2(middle.x,middle.y);
                case Clicks.Right:
                    return new Vector2(right.x,right.y);
                case Clicks.Hover:
                    return new Vector2(xHover,yHover);
                default:
                    return Vector2.Zero;
            }
        }
        public int GetScrolled(){
            return scrolled;
        }
        public BoolClick GetBoolClick(Clicks clickType){
            switch(clickType){
                case Clicks.Left:
                    return left;
                case Clicks.Middle:
                    return middle;
                case Clicks.Right:
                    return right;
                default:
                    return null;
            }
        }
    
        //Set functions
        public void Down(Clicks clickType, int x, int y){
            switch(clickType){
                case Clicks.Left:
                    left.down=true;
                    left.x=x;
                    left.y=y;
                    break;
                case Clicks.Middle:
                    middle.down=true;
                    middle.x=x;
                    middle.y=y;
                    break;
                case Clicks.Right:
                    right.down=true;
                    right.x=x;
                    right.y=y;
                    break;
            }
        }
        public void Held(Clicks clickType, int x, int y){
            switch(clickType){
                case Clicks.Left:
                    if(!left.down){
                        return;
                    }
                    left.held=true;
                    left.x=x;
                    left.y=y;
                    break;
                case Clicks.Middle:
                    if(!middle.down){
                        return;
                    }
                    middle.held=true;
                    middle.x=x;
                    middle.y=y;
                    break;
                case Clicks.Right:
                    if(!right.down){
                        return;
                    }
                    right.held=true;
                    right.x=x;
                    right.y=y;
                    break;
            }
        }
        public void Up(Clicks clickType, int x, int y){
            switch(clickType){
                case Clicks.Left:
                    left.up=true;
                    left.x=x;
                    left.y=y;
                    break;
                case Clicks.Middle:
                    middle.up=true;
                    middle.x=x;
                    middle.y=y;
                    break;
                case Clicks.Right:
                    right.up=true;
                    right.x=x;
                    right.y=y;
                    break;
            }
        }
    
        public void Hover(int x, int y){
            xHover=x;
            yHover=y;
        }
        public void Scroll(int scroll){
            scrolled=scroll;
        }
    
        //Constructor
        public MouseHandler(){
            left=new BoolClick();
            middle=new BoolClick();
            right=new BoolClick();
            scrolled=0;
            xHover=0;
            yHover=0;
        }
    }

    /// <summary>
    /// Represents a click, with a bool for being down, held and up. It also contains x and y values to locate the click.
    /// </summary>
    public class BoolClick{
        public bool down;
        public bool held;
        public bool up{
            get{
                return !down;
            }
            set{
                this.down=!value;
                if(value==true && held==true){
                    held=false;
                }
            }
        }
        public int x;
        public int y;

        //Even though these classes would not be necessary, I decided to include them anyways just to make things a lil bit more comfy
        public void setDown(int x, int y){
            down=true;
            this.x=x;
            this.y=y;
        }
        
        public void setUp(int x, int y){
            down=false;
            held=false;
            this.x=x; //Even if updating these values might seem useless, it might actually be useful in situations in which you want to know at which coords the mouse was released at.
            this.y=y;
        }
        
        public void setHeld(int x, int y){
            held=true;
            this.x=x;
            this.y=y;
        }

        //Constructor, though useless
        public BoolClick(){
            this.x=0;
            this.y=0;
            this.down=false;
            this.held=false;
        }
    }
}