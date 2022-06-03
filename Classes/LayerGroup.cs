using System.Collections.Generic;

namespace FCSG{
    /// <summary>
    /// A class which stores sprites in a list, keeping them ordered by their depth or by a custom delegate, with the ones with higher values in front
    /// </summary>
    public class LayerGroup{
        public int? layerCount=null;
        public List<SpriteBase> objects;
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
            if(layerCount!=null && (sprite==objects[(int)layerCount]||objects.IndexOf(sprite)<(int)layerCount)){ //The "if" is placed after the addition so that objects.IndexOf(sprite) works without trouble
                layerCount++;
            }
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
            if(layerCount!=null && (sprite==objects[(int)layerCount]||objects.IndexOf(sprite)<(int)layerCount)){
                layerCount--;
            }
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

        public static implicit operator List<SpriteBase>(LayerGroup group){
            return group.objects;
        }
    }
}