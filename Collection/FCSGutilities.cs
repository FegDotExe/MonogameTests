using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System;

namespace FCSG{
    public delegate void TypeAction<Type>(Type type);
    /// <summary>
    /// A class used to group together objects and perform actions on them
    /// </summary>
    public class ObjectGroup<Type>{
        public List<Type> objects;
        public ObjectGroup(){
            objects=new List<Type>();
        }
        public ObjectGroup(List<Type> list){
            objects=list;
        }
        public void Add(Type obj){
            objects.Add(obj);
        }
        public void Remove(Type obj){
            objects.Remove(obj);
        }
        public void PerformOnAll(TypeAction<Type> action){
            foreach(Type obj in objects){
                action(obj);
            }
        }
    }

    /// <summary>
    /// A class which stores useful static methods
    /// </summary>
    public class Utilities{

        /// <summary>
        /// Draws the given sprite on the target.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, SpriteBase sprite, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            sprite.BasicDraw(spriteBatch,drawMiddle:false);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given texture on the target. In order to draw, it uses the simplest Draw function possible (position at 0,0 and white as color)
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, Texture2D texture, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given textures on the target. In order to draw, it uses the simplest Draw function possible (position at 0,0 and white as color), and goes from first to last texture in list
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, List<Texture2D> textures, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            foreach(Texture2D texture in textures){
                spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            }
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        /// <summary>
        /// Draws the given sprites on the target. In order to draw, it uses each sprite's BasicDraw function, leaving the rest to the batch settings.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, LayerGroup sprites, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(sortMode:SpriteSortMode.FrontToBack,samplerState:SamplerState.PointClamp);
            foreach(SpriteBase sprite in sprites.objects){
                sprite.BasicDraw(spriteBatch, drawMiddle:false);
            }
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        public static Texture2D CombineTextures(List<Texture2D> textures, SpriteBatch spriteBatch){
            if(textures.Count==0){
                throw new System.ArgumentException("The list of textures is empty");
            }
            RenderTarget2D renderTarget=new RenderTarget2D(spriteBatch.GraphicsDevice, textures[0].Width, textures[0].Height);
            
            Utilities.DrawOntoTarget(renderTarget, textures, spriteBatch);
            
            return renderTarget;
        }
    
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

        /// <summary>
        /// Used to choose between two objects, giving priority to the second one; if o2 is null, o1 is returned, otherwise o2 is returned.
        /// </summary>
        public static T Choose<T>(T o1,T o2){ //TODO: make this method use generic types
            if(o2==null){
                return o1;
            }else{
                return o2;
            }
        }
    }

    /// <summary>
    /// A class which stores sprites in a list, keeping them ordered by their depth or by a custom delegate, with the ones with higher values in front
    /// </summary>
    public class LayerGroup{
        public List<SpriteBase> objects; //TODO: decide if this should be private or not->I'd say it should not
        private DoubleSpriteBaseDelegate comparer;
        /// <summary>
        /// Construct a new LayerGroup which will sort its sprites by their depth
        /// </summary>
        public LayerGroup(){
            objects=new List<SpriteBase>();
            comparer=(SpriteBase sb)=>sb.depth;
        }
        /// <summary>
        /// Construct a new LayerGroup which will sort its sprites by the given delegate
        /// </summary>
        public LayerGroup(DoubleSpriteBaseDelegate comparer){
            objects=new List<SpriteBase>();
            this.comparer=comparer;
        }
        /// <summary>
        /// Adds the given sprite to the LayerGroup, keeping it ordered by its depth
        /// </summary>
        public void Add(SpriteBase sprite){
            this.Add(sprite,this.objects);
        }
        private void Add(SpriteBase sprite, List<SpriteBase> objects){
            if(objects.Count==0){
                objects.Add(sprite);
            }else{
                int i=0;
                for(i=objects.Count; i>0 && comparer(objects[i-1])<comparer(sprite); i--){
                    objects.Insert(i,objects[i-1]);
                }
                if(objects.Count==i){ //This covers the edge case in which the index of the object which should be added is out of bounds.
                    objects.Add(sprite);
                }
                else{
                    objects[i]=sprite;
                }
            }
        }

