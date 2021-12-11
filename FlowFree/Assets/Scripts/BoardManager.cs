using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class BoardManager : MonoBehaviour
    {
        [Tooltip("Prefab de la casilla")]
        public Tile tilePrefab;

        // Circulo que aparece en la posicion del dedo/raton
        public SpriteRenderer circleFinger;

        // Tablero de tiles
        private Tile[,] _tiles;

        private int _width;
        private int _height;

        private TraceInput _traceInput;

        // Flag para invalidar el update y evitar el re procese el input
        private bool _invalidate = false;

        private int _screenWidth;
        private int _screenHeight;

        private float _scaleFactor;

        #region PRIVATE METHODS
        private void Awake()
        {
            _traceInput = new TraceInput();
        }
        private void Start()
        {
            transform.position = new Vector3(0, 0, 0);
        }

        private void Update()
        {
            if (_screenWidth != Screen.width || _screenHeight != Screen.height)
                MapRescaling();

            if (_invalidate)
                return;

            _traceInput.ProcessInput();
        }

        /// <summary>
        /// Metodo que se llama cuando se ha ganado el nivel. Muestra la ventana de victoria con los botones correspondientes
        /// </summary>
        public void Win()
        {
            SetActiveUpdate(false);
            GameManager.Instance().Win(_traceInput.GetNumMovements());
        }

        /// <summary>
        /// Metodo que sirve para saber si las coordenadas ij son el principio o final de un color
        /// </summary>
        /// <param name="tuberias">Lista de tuberias. Cada tuberia equivale a un color distinto</param>
        /// <param name="i">Coordenada en el eje y</param>
        /// <param name="j">Coordenada en el eje x</param>
        /// <returns>
        /// Devuelve 0 si la coordenada ij no equivale a ningun principio/fin del alguna tuberia.
        /// Devuelve h+1 siendo h el identificador de cada lista. Ej: [0(tuberia roja), 1(tuberia azul), etc.] + 1 
        /// </returns>
        private int IsEndOrStart(List<List<Utils.Coord>> tuberias, int i, int j)
        {
            for (int h = 0; h < tuberias.Count; h++)
            {
                if (tuberias[h][0].x == j && tuberias[h][0].y == i ||
                    tuberias[h][tuberias[h].Count - 1].x == j && tuberias[h][tuberias[h].Count - 1].y == i)
                    return h + 1;
            }

            return 0;
        }

        /// <summary>
        /// Resetea el tablero y elimina todas las casillas
        /// </summary>
        private void ResetBoard()
        {
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    Destroy(_tiles[i, j].gameObject);
            gameObject.transform.localScale = new Vector3(1, 1, 1);
        }



        /// <summary>
        /// Aplica al boardManager un escalado y una transformacion segun la cantidad de celdas del mapa y la resolucion de la pantalla
        /// </summary>
        private void MapRescaling()
        {
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;

            // agrandamos la pantalla para que quepan el baner de abajo y los botones de arriba
            float offsetLateral = 0.98f;
            float offsetVertical = 0.7f;

            float scaleFactorW, scaleFactorH;
            float offsetX, offsetY;

            float cameraSize = Camera.main.orthographicSize;

            // resolucion de la cámara en unidades de unity 
            float tilesByHeight = cameraSize * 2;
            float tilesByWidth = (Screen.width * tilesByHeight) / Screen.height;

            scaleFactorW = (tilesByWidth * offsetLateral) / _tiles.GetLength(1);
            scaleFactorH = (tilesByHeight * offsetVertical) / _tiles.GetLength(0);
            _scaleFactor = Mathf.Min(scaleFactorW, scaleFactorH);

            // Calculos para centrar la camara
            offsetX = (-tilesByWidth / 2) + 0.5f * _scaleFactor;
            offsetY = (-tilesByHeight / 2) + 0.5f * _scaleFactor;

            offsetX += (tilesByWidth - _tiles.GetLength(1) * _scaleFactor) / 2;
            offsetY += (tilesByHeight - _tiles.GetLength(0) * _scaleFactor) / 2;

            // asignamos los valores calculados 
            Camera.main.transform.position = new Vector3(-offsetX, offsetY, -10);

            // escalamos el tablero
            gameObject.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
        }

        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Devuelve el ancho del tablero
        /// </summary>
        /// <returns>Devuelve un INT que representa el ancho del tablero</returns>
        public int GetWidth()
        {
            return _width;
        }

        /// <summary>
        /// Devuelve el alto del tablero
        /// </summary>
        /// <returns>Devuelve un INT que representa el alto del tablero</returns>
        public int GetHeight()
        {
            return _height;
        }

        /// <summary>
        /// Devuelve el factor de escala del tablero
        /// </summary>
        /// <returns>Devuelve un FLOAT que representa el factor de escala del tablero</returns>
        public float GetScaleFactor()
        {
            return _scaleFactor;
        }

        /// <summary>
        /// Metodo para obtener el numeor de flows completos
        /// </summary>
        /// <returns>Devuelve un numero INT que representa de flows completos</returns>
        public int GetNumFlowsEnded()
        {
            return _traceInput.GetNumFlowsEnded();
        }

        /// <summary>
        /// Metodo para obtener el numero de flows que tiene el nivel
        /// </summary>
        /// <returns>Devuelve un INT que representa el numero de flows que tiene el nivel</returns>
        public int GetNumFlows()
        {
            return _traceInput.GetNumFlows();
        }

        /// <summary>
        /// Metodo para obtener el numero de moviminetos hechos
        /// </summary>
        /// <returns>Devuelve un numero INT que representa el numero de movimientos hechos</returns>
        public int GetNumMovements()
        {
            return _traceInput.GetNumMovements();
        }

        /// <summary>
        /// Invalida el update del board
        /// </summary>
        public void SetActiveUpdate(bool b)
        {
            _invalidate = !b;
        }

        /// <summary>
        /// Metodo que sirve para asignar el tablero
        /// </summary>
        /// <param name="map">Datos logicos del tablero</param>
        public void SetMap(Logic.Level map)
        {
            _invalidate = false;

            ResetBoard();

            _height = map.GetAlto();
            _width = map.GetAncho();

            _tiles = new Tile[_height, _width];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _tiles[i, j] = Instantiate(tilePrefab);
                    _tiles[i, j].gameObject.transform.SetParent(gameObject.transform);
                    // El Tile (0,0) esta en la esquina superior-izquierda del "grid"
                    _tiles[i, j].gameObject.transform.localPosition = new Vector2(j, -i);

                    _tiles[i, j].id = IsEndOrStart(map.GetTuberias(), i, j);

                    if (_tiles[i, j].id != 0)
                    {
                        _tiles[i, j].SetColor(GameManager.Instance().currentSkin.colores[_tiles[i, j].id - 1]);
                        _tiles[i, j].SetCircleEnd(true);
                        _tiles[i, j].SetTick(true);
                    }
                    //_tiles[i, j].SetThinWalls(false, false, false, false);
                    _tiles[i, j].SetThinWalls(true, true, true, true);

                    //_tiles[i, j].SetThickWalls(false, false, true, true);
                }
            }

            _traceInput.Init(this, _tiles, circleFinger, map.GetFlujos());

            MapRescaling();
        }
        #endregion
    }

}