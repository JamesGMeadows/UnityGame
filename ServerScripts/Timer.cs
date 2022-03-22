using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Timer
    {
        private float finishTime;
        private float interval;

        public Timer(float interval)
        {
            this.interval = interval;
            NextTime();
        }



        public bool CheckFinished(bool reset)
        {
            bool finished = Time.time > finishTime;
            if (finished && reset)
            {
                NextTime();
            }
            return finished;
        }

        public void SetInterval(float interval)
        {
            this.interval = interval;
        }

        public float GetInterval()
        {
            return this.interval;
        }

        public void NextTime()
        {
            finishTime = Time.time + interval;
        }
    }

}
