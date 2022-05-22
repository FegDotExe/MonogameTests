using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using FCSG;

namespace MonogameTests
{
    public class Game1 : Game
    {
        bool left_held=false;
        bool middle_held=false;
        bool right_held=false;
        Dictionary<string, SpriteBase> spriteDict;
        Texture2D totem_of_time;
        Texture2D red;
        Texture2D blue;
        SpriteFont font;

        TextSprite texto;
        Sprite randomSprite;
        Sprite colorSprite; //Should make a dictionary

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Wrapper wrapper;

        LinkedVariable x=new LinkedVariable((SpriteBase sb)=>0);
        LinkedVariable y=new LinkedVariable((SpriteBase sb)=>0);
        float fps=0f;
        ObjectGroup<SpriteObject> group;

        private GameClock clock;
        private TimeEvent timeEvent=null;

        private LinkedVariable resizeVariableW;
        private LinkedVariable resizeVariableH;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            IsFixedTimeStep=true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteDict = new Dictionary<string, SpriteBase>();
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            wrapper=new Wrapper(_spriteBatch);

            totem_of_time=Content.Load<Texture2D>("assets/totem_of_time");
            red=Content.Load<Texture2D>("red");
            blue=Content.Load<Texture2D>("blue");
            Texture2D white=Content.Load<Texture2D>("white");
            Texture2D whiteframe=Content.Load<Texture2D>("whiteframe");
            Texture2D redl=Content.Load<Texture2D>("redl");
            font=Content.Load<SpriteFont>("FreeSans");

            //Precise texturing test
            Texture2D precise=Content.Load<Texture2D>("precise");
            Console.WriteLine("Precise pixel: "+precise.GetPixel(0,0));
            Console.WriteLine("Precise pixel: "+precise.GetPixel(0,1));
            Console.WriteLine("Precise pixel: "+precise.GetPixel(1,0));
            Console.WriteLine("Precise pixel: "+precise.GetPixel(1,1));

            group=new ObjectGroup<SpriteObject>();

            colorSprite=new Sprite(
                new SpriteParameters(spriteBatch:_spriteBatch,
                texture:whiteframe,
                depth:0.5f,
                x:200,
                y:0,
                width:100,
                height:100,
                color: new Color(0,255,0,255))
            );

            LinkedVariable sVariableTest=new LinkedVariable(colorSprite, (SpriteBase sb)=>"A");
            LinkedVariable sVb=new LinkedVariable(colorSprite, (SpriteBase sb)=>sVariableTest+"B", new LinkedVariable[] {sVariableTest});
            LinkedVariable sVc=new LinkedVariable(colorSprite, (SpriteBase sb)=>sVariableTest+"C", new LinkedVariable[] {sVariableTest});
            LinkedVariable sVd=new LinkedVariable(colorSprite, (SpriteBase sb)=>(string)sVariableTest+sVb+sVc+"D", new LinkedVariable[] {sVariableTest,sVb,sVc});
            LinkedVariable sVe=new LinkedVariable(colorSprite, (SpriteBase sb)=>sVb+"E", new LinkedVariable[] {sVb});
            sVariableTest.Set("X");

            //wrapper.Add(colorSprite);

            LayerGroup group2=new LayerGroup();

            int x_chess=10;
            int y_chess=10;
            RenderTarget2D chessThing=new RenderTarget2D(GraphicsDevice,16*x_chess,16*y_chess);
            for(int i=0; i<x_chess;i++){
                for(int j=0; j<y_chess;j++){
                    Color thisColor=new Color(0,255,255,255);
                    if((i+j)%2==0){
                        thisColor=new Color(0,0,255,255);
                    }
                    //Console.WriteLine("Adding "+i+","+j);
                    group2.Add(new Sprite(
                        new SpriteParameters(spriteBatch:_spriteBatch,
                        texture:whiteframe,
                        depth:0.5f,
                        x:i*16,
                        y:j*16,
                        width:16,
                        height:16,
                        color: thisColor)
                    ));
                }
            }

            TextureGenerator chessTextureGenerator=new TextureGenerator(GraphicsDevice,group2,16*x_chess,16*y_chess,_spriteBatch);

            resizeVariableW=new LinkedVariable(null, (SpriteBase sb)=>GraphicsDevice.Viewport.Width);
            resizeVariableH=new LinkedVariable(null, (SpriteBase sb)=>GraphicsDevice.Viewport.Height);

