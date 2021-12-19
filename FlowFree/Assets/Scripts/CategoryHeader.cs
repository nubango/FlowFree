using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{

    public class CategoryHeader : MonoBehaviour
    {
        public Image headerBackground;
        public Image headerLine;
        public Text headerText;
        public Image tick;
        public Image star;

        public void SetActiveTick(bool b) { tick.enabled = b; }
        public void SetActiveStar(bool b) { star.enabled = b; }
    }

}