using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExerciseAppv2.Common;
using ExerciseAppv2.Model;
using ExerciseAppv2.Properties;
using Newtonsoft.Json;

namespace ExerciseAppv2.ViewModel
{
    public class NewWorkoutViewModel : INotifyPropertyChanged
    {
        private List<Exercise> _listOfExercises;
        private List<Set> _listOfSets;
        private Workout _tempWorkout;
        private Exercise _selectedExercise;
        private Set _setToAdd;

        public NewWorkoutViewModel()
        {
            AddSetCommand = new RelayCommand(AddSetMethod);
            //_tempWorkout = new Workout(DateTime.Now);
            _tempWorkout = CatalogWorkouts.Instance.GetList()[0];

            _listOfExercises = new List<Exercise>();
            _listOfExercises = CatalogExercises.Instance.GetList();
            //_listOfExercises.Add(new Exercise("Ex1", "Muscle1", "Des1"));
            //_listOfExercises.Add(new Exercise("Ex2", "Muscle2", "Des2"));
            //_listOfExercises.Add(new Exercise("Ex3", "Muscle3", "Des3"));
            //_listOfExercises.Add(new Exercise("Ex4", "Muscle4", "Des4"));
            //_listOfExercises.Add(new Exercise("Ex5", "Muscle5", "Des5"));

            _listOfSets = new List<Set>();
            _listOfSets = CatalogSets.Instance.GetList();
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[0].Id, 12, 50));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[0].Id, 10, 55));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[0].Id, 8, 60));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[2].Id, 8, 80));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[2].Id, 8, 80));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[2].Id, 6, 90));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[2].Id, 4, 95));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[4].Id, 5, 120));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[4].Id, 5, 120));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[4].Id, 5, 120));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[4].Id, 5, 120));
            //_listOfSets.Add(new Set(_tempWorkout.Id, _listOfExercises[4].Id, 5, 120));

            //CatalogWorkouts.Instance.Create(_tempWorkout);

            //foreach (Exercise e in ListOfExercises)
            //{
            //    CatalogExercises.Instance.Create(e);
            //}

            //foreach (Set s in _listOfSets)
            //{
            //    CatalogSets.Instance.Create(s);
            //}
        }

        public ICommand AddSetCommand { get; set; }

        public Exercise SelectedExercise
        {
            get
            {
                if (_selectedExercise == null) _selectedExercise = new Exercise();
                return _selectedExercise;
            }
            set
            {
                _selectedExercise = value; 
                OnPropertyChanged(nameof(ListOfSets));
                OnPropertyChanged();
            }
        }

        public Set SetToAdd
        {
            get
            {
                if (_setToAdd == null) _setToAdd = new Set();
                return _setToAdd;
            }
            set { _setToAdd = value; }
        }

        public ObservableCollection<Exercise> ListOfExercises
        {
            get { return new ObservableCollection<Exercise>(_listOfExercises); }
        }
        public ObservableCollection<Set> ListOfSets
        {
            get
            {
                List<Set> listToReturn = CatalogSets.Instance.GetList().FindAll(i => i.ExerciseId == SelectedExercise.Id);

                return new ObservableCollection<Set>(listToReturn);
            }
        }

        //public int[] LatestFiveWorkouts
        //{
        //    get
        //    {
        //        int[] a = new int[5];
        //        foreach (Set s in ListOfSets)
        //        {
        //            if(a[0] )
        //        }
        //    }
        //}

        public void AddSetMethod()
        {
            if (SelectedExercise != null && SetToAdd.Reps > 0)
            {
                //_listOfSets.Add(new Set(_tempWorkout.Id, SelectedExercise.Id, SetToAdd.Reps, SetToAdd.Weight));
                Set tempSet = new Set(_tempWorkout.Id, SelectedExercise.Id, SetToAdd.Reps, SetToAdd.Weight);
                CatalogSets.Instance.Create(tempSet);
                OnPropertyChanged(nameof(ListOfSets));
                
            }
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion
    }
}
