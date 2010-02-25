using System;
using System.Collections.Generic;
using System.Text;

namespace SourceAFIS.Tuning
{
    public sealed class BenchmarkTimer
    {
        public DateTime StartTime;
        public DateTime StopTime;
        public DateTime LastTime;

        public TimeSpan TotalTime { get { return StopTime - StartTime; } }
        public TimeSpan Elapsed { get { return LastTime - StartTime; } }

        public void Start() { StartTime = DateTime.Now; }
        public void Stop() { StopTime = DateTime.Now; }
        public void Update() { LastTime = DateTime.Now; }
    }
}
