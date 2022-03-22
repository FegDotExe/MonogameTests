//Home to all the random methods and classes that are used in the FCSG namespace.

namespace FCSG{
    public delegate int IntSpriteDelegate(Sprite sprite);
    public delegate int IntSpriteObjDelegate(SpriteObject sprite);
    public delegate bool ClickDelegate(SpriteBase sprite, int x, int y);
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
    public enum Clicks{
        Left,
        Middle,
        Right,
        WheelHover,
        Hover
    }
}