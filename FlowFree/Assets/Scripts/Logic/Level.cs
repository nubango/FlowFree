using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow.Logic
{
    public class Level
    {
        [Tooltip("Nombre del lote")]
        public string packName;
        [Tooltip("Mapas del nivel")]
        public Map[] maps;
    }
}
