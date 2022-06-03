using Microsoft.Xna.Framework.Input;

namespace FCSG{
    public partial class Utilities{
        public static void UpdateMouse(MouseState mouseState, MouseHandler mouseHandler){
            if((mouseState.LeftButton==ButtonState.Pressed)){
                if(mouseHandler.IsNewDown(Clicks.Left)){
                    mouseHandler.Held(Clicks.Left,mouseState.X,mouseState.Y);
                }
                mouseHandler.Down(Clicks.Left, mouseState.X, mouseState.Y);
            }else{
                mouseHandler.Up(Clicks.Left,mouseState.X,mouseState.Y);
            }
            //TODO: finish this and reevaluate life choices (is a mouse handler really needed?)->I think not
        }
    }
}