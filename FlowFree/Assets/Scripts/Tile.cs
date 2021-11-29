using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class Tile : MonoBehaviour
    {
        [Header("SpriteRenderers")]
        public SpriteRenderer circleEnd;
        public SpriteRenderer circleTrace;
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
            if (!circleEnd || !tick || !trace || !upWallThin || !downWallThin || !leftWallThin || !rightWallThin ||
                !upWallThick || !downWallThick || !leftWallThick || !rightWallThick)
            {
                Debug.LogError("No están asignados todos los SpriteRenderers");
            }
        }
#endif

        /// <summary>
        /// devuelve el color del circulo
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            return circleEnd.color;
        }

        /// <summary>
        /// Métodos para activar/desactivar los distintos SpriteRenderer incluidos en el prefab
        /// </summary>
        /// <param name="c"></param>
        public void SetColor(Color c)
        {
            circleEnd.color = c;
            circleTrace.color = c;
            trace.color = c;
        }

        /// <summary>
        /// Activa/desactiva el circulo (extremo) en una casilla
        /// </summary>
        /// <param name="active"></param>
        public void SetCircleEnd(bool active)
        {
            circleEnd.enabled = active;
        }
        
        /// <summary>
        /// Activa/desactiva el circulo del rastro en una casilla (cuando un rastro se queda a mitad)
        /// </summary>
        /// <param name="active"></param>
        public void SetCircleTrace(bool active)
        {
            circleTrace.enabled = active;
        }

        /// <summary>
        /// Activa/desactiva el circulo pequeño de la ultima casilla del rastro
        /// </summary>
        /// <param name="active"></param>
        public void SetSmallCircle(bool active)
        {
            circleEnd.GetComponent<SpriteRenderer>().enabled = active;
            circleEnd.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }

        /// <summary>
        /// Devuelve TRUE si es el final del path
        /// </summary>
        /// <returns></returns>
        public bool IsEnd() { return circleEnd.enabled; }

        /// <summary>
        /// Activa/desactiva el sprite del Tick 
        /// </summary>
        /// <param name="active"></param>
        public void SetTick(bool active)
        {
            tick.enabled = active;
        }

        /// <summary>
        /// Activa/desactiva los bordes verdes de las casillas
        /// </summary>
        /// <param name="up"></param>
        /// <param name="down"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void SetThinWalls(bool up, bool down, bool left, bool right)
        {
            upWallThin.enabled = up;
            downWallThin.enabled = down;
            leftWallThin.enabled = left;
            rightWallThin.enabled = right;
        }

        public void SetThickWalls(bool up, bool down, bool left, bool right)
        {
            upWallThick.enabled = up;
            downWallThick.enabled = down;
            leftWallThick.enabled = left;
            rightWallThick.enabled = right;
        }

        /// <summary>
        /// TRUE si esta activaso el rastro, FALSE en caso contrario
        /// </summary>
        /// <returns></returns>
        public bool IsTraceActive()
        {
            return trace.enabled;
        }

        /// <summary>
        /// Devuelve la direccion en la que esta activa el trazo
        /// </summary>
        /// <returns></returns>
        public Vector2 WhichDirectionIsTraceActive()
        {
            return trace.transform.localPosition;
        }

        /* -Metodos para activar o desactivar el rastro- */

        /// <summary>
        /// Activa el trazo en la direccion pasada por parametro
        /// </summary>
        /// <param name="direction"></param>
        public void ActiveTrace(Vector2 direction)
        {
            if (direction.y < 0)
                SetUp(true);
            else if (direction.y > 0)
                SetDown(true);
            else if (direction.x > 0)
                SetRight(true);
            else if (direction.x < 0)
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