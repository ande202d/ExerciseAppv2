using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExerciseAppv2.Model
{
    public class CatalogWorkouts
    {
        private List<Workout> _list;
        private string _fileName = "dataWorkouts.json";
        private CatalogWorkouts()
        {
            if (!File.Exists(_fileName)) File.Create(_fileName).Dispose();

            var v = ReadAll();
            v.Wait();
            _list = v.Result;
        }

        #region Singleton

        private static CatalogWorkouts _instance;

        public static CatalogWorkouts Instance
        {
            get
            {
                if (_instance == null) _instance = new CatalogWorkouts();
                return _instance;
            }
        }

        #endregion

        #region Methods

        public async Task Create(Workout obj)
        {
            if (_list.Count <= 0) obj.Id = 0;
            else obj.Id = GetList().Last().Id + 1;
            File.AppendAllLines(_fileName, new string[]{ JsonConvert.SerializeObject(obj)});
            _list = ReadAll().Result;
        }
        public async Task<List<Workout>> ReadAll()
        {
            List<Workout> tempList = new List<Workout>();
            foreach (string s in File.ReadAllLines(_fileName))
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    Workout item = JsonConvert.DeserializeObject<Workout>(s);
                    tempList.Add(item);
                }
                //tempList.Add(JsonConvert.DeserializeObject<T>(s));
            }

            return tempList;
        }

        //public async Task<T> Read(int key)
        //{
        //    _list.Find(i=>i.)
        //}

        public List<Workout> GetList()
        {
            return _list;
        }

        #endregion
    }
}
