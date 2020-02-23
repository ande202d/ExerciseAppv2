using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExerciseAppv2.Model
{
    public class CatalogSets
    {
        private List<Set> _list;
        private string _fileName = "dataSets.json";
        private CatalogSets()
        {
            if (!File.Exists(_fileName)) File.Create(_fileName);
            _list = ReadAll().Result;
        }

        #region Singleton

        private static CatalogSets _instance;

        public static CatalogSets Instance
        {
            get
            {
                if (_instance == null) return new CatalogSets();
                return Instance;
            }
        }

        #endregion

        #region Methods

        public async Task Create(Set obj)
        {
            obj.Id = GetList().Last().Id + 1;
            File.AppendAllLines(_fileName, new string[]{ JsonConvert.SerializeObject(obj)});
            _list = ReadAll().Result;
        }
        public async Task<List<Set>> ReadAll()
        {
            List<Set> tempList = new List<Set>();
            foreach (string s in File.ReadAllLines(_fileName))
            {
                Set item = JsonConvert.DeserializeObject<Set>(s);
                tempList.Add(item);
                //tempList.Add(JsonConvert.DeserializeObject<T>(s));
            }

            return tempList;
        }

        //public async Task<T> Read(int key)
        //{
        //    _list.Find(i=>i.)
        //}

        public List<Set> GetList()
        {
            return _list;
        }

        #endregion
    }
}
