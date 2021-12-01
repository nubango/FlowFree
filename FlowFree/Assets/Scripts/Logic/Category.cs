using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow.Logic
{
    public class Category
    {
        [Tooltip("Color de la categoria")]
        private Color _categoryColor;
        [Tooltip("Paquetes de la categoria")]
        private Package[] _packages;

        public Category(Package[] packages, Color color)
        {
            _packages = packages;
            _categoryColor = color;
        }

        public Package[] GetLevels() { return _packages; }
        public Color GetColor() { return _categoryColor; }
    }
}
