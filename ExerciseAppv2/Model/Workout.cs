using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseAppv2.Model
{
    public class Workout
    {
        private static int _idCounter = 0;
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Workout(DateTime timeRightNow)
        {
            Id = _idCounter;
            _idCounter++;
            StartTime = timeRightNow;
        }

        public Workout()
        {

        }

        public void EndWorkout()
        {
            EndTime = DateTime.Now;
        }
    }
}
