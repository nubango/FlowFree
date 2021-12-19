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
        public SpriteRenderer square;
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

        private bool _empty;
        private Vector2 _directionTrace;


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
        /// Metodo para saber si la casilla es vacia, es decir, que no pertenece al tablero
        /// </summary>
        /// <returns>Devuelve TRUE si es vacia y FALSE en caso contrario</returns>
        public bool IsEmpty() { return _empty; }

        /// <summary>
        /// Metodo para determinar si una casilla es vacia, es decir, que no pertenece al tablero, o no
        /// </summary>
        /// <param name="empty"></param>
        public void SetEmpty(bool empty)
        {
            if (empty)
            {
                square.enabled = false;
            }
            _empty = empty;
        }

        /// <summary>
        /// devuelve el color del circulo
        /// </summary>
        /// <returns></returns>
        public Color GetColor()
        {
            return circleTrace.color;
        }

        /// <summary>
        /// Cambia el color del trazo
        /// </summary>
        /// <param name="c"></param>
        public void SetColorTrace(Color c)
        {
            if (!circleEnd.enabled)
            {
                circleTrace.color = c;
                trace.color = c;
            }
        }

        /// <summary>
        /// Cambia el color de la casilla 
        /// </summary>
        /// <param name="c"></param>
        public void SetColorStart(Color c)
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
            circleEnd.enabled = active;
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

        public void SetThickWalls(Utils.Coord direction, bool active)
        {
            if (direction.x == direction.y)
                return;

            if (direction.y == -1)
                upWallThick.enabled = active;
            else if (direction.y == 1)
                downWallThick.enabled = active;
            else if (direction.x == -1)
                leftWallThick.enabled = active;
            else if (direction.x == 1)
                rightWallThick.enabled = active;
        }

        /// <summary>
        /// TRUE si esta activaso el rastro, FALSE en caso contrario
        /// </summary>
        /// <returns></returns>
        public bool IsTraceActive()
        {
            return trace.enabled;
        }

        /*
        Para los muros hay que hacer un metodo en tile que le pasas una direccion (tileactual - anterior) 
        para saber si en esa direccion hay un muro. Devuelve un bool, TRUE si hay muro y FALSE si no hay muro
        */
        public bool IsWallInDirection(Utils.Coord direction)
        {
            bool isWall = false;
            if (direction.y == -1)
                isWall = upWallThick.enabled;
            else if (direction.y == 1)
                isWall = downWallThick.enabled;
            else if (direction.x == -1)
                isWall = leftWallThick.enabled;
            else if (direction.x == 1)
                isWall = rightWallThick.enabled;

            return isWall;
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

            _directionTrace = direction;
        }

        public Vector2 GetDirectionTrace()
        {
            return _directionTrace;
        }

        public void DesactiveTrace()
        {
            trace.enabled = false;
            SetColorTrace(Color.clear);
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