        /// <summary>
        /// Removes the given sprite from the LayerGroup
        /// </summary>
        public void Remove(SpriteBase sprite){
            objects.Remove(sprite);
        }

        /// <summary>
        /// Reorders all the objects in the group
        /// </summary>
        public void Update(){
            List<SpriteBase> newObjects=new List<SpriteBase>();
            foreach(SpriteBase sprite in objects){
                this.Add(sprite,newObjects);
            }
            objects=newObjects;
        }
    }

    /// <summary>
    /// A class used to simply generate textures composed of many different sprites.
    /// </summary>
    public class TextureGenerator{
        public LayerGroup sprites;
        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        public int x;
        public int y;
        public TextureGenerator(GraphicsDevice graphicsDevice){
            sprites=new LayerGroup();
            this.graphicsDevice=graphicsDevice;
            this.x=100;
            this.y=100;
        }
        /// <param name="sprites">A layer group containing the sprites which will be drawn on the texture</param>
        /// <param name="x">The width of the result texture</param>
        /// <param name="y">The height of the result texture</param>
        /// <summary>
        /// Construct a texture generator which will automatically generate a new SpriteBatch from its GraphicsDevice
        /// </summary>
        public TextureGenerator(GraphicsDevice graphicsDevice, LayerGroup sprites, int x, int y){
            this.graphicsDevice=graphicsDevice;
            this.sprites=sprites;
            this.x=x;
            this.y=y;
        }

        /// <param name="sprites">A layer group containing the sprites which will be drawn on the texture</param>
        /// <param name="x">The width of the result texture</param>
        /// <param name="y">The height of the result texture</param>
        /// <summary>
        /// Construct a texture generator which will use the given SpriteBatch
        /// </summary>
        public TextureGenerator(GraphicsDevice graphicsDevice, LayerGroup sprites, int x, int y, SpriteBatch spriteBatch){
            this.graphicsDevice=graphicsDevice;
            this.sprites=sprites;
            this.x=x;
            this.y=y;
            this.spriteBatch=spriteBatch;
        }
        public Texture2D Generate(){
            SpriteBatch spriteBatch=this.spriteBatch;
            if(spriteBatch==null){
                spriteBatch=new SpriteBatch(graphicsDevice);
            }
            RenderTarget2D renderTarget=new RenderTarget2D(graphicsDevice, x, y);
            Utilities.DrawOntoTarget(renderTarget, sprites, spriteBatch);
            return renderTarget;
        }

        public void Add(SpriteBase sprite){
            sprites.Add(sprite);
        }
        public void Remove(SpriteBase sprite){
            sprites.Remove(sprite);
        }
    }

    /// <summary>
    /// A class which represents a sprite's collision box. It was created so that delegates can be used to handle collisions. It can cast implicitly to a rectangle.
    /// </summary>
    public class CollisionRectangle{
        private LinkedVariable xVariable;
        private LinkedVariable yVariable;
        private LinkedVariable widthVariable;
        private LinkedVariable heightVariable;
        public SpriteBase sprite; //The sprite which this collision rectangle is associated with

        public int x{
            get{
                return xVariable;
            }
        }
        public int y{
            get{
                return yVariable;
            }
        }
        public int width{
            get{
                return widthVariable;
            }
        }
        public int height{
            get{
                return heightVariable;
            }
        }

