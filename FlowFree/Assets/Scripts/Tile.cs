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
        [Space]
        public SpriteRenderer upWallThin;
        public SpriteRenderer downWallThin;
        public SpriteRenderer leftWallThin;
        public SpriteRenderer rightWallThin;
        [Space]
        public SpriteRenderer upWallThick;
        public SpriteRenderer downWallThick;
        public SpriteRenderer leftWallThick;
        public SpriteRenderer rightWallThick;

        [HideInInspector]
        public int id;

        private Color _color;

#if UNITY_EDITOR
        void Start()
        {
            if (!circle || !tick || !upWallThin || !downWallThin || !leftWallThin || !rightWallThin ||
                !upWallThick || !downWallThick || !leftWallThick || !rightWallThick)
            {
                Debug.LogError("No están asignados todos los SpriteRenderers");
            }
        }
#endif

        // Métodos para activar/desactivar los distintos SpriteRenderer incluidos en el prefab

        public void SetColor(Color c)
        {
            circle.color = c;
            //_color = c;
        }

        public void SetCircle(bool active)
        {
            circle.GetComponent<SpriteRenderer>().enabled = active;
        }

        public void SetTick(bool active)
        {
            tick.GetComponent<SpriteRenderer>().enabled = active;
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
    }

}