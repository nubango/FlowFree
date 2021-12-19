using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    public class PackageButton : MonoBehaviour
    {
        public Text packName;
        public Text completedLevels;
        public Image tick;
        public Image star;

        public void SetActiveTick(bool b) { tick.enabled = b; }
        public void SetActiveStar(bool b) { star.enabled = b; }
    }

}