        #region Constructors
        /// <summary>
        /// Construct a new CollisionRectangle with the given settings. It will need to be activated with CollisionRectangle.Activate() in order for it to work.
        /// </summary>
        public CollisionRectangle(LinkedVariableParams xVariable=null, LinkedVariableParams yVariable=null, LinkedVariableParams widthVariable=null, LinkedVariableParams heightVariable=null){
            if(xVariable!=null){
                this.xVariable=new LinkedVariable(this.sprite,xVariable);
            }else
                this.xVariable=new LinkedVariable(new LinkedVariableParams((SpriteBase sb)=>(int)sb.x, sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.xVariable})); //TODO: add a way to choose wether the coords are relative or not to the sprite.
            if(yVariable!=null){
                this.yVariable=new LinkedVariable(this.sprite,yVariable);
            }else
                this.yVariable=new LinkedVariable(new LinkedVariableParams((SpriteBase sb)=>(int)sb.y, sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.yVariable}));
            if(widthVariable!=null){
                this.widthVariable=new LinkedVariable(widthVariable);
            }else
                this.widthVariable=new LinkedVariable(new LinkedVariableParams((SpriteBase sb)=>(int)sb.width, sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.widthVariable}));
            if(heightVariable!=null){
                this.heightVariable=new LinkedVariable(heightVariable);
            }else
                this.heightVariable=new LinkedVariable(new LinkedVariableParams((SpriteBase sb)=>(int)sb.height, sensitiveDelegate: (SpriteBase sb)=>new LinkedVariable[] {sb.heightVariable}));
        }

        public CollisionRectangle(LinkedVariable xVariable, LinkedVariable yVariable, LinkedVariable widthVariable, LinkedVariable heightVariable){
            this.xVariable=xVariable;
            this.yVariable=yVariable;
            this.widthVariable=widthVariable;
            this.heightVariable=heightVariable;
        }
        /// <summary>
        /// Construct a collision rectangle with values corresponding to the ones of the sprite.
        /// </summary>
        public CollisionRectangle(SpriteBase sprite){
            this.sprite=sprite;
            this.xVariable=new LinkedVariable(this.sprite,(SpriteBase sb)=>this.sprite.xVariable,new LinkedVariable[] {this.sprite.xVariable});
            this.yVariable=sprite.yVariable;
            this.widthVariable=sprite.widthVariable;
            this.heightVariable=sprite.heightVariable;
        }
        public CollisionRectangle(SpriteBase sprite, LinkedVariable xVariable=null, LinkedVariable yVariable=null, LinkedVariable widthVariable=null, LinkedVariable heightVariable=null){
            if(sprite!=null){
                this.sprite=sprite;
            }
            if(xVariable!=null){
                this.xVariable=xVariable;
            }else
                this.xVariable=new LinkedVariable(this.sprite,(SpriteBase sprite)=>(int)sprite.x, new LinkedVariable[] {sprite.xVariable}); //TODO: add a way to choose wether the coords are relative or not to the sprite.
            if(yVariable!=null){
                this.yVariable=yVariable;
            }else
                this.yVariable=new LinkedVariable(this.sprite,(SpriteBase sprite)=>(int)sprite.y, new LinkedVariable[] {sprite.yVariable});
            if(widthVariable!=null){
                this.widthVariable=widthVariable;
            }else
                this.widthVariable=new LinkedVariable(this.sprite,(SpriteBase sprite)=>(int)sprite.width, new LinkedVariable[] {sprite.widthVariable});
            if(heightVariable!=null){
                this.heightVariable=heightVariable;
            }else
                this.heightVariable=new LinkedVariable(this.sprite,(SpriteBase sprite)=>(int)sprite.height, new LinkedVariable[] {sprite.heightVariable});
        }
        #endregion Constructors

        public void Activate(SpriteBase sb){
            this.sprite=sb;
            this.xVariable.Activate(sb);
            this.yVariable.Activate(sb);
            this.widthVariable.Activate(sb);
            this.heightVariable.Activate(sb);
        }

        public void SetSprite(SpriteBase sprite){
            this.sprite=sprite;
        }

        /// <summary>
        /// Returns true if the given point is inside the collision rectangle. It is not relative to the rectangle.
        /// </summary>
        public bool CollidesWith(int x,int y){
            return x>=this.x && x<=this.x+this.width && y>=this.y && y<=this.y+this.height;
        }

        public static implicit operator Rectangle(CollisionRectangle collisionRectangle){
            return new Rectangle(collisionRectangle.x, collisionRectangle.y, collisionRectangle.width, collisionRectangle.height);
        }
    }

    public class SpriteParameters{
        #region Fields
        public SpriteBatch spriteBatch;
        public Texture2D texture;
        public SpriteFont font;
        public string text;
        public TextSprite.WrapMode wrapMode;
        public TextSprite.LayoutMode layoutMode;
        public int offsetX; //Only for TextSprite
        public int offsetY; //Only for TextSprite
        public LinkedVariableParams originalWidthVariable; //Only for TextSprite
        public LinkedVariableParams originalHeightVariable; //Only for TextSprite
        public Wrapper wrapper;
        public float depth;
        public LinkedVariableParams xVariable;
        public int? x;
        public LinkedVariableParams yVariable;
        public int? y;
        public LinkedVariableParams widthVariable;
        public int? width;
        public LinkedVariableParams heightVariable;
        public int? height;
        public float rotation; 
        public Vector2 origin; 
        public Color color;
        public ObjectGroup<SpriteObject> group;
        public List<ObjectGroup<SpriteObject>> groups;
        public ClickDelegate leftClickDelegate;
        public ClickDelegate middleClickDelegate;
        public ClickDelegate rightClickDelegate;
        public ClickDelegate wheelHoverDelegate;
        public ClickDelegate hoverDelegate;
        public Dictionary<string, SpriteBase> spritesDict;
        public string dictKey;
        public CollisionRectangle collisionRectangle;
        public bool precise; //Wether clicks should be pixel-precise or not.

        #endregion Fields
        #region Constructor
        public SpriteParameters(
            SpriteBatch spriteBatch=null,
            Texture2D texture=null, //Only for Sprite
            SpriteFont font=null, //Only for TextSprite
            string text=null, //Only for TextSprite
            TextSprite.WrapMode wrapMode=TextSprite.WrapMode.Word, //Only for TextSprite
            TextSprite.LayoutMode layoutMode=TextSprite.LayoutMode.Left, //Only for TextSprite
            int offsetX=0, //Only for TextSprite
            int offsetY=0, //Only for TextSprite
            LinkedVariableParams originalWidthVariable=null, //Only for TextSprite
            LinkedVariableParams originalHeightVariable=null, //Only for TextSprite
            Wrapper wrapper=null,
            float depth=0, 
            LinkedVariableParams xVariable=null,
            int? x=null,
            LinkedVariableParams yVariable=null,
            int? y=null,
            LinkedVariableParams widthVariable=null, 
            int? width=null,
            LinkedVariableParams heightVariable=null,
            int? height=null,
            float rotation=0, 
            Vector2? origin=null, 
            Color? color=null,
            ObjectGroup<SpriteObject> group=null,
            List<ObjectGroup<SpriteObject>> groups=null,
            ClickDelegate leftClickDelegate=null,
            ClickDelegate middleClickDelegate=null,
            ClickDelegate rightClickDelegate=null,
            ClickDelegate wheelHoverDelegate=null,
            ClickDelegate hoverDelegate=null,
            bool precise=false,
            Dictionary<string, SpriteBase> spritesDict=null,
            string dictKey=null,
            CollisionRectangle collisionRectangle=null
        ){
            //Variables which are not modified by default
            this.spriteBatch=spriteBatch;
            this.texture=texture;
            this.font=font;
            this.text=text;
            this.wrapMode=wrapMode;
            this.layoutMode=layoutMode;
            this.offsetX=offsetX;
            this.offsetY=offsetY;
            this.originalWidthVariable=originalWidthVariable;
            this.originalHeightVariable=originalHeightVariable;
            this.wrapper=wrapper;
            this.depth=depth;
            this.width=width;
            this.height=height;
            this.rotation=rotation;
            this.group=group;
            this.groups=groups;
            this.spritesDict=spritesDict;
            this.dictKey=dictKey;
            this.collisionRectangle=collisionRectangle;

            // Position variables
                this.xVariable=xVariable;
                this.x=x;
                this.yVariable=yVariable;
                this.y=y;

            //Size variables
                this.widthVariable=widthVariable;
                this.heightVariable=heightVariable;

            //Click delegates
                this.leftClickDelegate = leftClickDelegate;
                this.middleClickDelegate = middleClickDelegate;
                this.rightClickDelegate = rightClickDelegate;
                this.wheelHoverDelegate = wheelHoverDelegate;
                this.hoverDelegate = hoverDelegate;
            this.precise=precise;

            //Origin
            if(origin!=null){
                this.origin = (Vector2)origin;
            }else{
                this.origin = new Vector2(0,0);
            }

            //Color
            if(color!=null){
                this.color = (Color)color;
            }else{
                this.color = Color.White;
            }
        }
        #endregion Constructor
    
        #region Operators
        public static SpriteParameters operator +(SpriteParameters sp1, SpriteParameters sp2){
            SpriteParameters output=new SpriteParameters();
            output.spriteBatch=Utilities.Choose<SpriteBatch>(sp1.spriteBatch,sp2.spriteBatch);
            output.texture=Utilities.Choose<Texture2D>(sp1.texture,sp2.texture);
            output.font=Utilities.Choose<SpriteFont>(sp1.font,sp2.font);
            output.text=Utilities.Choose<string>(sp1.text,sp2.text);
            output.wrapMode=Utilities.Choose<TextSprite.WrapMode>(sp1.wrapMode,sp2.wrapMode);
            output.layoutMode=Utilities.Choose<TextSprite.LayoutMode>(sp1.layoutMode,sp2.layoutMode);
            output.offsetX=Utilities.Choose<int>(sp1.offsetX,sp2.offsetX);
            output.offsetY=Utilities.Choose<int>(sp1.offsetY,sp2.offsetY);
            output.originalWidthVariable=Utilities.Choose<LinkedVariableParams>(sp1.originalWidthVariable,sp2.originalWidthVariable);
            output.originalHeightVariable=Utilities.Choose(sp1.originalHeightVariable,sp2.originalHeightVariable);
            output.wrapper=Utilities.Choose(sp1.wrapper,sp2.wrapper);
            output.depth=Utilities.Choose(sp1.depth,sp2.depth);
            output.xVariable=Utilities.Choose(sp1.xVariable,sp2.xVariable);
            output.x=Utilities.Choose(sp1.x,sp2.x);
            output.yVariable=Utilities.Choose(sp1.yVariable,sp2.yVariable);
            output.y=Utilities.Choose(sp1.y,sp2.y);
            output.widthVariable=Utilities.Choose(sp1.widthVariable,sp2.widthVariable);
            output.width=Utilities.Choose(sp1.width,sp2.width);
            output.heightVariable=Utilities.Choose(sp1.heightVariable,sp2.heightVariable);
            output.height=Utilities.Choose(sp1.height,sp2.height);
            output.rotation=Utilities.Choose(sp1.rotation,sp2.rotation);
            output.origin=Utilities.Choose(sp1.origin,sp2.origin);
            output.color=Utilities.Choose(sp1.color,sp2.color);
            output.group=Utilities.Choose(sp1.group,sp2.group);
            output.groups=Utilities.Choose(sp1.groups,sp2.groups);
            output.leftClickDelegate=Utilities.Choose(sp1.leftClickDelegate,sp2.leftClickDelegate);
            output.middleClickDelegate=Utilities.Choose(sp1.middleClickDelegate,sp2.middleClickDelegate);
            output.rightClickDelegate=Utilities.Choose(sp1.rightClickDelegate,sp2.rightClickDelegate);
            output.wheelHoverDelegate=Utilities.Choose(sp1.wheelHoverDelegate,sp2.wheelHoverDelegate);
            output.hoverDelegate=Utilities.Choose(sp1.hoverDelegate,sp2.hoverDelegate);
            output.spritesDict=Utilities.Choose(sp1.spritesDict,sp2.spritesDict);
            output.dictKey=Utilities.Choose(sp1.dictKey,sp2.dictKey);
            output.collisionRectangle=Utilities.Choose(sp1.collisionRectangle,sp2.collisionRectangle);

            return output;
        }
        #endregion Operators
    }
}