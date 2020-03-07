using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseAppv2.Model
{
    public class Exercise
    {
        private static int _idCounter = 0;
        public int Id { get; set; }
        public string Name { get; set; }
        public string MuscleGroup { get; set; }
        public string Description { get; set; }

        public Exercise()
        {
            
        }

        public Exercise(string name, string muscleGroup, string description)
        {
            Id = _idCounter;
            _idCounter++;

            Name = name;
            MuscleGroup = muscleGroup;
            Description = description;
        }
    }
}
