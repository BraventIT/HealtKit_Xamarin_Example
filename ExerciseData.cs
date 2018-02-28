using System;
using Foundation;

namespace HealthKitPoc
{
    public class ExerciseData
    {
		public NSDate DateInit { get; set; }
		public NSDate DateEnd { get; set; }
        public double Kilocalories { get; set; }
		public TimeSpan Time { get; set;}
    }
}
