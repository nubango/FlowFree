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

        private bool _efectiveInvalidate = false;

        private int _screenWidth;
        private int _screenHeight;

        private float _scaleFactor;

        private int _emptyTiles;

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
            if (_screenWidth != Screen.width || _screenHeight != (int)GameManager.Instance().GetCenterPixelSize())
                MapRescaling();

            if (!_efectiveInvalidate)
                _traceInput.ProcessInput();

            _efectiveInvalidate = _invalidate;
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
            _screenHeight = (int)GameManager.Instance().GetCenterPixelSize();

            // agrandamos la pantalla para que quepan el baner de abajo y los botones de arriba
            float offsetLateral = 0.98f;

            float scaleFactorW, scaleFactorH;

            float centerUnitySize = GameManager.Instance().GetCenterUnitySize();

            // unidades de unity que ocupa la zona central
            float unitsUnityByHeight = centerUnitySize;
            float unitsUnityByWidth = (_screenWidth * unitsUnityByHeight) / _screenHeight;

            scaleFactorW = (unitsUnityByWidth * offsetLateral) / _tiles.GetLength(1);
            scaleFactorH = (unitsUnityByHeight) / _tiles.GetLength(0);
            _scaleFactor = Mathf.Min(scaleFactorW, scaleFactorH);



            // Calculos para centrar la camara
            float camPosX = (_tiles.GetLength(1) * _scaleFactor / 2) - (0.5f * _scaleFactor);
            float camPosY = (_tiles.GetLength(0) * _scaleFactor / 2) - (0.5f * _scaleFactor) -
                (GameManager.Instance().GetTopUnitySize() / 2) + (GameManager.Instance().GetBottomUnitySize() / 2);

            // asignamos los valores calculados 
            Camera.main.transform.position = new Vector3(camPosX, -camPosY, -10);

            // escalamos el tablero
            gameObject.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);
        }

        private Utils.Coord[] _directions = { new Utils.Coord(0, -1), new Utils.Coord(0, 1), new Utils.Coord(-1, 0), new Utils.Coord(1, 0) };
        private void ActivateExternalWalls(Utils.Coord pos)
        {
            foreach (Utils.Coord d in _directions)
                if (!ValidCoords(pos + d))
                {
                    _tiles[pos.y, pos.x].SetThickWalls(d, true);
                }
        }

        private void ActivateEmptyTile(List<Utils.Coord> empties)
        {
            foreach (Utils.Coord pos in empties)
            {
                _tiles[pos.y, pos.x].SetThickWalls(false, false, false, false);
                _tiles[pos.y, pos.x].SetThinWalls(false, false, false, false);
                _tiles[pos.y, pos.x].SetEmpty(true);

                foreach (Utils.Coord d in _directions)
                {
                    Utils.Coord nextPos = pos + d;
                    if (ValidCoords(nextPos) && !_tiles[nextPos.y, nextPos.x].IsEmpty())
                    {
                        _tiles[pos.y, pos.x].SetThickWalls(d, true);
                    }
                    else if (ValidCoords(nextPos) && _tiles[nextPos.y, nextPos.x].IsEmpty())
                    {
                        // desactivar en la direccion d (direccion donde hay un empty)
                        _tiles[nextPos.y, nextPos.x].SetThickWalls(d * -1, false);
                    }
                }
            }
        }

        private void ActivateInternalWalls(List<Utils.Wall> walls)
        {
            foreach (Utils.Wall wall in walls)
            {
                _tiles[wall.init.y, wall.init.x].SetThickWalls(wall.end - wall.init, true);
            }
        }

        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Metodo que muestra un path completo
        /// </summary>
        public void ShowPath()
        {
            _traceInput.ShowPath();
        }

        public int GetPercentage()
        {
            return _traceInput.GetPercentage();
        }

        /// <summary>
        /// Metodo que comprueba si las coordenadas pasadas por parametro estan dentro del tablero
        /// </summary>
        /// <param name="coord"></param>
        /// <returns>TRUE si esta dentro de los limites. False en caso contrario</returns>
        public bool ValidCoords(Utils.Coord coord)
        {
            return coord.x >= 0 && coord.y >= 0 && coord.x < GetWidth() && coord.y < GetHeight();
        }

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


        public int GetNumEmptyTiles()
        {
            return _emptyTiles;
        }
        /// <summary>
        /// Metodo que sirve para asignar el tablero
        /// </summary>
        /// <param name="map">Datos logicos del tablero</param>
        public void SetMap(Logic.Level map)
        {
            _invalidate = false;
            _emptyTiles = 0;

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
                        _tiles[i, j].SetCircleEnd(true);
                        _tiles[i, j].SetColorStart(GameManager.Instance().currentSkin.colores[_tiles[i, j].id - 1]);
                        _tiles[i, j].SetTick(true);
                    }
                    _tiles[i, j].SetThinWalls(true, true, true, true);
                    _tiles[i, j].SetEmpty(false);

                    Utils.Coord pos = new Utils.Coord(j, i);
                    if (map.GetVacios().Count > 0 || map.GetMuros().Count > 0)
                    {
                        pos.x = j;
                        pos.y = i;
                        ActivateExternalWalls(pos);
                    }
                }
            }

            _emptyTiles = map.GetVacios().Count;

            if (map.GetVacios().Count > 0)
                ActivateEmptyTile(map.GetVacios());

            if (map.GetMuros().Count > 0)
                ActivateInternalWalls(map.GetMuros());

            _traceInput.Init(this, _tiles, map.GetTuberias(), circleFinger, map.GetFlujos());

            MapRescaling();

        }
        #endregion
    }

}