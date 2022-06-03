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
}