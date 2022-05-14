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
        /// Draws the given Object2Ds on the target.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, ObjectGroup<Object2D> group, SpriteBatch spriteBatch){
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);//TODO: add spriteBatch settings
            group.PerformOnAll((Object2D obj)=>{
                obj.Draw(drawMiddle:false);
            });
            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }
        /// <summary>
        /// Draws the given sprite on the target.
        /// </summary>
        public static void DrawOntoTarget(RenderTarget2D renderTarget, SpriteObject spriteObject, SpriteBatch spriteBatch){
            spriteBatch=new SpriteBatch(spriteBatch.GraphicsDevice);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);// TODO: make this optional 

            spriteBatch.Begin(samplerState:SamplerState.PointClamp);
            spriteObject.Draw(drawMiddle:false);
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
        public static object Choose(object o1,object o2){ //TODO: make this method use generic types
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
        private IntSpriteBaseDelegate xDelegate;
        private IntSpriteBaseDelegate yDelegate;
        private IntSpriteBaseDelegate widthDelegate;
        private IntSpriteBaseDelegate heightDelegate;
        private SpriteBase sprite; //The sprite which this collision rectangle is associated with

        public int x{
            get{
                return xDelegate(sprite);
            }
        }
        public int y{
            get{
                return yDelegate(sprite);
            }
        }
        public int width{
            get{
                return widthDelegate(sprite);
            }
        }
        public int height{
            get{
                return heightDelegate(sprite);
            }
        }

        public CollisionRectangle(SpriteBase sprite, IntSpriteBaseDelegate xDelegate, IntSpriteBaseDelegate yDelegate, IntSpriteBaseDelegate widthDelegate, IntSpriteBaseDelegate heightDelegate){
            this.sprite=sprite;
            this.xDelegate=xDelegate;
            this.yDelegate=yDelegate;
            this.widthDelegate=widthDelegate;
            this.heightDelegate=heightDelegate;
        }
        public CollisionRectangle(IntSpriteBaseDelegate xDelegate, IntSpriteBaseDelegate yDelegate, IntSpriteBaseDelegate widthDelegate, IntSpriteBaseDelegate heightDelegate){
            this.xDelegate=xDelegate;
            this.yDelegate=yDelegate;
            this.widthDelegate=widthDelegate;
            this.heightDelegate=heightDelegate;
        }
        /// <summary>
        /// Construct a collision rectangle with values corresponding to the ones of the sprite.
        /// </summary>
        public CollisionRectangle(SpriteBase sprite){
            this.sprite=sprite;
            this.xDelegate=(SpriteBase sprite)=>sprite.x; //TODO: add a way to choose wether the coords are relative or not to the sprite.
            this.yDelegate=(SpriteBase sprite)=>sprite.y;
            this.widthDelegate=(SpriteBase sprite)=>sprite.width;
            this.heightDelegate=(SpriteBase sprite)=>sprite.height;
        }

        public void SetSprite(SpriteBase sprite){
            this.sprite=sprite;
        }

        /// <summary>
        /// Returns true if the given point is inside the collision rectangle.
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
        public IntSpriteObjDelegate originalWidthDelegate; //Only for TextSprite
        public IntSpriteObjDelegate originalHeightDelegate; //Only for TextSprite
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
            IntSpriteObjDelegate originalWidthDelegate=null, //Only for TextSprite
            IntSpriteObjDelegate originalHeightDelegate=null, //Only for TextSprite
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
            this.originalWidthDelegate=originalWidthDelegate;
            this.originalHeightDelegate=originalHeightDelegate;
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
            output.spriteBatch=(SpriteBatch)Utilities.Choose(sp1.spriteBatch,sp2.spriteBatch);
            output.texture=(Texture2D)Utilities.Choose(sp1.texture,sp2.texture);
            output.font=(SpriteFont)Utilities.Choose(sp1.font,sp2.font);
            output.text=(string)Utilities.Choose(sp1.text,sp2.text);
            output.wrapMode=(TextSprite.WrapMode)Utilities.Choose(sp1.wrapMode,sp2.wrapMode);
            output.layoutMode=(TextSprite.LayoutMode)Utilities.Choose(sp1.layoutMode,sp2.layoutMode);
            output.offsetX=(int)Utilities.Choose(sp1.offsetX,sp2.offsetX);
            output.offsetY=(int)Utilities.Choose(sp1.offsetY,sp2.offsetY);
            output.originalWidthDelegate=(IntSpriteObjDelegate)Utilities.Choose(sp1.originalWidthDelegate,sp2.originalWidthDelegate);
            output.originalHeightDelegate=(IntSpriteObjDelegate)Utilities.Choose(sp1.originalHeightDelegate,sp2.originalHeightDelegate);
            output.wrapper=(Wrapper)Utilities.Choose(sp1.wrapper,sp2.wrapper);
            output.depth=(float)Utilities.Choose(sp1.depth,sp2.depth);
            output.xVariable=(LinkedVariableParams)Utilities.Choose(sp1.xVariable,sp2.xVariable);
            output.x=(int?)Utilities.Choose(sp1.x,sp2.x);
            output.yVariable=(LinkedVariableParams)Utilities.Choose(sp1.yVariable,sp2.yVariable);
            output.y=(int?)Utilities.Choose(sp1.y,sp2.y);
            output.widthVariable=(LinkedVariableParams)Utilities.Choose(sp1.widthVariable,sp2.widthVariable);
            output.width=(int?)Utilities.Choose(sp1.width,sp2.width);
            output.heightVariable=(LinkedVariableParams)Utilities.Choose(sp1.heightVariable,sp2.heightVariable);
            output.height=(int?)Utilities.Choose(sp1.height,sp2.height);
            output.rotation=(float)Utilities.Choose(sp1.rotation,sp2.rotation);
            output.origin=(Vector2)Utilities.Choose(sp1.origin,sp2.origin);
            output.color=(Color)Utilities.Choose(sp1.color,sp2.color);
            output.group=(ObjectGroup<SpriteObject>)Utilities.Choose(sp1.group,sp2.group);
            output.groups=(List<ObjectGroup<SpriteObject>>)Utilities.Choose(sp1.groups,sp2.groups);
            output.leftClickDelegate=(ClickDelegate)Utilities.Choose(sp1.leftClickDelegate,sp2.leftClickDelegate);
            output.middleClickDelegate=(ClickDelegate)Utilities.Choose(sp1.middleClickDelegate,sp2.middleClickDelegate);
            output.rightClickDelegate=(ClickDelegate)Utilities.Choose(sp1.rightClickDelegate,sp2.rightClickDelegate);
            output.wheelHoverDelegate=(ClickDelegate)Utilities.Choose(sp1.wheelHoverDelegate,sp2.wheelHoverDelegate);
            output.hoverDelegate=(ClickDelegate)Utilities.Choose(sp1.hoverDelegate,sp2.hoverDelegate);
            output.spritesDict=(Dictionary<string, SpriteBase>)Utilities.Choose(sp1.spritesDict,sp2.spritesDict);
            output.dictKey=(string)Utilities.Choose(sp1.dictKey,sp2.dictKey);
            output.collisionRectangle=(CollisionRectangle)Utilities.Choose(sp1.collisionRectangle,sp2.collisionRectangle);

            return output;
        }
        #endregion Operators
    }
}