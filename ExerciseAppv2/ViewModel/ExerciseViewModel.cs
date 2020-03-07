using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ExerciseAppv2.Annotations;
using ExerciseAppv2.Common;
using ExerciseAppv2.Model;

namespace ExerciseAppv2.ViewModel
{
    public class ExerciseViewModel : INotifyPropertyChanged
    {

        #region instance field

        private Exercise _selectedExercise;
        private List<Exercise> _listOfExercises;
        private static int _numberOfWorkouts = 10;

        #endregion
        #region constructor

        public ExerciseViewModel()
        {
            UpdateExerciseCommand = new RelayCommand(UpdateExerciseMethod);
            CreateExerciseCommand = new RelayCommand(CreateExerciseMethod);
        }

        #endregion
        #region properties

        public ICommand UpdateExerciseCommand { get; set; }
        public ICommand CreateExerciseCommand { get; set; }
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
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSelectedExercisePicked));
                OnPropertyChanged(nameof(ShowPrevData));
            }
        }

        public bool IsSelectedExercisePicked
        {
            get { return (SelectedExercise.Id != -1); }
        }

        public List<Exercise> ListOfExercises
        {
            get { return Catalog.Instance.GetAllExercises(); }
        }

        public List<List<Set>> PrevData
        {
            get
            {
                List<List<Set>> listToReturn = new List<List<Set>>();
                List<Set> fullList = Catalog.Instance.GetSetsFromText($"SELECT * FROM Sets WHERE ExerciseId={SelectedExercise.Id}");
                var query1 =
                    from s in fullList
                    group s by s.WorkoutId;
                foreach (var v in query1)
                {
                    List<Set> tempList = new List<Set>();
                    foreach (Set ss in v)
                    {
                        tempList.Add(ss);
                    }
                    listToReturn.Add(tempList);
                }

                return listToReturn;
            }
        }

        public List<KeyValuePair<Workout, List<Set>>> ShowPrevData
        {
            get
            {
                List<KeyValuePair<Workout, List<Set>>> listToReturn = new List<KeyValuePair<Workout, List<Set>>>();

                List<List<Set>> prevData = PrevData;
                foreach (List<Set> mainList in prevData)
                {
                    Workout wToUse = Catalog.Instance.GetWorkoutsFromText($"SELECT * FROM Workouts WHERE Id={mainList[0].WorkoutId}")[0];
                    List<Set> listToUse = mainList;
                    listToReturn.Add(new KeyValuePair<Workout, List<Set>>(wToUse, listToUse));
                }

                return listToReturn;
            }
        }

        #endregion
        #region Methods

        public void CreateExerciseMethod()
        {
            if (!string.IsNullOrEmpty(
                Catalog.Instance.GetExercisesFromText("SELECT * FROM Exercises ORDER BY Id DESC LIMIT 1")[0].Name))
            {
                Exercise e = new Exercise();
                Catalog.Instance.Add(e);
                OnPropertyChanged(nameof(ListOfExercises));
            }
        }

        public void UpdateExerciseMethod()
        {
            if (SelectedExercise.Id != -1)
            {
                //Catalog.Instance.Update(SelectedExercise);
                //OnPropertyChanged(nameof(SelectedExercise));
                //OnPropertyChanged(nameof(ListOfExercises));
                var hej = PrevData;
            }
        }

        #endregion

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        #endregion

    }
}
