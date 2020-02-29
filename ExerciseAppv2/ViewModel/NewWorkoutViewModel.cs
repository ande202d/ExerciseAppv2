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

            _listOfExercises = new List<Exercise>();

            //_tempWorkout = CatalogWorkouts.Instance.GetList()[0];
            _listOfExercises = CatalogExercises.Instance.GetList();
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
            get { return new ObservableCollection<Exercise>(_listOfExercises); }
        }

        public List<Workout> LatestWorkouts
        {
            get
            {
                int lenght = 5;
                List<Workout> listToReturn = new List<Workout>();
                foreach (Workout w in FindLatestWorkouts(SelectedExercise, lenght))
                {
                    listToReturn.Add(w);
                }

                for (int i = listToReturn.Count; i < lenght; i++)
                {
                    listToReturn.Add(new Workout());
                }
                return listToReturn;
            }
        }

        public List<List<Set>> LatestWorkoutsSets
        {
            get
            {
                List<List<Set>> tempListSets = new List<List<Set>>();
                List<Workout> tempLatestWorkouts = LatestWorkouts;

                for (int i = 0; i < tempLatestWorkouts.Count; i++)
                {
                    if (tempLatestWorkouts[i].StartTime > DateTime.MinValue.AddDays(5))
                    {
                        List<Set> tempSets = FindAllRelatedSets(SelectedExercise)
                            .FindAll(ii => ii.WorkoutId == tempLatestWorkouts[i].Id);
                        tempListSets.Add(tempSets);
                    }
                }

                return tempListSets;
            }
        }

        public List<Set> FindAllRelatedSets(Exercise exercise)
        {
            List<Set> tempSets = CatalogSets.Instance.GetList().FindAll(i => i.ExerciseId == exercise.Id);
            return tempSets;
        }
        public List<Workout> FindLatestWorkouts(Exercise exercise, int amount)
        {
            List<Set> tempSets = FindAllRelatedSets(exercise);

            List<int> tempInts = new List<int>();
            foreach (Set s in tempSets)
            {
                if(!tempInts.Contains(s.WorkoutId)) tempInts.Add(s.WorkoutId);
            }

            List<Workout> tempWorkouts = CatalogWorkouts.Instance.GetList().FindAll(i => tempInts.Contains(i.Id));
            List<Workout> listToReturn = new List<Workout>();

            for (int i = 0; i < amount; i++)
            {
                listToReturn.Add(new Workout());
            }

            for (int i = 0; i < amount; i++)
            {
                List<Workout> temptempWorkouts = tempWorkouts;
                Workout latesWorkout = new Workout();
                foreach (Workout w in temptempWorkouts)
                {
                    if (w.StartTime > latesWorkout.StartTime) latesWorkout = w;
                }

                listToReturn[i] = latesWorkout;
                temptempWorkouts.Remove(latesWorkout);
            }
            

            return listToReturn;
        }


        public async void AddSetMethod()
        {
            /*if (SelectedExercise != null && SetToAdd.Reps > 0)
            {
                //_listOfSets.Add(new Set(_tempWorkout.Id, SelectedExercise.Id, SetToAdd.Reps, SetToAdd.Weight));
                if (_currentWorkout == null)
                {

                    await CatalogWorkouts.Instance.Create(new Workout(DateTime.Now));
                    _currentWorkout = CatalogWorkouts.Instance.GetList().Last();
                }

                if (SelectedExercise.Id != -1)
                {
                    Set tempSet = new Set(_currentWorkout.Id, SelectedExercise.Id, SetToAdd.Reps, SetToAdd.Weight);
                    CatalogSets.Instance.Create(tempSet);
                }
                OnPropertyChanged(nameof(SelectedExercise));
                OnPropertyChanged(nameof(LatestWorkouts));
                OnPropertyChanged(nameof(LatestWorkoutsSets));
            }*/

            //string path = "C:\Users\ander\Documents\Private\Programmer\dataDB.mdf";
            string path = "Data Source=" + @"F:\Users\Anders\Private\Programering\Visual Studio UWP Apps\ExerciseAppv2\data.db; New=True; Compress=True; Version=3";
            string connString =
                @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ander\Documents\Private\Programmer\dataDB.mdf;Integrated Security=True;Connect Timeout=30";
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
            connBuilder.DataSource = "F:/Users/Anders/Private/Programering/Visual Studio UWP Apps/ExerciseAppv2/data.db";

            SqlConnection conn = new SqlConnection(path);
            conn.Open();
            string insertQuery = "INSERT INTO Workout (StartTime, EndTime) VALUES (p1, p2)";

            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                Workout w = new Workout(DateTime.Now);
                cmd.Parameters.AddWithValue("p1", w.StartTime);
                cmd.Parameters.AddWithValue("p2", DateTime.MaxValue);

                cmd.ExecuteNonQuery();
            }
            conn.Dispose();
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
