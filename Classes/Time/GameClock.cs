using System;

namespace FCSG{
    public class GameClock{
        private long startTime;
        private long lastTime;
        public long elapsed{
            get{
                return DateTimeOffset.Now.ToUnixTimeMilliseconds()-lastTime;
            }
        }

        public GameClock(){
            startTime=DateTimeOffset.Now.ToUnixTimeMilliseconds();
            lastTime=startTime;
        }

        public void Update(){
            long currentTime=DateTimeOffset.Now.ToUnixTimeMilliseconds();
            lastTime=currentTime;
        }
    }
}