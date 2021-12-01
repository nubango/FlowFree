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
        private Map[] _levels;

        public Package(Map[] levels, string name)
        {
            _levels = levels;
            _packName = name;
        }

        public Map[] GetMaps() { return _levels; }
    
        public string GetPackName() { return _packName; }
    }
}
