using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class Tile : MonoBehaviour
    {
        [Tooltip("Sprite del círculo")]
        [SerializeField]
        private SpriteRenderer circle;

        // DEBUG
        public int num;

#if UNITY_EDITOR
        void Start()
        {
            if (circle == null)
            {
                Debug.LogError("No me has dado el círculo, alma de cántaro");
            }
        }
#endif

        public void SetColor(Color c)
        {
            circle.color = c;
            _color = c;
        }

        public void EnableCircle()
        {

        }

        private Color _color;
    }

}