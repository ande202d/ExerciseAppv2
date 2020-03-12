using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseAppv2.Model
{
    public class NewWorkoutViewSetToAdd
    {
        public string Reps { get; set; }
        public string Weight { get; set; }

        public int RepsInt
        {
            get {return int.Parse(Reps); }
        }

        public double WeightDouble
        {
            get
            {
                return double.Parse(Weight.Replace(',', '.'));
            }
        }
    }
}
