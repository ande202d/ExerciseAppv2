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
using Windows.Media.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
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
        private NewWorkoutViewSetToAdd _setToAdd2;
        private bool _workoutStarted;

        public NewWorkoutViewModel()
        {
            AddSetCommand = new RelayCommand(AddSetMethod);
            WorkoutStartedCommand = new RelayCommand(WorkoutStartedMethod);
            _workoutStarted = false;
            _listOfExercises = Catalog.Instance.GetAllExercises();
        }

        public ICommand AddSetCommand { get; set; }
        public ICommand WorkoutStartedCommand { get; set; }


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

        public Set SetToAdd
        {
            get
            {
                if (_setToAdd == null) _setToAdd = new Set();
                return _setToAdd;
            }
            set { _setToAdd = value; }
        }

        public NewWorkoutViewSetToAdd SetToAdd2
        {
            get
            {
                if (_setToAdd2 == null) _setToAdd2 = new NewWorkoutViewSetToAdd();
                return _setToAdd2;
            }
            set { _setToAdd2 = value; }
        }

        #region WorkoutStarted (CurrentWorkout)

        public bool WorkoutStarted
        {
            get { return _workoutStarted;}
            set { _workoutStarted = value; }
        }

        public SolidColorBrush WorkoutStartedColor
        {
            get
            {
                if (WorkoutStarted) return new SolidColorBrush(Colors.Red);
                else return new SolidColorBrush(Colors.ForestGreen);
            }
        }

        public string WorkoutStartedText
        {
            get
            {
                if (WorkoutStarted) return "STOP";
                else return "START";
            }
        }

        public Workout CurrentWorkout
        {
            get { return _currentWorkout; }
            set
            {
                _currentWorkout = value;
                OnPropertyChanged();
            }
        } 

        #endregion

        #region List to be seen

        public ObservableCollection<Exercise> ListOfExercises
        {
            get { return new ObservableCollection<Exercise>(Catalog.Instance.GetAllExercises()); }
        }

        public List<Workout> LatestWorkouts
        {
            get
            {
                int lenght = 5;
                List<Workout> listToReturn = new List<Workout>();

                //string text = "SELECT * FROM Workouts " +
                //              $"WHERE Id IN (SELECT WorkoutId FROM Sets WHERE ExerciseId={SelectedExercise.Id})";

                //foreach (Workout w in Catalog.Instance.GetWorkoutsFromText(text))
                //{
                //    if (listToReturn.Count < lenght) listToReturn.Add(w);
                //    else
                //    {
                //        Workout replaceHolder = new Workout(DateTime.MaxValue);
                //        for (int i = 0; i < listToReturn.Count; i++)
                //        {
                //            if (listToReturn[i].StartTime < w.StartTime)
                //            {
                //                if (listToReturn[i].StartTime < replaceHolder.StartTime)
                //                {
                //                    replaceHolder = listToReturn[i];
                //                }
                //            }
                //        }

                //        if (replaceHolder.StartTime != DateTime.MaxValue)
                //        {
                //            listToReturn.Remove(replaceHolder);
                //            listToReturn.Add(w);
                //        }
                //    }
                //}

                //listToReturn.Sort((x, y) => DateTime.Compare(y.StartTime, x.StartTime));

                string text = $"SELECT * FROM Workouts " +
                              $"WHERE Id IN " +
                                    $"(SELECT WorkoutId FROM Sets " +
                                    $"WHERE ExerciseId={SelectedExercise.Id}) " +
                              //$"ORDER BY StartTime DESC " +
                              $"LIMIT {lenght}";

                listToReturn = Catalog.Instance.GetWorkoutsFromText(text);

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
                    //string dataToConvert = Catalog.Instance.GetSetData(SelectedExercise.Id);
                    //List<Set> listOfSets = new List<Set>();
                    //foreach (string s in dataToConvert.Split("$"))
                    //{
                    //    listOfSets.Add(new Set());
                    //}

                    //Catalog.Instance.GetSetsFromText()

                    listToReturn.Add(Catalog.Instance.GetSetsFromText(text));
                }

                return listToReturn;
            }
        }

        #endregion

        #region Methods

        public async void AddSetMethod()
        {
            if (WorkoutStarted && SelectedExercise.Id != -1)
            {
                Set s = new Set();
                s.WorkoutId = CurrentWorkout.Id;
                s.ExerciseId = SelectedExercise.Id;
                s.Reps = SetToAdd2.RepsInt;
                s.Weight = SetToAdd2.WeightDouble;
                Catalog.Instance.Add(s);
            }
            OnPropertyChanged(nameof(LatestWorkouts));
            OnPropertyChanged(nameof(LatestWorkoutsSets));
        }

        public void WorkoutStartedMethod()
        {
            if (WorkoutStarted)
            {
                if (Catalog.Instance.GetSetsFromText("SELECT * FROM Sets " +
                                                     $"WHERE WorkoutId={CurrentWorkout.Id}").Count > 0)
                {
                    CurrentWorkout.EndTime = DateTime.Now;
                    Catalog.Instance.Update(CurrentWorkout);
                }
                else
                {
                    Catalog.Instance.Remove(typeof(Workout), CurrentWorkout.Id);
                }
                CurrentWorkout = null;
                WorkoutStarted = false;
            }
            else
            {
                Catalog.Instance.Add(new Workout(DateTime.Now));
                CurrentWorkout = Catalog.Instance.GetWorkoutsFromText("SELECT * FROM Workouts ORDER BY Id DESC LIMIT 1")[0];
                WorkoutStarted = true;
            }
            OnPropertyChanged(nameof(WorkoutStarted));
            OnPropertyChanged(nameof(WorkoutStartedColor));
            OnPropertyChanged(nameof(WorkoutStartedText));
        } 

        #endregion

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
