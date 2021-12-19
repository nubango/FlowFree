using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    public class LevelButton : MonoBehaviour
    {
        public Text text;
        public Image image;

        private int _id;

        public void SetId(int id) { _id = id; }
        public int GetId() { return _id; }
    }
}