            wrapper.Add(new Sprite(
                new SpriteParameters(spriteBatch:_spriteBatch,
                texture:chessTextureGenerator.Generate(),
                depth:0.1f,
                xVariable:new LinkedVariableParams((SpriteBase sb)=>(GraphicsDevice.Viewport.Width-Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height))/2,sensitiveVariables:new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                y:0,
                widthVariable:new LinkedVariableParams((SpriteBase sb)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                heightVariable:new LinkedVariableParams((SpriteBase so)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                spritesDict: spriteDict,
                dictKey: "chess",
                leftClickDelegate: (SpriteBase sb, int x, int y)=>false)
            ));

            int x_pos=3;
            int y_pos=3;
            wrapper.Add(new Sprite(
                new SpriteParameters(spriteBatch:_spriteBatch,
                texture:white,
                depth:0.11f,
                xVariable:new LinkedVariableParams(
                    (SpriteBase sb)=>Math.Round((double)((int)spriteDict["chess"].x+((((double)spriteDict["chess"].width)/(double)x_chess)*x_pos))), 
                    new LinkedVariable[] {spriteDict["chess"].xVariable}
                ),
                yVariable:new LinkedVariableParams(
                    (SpriteBase sb)=>(int)Math.Round((double)(spriteDict["chess"].height+spriteDict["chess"].y-sb.height-((((double)spriteDict["chess"].width)/(double)x_chess)*y_pos))),
                    sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {spriteDict["chess"].yVariable,sb.heightVariable}
                ),
                widthVariable:new LinkedVariableParams((SpriteBase so)=>(int)Math.Round(((double)spriteDict["chess"].width)/(double)x_chess),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                heightVariable:new LinkedVariableParams((SpriteBase so)=>(int)Math.Round(((double)spriteDict["chess"].width)/(double)y_chess),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                spritesDict: spriteDict,
                dictKey: "cock"
                )
            ));

            // Example of surface-draw->way to create new 2d texture
            // Methods.createGrid(100,100,blue,GraphicsDevice,spriteGroup:group,_spriteBatch);
            // RenderTarget2D renderTarget=new RenderTarget2D(GraphicsDevice,2000,2000);
            // Utilities.DrawOntoTarget(renderTarget,new ObjectGroup<Object2D>(group.objects.ConvertAll<Object2D>(x=>(Object2D)x)),_spriteBatch);
            
            //randomSprite=wrapper.NewSprite(renderTarget,widthVariable:(SpriteObject sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),heightVariable:(SpriteObject sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),xDelegate:(SpriteObject sprite)=>x,yDelegate:(SpriteObject sprite)=>y);
            randomSprite=new Sprite(new SpriteParameters(_spriteBatch,red,
                widthVariable:new LinkedVariableParams((SpriteBase sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                heightVariable:new LinkedVariableParams((SpriteBase sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                xVariable:new LinkedVariableParams((SpriteBase sprite)=>x), //This sets the thing to literally be another LinkedList, so that it updates when it is needed to
                yVariable:new LinkedVariableParams((SpriteBase sprite)=>(int)y,new LinkedVariable[] {y}), //This is just another way to do the same exact thing
                leftClickDelegate:(SpriteBase sprite, int x, int y)=>{
                    if(randomSprite.texture==red){
                        randomSprite.texture=blue;
                    }else{
                        randomSprite.texture=red;
                    }
                    return true;
                }, 
                spritesDict: spriteDict,
                dictKey:"bigSquare")
            );
            wrapper.Add(randomSprite);

            texto=new TextSprite(
                new SpriteParameters(text:"",font:font,spriteBatch:_spriteBatch,
                widthVariable:new LinkedVariableParams((SpriteBase sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height)/2),
                heightVariable:new LinkedVariableParams((SpriteBase sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height)/2),
                wrapMode:TextSprite.WrapMode.Word,originalHeightVariable:new LinkedVariableParams((SpriteBase sprite)=>2000),originalWidthVariable:new LinkedVariableParams((SpriteBase sprite)=>2000),depth:1f)
            );

            //TextSprite texto=new TextSprite("AaaaaaaaaaaaaaaaAAAAAAAAAAAAAaaaaaaaaaaaaaaaAAAaaaa",font,_spriteBatch,widthDelegate:(SpriteObject sprite)=>400,heightDelegate:(SpriteObject sprite)=>400,wrapMode:TextSprite.WrapMode.Word,originalHeightDelegate:(SpriteObject sprite)=>2000,originalWidthDelegate:(SpriteObject sprite)=>1000);
            // wrapper.NewSprite(texto.texture,depth:1f,widthDelegate:(Sprite sprite)=>400,heightDelegate:(Sprite sprite)=>800);
            wrapper.Add(texto);

            //wrapper.NewSprite(blue,width:100,height:100,xDelegate:(SpriteObject sprite)=>x);
            //wrapper.NewSprite(blue,width:100,height:100,x:100,y:50);

            // LayerGroup layerGroup=new LayerGroup();
            // layerGroup.Add(randomSprite);
            // layerGroup.Add(texto);
            // layerGroup.Add(wrapper.NewSprite(blue,width:100,height:100,depth:0.5f));
            // layerGroup.Add(wrapper.NewSprite(red,width:100,height:100,x:100,y:50,depth:0.5f));
            // foreach(SpriteBase sprite in layerGroup.objects){
            //     Console.WriteLine("depth: "+sprite.depth);
            // }

            clock=new GameClock();
            
            Console.WriteLine("Supposed width: "+GraphicsDevice.Viewport.Width); //This is how you ACTUALLY get the size of the user window
        }

        protected override void Update(GameTime gameTime)
        {
            resizeVariableW.Set(GraphicsDevice.Viewport.Width);
            resizeVariableH.Set(GraphicsDevice.Viewport.Height);
            // Console.WriteLine((int)resizeVariable);

            double requiredTime=0.1d;
            int amount=100;
            //Console.WriteLine("x: "+x);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if(Keyboard.GetState().IsKeyDown(Keys.D)){
                //x=x+1;
                if(timeEvent==null){
                    int xCopy=x;
                    timeEvent=new TimeEvent(requiredTime,(double time)=>{x.Set(xCopy+(int)(amount*time));}, (double time)=>{x.Set(xCopy+amount);},(int)clock.elapsed);
                }
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.A)){
                if(timeEvent==null){
                    int xCopy=x;
                    timeEvent=new TimeEvent(requiredTime,(double time)=>{x.Set(xCopy+(int)(-amount*time));}, (double time)=>{x.Set(xCopy-amount);},(int)clock.elapsed);
                }
            }
            if(Keyboard.GetState().IsKeyDown(Keys.W)){
                y.Set(y-1);
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.S)){
                y.Set(y+1);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.L)){
                //randomSprite.Remove();
                spriteDict["bigSquare"].draw=false;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.K)){
                spriteDict["bigSquare"].draw=true;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down)){
                //texto.offsetY+=10;
                if(colorSprite.color.G>0){
                    colorSprite.color.G=(byte)(colorSprite.color.G-1);
                    colorSprite.color.B=(byte)(255-colorSprite.color.G);
                }
            }else if(Keyboard.GetState().IsKeyDown(Keys.Up)){
                //texto.offsetY-=10;
                if(colorSprite.color.G<255){
                    colorSprite.color.G=(byte)(colorSprite.color.G+1);
                    colorSprite.color.B=(byte)(255-colorSprite.color.G);
                }
            }

            //TODO: add an input controller to check if mouse was just pressed or is being held down.
            if(!left_held && Mouse.GetState().LeftButton==ButtonState.Pressed){
                wrapper.Click(Clicks.Left,Mouse.GetState().X,Mouse.GetState().Y);
                left_held=true;
            }
            if(left_held && Mouse.GetState().LeftButton==ButtonState.Released){
                // Console.WriteLine("Released");
                left_held=false;
            }
            if(Mouse.GetState().ScrollWheelValue!=0){
                // Console.WriteLine("Mouse scroll: "+Mouse.GetState().ScrollWheelValue);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {if(gameTime.ElapsedGameTime.Milliseconds!=0){
                fps=(float)(1000/gameTime.ElapsedGameTime.Milliseconds);
            }
            else{
                fps=0;
            }
            //Console.WriteLine("FPS: "+fps);

            texto.text="FPS: "+fps;

            clock.Update();
            //Console.WriteLine("Elapsed time: "+clock.elapsed);
            
            if(timeEvent!=null){
                timeEvent.RunFunction();
                if(timeEvent.isFinished){
                    timeEvent=null;
                }
            }

            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(new Color(0, 0, 0));

            wrapper.Draw();

            // _spriteBatch.Begin();
            // _spriteBatch.Draw(red,new Rectangle(0,0,100,100),Color.White);
            // _spriteBatch.End();

            // _spriteBatch.Begin();
            // _spriteBatch.Draw(totem_of_time, new Vector2(100, 100), Color.White);
            // _spriteBatch.End();//The program waits for the spritebatch to finish before continuing

            // RenderTarget2D renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            // GraphicsDevice.SetRenderTarget(renderTarget);

            // _spriteBatch.Begin();

            // GraphicsDevice.Clear(Color.Transparent);
            // // _spriteBatch.Begin();
            // _spriteBatch.DrawString(font,"Yoyo",new Vector2(x,y),Color.White,0f,new Vector2(0,0),1f,SpriteEffects.None,0f);
            // // _spriteBatch.End();

            // GraphicsDevice.SetRenderTarget(null);

            // _spriteBatch.Draw((Texture2D)renderTarget, new Vector2(0, 0), Color.White);
            // _spriteBatch.End();

            base.Draw(gameTime);
        }
    }

    public class Methods{
        private const int DIVIDER=100;
        public static void createGrid(int xSize, int ySize, Texture2D texture, GraphicsDevice graphicsDevice, ObjectGroup<SpriteObject> spriteGroup, SpriteBatch spriteBatch){
            for(int i=0;i<xSize;i++){
                int copyX=i;
                for(int j=0;j<ySize;j++){
                    int copyY=j;
                    new Sprite(
                        new SpriteParameters(spriteBatch,
                        texture,
                        depth:0f,
                        // xDelegate: (SpriteObject sprite)=>(copyX*(graphicsDevice.Viewport.Width/DIVIDER)), 
                        // yDelegate: (SpriteObject sprite)=>(copyY*(graphicsDevice.Viewport.Width/DIVIDER)), 
                        // widthDelegate: (SpriteObject sprite)=>graphicsDevice.Viewport.Width/DIVIDER, 
                        // heightDelegate: (SpriteObject sprite)=>graphicsDevice.Viewport.Width/DIVIDER,
                        group:spriteGroup)
                    );
                }
            }
        }
    }
}
