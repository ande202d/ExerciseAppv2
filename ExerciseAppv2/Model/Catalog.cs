using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExerciseAppv2.Model
{
    public class Catalog<T>
    {
        private List<T> _list;
        private string _fileName = "data.json";
        private Catalog()
        {
            if (!File.Exists(_fileName)) File.Create(_fileName);
            _list = ReadAll().Result;
        }

        #region Singleton

        private static Catalog<T> _instance;

        //public static Catalog<T> Instance
        //{
        //    get
        //    {
        //        if (_instance == null) return new Catalog<T>();
        //        return Instance;
        //    }
        //}

        #endregion

        #region Methods

        public async Task Create(T obj)
        {
            File.AppendAllLines(_fileName, new string[]{ JsonConvert.SerializeObject(obj)});
            _list = ReadAll().Result;
        }
        public async Task<List<T>> ReadAll()
        {
            List<T> tempList = new List<T>();
            foreach (string s in File.ReadAllLines(_fileName))
            {
                T item = JsonConvert.DeserializeObject<T>(s);
                tempList.Add(item);
                //tempList.Add(JsonConvert.DeserializeObject<T>(s));
            }

            return tempList;
        }

        //public async Task<T> Read(int key)
        //{
        //    _list.Find(i=>i.)
        //}

        public List<T> GetList()
        {
            return _list;
        }

        #endregion
    }
}
