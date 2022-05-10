using System.Collections.Generic;
using System;
namespace FCSG{
    public delegate bool BoolSVariable(SVariable var);
    public class SVariable{
        protected bool round=true; //Wether int values should be rounded or not
        protected bool updating; //If true, it means that the value is being updated (and has not yet been set)
        protected ObjectSpriteBaseDelegate objectDelegate;
        protected BoolSVariable changedDelegate=(SVariable var)=>var._value!=var.objectDelegate(var.spriteBase);
        protected SpriteBase spriteBase;
        protected List<SVariable> linkedVariables;

        protected object _value;
        protected object objectValue{
            get{
                if(!updating){
                    return _value;
                }
                else{
                    return objectDelegate(spriteBase);
                }
            }
            set{
                _value=value;
            }
        }

        public void Set(object value){
            this.objectDelegate=(SpriteBase sb)=>value;
            SetUpdating();
            Update();
        }

        public void Round(bool round){
            this.round=round;
        }

        private void SetUpdating(){
            updating=true;
            foreach(SVariable sv in linkedVariables){
                // Console.WriteLine("Setting "+sv+" to updating");
                if(!sv.updating){
                    // Console.WriteLine("Set "+sv+" to updating");
                    sv.SetUpdating();
                }
            }
        }

        private void Update(){
            if(changedDelegate(this)){ //Is true if the value should be considered as changed
                _value=objectDelegate(spriteBase);
                updating=false;
                // Console.WriteLine("SVariable updated: "+this.ToString());
                foreach(SVariable sv in linkedVariables){
                    if(sv.updating){
                        sv.Update();
                    }
                }
            }else{
                updating=false;
            }
        }

        ///<summary>
        ///Adds a variable which will be updated when this variable is updated
        ///</summary>
        public void AddLinkedVariable(SVariable sv){//Adds a variable (sv) which is sensitive to this
            if(!linkedVariables.Contains(sv)){
                // Console.WriteLine("Adding "+this+" to "+sv);
                linkedVariables.Add(sv);
            }
        }
        ///<summary>
        ///Adds this variable to sv's linked variables, so that this is updated when sv is updated
        ///</summary>
        public void LinkTo(SVariable sv){//Makes this variable sensitive to sv
            // Console.WriteLine("Adding "+this+" to "+sv);
            sv.AddLinkedVariable(this);
        }

        #region Constructors
        //Without anything
        public SVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate){
            this.spriteBase=spriteBase;
            linkedVariables=new List<SVariable>();
            this.objectDelegate=objectDelegate;
            _value=objectDelegate(spriteBase);
        }

        public SVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, BoolSVariable changedDelegate){
            this.spriteBase=spriteBase;
            linkedVariables=new List<SVariable>();
            this.changedDelegate=changedDelegate;
            this.objectDelegate=objectDelegate;
            _value=objectDelegate(spriteBase);
        }

        //With lists
        public SVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, List<SVariable> sensitiveVariables){
            this.spriteBase=spriteBase;
            linkedVariables=new List<SVariable>();
            this.objectDelegate=objectDelegate;
            _value=objectDelegate(spriteBase);
            foreach(SVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        public SVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, BoolSVariable changedDelegate, List<SVariable> sensitiveVariables){
            this.spriteBase=spriteBase;
            linkedVariables=new List<SVariable>();
            this.changedDelegate=changedDelegate;
            this.objectDelegate=objectDelegate;
            _value=objectDelegate(spriteBase);
            foreach(SVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        //With arrays
        public SVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, SVariable[] sensitiveVariables){
            this.spriteBase=spriteBase;
            linkedVariables=new List<SVariable>();
            this.objectDelegate=objectDelegate;
            _value=objectDelegate(spriteBase);
            foreach(SVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        public SVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, BoolSVariable changedDelegate, SVariable[] sensitiveVariables){
            this.spriteBase=spriteBase;
            linkedVariables=new List<SVariable>();
            this.changedDelegate=changedDelegate;
            this.objectDelegate=objectDelegate;
            _value=objectDelegate(spriteBase);
            foreach(SVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }
        #endregion Constructors

        public static implicit operator int(SVariable sv){
            try{
                if(!sv.round){
                    return (int)sv.objectValue;
                }else{
                    return (int)Math.Round((double)sv.objectValue);
                }
            }catch(System.InvalidCastException){
                throw new System.InvalidCastException("The SVariable cannot be cast to an int; its type is "+sv.objectValue.GetType().ToString());
            }
        }

        public static implicit operator string(SVariable sv){
            try{
                return (string)sv.objectValue;
            }catch(System.InvalidCastException){
                throw new System.InvalidCastException("The SVariable cannot be cast to a string; its type is "+sv.objectValue.GetType().ToString());
            }
        }

        public override string ToString(){
            return "§"+objectValue.ToString()+"§";
        }
    }
}