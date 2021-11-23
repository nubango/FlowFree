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
        [Tooltip("Sprite ninepatch para dibujar el rastro")]
        public SpriteRenderer trace;
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

#if UNITY_EDITOR
        void Start()
        {
            if (!circle || !tick || !trace || !upWallThin || !downWallThin || !leftWallThin || !rightWallThin ||
                !upWallThick || !downWallThick || !leftWallThick || !rightWallThick)
            {
                Debug.LogError("No están asignados todos los SpriteRenderers");
            }
        }
#endif
        public bool IsCircleActive()
        {
            return circle.enabled;
        }

        public Color GetColor()
        {
            return circle.color;
        }

        // Métodos para activar/desactivar los distintos SpriteRenderer incluidos en el prefab

        public void SetColor(Color c)
        {
            circle.color = c;
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


        // Cambia el color del rastro
        public void SetTraceColor(Color color)
        {
            trace.color = color;
        }

        public Color GetTraceColor()
        {
            return trace.color;
        }


        // Metodos para activar o desactivar el rastro 

        public void ActiveTrace(Vector2 direction)
        {
            if (direction.y > 0)
                SetUp(true);
            else if(direction.y < 0)
                SetDown(true);
            else if(direction.x > 0)
                SetRight(true);
            else if(direction.x < 0)
                SetLeft(true);
        }

        public void DesactiveTrace()
        {
            trace.enabled = false;
        }

        private void SetUp(bool enabled)
        {
            trace.enabled = enabled;
            // escalamos el ninepatch hacia arriba
            trace.size = new Vector2(0.25f, 1.25f);
            // reposicionamos para que quede centrado
            trace.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
        private void SetDown(bool enabled)
        {
            trace.enabled = enabled;
            // escalamos el ninepatch hacia abajo
            trace.size = new Vector2(0.25f, 1.25f);
            // reposicionamos para que quede centrado
            trace.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
        private void SetLeft(bool enabled)
        {
            trace.enabled = enabled;
            // escalamos el ninepatch hacia la izquierda
            trace.size = new Vector2(1.25f, 0.25f);
            // reposicionamos para que quede centrado
            trace.transform.localPosition = new Vector3(-0.5f, 0, 0);
        }
        private void SetRight(bool enabled)
        {
            trace.enabled = enabled;
            // escalamos el ninepatch hacia la derecha
            trace.size = new Vector2(1.25f, 0.25f);
            // reposicionamos para que quede centrado
            trace.transform.localPosition = new Vector3(0.5f, 0, 0);
        }
    }

}