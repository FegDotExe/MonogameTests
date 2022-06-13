using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;


namespace FCSG{
    /// <summary>
    /// An elaborate sprite which contains text.
    /// </summary>
    /// <remarks>
    /// The text is first drawn on a texture, the size of which is determined by originalWidth and originalHeight. The texture is then drawn on the screen. The texture is updated when ElaborateTexture() is called.
    /// </remarks>
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
        private LinkedVariable originalWidthVariable; //The size of the 2DRenderTarget used to first draw the text on; then the target is used to draw the text on the screen. This is done to save on performance.
        public int originalWidth{
            get{return originalWidthVariable;}
            set{originalWidthVariable.Set(value);}
        }
        private LinkedVariable originalHeightVariable; //The size of the 2DRenderTarget used to first draw the text on; then the target is used to draw the text on the screen. This is done to save on performance.
        public int originalHeight{
            get{return originalHeightVariable;}
            set{originalHeightVariable.Set(value);}
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
        private int _offsetX; //Offset of the text when Elaborated
        public int offsetX{
            get{
                return _offsetX;
            }
            set{
                _offsetX=value;
                ElaborateTexture(reloadDimension:false,reloadLines:false);
            }
        }
        private int _offsetY; //Offset of the text when Elaborated
        public int offsetY{
            get{
                return _offsetY;
            }
            set{
                _offsetY=value;
                ElaborateTexture(reloadDimension:false,reloadLines:false);
            }
        }
        public SpriteBatchParameters textBatchParameters;
        private List<string> lines;
        private RenderTarget2D renderTarget;
        
        /// <summary>
        /// Constructs a new TextSprite.
        /// </summary>
        public TextSprite(
            SpriteParameters spriteParameters
        ):base(
            spriteParameters: spriteParameters
        ){
            this.font = spriteParameters.font;
            this._text = spriteParameters.text;// Private reference is used so that the texture isn't elaborated before it can be
            
            if(this.font==null || this._text==null){
                throw new ArgumentException("The font or text of the sprite is null");
            }
            
            if(spriteParameters.originalHeightVariable!=null)
                this.originalHeightVariable = new LinkedVariable(this,spriteParameters.originalHeightVariable);
            else
                this.originalHeightVariable = new LinkedVariable(this, (SpriteBase sprite) => 1000);
            if(spriteParameters.originalWidthVariable!=null)
                this.originalWidthVariable = new LinkedVariable(this,spriteParameters.originalWidthVariable);
            else
                this.originalWidthVariable = new LinkedVariable(this, (SpriteBase sprite) => 1000);

            if(spriteParameters.textBatchParameters!=null)
                this.textBatchParameters = spriteParameters.textBatchParameters;
            else
                this.textBatchParameters = new SpriteBatchParameters();

            this.wrapMode = spriteParameters.wrapMode;
            this.layoutMode = spriteParameters.layoutMode;
            this._offsetX = spriteParameters.offsetX;
            this._offsetY = spriteParameters.offsetY;
            
            ElaborateTexture();
        }

        /// <summary>
        /// Updates the texture of the TextSprite.
        /// </summary>
        private void ElaborateTexture(bool reloadDimension=true,bool reloadLines=true){
            if(reloadDimension){
                renderTarget = new RenderTarget2D(spriteBatch.GraphicsDevice, originalWidthVariable, originalHeightVariable);
            }

            if(reloadLines){
                lines=toLines(text,wrapMode:this.wrapMode);
            }

            int height=font.LineSpacing;
            int line=0;

            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(this.textBatchParameters);

            foreach(string lineText in lines){
                int x=0;
                switch(layoutMode){
                    case LayoutMode.Left:
                        x=0;
                        break;
                    case LayoutMode.Center:
                        x=(int)((originalWidthVariable-font.MeasureString(lineText).X)/2);
                        break;
                    case LayoutMode.Right:
                        x=(int)(originalWidthVariable-font.MeasureString(lineText).X);
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
            BasicDraw(this.spriteBatch,drawMiddle);
        }
        public override void BasicDraw(SpriteBatch spriteBatch, bool drawMiddle = true)
        {
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