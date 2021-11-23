using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class BoardManager : MonoBehaviour
    {
        [Tooltip("Prefab de la casilla")]
        public Tile tilePrefab;

        private Tile[,] _tiles;

        private int _width;
        private int _height;

        private Vector2 _currentInputPoint;

        void Start()
        {
            transform.position = new Vector3(0, 0, 0);
            _currentInputPoint = new Vector2(0, 0);
            _lastIndex = new Vector2(-1, -1);
        }


        // Contiene la posicion de la ultima pulsacion detectada
        Vector2 _lastIndex;

        // Color actual del trazo
        Color _colorTrace;
        private void Update()
        {
            Vector3 unityPos = new Vector3(-1, -1);
            Vector2 direction = new Vector2(0, 0);
            Vector3 touchPos;
            // Boton izquierdo del raton
            if (Input.GetMouseButton(0))
            {
                touchPos = Input.mousePosition;
                unityPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, Camera.main.nearClipPlane));
            }

            // Input tactil
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                touchPos = touch.position;
                unityPos = Camera.main.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, Camera.main.nearClipPlane));

                Vector2 indexTile = new Vector2((int)unityPos.x, (int)-unityPos.y);
                if (indexTile.x < 0 || indexTile.y < 0 || indexTile.x >= _width || indexTile.y >= _height)
                    return;


                if (touch.phase == TouchPhase.Began && _tiles[(int)indexTile.y, (int)indexTile.x].IsCircleActive())
                {
                    _colorTrace = _tiles[(int)indexTile.y, (int)indexTile.x].GetColor();
                    _lastIndex = indexTile;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    direction = touch.deltaPosition;
                    if (Mathf.Abs(touch.deltaPosition.x) > Mathf.Abs(touch.deltaPosition.y))
                    {
                        direction.y = 0;
                        if (direction.x > 0) direction.x = 1;
                        else direction.x = -1;
                    }
                    else
                    {
                        direction.x = 0;
                        if (direction.y > 0) direction.y = 1;
                        else direction.y = -1;
                    }

                    if (!_tiles[(int)indexTile.y, (int)indexTile.x].IsCircleActive() && (_lastIndex.x != indexTile.x || _lastIndex.y != indexTile.y))
                    {
                        _tiles[(int)_lastIndex.y, (int)_lastIndex.x].SetTraceColor(_colorTrace);
                        _tiles[(int)_lastIndex.y, (int)_lastIndex.x].ActiveTrace(direction);
                        _lastIndex = indexTile;
                    }
                }
            }
        }

        /*
        Cuando pulso una casilla es como si cogiese el color de esa casilla (si tiene circulo de color) y lo arrastro por el tablero.
        Si pulso sobre una casilla, desactivo todas las casillas con el mismo color que la casilla pulsada
        
        Para saber que direccion activar en cada casilla, hay que guardar la posicion anterior y calcular la direccion(normalizada) en la que se mueve.

        Si coincide la direccion y el color entonces hay que desactivar el camino porque estamos yendo hacia atras.

         
         */

        public void SetCurrentInputPoint(Vector2 point)
        {
            _currentInputPoint = point;
        }

        public void SetMap(Logic.Map map)
        {
        }

        // DEBUG
        public void SetLevel()
        {
            _tiles = new Tile[5, 5];
            // nivel "copiado" del #1
            // 0: casilla vacia
            // n>0: color, se asignan a pares
            int[] fakeLevelIDs = { 1, 0, 2, 0, 3, 0, 0, 4, 0, 5, 0, 0, 0, 0, 0, 0, 2, 0, 3, 0, 0, 1, 4, 5, 0 };

            _width = _height = 5;

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    _tiles[i, j] = Instantiate(tilePrefab);
                    _tiles[i, j].gameObject.transform.SetParent(this.gameObject.transform);
                    // El Tile (0,0) esta en la esquina superior-izquierda del "grid"
                    _tiles[i, j].gameObject.transform.localPosition = new Vector2(j, -i);

                    _tiles[i, j].id = fakeLevelIDs[(i * 5) + j];

                    if (_tiles[i, j].id != 0)
                    {
                        _tiles[i, j].SetCircle(true);
                        _tiles[i, j].SetTick(true);
                    }
                    _tiles[i, j].SetThinWalls(true, true, true, true);

                    //_tiles[i, j].SetThickWalls(false, false, true, true);

                    switch (_tiles[i, j].id)
                    {
                        case 1:
                            _tiles[i, j].SetColor(Color.red);

                            break;
                        case 2:
                            _tiles[i, j].SetColor(Color.green);
                            break;
                        case 3:
                            _tiles[i, j].SetColor(Color.yellow);
                            break;
                        case 4:
                            _tiles[i, j].SetColor(Color.blue);
                            break;
                        case 5:
                            _tiles[i, j].SetColor(Color.magenta);
                            break;
                    }
                }
            }
        }
    }

}