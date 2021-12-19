using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow.Logic
{
    public class Category
    {
        [Tooltip("Nombre de la categoria")]
        private string _categoryName;
        [Tooltip("Color de la categoria")]
        private Color _categoryColor;
        [Tooltip("Paquetes de la categoria")]
        private Package[] _packages;

        public void SetCategoryName(string name) { _categoryName = name; }
        public void SetCategoryColor(Color c) { _categoryColor = c; }
        public void SetPackage(Package[] packages) { _packages = packages; }

        public string GetName() { return _categoryName; }
        public Package[] GetPackages() { return _packages; }
        public Color GetColor() { return _categoryColor; }

        public bool IsCompleted()
        {
            for (int i = 0; i < _packages.Length; i++)
            {
                if (!_packages[i].IsCompleted())
                    return false;
            }
            return true;
        }
        public bool IsPerfectCompleted()
        {
            for (int i = 0; i < _packages.Length; i++)
            {
                if (!_packages[i].IsPerfectCompleted())
                    return false;
            }
            return true;
        }
    }
}
