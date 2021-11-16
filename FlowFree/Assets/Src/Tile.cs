using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class Tile : MonoBehaviour
    {
        [Tooltip("Sprite del c�rculo")]
        [SerializeField]
        private SpriteRenderer circle;

#if UNITY_EDITOR
        void Start()
        {
            if (circle == null)
            {
                Debug.LogError("No me has dado el c�rculo, alma de c�ntaro");
            }
        }
#endif

        public void SetColor(Color c)
        {
            _color = c;
        }

        public void EnableCircle()
        {

        }

        private Color _color;
    }

}