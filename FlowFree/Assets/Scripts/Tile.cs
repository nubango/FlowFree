using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class Tile : MonoBehaviour
    {
        [Header("SpriteRenderers")]
        public SpriteRenderer circle;
        public SpriteRenderer tick;
        public SpriteRenderer upWallThin;
        public SpriteRenderer downWallThin;
        public SpriteRenderer leftWallThin;
        public SpriteRenderer rightWallThin;
        public SpriteRenderer upWallThick;
        public SpriteRenderer downWallThick;
        public SpriteRenderer leftWallThick;
        public SpriteRenderer rightWallThick;

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
            circle.GetComponent<SpriteRenderer>().enabled = true;
        }

        public void EnableTick()
        {
            tick.GetComponent<SpriteRenderer>().enabled = true;
        }

        public void SetThinWalls(bool up, bool down, bool left, bool right)
        {
            upWallThin.GetComponent<SpriteRenderer>().enabled = up;
            downWallThin.GetComponent<SpriteRenderer>().enabled = down;
            leftWallThin.GetComponent<SpriteRenderer>().enabled = left;
            rightWallThin.GetComponent<SpriteRenderer>().enabled = right;
        }

        public void SetThickWalls(bool up, bool down, bool left, bool right)
        {
            upWallThick.GetComponent<SpriteRenderer>().enabled = up;
            downWallThick.GetComponent<SpriteRenderer>().enabled = down;
            leftWallThick.GetComponent<SpriteRenderer>().enabled = left;
            rightWallThick.GetComponent<SpriteRenderer>().enabled = right;
        }

        private Color _color;
    }

}