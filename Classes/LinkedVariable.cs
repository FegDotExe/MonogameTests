using System.Collections.Generic;
using System;
namespace FCSG{
    public delegate bool BoolLinkedVariableDelegate(LinkedVariable var);
    public delegate LinkedVariable[] LinkedVarListSpriteBaseDelegate(SpriteBase var);
    public class LinkedVariable{
        protected bool round=true; //Wether int values should be rounded or not
        protected bool updating=false; //If true, it means that the value is being updated (and has not yet been set)
        protected ObjectSpriteBaseDelegate objectDelegate;
        protected SpriteBase spriteBase;
        protected List<LinkedVariable> linkedVariables=new List<LinkedVariable>(); //List of variables which will be updated when this variable is updated
        protected LinkedVarListSpriteBaseDelegate linkedVariablesDelegate=(SpriteBase sb)=>new LinkedVariable[] {}; //This is used to retrieve self-referencial values

        protected object _value=null;
        protected object objectValue{
            get{
                if(_value==null){ //The first implementation would instantly calculate the value of the variable, leading to many nullPointer errors as the Sprite was not linked yet. This solves the errors by calculating the value the first time it is actually used.
                    _value=objectDelegate(spriteBase);
                }
                if(!updating){
                    return _value;
                }
                else{
                    // if(spriteBase!=null){
                    //     Console.WriteLine("Accessed delegate ("+objectDelegate(spriteBase)+"); sprite is "+spriteBase.name);
                    // }
                    _value=objectDelegate(spriteBase);//HACK: RANDOM ADDITION SEE IF THIS ACTUALLY WORKS->should be fine
                    updating=false;
                    return objectDelegate(spriteBase);
                }
            }
            set{
                _value=value;
            }
        }

        /// <summary>
        /// Set the value of the variable to the given value; if the value is different from the previous one, an update will be triggered.
        /// </summary>
        public void Set(object value){
            this.objectDelegate=(SpriteBase sb)=>value;
            if(Changed()){
                SetUpdating();
                Update();
            }
        }

        public void Round(bool round){
            this.round=round;
        }
        public void SetSprite(SpriteBase sprite){
            this.spriteBase=sprite;
        }

        /// <summary>
        /// Sets updating to true for all the variables linked to this (and variables linked to variables linked to this, and so on)
        /// </summary>
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

