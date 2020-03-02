using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using ExerciseAppv2.Common;
using ExerciseAppv2.Model;
using ExerciseAppv2.Properties;
using Newtonsoft.Json;
using Microsoft.Data.Sqlite;

namespace ExerciseAppv2.ViewModel
{
    public class NewWorkoutViewModel : INotifyPropertyChanged
    {
        private List<Exercise> _listOfExercises;
        private Workout _currentWorkout;
        private Exercise _selectedExercise;
        private Set _setToAdd;

        public NewWorkoutViewModel()
        {
            AddSetCommand = new RelayCommand(AddSetMethod);

            _listOfExercises = Catalog.Instance.GetAllExercises();
        }

        public ICommand AddSetCommand { get; set; }

        public Exercise SelectedExercise
        {
            get
            {
                if (_selectedExercise == null)
                {
                    Exercise e = new Exercise();
                    e.Id = -1;
                    _selectedExercise = e;
                }
                return _selectedExercise;
            }
            set
            {
                _selectedExercise = value; 
                OnPropertyChanged(nameof(LatestWorkouts));
                OnPropertyChanged(nameof(LatestWorkoutsSets));
                OnPropertyChanged();
            }
        }

        public bool isWorkoutStarted
        {
            get { return (_currentWorkout != null); }
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
            get { return new ObservableCollection<Exercise>(Catalog.Instance.GetAllExercises()); }
        }

        public List<Workout> LatestWorkouts
        {
            get
            {
                int lenght = 5;

                string text = "SELECT * FROM Workouts " +
                              $"WHERE Id IN (SELECT WorkoutId FROM Sets WHERE ExerciseId={SelectedExercise.Id})";

                List<Workout> listToReturn = new List<Workout>();

                foreach (Workout w in Catalog.Instance.GetWorkoutsFromText(text))
                {
                    if (listToReturn.Count < lenght) listToReturn.Add(w);
                    else
                    {
                        Workout replaceHolder = new Workout(DateTime.MaxValue);
                        for (int i = 0; i < listToReturn.Count; i++)
                        {
                            if (listToReturn[i].StartTime < w.StartTime)
                            {
                                if (listToReturn[i].StartTime < replaceHolder.StartTime)
                                {
                                    replaceHolder = listToReturn[i];
                                }
                            }
                        }

                        if (replaceHolder.StartTime != DateTime.MaxValue)
                        {
                            listToReturn.Remove(replaceHolder);
                            listToReturn.Add(w);
                        }
                    }
                }

                listToReturn.Sort((x, y) => DateTime.Compare(y.StartTime, x.StartTime));

                return listToReturn;
            }
        }

        public List<List<Set>> LatestWorkoutsSets
        {
            get
            {
                List<List<Set>> listToReturn = new List<List<Set>>();
                List<Workout> tempLatestWorkouts = LatestWorkouts;

                foreach (Workout w in tempLatestWorkouts)
                {
                    string text = "SELECT * FROM Sets " +
                                  $"WHERE (WorkoutId={w.Id} AND ExerciseId={SelectedExercise.Id})";
                    listToReturn.Add(Catalog.Instance.GetSetsFromText(text));
                }

                return listToReturn;
            }
        }


        public async void AddSetMethod()
        {
            //foreach (var i in CatalogExercises.Instance.GetList())
            //{
            //    i.Id++;
            //    Catalog.Instance.Add(i);
            //}
            //foreach (var i in CatalogWorkouts.Instance.GetList())
            //{
            //    i.Id++;
            //    Catalog.Instance.Add(i);
            //}
            //foreach (var i in CatalogSets.Instance.GetList())
            //{
            //    i.ExerciseId++;
            //    i.WorkoutId++;
            //    Catalog.Instance.Add(i);
            //}

            //Exercise ee = (Exercise)Catalog.Instance.Read(typeof(Exercise), 5);
            //Workout ww = (Workout)Catalog.Instance.Read(typeof(Workout), 10);
            //Set ss = (Set)Catalog.Instance.Read(typeof(Set), 5);

            List<Exercise> list1 = Catalog.Instance.GetAllExercises();
            List<Workout> list2 = Catalog.Instance.GetAllWorkouts();
            List<Set> list3 = Catalog.Instance.GetAllSets();
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
