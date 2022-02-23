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

    /// <summary>
    /// A function which performs actions depending of the given time. The time ranges from 0 to 1, where 0 is "start" and 1 is "end".
    /// </summary>
    public delegate void TimeFunction(double time);
    public class TimeEvent{//FIXME: should add an alternative which uses delta time
        private long totalTime;
        private TimeFunction normalFunction;
        private TimeFunction finalFunction;
        private long startTime;
        private int deltaTime;// Initial delta time used to make the first call not be 0 (which would always happen with deltaTime=0)
        public bool isFinished{get; private set;}
        public TimeEvent(double totalSeconds, TimeFunction normalFunction, TimeFunction finalFunction=null, int deltaTime=0){
            this.totalTime=(long)(totalSeconds*1000);
            this.normalFunction=normalFunction;
            this.finalFunction=finalFunction;
            startTime=DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.deltaTime=deltaTime;
        }

        public void RunFunction(){
            long currentTime=DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long timeElapsed=currentTime-startTime+deltaTime;
            if(timeElapsed>=totalTime){
                if(finalFunction!=null){
                    finalFunction(1);
                }
                else{
                    normalFunction(((double)timeElapsed)/(double)totalTime);
                }
                isFinished=true;
            }else{
                normalFunction(((double)timeElapsed)/(double)totalTime);
            }
        }
    }
}