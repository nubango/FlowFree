using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow.Logic
{
    public class Category
    {
        [Tooltip("Color de la categoria")]
        public Color categoryColor;
        [Tooltip("Paquetes de la categoria")]
        public Level[] levels;
    }
}
