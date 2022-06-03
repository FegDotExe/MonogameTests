namespace FCSG{
    public partial class Utilities{
        /// <summary>
        /// Used to choose between two objects, giving priority to the second one; if o2 is null, o1 is returned, otherwise o2 is returned.
        /// </summary>
        public static T Choose<T>(T o1,T o2){
            if(o2==null){
                return o1;
            }else{
                return o2;
            }
        }
    }
}