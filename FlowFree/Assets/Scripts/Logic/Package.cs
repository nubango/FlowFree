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

        public Package(Level[] levels, string name)
        {
            _levels = levels;
            _packName = name;
        }
        public Package() { }
        public void SetMaps(Level[] levels)
        {
            _levels = levels;
        }
        public void SetPackName(string packName)
        {
            _packName = packName;
        }
        public Level[] GetMaps() { return _levels; }
    
        public string GetPackName() { return _packName; }
    }
}
