using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace FCSG{
    public delegate int IntSpriteDelegate(Sprite sprite);
    public delegate int IntSpriteObjDelegate(SpriteObject sprite);

    ///<summary>
    ///A class which rapresents a sprite
    ///</summary>
    public class Sprite : SpriteObject{
        private SpriteBatch spriteBatch;
        //Position values
            public int x{
                get{return xDelegate(this);}
                set{xDelegate=(Sprite sprite)=>value;}
            }
            public int y{
                get{return yDelegate(this);}
                set{yDelegate=(Sprite sprite)=>value;}
            }
            private IntSpriteDelegate xDelegate;
            private IntSpriteDelegate yDelegate;
        //Size values
            private IntSpriteDelegate widthDelegate;
            private IntSpriteDelegate heightDelegate;
            public int width{
                get{return widthDelegate(this);}
                set{widthDelegate=(Sprite sprite)=>value;}
            }
            public int height{
                get{return heightDelegate(this);}
                set{heightDelegate=(Sprite sprite)=>value;}
            }
        public Texture2D texture{get;set;}
        public Color color{get;set;}
        public float rotation{get;set;}
        public Vector2 origin{get;set;}
        public float depth{get;set;}
        public SpriteEffects effects=SpriteEffects.None;
        public bool draw{get;set;}
        private List<ObjectGroup<Sprite>> groups{get;set;} //A list of all the groups this sprite is in

        //TODO: add ability to give int instead of delegate
        public Sprite(
                SpriteBatch spriteBatch, 
                Texture2D texture,
                ObjectGroup<Sprite> group=null,
                List<ObjectGroup<Sprite>> groups=null,
                float? depth=null, 
                IntSpriteDelegate xDelegate=null, 
                IntSpriteDelegate yDelegate=null,
                IntSpriteDelegate widthDelegate=null, 
                IntSpriteDelegate heightDelegate=null,
                float? rotation=null, 
                Vector2? origin=null, 
                Color? color=null
        ){
            this.spriteBatch = spriteBatch;
            this.texture = texture;

            if(depth != null) //Sets the depth of the sprite
                this.depth = (float)depth;
            else
                this.depth = 0;

            //Position delegates
                if(xDelegate != null)
                    this.xDelegate = xDelegate;
                else
                    this.xDelegate = (Sprite sprite) => 0;
                if(yDelegate != null)
                    this.yDelegate = yDelegate;
                else
                    this.yDelegate = (Sprite sprite) => 0;

            //Size delegates
                if(widthDelegate!=null)
                    this.widthDelegate = widthDelegate;
                else
                    this.widthDelegate = (Sprite sprite) => texture.Width;
                if(heightDelegate!=null)
                    this.heightDelegate = heightDelegate;
                else
                    this.heightDelegate = (Sprite sprite) => texture.Height;

            if(rotation!=null){ //Sets the rotation of the sprite
                this.rotation = (float)rotation;
            }else{
                this.rotation = 0;
            }

            if(origin!=null){
                this.origin = (Vector2)origin;
            }else{
                this.origin = new Vector2(0.5f,0.5f);
            }

            if(color!=null){
                this.color = (Color)color;
            }else{
                this.color = Color.White;
            }

            this.groups=new List<ObjectGroup<Sprite>>();

            if(group!=null){ //Adds the sprite to the group
                this.groups.Add(group);
                group.Add(this);
            }
            if (groups!=null){
                foreach(ObjectGroup<Sprite> spriteGroup in groups){
                    this.groups.Add(spriteGroup);
                    spriteGroup.Add(this);
                }
            }

            this.draw=true;
        }
        public void Draw(bool drawMiddle=true){
            if(draw){
                spriteBatch.Draw(texture, new Rectangle(this.x,this.y,this.width,this.height),null,color,rotation,origin,effects,depth);
            }
        }
    }

    public class TextSprite : SpriteBase{
        SpriteFont font;
        public string text{
            get{
                return _text;
            }
            set{
                _text=value;
                ElaborateTexture();
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
        public TextSprite(
            string text,
            SpriteFont font,
            SpriteBatch spriteBatch,
            WrapMode wrapMode=WrapMode.Word,
            LayoutMode layoutMode=LayoutMode.Left,
            float? depth=null,
            IntSpriteObjDelegate originalWidthDelegate=null,
            IntSpriteObjDelegate originalHeightDelegate=null,
            IntSpriteObjDelegate xDelegate=null, 
            IntSpriteObjDelegate yDelegate=null,
            IntSpriteObjDelegate widthDelegate=null, 
            IntSpriteObjDelegate heightDelegate=null,
            float? rotation=null, 
            Vector2? origin=null, 
            Color? color=null
        ):base(
            spriteBatch,
            depth,
            xDelegate,
            yDelegate,
            widthDelegate,
            heightDelegate,
            rotation,
            origin,
            color
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
            
            ElaborateTexture();
        }

        private void ElaborateTexture(){
            RenderTarget2D renderTarget = new RenderTarget2D(spriteBatch.GraphicsDevice, originalWidthDelegate(this), originalHeightDelegate(this));

            List<string> lines=toLines(text,wrapMode:this.wrapMode);

            float height = font.MeasureString("a").Y;
            int line=0;

            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();

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
                spriteBatch.DrawString(font,lineText,new Vector2(x,line*height),Color.White);
                line++;
            }

            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(null);

            texture=renderTarget;
        }

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
                            string[]  dividedLine=makeLine(tempLine,words[i],wrapMode:WrapMode.Character);
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
    
    public class Wrapper{
        private List<SpriteObject> sprites;
        private SpriteBatch spriteBatch;

        public Wrapper(SpriteBatch spriteBatch){
            this.spriteBatch = spriteBatch;
            sprites = new List<SpriteObject>();
        }

        public void NewSprite(
                Texture2D texture, 
                float? depth=null, 
                ObjectGroup<Sprite> group=null,
                List<ObjectGroup<Sprite>> groups=null,
                IntSpriteDelegate xDelegate=null, 
                IntSpriteDelegate yDelegate=null,
                IntSpriteDelegate widthDelegate=null, 
                IntSpriteDelegate heightDelegate=null,
                float? rotation=null, 
                Vector2? origin=null, 
                Color? color=null
            ){
            sprites.Add(new Sprite(spriteBatch, texture:texture, group:group, groups:groups, depth:depth, xDelegate:xDelegate, yDelegate:yDelegate, widthDelegate:widthDelegate, heightDelegate:heightDelegate, rotation:rotation, origin:origin, color:color));
        }

        public void Add(SpriteObject sprite){
            sprites.Add(sprite);
        }

        public void Draw(){
            spriteBatch.Begin(sortMode:SpriteSortMode.FrontToBack); //TODO: Should add options
            foreach(SpriteObject sprite in sprites){
                sprite.Draw();
            }
            spriteBatch.End();
        }
    }
}