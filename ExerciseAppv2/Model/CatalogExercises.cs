﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExerciseAppv2.Model
{
    public class CatalogExercises
    {
        private List<Exercise> _list;
        private string _fileName = "dataExercises.json";
        private CatalogExercises()
        {
            if (!File.Exists(_fileName)) File.Create(_fileName);
            _list = ReadAll().Result;
        }

        #region Singleton

        private static CatalogExercises _instance;

        public static CatalogExercises Instance
        {
            get
            {
                if (_instance == null) return new CatalogExercises();
                return Instance;
            }
        }

        #endregion

        #region Methods

        public async Task Create(Exercise obj)
        {
            obj.Id = GetList().Last().Id + 1;
            File.AppendAllLines(_fileName, new string[]{ JsonConvert.SerializeObject(obj)});
            _list = ReadAll().Result;
        }
        public async Task<List<Exercise>> ReadAll()
        {
            List<Exercise> tempList = new List<Exercise>();
            foreach (string s in File.ReadAllLines(_fileName))
            {
                Exercise item = JsonConvert.DeserializeObject<Exercise>(s);
                tempList.Add(item);
                //tempList.Add(JsonConvert.DeserializeObject<T>(s));
            }

            return tempList;
        }

        //public async Task<T> Read(int key)
        //{
        //    _list.Find(i=>i.)
        //}

        public List<Exercise> GetList()
        {
            return _list;
        }

        #endregion
    }
}
