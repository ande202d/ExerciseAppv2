using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Calls.Background;
using Windows.ApplicationModel.Chat;
using Windows.Storage;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace ExerciseAppv2.Model
{
    public class Catalog
    {
        public static string dbname = "dataDB.db";
        public static string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
        public static bool dbcreated;

        //PATH = C:\Users\Anders\AppData\Local\Packages\56cade51-3641-480e-afdb-7fbd4d621f43_pth98vj66mbec\LocalState\dataDB.db

        private Catalog()
        {
            CreateDB();
        }

        #region Singleton

        private static Catalog _instance;

        public static Catalog Instance
        {
            get
            {
                if (_instance == null) _instance = new Catalog();
                return _instance;
            }
        }

        #endregion


        #region Private Methods

        private async Task CreateDB()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(dbname, CreationCollisionOption.OpenIfExists);

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                //EXERCISES TABLE
                string cmdText = "CREATE TABLE IF NOT EXISTS Exercises" +
                                 "(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                 "Name TEXT NOT NULL, " +
                                 "MuscleGroup TEXT NOT NULL, " +
                                 "Description TEXT NOT NULL)";
                using (SqliteCommand cmd = new SqliteCommand(cmdText, db))
                {
                    cmd.ExecuteNonQuery();
                }

                //WORKOUTS TABLE
                cmdText = "CREATE TABLE IF NOT EXISTS Workouts" +
                          "(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                          "StartTime TEXT NOT NULL, " +
                          "EndTime TEXT NOT NULL)";
                using (SqliteCommand cmd = new SqliteCommand(cmdText, db))
                {
                    cmd.ExecuteNonQuery();
                }

                //SETS TABLE
                cmdText = "CREATE TABLE IF NOT EXISTS Sets" +
                          "(Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                          "WorkoutId INTEGER NOT NULL, " +
                          "ExerciseId INTEGER NOT NULL, " +
                          "SetData String NOT NULL, " +
                          //"Reps INTEGER NOT NULL, " +
                          //"Weight INTEGER NOT NULL, " +
                          "UNIQUE(WorkoutId, ExerciseId)" +
                          "FOREIGN KEY(ExerciseId) REFERENCES Exercises(Id) ON DELETE NO ACTION," +
                          "FOREIGN KEY(WorkoutId) REFERENCES Workouts(Id) ON DELETE NO ACTION)";
                using (SqliteCommand cmd = new SqliteCommand(cmdText, db))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ExecuteQuery(string s)
        {
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand cmd = new SqliteCommand();
                cmd.Connection = db;

                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();

                cmd.CommandText = s;
                cmd.ExecuteNonQuery();
                db.Close();
            }
        }

        private List<Exercise> DataToExercises(string textToExecute)
        {
            List<Exercise> listToReturn = new List<Exercise>();
            Exercise item = new Exercise();

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand cmd = db.CreateCommand();
                string cmdText = textToExecute;
                cmd.CommandText = cmdText;

                SqliteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    item.Id = reader.GetInt32(0);
                    item.Name = reader.GetString(1);
                    item.MuscleGroup = reader.GetString(2);
                    item.Description = reader.GetString(3);

                    listToReturn.Add(item);
                    item = new Exercise();
                }
            }

            return listToReturn;
        }

        private List<Workout> DataToWorkouts(string textToExecute)
        {
            List<Workout> listToReturn = new List<Workout>();
            Workout item = new Workout();

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand cmd = db.CreateCommand();
                string cmdText = textToExecute;
                cmd.CommandText = cmdText;

                SqliteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    item.Id = reader.GetInt32(0);
                    item.StartTime = DateTime.Parse(reader.GetString(1));
                    item.EndTime = DateTime.Parse(reader.GetString(2));

                    listToReturn.Add(item);
                    item = new Workout();
                }
            }

            return listToReturn;
        }

        private List<Set> DataToSets(string textToExecute)
        {
            List<Set> listToReturn = new List<Set>();
            Set item = new Set();

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand cmd = db.CreateCommand();
                string cmdText = textToExecute;
                cmd.CommandText = cmdText;

                SqliteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string s = reader.GetString(3);
                    foreach (string s1 in s.Split("$", StringSplitOptions.RemoveEmptyEntries))
                    {
                        item.Id = reader.GetInt32(0);
                        item.WorkoutId = reader.GetInt32(1);
                        item.ExerciseId = reader.GetInt32(2);

                        string[] hej = s1.Split("x");
                        int hej1 = int.Parse(hej[0]);
                        double hej2 = double.Parse(hej[1]);

                        item.Reps = int.Parse(s1.Split("x")[0]);
                        item.Weight = double.Parse(s1.Split("x")[1]);

                        listToReturn.Add(item);
                        item = new Set();
                    }
                }
            }

            return listToReturn;
        }

        #endregion


        #region Methods

        public string GetSetData(int id)
        {
            string prevData = "";

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand cmd = new SqliteCommand();
                cmd.Connection = db;

                cmd.CommandText = "PRAGMA foreign_keys = ON";
                cmd.ExecuteNonQuery();

                cmd.CommandText =
                    $"SELECT SetData FROM Sets WHERE Id={id}";
                SqliteDataReader reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    prevData = reader.GetString(0);
                }
            }

            return prevData;
        }

        public void Add(object item)
        {
            if (item.GetType() == typeof(Set))
            {
                Set s = (Set) item;
                string thisSetData = $"{s.Reps}x{s.Weight}$";

                //string cmdText = "INSERT INTO Sets (WorkoutId, ExerciseId, Reps, Weight) ";
                //cmdText += string.Format("VALUES ('{0}','{1}','{2}','{3}')",
                //    s.WorkoutId, s.ExerciseId, s.Reps, s.Weight);
                //ExecuteQuery($"TYPEOF(SELECT SetData FROM Sets WHERE (WorkoutId = {s.WorkoutId} AND ExerciseId = {s.ExerciseId}))");
                //string prevData = ExecuteQuery($"SELECT SetData FROM Sets WHERE (WorkoutId = {s.WorkoutId} AND ExerciseId = {s.ExerciseId})");



                //string cmdText = "REPLACE INTO Sets (WorkoutId, ExerciseId, SetData)";
                //cmdText += string.Format("VALUES ('{0}','{1}','{2}')",
                //    s.WorkoutId, s.ExerciseId, $"SELECT SetData FROM Sets WHERE (WorkoutId = {s.WorkoutId} AND ExerciseId = {s.ExerciseId})");

                //ExecuteQuery(cmdText);
                string prevData = "";
                using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();
                    SqliteCommand cmd = new SqliteCommand();
                    cmd.Connection = db;

                    cmd.CommandText = "PRAGMA foreign_keys = ON";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText =
                        $"SELECT SetData FROM Sets WHERE (WorkoutId = {s.WorkoutId}) AND (ExerciseId = {s.ExerciseId})";
                    SqliteDataReader reader = cmd.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        prevData = reader.GetString(0);
                    }
                }
                //    string cmdText = "REPLACE INTO Sets (WorkoutId, ExerciseId, SetData)";
                    //    cmdText += string.Format("VALUES ('{0}','{1}','{2}')",
                    //        s.WorkoutId, s.ExerciseId, prevData + thisSetData);

                    //    ExecuteQuery(cmdText);
                    //}
                    string cmdText = "REPLACE INTO Sets (WorkoutId, ExerciseId, SetData)";
                cmdText += string.Format("VALUES ('{0}','{1}','{2}')",
                    s.WorkoutId, s.ExerciseId, prevData + thisSetData);

                ExecuteQuery(cmdText);

            }

            else if (item.GetType() == typeof(Workout))
            {
                Workout w = (Workout) item;

                string cmdText = "INSERT INTO Workouts (StartTime, EndTime) ";
                cmdText += string.Format("VALUES ('{0}','{1}')",
                    w.StartTime, w.EndTime);

                ExecuteQuery(cmdText);
            }

            else if (item.GetType() == typeof(Exercise))
            {
                Exercise e = (Exercise) item;
                if (e.Name == null) e.Name = "";
                if (e.MuscleGroup == null) e.MuscleGroup = "";
                if (e.Description == null) e.Description = "";

                string cmdText = "INSERT INTO Exercises (Name, MuscleGroup, Description) ";
                cmdText += string.Format("VALUES ('{0}','{1}','{2}')",
                    e.Name, e.MuscleGroup, e.Description);

                ExecuteQuery(cmdText);
            }
        }

        public void Remove(Type t, int id)
        {
            string cmdText = $"DELETE FROM {t.Name}s " + $"WHERE Id={id}";
            ExecuteQuery(cmdText);
        }

        public object Read(Type t, int id)
        {
            if (t != typeof(Exercise) && t != typeof(Workout) && t != typeof(Set))
            {
                return null;
            }

            string cmdText = $"SELECT * FROM {t.Name}s " + $"WHERE Id={id}";

            if (t == typeof(Exercise)) return DataToExercises(cmdText)[0];
            else if (t == typeof(Workout)) return DataToWorkouts(cmdText)[0];
            else if (t == typeof(Set)) return DataToSets(cmdText)[0];

            return null;
        }

        public void Update(object item)
        {
            if (item.GetType() == typeof(Set))
            {
                //Set s = (Set)item;

                //string cmdText = "REPLACE INTO Sets (Id, WorkoutId, ExerciseId, Reps, Weight) ";
                //cmdText += string.Format("VALUES ('{0}','{1}','{2}','{3}','{4}')",
                //    s.Id, s.WorkoutId, s.ExerciseId, s.Reps, s.Weight);

                ////string cmdText = "REPLACE INTO Sets (Id, WorkoutId, ExerciseId, SetData) ";
                ////cmdText += string.Format("VALUES ('{0}','{1}','{2}','{3}')",
                ////    s.Id, s.WorkoutId, s.ExerciseId, "???");

                //ExecuteQuery(cmdText);
            }

            else if (item.GetType() == typeof(Workout))
            {
                Workout w = (Workout)item;

                string cmdText = "REPLACE INTO Workouts (Id, StartTime, EndTime) ";
                cmdText += string.Format("VALUES ('{0}','{1}','{2}')",
                    w.Id, w.StartTime, w.EndTime);

                ExecuteQuery(cmdText);
            }

            else if (item.GetType() == typeof(Exercise))
            {
                Exercise e = (Exercise)item;

                string cmdText = "REPLACE INTO Exercises (Id, Name, MuscleGroup, Description) ";
                cmdText += string.Format("VALUES ('{0}','{1}','{2}','{3}')",
                    e.Id, e.Name, e.MuscleGroup, e.Description);

                ExecuteQuery(cmdText);
            }
        }

        public List<Exercise> GetAllExercises() { return DataToExercises($"SELECT * FROM Exercises"); }
        public List<Workout> GetAllWorkouts() { return DataToWorkouts($"SELECT * FROM Workouts"); }
        public List<Set> GetAllSets() { return DataToSets($"SELECT * FROM Sets"); }

        public List<Exercise> GetExercisesFromText(string queryText) { return DataToExercises(queryText); }
        public List<Workout> GetWorkoutsFromText(string queryText) { return DataToWorkouts(queryText); }
        public List<Set> GetSetsFromText(string queryText) { return DataToSets(queryText); }

        #endregion
    }
}
