using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    /// <summary>
    /// Gestiona los datos de un paquete de niveles del canvas 
    /// </summary>
    public class PackageButton : MonoBehaviour
    {
        public Text packName;
        public Text completedLevels;
        public Image tick;
        public Image star;

        private int _package;
        private int _category;

        public void SetPackage(int pack) { _package = pack; }
        public int GetPackage() { return _package; }
        public void SetCategory(int cat) { _category = cat; }
        public int GetCategory() { return _category; }

        public void SetActiveTick(bool b) { tick.enabled = b; }
        public void SetActiveStar(bool b) { star.enabled = b; }
    }

}