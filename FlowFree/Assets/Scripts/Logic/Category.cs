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
        public Category() { }
        public void SetCategoriColor(Color c) { _categoryColor = c; }
        public void SetPackage(Package[] packages) { _packages = packages; }

        public Package[] GetPackages() { return _packages; }
        public Color GetColor() { return _categoryColor; }
    }
}