        /// <summary>
        /// Updates the value of _value and if it has changed, updates all variables linked to this one.
        /// </summary>
        private void Update(){
            if(Changed()){ //Is true if the value should be considered as changed
                // if(objectDelegate(spriteBase).GetType()!=typeof(string)){
                //     Console.WriteLine("SVariable updated: "+this.ToString());
                //     Console.WriteLine(_value+" changed to "+ objectDelegate(this.spriteBase));
                // }
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

        /// <summary>
        /// Returns true if the value of the variable has changed since the last update.
        /// </summary>
        private bool Changed(){
            bool changed=false;
            if(_value==null){
                changed=true;
            }else if(_value.GetType()==typeof(int)){
                changed=(int)_value!=(int)objectDelegate(this.spriteBase);
            }else if(_value.GetType()==typeof(double)){
                changed=(double)_value!=(double)objectDelegate(this.spriteBase);
            }else{
                Console.WriteLine("Warning. A LinkedVariable has no way to tell if a "+_value.GetType()+" has changed. It will be assumed that it has not. If you want LinkedVariable to recognize "+_value.GetType()+" changes, modify LinkedVariable.Changed");
            }
            return changed;
        }

        /// <summary>
        /// If the value is updating, updates _value and deactivates updating
        /// </summary>
        private void FixValue(){
            if(updating){
                _value=objectDelegate(spriteBase);
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
            // linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            // FixValue();
        }

        //With arrays
        public LinkedVariable(SpriteBase spriteBase, ObjectSpriteBaseDelegate objectDelegate, LinkedVariable[] sensitiveVariables){
            this.spriteBase=spriteBase;
            // linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            // FixValue();
        }

        //Without spritebase, with arrays
        public LinkedVariable(ObjectSpriteBaseDelegate objectDelegate, LinkedVariable[] sensitiveVariables){
            // linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            foreach(LinkedVariable sv in sensitiveVariables){
                LinkTo(sv);
            }
        }

        //Without spritebase, without arrays
        public LinkedVariable(ObjectSpriteBaseDelegate objectDelegate){
            // linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=objectDelegate;
            // _value=objectDelegate(spriteBase);
            // FixValue();
        }

        //With settings
        /// <summary>
        /// Creates a LinkedVariable with the given settings; the variable will then need to be activated using Activate() in order for variables to link correctly
        /// </summary>
        public LinkedVariable(SpriteBase spriteBase, LinkedVariableParams parameters){
            this.spriteBase=spriteBase;
            // linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=parameters.objectDelegate;
            // _value=objectDelegate(spriteBase);
            // FixValue();
            if(parameters.sensitiveVariables!=null){
                this.linkedVariablesDelegate=(SpriteBase sb)=>parameters.sensitiveVariables;
            }else if(parameters.linkedVariableDelegate!=null){
                this.linkedVariablesDelegate=parameters.linkedVariableDelegate;
            }
        }

        //With settings but without SpriteBase
        /// <summary>
        /// Creates a LinkedVariable with the given settings; the variable will then need to be activated using Activate(SpriteBase) in order for variables to link correctly
        /// </summary>
        public LinkedVariable(LinkedVariableParams parameters){
            // linkedVariables=new List<LinkedVariable>();
            this.objectDelegate=parameters.objectDelegate;
            // _value=objectDelegate(spriteBase);
            // FixValue();
            if(parameters.sensitiveVariables!=null){
                this.linkedVariablesDelegate=(SpriteBase sb)=>parameters.sensitiveVariables;
            }else if(parameters.linkedVariableDelegate!=null){
                this.linkedVariablesDelegate=parameters.linkedVariableDelegate;
            }
        }
        #endregion Constructors

        ///<summary>
        ///Links this to the LinkedVariables it should be linked to. Is used to link everything once it is sure that values are not null. It uses the linkedVariablesDelegate to get the linked variables
        ///</summary>
        public void Activate(){
            foreach(LinkedVariable sv in linkedVariablesDelegate(spriteBase)){
                LinkTo(sv);
            }
        }

        ///<summary>
        ///Links this to the LinkedVariables it should be linked to. Is used to link everything once it is sure that values are not null. It uses the linkedVariablesDelegate to get the linked. This overload of activate also links a new SpriteBase.
        ///</summary>
        public void Activate(SpriteBase sb){
            this.spriteBase=sb;
            foreach(LinkedVariable sv in linkedVariablesDelegate(spriteBase)){
                LinkTo(sv);
            }
        }

        public static implicit operator int(LinkedVariable sv){
            if(!sv.round){ //If the value should not be rounded
                if(sv.objectValue.GetType()==typeof(int)){
                    return (int)sv.objectValue;
                }
                else if(sv.objectValue.GetType()==typeof(double))
                {
                    return (int)(double)sv.objectValue;
                }else{
                    throw new System.InvalidCastException("Cannot cast "+sv.objectValue.GetType()+" to int");
                }
            }else{ //If the value should be rounded
                if(sv.objectValue.GetType()==typeof(int)){
                    return (int)sv.objectValue;
                }
                else if(sv.objectValue.GetType()==typeof(double))
                {
                    return (int)Math.Round((double)sv.objectValue);
                }else if(sv.objectValue.GetType()==typeof(LinkedVariable)){
                    return (int)(LinkedVariable)sv.objectValue;
                }
                else{
                    throw new System.InvalidCastException("Cannot cast "+sv.objectValue.GetType()+" to int");
                }
            }
        }

        public static implicit operator double(LinkedVariable lv){
            if(lv.objectValue.GetType()==typeof(double)){
                return (double)lv.objectValue;
            }else{
                throw new System.InvalidCastException("Cannot cast "+lv.objectValue.GetType()+" to double");
            }
        }

        public static implicit operator string(LinkedVariable sv){
            if(sv.objectValue.GetType()==typeof(string)){
                return (string)sv.objectValue;
            }else{
                throw new System.InvalidCastException("Cannot cast "+sv.objectValue.GetType()+" to string");
            }
        }

        public override string ToString(){
            return "lv["+objectValue.ToString()+"]";
        }
    }

    public class LinkedVariableParams{
        public ObjectSpriteBaseDelegate objectDelegate;
        public LinkedVariable[] sensitiveVariables;
        public LinkedVarListSpriteBaseDelegate linkedVariableDelegate;

        public LinkedVariableParams(ObjectSpriteBaseDelegate objectDelegate, LinkedVariable[] sensitiveVariables=null, LinkedVarListSpriteBaseDelegate sensitiveDelegate=null){
            this.objectDelegate=objectDelegate;
            this.sensitiveVariables=sensitiveVariables;
            this.linkedVariableDelegate=sensitiveDelegate;
        }
    }
}