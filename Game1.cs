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
        Texture2D totem_of_time;
        SpriteFont font;

        TextSprite texto;
        Sprite randomSprite;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Wrapper wrapper;

        int x=0;
        int y=0;
        float fps=0f;
        ObjectGroup<SpriteObject> group;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.AllowUserResizing = true;
            IsFixedTimeStep=true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            wrapper=new Wrapper(_spriteBatch);

            totem_of_time=Content.Load<Texture2D>("assets/totem_of_time");
            font=Content.Load<SpriteFont>("FreeSans");

            group=new ObjectGroup<SpriteObject>();

            // Example of surface-draw->way to create new 2d texture
            Methods.createGrid(wrapper,100,100,totem_of_time,GraphicsDevice,spriteGroup:group,_spriteBatch);
            RenderTarget2D renderTarget=new RenderTarget2D(GraphicsDevice,2000,2000);
            Utilities.DrawOntoTarget(renderTarget,new ObjectGroup<Object2D>(group.objects.ConvertAll<Object2D>(x=>(Object2D)x)),GraphicsDevice,_spriteBatch);
            
            randomSprite=wrapper.NewSprite(renderTarget,widthDelegate:(SpriteObject sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),heightDelegate:(SpriteObject sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height),xDelegate:(SpriteObject sprite)=>x,yDelegate:(SpriteObject sprite)=>y);

            texto=new TextSprite("Simpatico testo di prova che dovrebbe servire a vedere se tutte le cose funzionano a dovere",font,_spriteBatch,widthDelegate:(SpriteObject sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height)/2,heightDelegate:(SpriteObject sprite)=>Math.Min(GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height)/2,wrapMode:TextSprite.WrapMode.Word,originalHeightDelegate:(SpriteObject sprite)=>2000,originalWidthDelegate:(SpriteObject sprite)=>2000);
            //TextSprite texto=new TextSprite("AaaaaaaaaaaaaaaaAAAAAAAAAAAAAaaaaaaaaaaaaaaaAAAaaaa",font,_spriteBatch,widthDelegate:(SpriteObject sprite)=>400,heightDelegate:(SpriteObject sprite)=>400,wrapMode:TextSprite.WrapMode.Word,originalHeightDelegate:(SpriteObject sprite)=>2000,originalWidthDelegate:(SpriteObject sprite)=>1000);
            // wrapper.NewSprite(texto.texture,depth:1f,widthDelegate:(Sprite sprite)=>400,heightDelegate:(Sprite sprite)=>800);
            wrapper.Add(texto);

            Console.WriteLine("Supposed width: "+GraphicsDevice.Viewport.Width); //This is how you ACTUALLY get the size of the user window
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if(Keyboard.GetState().IsKeyDown(Keys.D)){
                x=x+1;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.A)){
                x=x-1;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.W)){
                y=y-1;
            }
            else if(Keyboard.GetState().IsKeyDown(Keys.S)){
                y=y+1;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.L)){
                randomSprite.Remove();
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down)){
                texto.offsetY+=10;
            }else if(Keyboard.GetState().IsKeyDown(Keys.Up)){
                texto.offsetY-=10;
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
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(new Color(0, 0, 0));

            // TODO: Add your drawing code here

            wrapper.Draw();

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
        public static void createGrid(Wrapper wrapper,int xSize, int ySize, Texture2D texture, GraphicsDevice graphicsDevice, ObjectGroup<SpriteObject> spriteGroup, SpriteBatch spriteBatch){
            for(int i=0;i<xSize;i++){
                int copyX=i;
                for(int j=0;j<ySize;j++){
                    int copyY=j;
                    new Sprite(
                        spriteBatch,
                        texture,
                        depth:0f,
                        xDelegate: (SpriteObject sprite)=>(copyX*(graphicsDevice.Viewport.Width/DIVIDER)), 
                        yDelegate: (SpriteObject sprite)=>(copyY*(graphicsDevice.Viewport.Width/DIVIDER)), 
                        widthDelegate: (SpriteObject sprite)=>graphicsDevice.Viewport.Width/DIVIDER, 
                        heightDelegate: (SpriteObject sprite)=>graphicsDevice.Viewport.Width/DIVIDER,
                        group:spriteGroup
                        );
                }
            }
        }
    }
}
