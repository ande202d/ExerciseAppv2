using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseAppv2.Model
{
    public class Set
    {
        private static int _idCounter = 0;
        public int Id { get; set; }
        public int WorkoutId { get; set; }
        public int ExerciseId { get; set; }
        public int Reps { get; set; }
        public int Weight { get; set; }
        public DateTime Time { get; set; }

        public Set()
        {
            
        }

        public Set(int workoutId, int exerciseId, int reps, int weight)
        {
            Id = _idCounter;
            _idCounter++;

            WorkoutId = workoutId;
            ExerciseId = exerciseId;
            Reps = reps;
            Weight = weight;
            Time = DateTime.Now;
        }
    }
}
