using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    /// <summary>
    /// Gestiona el boton del nivel en la escena levelsScene
    /// </summary>
    public class LevelButton : MonoBehaviour
    {
        public Text text;
        public Image backgroud;
        public Image tick;
        public Image star;
        public Image locked;

        private int _id;

        public void SetId(int id) { _id = id; }
        public int GetId() { return _id; }

        public void SetActiveTick(bool b) { tick.enabled = b; }
        public void SetActiveStar(bool b) { star.enabled = b; }
        public void SetActiveLocked(bool b) { locked.enabled = b; }
    }
}