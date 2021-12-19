using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow.Logic
{
    public class Package
    {
        [Tooltip("Nombre del lote")]
        private string _packName;
        [Tooltip("Mapas del nivel")]
        private Level[] _levels;

        public void SetMaps(Level[] levels)
        {
            _levels = levels;
        }
        public void SetPackName(string packName)
        {
            _packName = packName;
        }
        public Level[] GetLevels() { return _levels; }

        public string GetPackName() { return _packName; }

        public bool IsCompleted()
        {
            for (int i = 0; i < _levels.Length; i++)
            {
                if (!_levels[i].IsCompleted())
                    return false;
            }
            return true;
        }
        public bool IsPerfectCompleted()
        {
            for (int i = 0; i < _levels.Length - 2; i++)
            {
                if (!_levels[i].IsPerfectCompleted())
                    return false;
            }
            return true;
        }

        public int GetTotalNumLevels()
        {
            return _levels.Length - 1;
        }

        public int GetNumCompletedLevels()
        {
            int total = 0;
            for (int i = 0; i < _levels.Length - 2; i++)
            {
                if (_levels[i].IsCompleted())
                    total++;
            }
            return total;
        }
    }
}
