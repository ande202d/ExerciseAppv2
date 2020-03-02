using System;
using System.Collections.Generic;
using System.Data;
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

        #region Methods

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
                          "Reps INTEGER NOT NULL, " +
                          "Weight INTEGER NOT NULL, " +
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

        public void Add(object item)
        {
            if (item.GetType() == typeof(Set))
            {
                Set s = (Set) item;

                string cmdText = "INSERT INTO Sets (WorkoutId, ExerciseId, Reps, Weight) ";
                cmdText += string.Format("VALUES ('{0}','{1}','{2}','{3}')",
                    s.WorkoutId, s.ExerciseId, s.Reps, s.Weight);

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

                string cmdText = "INSERT INTO Exercises (Name, MuscleGroup, Description) ";
                cmdText += string.Format("VALUES ('{0}','{1}','{2}')",
                    e.Name, e.MuscleGroup, e.Descreption);

                ExecuteQuery(cmdText);
            }
        }

        public object Read(Type t, int id)
        {
            if (t != typeof(Exercise) && t != typeof(Workout) && t != typeof(Set))
            {
                return null;
            }

            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand cmd = db.CreateCommand();
                string cmdText = $"SELECT * FROM {t.Name}s " + $"WHERE Id={id}";
                cmd.CommandText = cmdText;

                SqliteDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (t == typeof(Exercise))
                {
                    Exercise item = new Exercise();
                    while (reader.Read())
                    {
                        item.Id = reader.GetInt32(0);
                        item.Name = reader.GetString(1);
                        item.MuscleGroup = reader.GetString(2);
                        item.Descreption = reader.GetString(3);
                    }
                    return item;
                }
                else if (t == typeof(Workout))
                {
                    Workout item = new Workout();
                    while (reader.Read())
                    {
                        item.Id = reader.GetInt32(0);
                        item.StartTime = DateTime.Parse(reader.GetString(1));
                        item.EndTime = DateTime.Parse(reader.GetString(2));
                    }
                    return item;
                }
                else if (t == typeof(Set))
                {
                    Set item = new Set();
                    while (reader.Read())
                    {
                        item.Id = reader.GetInt32(0);
                        item.WorkoutId = reader.GetInt32(1);
                        item.ExerciseId = reader.GetInt32(2);
                        item.Reps = reader.GetInt32(3);
                        item.Weight = reader.GetInt32(4);
                    }
                    return item;
                }
                return null;
            }
        }

        #endregion
    }
}
