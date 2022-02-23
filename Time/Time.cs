using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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

    public delegate void TimeFunction(double time);
    public class TimeEvent{
        private long totalTime;
        private TimeFunction normalFunction;
        private TimeFunction finalFunction;
        private long startTime;
        public bool isFinished{get; private set;}
        public TimeEvent(double totalSeconds, TimeFunction normalFunction, TimeFunction finalFunction){
            this.totalTime=(long)(totalSeconds*1000);
            this.normalFunction=normalFunction;
            this.finalFunction=finalFunction;
            startTime=DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public void RunFunction(){
            long currentTime=DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long timeElapsed=currentTime-startTime;
            if(timeElapsed>=totalTime){
                finalFunction(1);
                isFinished=true;
            }else{
                normalFunction(((double)timeElapsed)/(double)totalTime);
            }
        }
    }
}