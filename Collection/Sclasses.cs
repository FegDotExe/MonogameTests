using System.Collections.Generic;
using System;
namespace FCSG{
    public delegate bool BoolLinkedVariableDelegate(LinkedVariable var);
    public class LinkedVariable{
        protected bool round=true; //Wether int values should be rounded or not
        protected bool updating; //If true, it means that the value is being updated (and has not yet been set)
        protected ObjectSpriteBaseDelegate objectDelegate;
        protected BoolLinkedVariableDelegate changedDelegate=(LinkedVariable var)=>var._value!=var.objectDelegate(var.spriteBase);
        protected SpriteBase spriteBase;
        protected List<LinkedVariable> linkedVariables;

        protected object _value=null;
        protected object objectValue{
            get{
                if(_value==null){
                    _value=objectDelegate(spriteBase);
                }
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
        public void SetSprite(SpriteBase sprite){
            this.spriteBase=sprite;
        }

        private void SetUpdating(){
            updating=true;
            foreach(LinkedVariable sv in linkedVariables){
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
                foreach(LinkedVariable sv in linkedVariables){
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
        public void AddLinkedVariable(LinkedVariable sv){//Adds a variable (sv) which is sensitive to this
            if(!linkedVariables.Contains(sv)){
                // Console.WriteLine("Adding "+this+" to "+sv);
                linkedVariables.Add(sv);
            }
        }
        ///<summary>
        ///Adds this variable to sv's linked variables, so that this is updated when sv is updated
        ///</summary>
        public void LinkTo(LinkedVariable sv){//Makes this variable sensitive to sv
            // Console.WriteLine("Adding "+this+" to "+sv);
            sv.AddLinkedVariable(this);
        }

        #region Constructors
        //Without anything
        public LinkedVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate){
            this.spriteBase=spriteBase;
            linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
        }

        public LinkedVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, BoolLinkedVariableDelegate changedDelegate){
            this.spriteBase=spriteBase;
            linkedVariables=new List<LinkedVariable>();
            this.changedDelegate=changedDelegate;
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
        }

        //With lists
        public LinkedVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, List<LinkedVariable> sensitiveVariables){
            this.spriteBase=spriteBase;
            linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            foreach(LinkedVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        public LinkedVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, BoolLinkedVariableDelegate changedDelegate, List<LinkedVariable> sensitiveVariables){
            this.spriteBase=spriteBase;
            linkedVariables=new List<LinkedVariable>();
            this.changedDelegate=changedDelegate;
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            foreach(LinkedVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        //With arrays
        public LinkedVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, LinkedVariable[] sensitiveVariables){
            this.spriteBase=spriteBase;
            linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            foreach(LinkedVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        public LinkedVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, BoolLinkedVariableDelegate changedDelegate, LinkedVariable[] sensitiveVariables){
            this.spriteBase=spriteBase;
            linkedVariables=new List<LinkedVariable>();
            this.changedDelegate=changedDelegate;
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            foreach(LinkedVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        //Without spritebase, with arrays
        public LinkedVariable(ObjectSpriteBaseDelegate objectDelegate, LinkedVariable[] sensitiveVariables){
            linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            foreach(LinkedVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        public LinkedVariable(ObjectSpriteBaseDelegate objectDelegate, BoolLinkedVariableDelegate changedDelegate, LinkedVariable[] sensitiveVariables){
            linkedVariables=new List<LinkedVariable>();
            this.changedDelegate=changedDelegate;
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            foreach(LinkedVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        //Without spritebase, without arrays
        public LinkedVariable(ObjectSpriteBaseDelegate objectDelegate){
            linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
        }

        public LinkedVariable(ObjectSpriteBaseDelegate objectDelegate, BoolLinkedVariableDelegate changedDelegate){
            linkedVariables=new List<LinkedVariable>();
            this.changedDelegate=changedDelegate;
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
        }

        //With settings
        public LinkedVariable(SpriteBase spriteBase, LinkedVariableParams parameters){
            this.spriteBase=spriteBase;
            linkedVariables=new List<LinkedVariable>();
            if(parameters.changedDelegate!=null){
                this.changedDelegate=parameters.changedDelegate;
            }
            this.objectDelegate=parameters.objectDelegate;
            // _value=objectDelegate(spriteBase);
            if(parameters.sensitiveVariables!=null){
                foreach(LinkedVariable sv in parameters.sensitiveVariables){
                    LinkTo(sv);
                }   
            }
        }
        #endregion Constructors

        public static implicit operator int(LinkedVariable sv){
            if(!sv.round){
                if(sv.objectValue.GetType()==typeof(int)){
                    return (int)sv.objectValue;
                }
                else if(sv.objectValue.GetType()==typeof(double))
                {
                    return (int)(double)sv.objectValue;
                }else{
                    throw new System.InvalidCastException("Cannot cast "+sv.objectValue.GetType()+" to int");
                }
            }else{
                if(sv.objectValue.GetType()==typeof(int)){
                    return (int)Math.Round((double)(int)sv.objectValue);
                }
                else if(sv.objectValue.GetType()==typeof(double))
                {
                    return (int)Math.Round((double)sv.objectValue);
                }else{
                    throw new System.InvalidCastException("Cannot cast "+sv.objectValue.GetType()+" to int");
                }
            }
        }

        public static implicit operator string(LinkedVariable sv){
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

    public class LinkedVariableParams{
        public ObjectSpriteBaseDelegate objectDelegate;
        public BoolLinkedVariableDelegate changedDelegate;
        public LinkedVariable[] sensitiveVariables;

        public LinkedVariableParams(ObjectSpriteBaseDelegate objectDelegate, LinkedVariable[] sensitiveVariables=null, BoolLinkedVariableDelegate changedDelegate=null){
            this.objectDelegate=objectDelegate;
            this.changedDelegate=changedDelegate;
            this.sensitiveVariables=sensitiveVariables;
        }
    }
}