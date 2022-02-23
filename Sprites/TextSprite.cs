using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FCSG{
    /// <summary>
    /// An elaborate sprite which contains text.
    /// </summary>
    public class TextSprite : SpriteBase{
        SpriteFont font;
        public string text{
            get{
                return _text;
            }
            set{
                _text=value;
                ElaborateTexture(reloadDimension:false);
            }
        }
        private string _text;
        private IntSpriteObjDelegate originalWidthDelegate;
        public int originalWidth{
            get{return originalWidthDelegate(this);}
            set{originalWidthDelegate=(SpriteObject sprite)=>value;}
        }
        private IntSpriteObjDelegate originalHeightDelegate;
        public int originalHeight{
            get{return originalHeightDelegate(this);}
            set{originalHeightDelegate=(SpriteObject sprite)=>value;}
        }
        public enum WrapMode {
            Word,
            Character
        }
        public enum LayoutMode {
            Left,
            Center,
            Right
        }
        public WrapMode wrapMode;
        public LayoutMode layoutMode;
        private int _offsetX;
        public int offsetX{
            get{
                return _offsetX;
            }
            set{
                _offsetX=value;
                ElaborateTexture(reloadDimension:false,reloadLines:false);
            }
        }
        private int _offsetY;
        public int offsetY{
            get{
                return _offsetY;
            }
            set{
                _offsetY=value;
                ElaborateTexture(reloadDimension:false,reloadLines:false);
            }
        }
        private List<string> lines;
        private RenderTarget2D renderTarget;
        public TextSprite(
            string text,
            SpriteFont font,
            SpriteBatch spriteBatch,
            Wrapper wrapper=null,
            WrapMode wrapMode=WrapMode.Word,
            LayoutMode layoutMode=LayoutMode.Left,
            int offsetX=0,
            int offsetY=0,
            float? depth=null,
            IntSpriteObjDelegate originalWidthDelegate=null,
            IntSpriteObjDelegate originalHeightDelegate=null,
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
            List<ObjectGroup<SpriteObject>> groups=null
        ):base(
            spriteBatch:spriteBatch,
            wrapper:wrapper,
            depth:depth,
            xDelegate:xDelegate,
            yDelegate:yDelegate,
            widthDelegate:widthDelegate,
            heightDelegate:heightDelegate,
            rotation:rotation,
            origin:origin,
            color:color,
            group:group,
            groups:groups,
            x:x,
            y:y,
            width:width,
            height:height
        ){
            this.font = font;
            this._text = text;// Private reference is used so that the texture isn't elaborated before it can be
            if(originalHeightDelegate!=null)
                this.originalHeightDelegate = originalHeightDelegate;
            else
                this.originalHeightDelegate = (SpriteObject sprite) => 1000;
            if(originalWidthDelegate!=null)
                this.originalWidthDelegate = originalWidthDelegate;
            else
                this.originalWidthDelegate = (SpriteObject sprite) => 1000;
            this.wrapMode = wrapMode;
            this.layoutMode = layoutMode;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            
            ElaborateTexture();
        }

        /// <summary>
        /// Updates the texture of the sprite.
        /// </summary>
        private void ElaborateTexture(bool reloadDimension=true,bool reloadLines=true){
            if(reloadDimension){
                renderTarget = new RenderTarget2D(spriteBatch.GraphicsDevice, originalWidthDelegate(this), originalHeightDelegate(this));
            }

            if(reloadLines){
                lines=toLines(text,wrapMode:this.wrapMode);
            }

            int height=font.LineSpacing;
            int line=0;

            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState:SamplerState.PointClamp);

            foreach(string lineText in lines){
                int x=0;
                switch(layoutMode){
                    case LayoutMode.Left:
                        x=0;
                        break;
                    case LayoutMode.Center:
                        x=(int)((originalWidthDelegate(this)-font.MeasureString(lineText).X)/2);
                        break;
                    case LayoutMode.Right:
                        x=(int)(originalWidthDelegate(this)-font.MeasureString(lineText).X);
                        break;
                }
                spriteBatch.DrawString(font,lineText,new Vector2(x+offsetX,(line*height)+offsetY),Color.White);
                line++;
            }

            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(null);

            texture=renderTarget;
        }

        /// <summary>
        /// Splits a string into lines, coherently with the settings of the TextSprite
        /// </summary>
        private List<string> toLines(string text,WrapMode wrapMode=WrapMode.Character){
            List<string> lines = new List<string>();
            if(wrapMode==WrapMode.Character){
                string tempLine = "";
                for(int i=0;i<text.Length;i++){
                    if(font.MeasureString(tempLine+text[i]).X<originalWidth && i!=text.Length-1){
                        tempLine+=text[i];
                    }else{
                        if(i==text.Length-1){
                            tempLine+=text[i];
                        }

                        lines.Add(tempLine);

                        tempLine="";
                        if((char)text[i]!=' '){
                            tempLine+=text[i];
                        }
                    }
                }
            }else{
                string[] words=text.Split(' ');
                string tempLine = "";
                for(int i=0; i<words.Length; i++){
                    if(font.MeasureString(tempLine+words[i]).X<originalWidth && i!=words.Length-1){
                        tempLine+=words[i]+" ";
                    }else{
                        if(i==words.Length-1){
                            string[] dividedLine=makeLine(tempLine,words[i],wrapMode:WrapMode.Character);
                            // Console.WriteLine("TempLine: "+tempLine);
                            // Console.WriteLine("Left: "+dividedLine[0]);
                            // Console.WriteLine("Right: "+dividedLine[1]);

                            tempLine=dividedLine[0];

                            lines.Add(tempLine);
                            tempLine="";

                            List<string> additional=toLines(dividedLine[1]);
                            foreach(string line in additional){
                                lines.Add(line);
                            }
                        }
                        else{
                            if(tempLine.Length>0 && tempLine[tempLine.Length-1]==' '){ //Deletes space at the end of the line
                                tempLine=tempLine.Substring(0,tempLine.Length-1);
                            }
                            lines.Add(tempLine);

                            tempLine="";
                            if(words[i]!=""){
                                tempLine+=words[i]+" ";
                            }
                        }
                    }
                }
            }
            return lines;
        }
        
        /// <summary>
        /// Adds the needed part of right to left and returns the new left with what remains of right. This method is made to fix the edge case in which the last line of a string exceeds the width of the texture on which it should be drawn.
        /// </summary>
        private string[] makeLine(string left, string right, WrapMode wrapMode=WrapMode.Character){
            // Console.WriteLine("Initial left: "+left);
            for(int i=0; i<right.Length;i++){
                if(font.MeasureString(left+right[i]).X<originalWidth){
                    left+=right[i];
                }else{
                    if(i==right.Length-1){
                        left+=right[i];
                    }

                    // Console.WriteLine("Final left: "+left);
                    return new string[]{left,right.Substring(i)};
                }
            }
            
            return new string[] {left,""};
        }

        public override void Draw(bool drawMiddle=true){
            if(draw){
                //Console.WriteLine("Outside of a sprite resize: w="+midWidth+"/"+width+" h="+midHeight+"/"+height);
                if(drawMiddle==true){
                    DrawMiddleTexture();
                }
                spriteBatch.Draw(texture, new Rectangle(this.x,this.y,this.width,this.height),null,color,rotation,origin,effects,depth);
            }
        }
    }
}