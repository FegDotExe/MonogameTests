using System.Collections.Generic;

namespace FCSG{
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
}