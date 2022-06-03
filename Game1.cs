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

            SpriteParameters defaultParameters=new SpriteParameters(spriteBatch:_spriteBatch);

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

            wrapper.Add(new Sprite( //FIXME: uncomment this
                new SpriteParameters(spriteBatch:_spriteBatch,
                texture:chessTextureGenerator.Generate(),
                depth:0.1f,
                xVariable:new LinkedVariableParams((SpriteBase sb)=>(GraphicsDevice.Viewport.Width-Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height))/2,sensitiveVariables:new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                y:0,
                widthVariable:new LinkedVariableParams((SpriteBase sb)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                heightVariable:new LinkedVariableParams((SpriteBase so)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                spritesDict: spriteDict,
                dictKey: "chess",
                leftClickDelegate: (SpriteBase sb, int x, int y)=>{
                    Console.WriteLine("Clicked on chess: x="+x+", y="+y);
                    spriteDict["bigSquare"].leftClickDelegate= (SpriteBase sb, int x, int y)=>{
                        Console.WriteLine("New click on bigSquare");
                        return false;
                    };
                    return false;
                }
                ,collisionRectangle: new CollisionRectangle(
                    xVariable:new LinkedVariableParams((SpriteBase sb)=>sb.x+(sb.width/2),sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.xVariable,sb.widthVariable}),
                    widthVariable: new LinkedVariableParams((SpriteBase sb)=>sb.width-(sb.width/2),sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.widthVariable})),
                precise:true
                )
            ));

            // int x_pos=3; //FIXME: uncomment
            // int y_pos=3;
            // wrapper.Add(new Sprite(
            //     new SpriteParameters(spriteBatch:_spriteBatch,
            //     texture:white,
            //     depth:0.11f,
            //     xVariable:new LinkedVariableParams(
            //         (SpriteBase sb)=>Math.Round((double)((int)spriteDict["chess"].x+((((double)spriteDict["chess"].width)/(double)x_chess)*x_pos))), 
            //         new LinkedVariable[] {spriteDict["chess"].xVariable}
            //     ),
            //     yVariable:new LinkedVariableParams(
            //         (SpriteBase sb)=>(int)Math.Round((double)(spriteDict["chess"].height+spriteDict["chess"].y-sb.height-((((double)spriteDict["chess"].width)/(double)x_chess)*y_pos))),
            //         sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {spriteDict["chess"].yVariable,sb.heightVariable}
            //     ),
            //     widthVariable:new LinkedVariableParams((SpriteBase so)=>(int)Math.Round(((double)spriteDict["chess"].width)/(double)x_chess),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
            //     heightVariable:new LinkedVariableParams((SpriteBase so)=>(int)Math.Round(((double)spriteDict["chess"].width)/(double)y_chess),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
            //     spritesDict: spriteDict,
            //     dictKey: "cock"
            //     )
            // ));

            // Example of surface-draw->way to create new 2d texture
            // Methods.createGrid(100,100,blue,GraphicsDevice,spriteGroup:group,_spriteBatch);
            // RenderTarget2D renderTarget=new RenderTarget2D(GraphicsDevice,2000,2000);
            // Utilities.DrawOntoTarget(renderTarget,new ObjectGroup<Object2D>(group.objects.ConvertAll<Object2D>(x=>(Object2D)x)),_spriteBatch);
            
            //randomSprite=wrapper.NewSprite(renderTarget,widthVariable:(SpriteObject sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),heightVariable:(SpriteObject sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),xDelegate:(SpriteObject sprite)=>x,yDelegate:(SpriteObject sprite)=>y);
            randomSprite=new Sprite(new SpriteParameters(_spriteBatch,red,
                widthVariable:new LinkedVariableParams((SpriteBase sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                heightVariable:new LinkedVariableParams((SpriteBase sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                xVariable:new LinkedVariableParams((SpriteBase sprite)=>(int)x,new LinkedVariable[] {x}), //This sets the thing to literally be another LinkedList, so that it updates when it is needed to
                yVariable:new LinkedVariableParams((SpriteBase sprite)=>(int)y,new LinkedVariable[] {y}), //This is just another way to do the same exact thing
                leftClickDelegate:(SpriteBase sprite, int x, int y)=>{
                    if(randomSprite.texture==red){
                        randomSprite.texture=blue;
                    }else{
                        randomSprite.texture=red;
                    }
                    sprite.leftClickDelegate=null;
                    return true;
                }, 
                spritesDict: spriteDict,
                dictKey:"bigSquare")
            );

            wrapper.Add(randomSprite);

            wrapper.Add(new Sprite(defaultParameters+new SpriteParameters(
                texture:white,
                yVariable:new LinkedVariableParams(
                    (SpriteBase sb)=>spriteDict["bigSquare"].x,
                    sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {spriteDict["bigSquare"].xVariable}),
                    spritesDict: spriteDict,
                    dictKey: "test1"
            )));
            wrapper.Add(new Sprite(defaultParameters+new SpriteParameters(
                texture:white,
                xVariable:new LinkedVariableParams(
                    (SpriteBase sb)=>spriteDict["test1"].y,
                    sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {spriteDict["test1"].yVariable}),
                    spritesDict: spriteDict,
                    dictKey: "test2"
            )));
            wrapper.Add(new Sprite(defaultParameters+new SpriteParameters(
                texture:white,
                xVariable:new LinkedVariableParams(
                    (SpriteBase sb)=>spriteDict["bigSquare"].x,
                    sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {spriteDict["bigSquare"].xVariable}),
                yVariable:new LinkedVariableParams(
                    (SpriteBase sb)=>spriteDict["test2"].x,
                    sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {spriteDict["test2"].xVariable}),
                spritesDict: spriteDict,
                dictKey: "test3"
            )));

            texto=new TextSprite(
                new SpriteParameters(text:"",font:font,spriteBatch:_spriteBatch,
                widthVariable:new LinkedVariableParams((SpriteBase sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height)/2,new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                heightVariable:new LinkedVariableParams((SpriteBase sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height)/2,new LinkedVariable[] {resizeVariableW,resizeVariableH}),
                wrapMode:TextSprite.WrapMode.Word,originalHeightVariable:new LinkedVariableParams((SpriteBase sprite)=>2000),originalWidthVariable:new LinkedVariableParams((SpriteBase sprite)=>2000),depth:1f)
            );

            //TextSprite texto=new TextSprite("AaaaaaaaaaaaaaaaAAAAAAAAAAAAAaaaaaaaaaaaaaaaAAAaaaa",font,_spriteBatch,widthDelegate:(SpriteObject sprite)=>400,heightDelegate:(SpriteObject sprite)=>400,wrapMode:TextSprite.WrapMode.Word,originalHeightDelegate:(SpriteObject sprite)=>2000,originalWidthDelegate:(SpriteObject sprite)=>1000);
            // wrapper.NewSprite(texto.texture,depth:1f,widthDelegate:(Sprite sprite)=>400,heightDelegate:(Sprite sprite)=>800);
            wrapper.Add(texto);

            //Isometric generator
            Texture2D isometric=Content.Load<Texture2D>("Isometric");

            LayerGroup isometricGroup=new LayerGroup();
            int x_iso=8;
            int y_iso=8;
            //RenderTarget2D isometricTarget=new RenderTarget2D(GraphicsDevice,32+(8*x_iso),32+(16*y_iso));
            for(int i=0; i<x_iso;i++){
                for(int j=0; j<y_iso;j++){
                    Color thisColor=new Color(0,255,255);
                    if((i+j)%2==0){
                        thisColor=new Color(0,0,255);
                    }
                    isometricGroup.Add(new Sprite(
                        new SpriteParameters(spriteBatch:_spriteBatch,
                        texture:isometric,
                        depth:0.5f,
                        x:16*(x_iso-1)+((-16*i)+(16*j)),
                        y:(8*i)+(8*j),
                        width:32,
                        height:32,
                        color:thisColor
                        )
                    ));
                }
            }

            TextureGenerator generator=new TextureGenerator(GraphicsDevice,isometricGroup,32+(16*(x_iso-1))+(16*(y_iso-1)),16+(8*x_iso)+(8*y_iso),_spriteBatch,new SpriteBatchParameters(sortMode:SpriteSortMode.Deferred,samplerState:SamplerState.PointClamp));

            wrapper.Add(new Sprite(defaultParameters+new SpriteParameters(
                texture:generator.Generate(),
                depth:0.5f,
                widthVariable:new LinkedVariableParams((SpriteBase sb)=>sb.texture.Width),
                heightVariable:new LinkedVariableParams((SpriteBase sb)=>sb.texture.Height),
                x:0,
                y:0
            )));

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
