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

        // Color actual del trazo
        private Color _currentTraceColor;

        // Array de pilas que contienen los caminos actuales de cada color
        private Stack<Utils.Coord>[] _traceStacks;

        // Array que guarda TRUE si un flujo esta completo y FALSE en caso contrario
        private bool[] _traceEnds;

        // Tile actual por donde esta pasando el dedo/raton
        private Utils.Coord _currentTilePress;
        private Utils.Coord _lastTilePress;

        // Flag para no pintar despues de los extremos
        private bool _isDiffEnd = false;
        private bool _isEndPath = false;

        // Flag para invalidar el update y evitar el re procese el input
        private bool _invalidate = false;

        // cuenta de los movimientos que llevamos en el nivel
        private int _movementsCount = 0;

        // flag que muestra si ha habido algun cambio en el tablero
        private bool _isMove = false;

        // flag que muestra si hemos cambiado de color pulsado
        private bool _changeColor = false;

        // cuenta de la cantidad de casillas que tienen color asignado
        private int _countTileWithColor = 0;

        private int _screenWidth;
        private int _screenHeight;

        private float _scaleFactor;

        #region PRIVATE METHODS
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

            ProcessInput();
        }

        /// <summary>
        /// Metodo que comprueba si las coordenadas pasadas por parametro estan dentro del tablero
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        private bool ValidCoords(Utils.Coord coord)
        {
            return coord.x >= 0 && coord.y >= 0 && coord.x < _width && coord.y < _height;
        }

        /// <summary>
        ///  Metodo que devuelve el indice de la pila de moviminetos correspondiente al color que se le pasa por parametro
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private int GetColorIndex(Color color)
        {
            for (int i = 0; i < GameManager.Instance().currentSkin.colores.Length; i++)
            {
                if (color == GameManager.Instance().currentSkin.colores[i])
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Metodo que activa un rastro en la posicion indicada
        /// </summary>
        private void PutTraceInTile(Utils.Coord coord)
        {
            // Accedo al tile correspondiente a las coordenadas pasadas por parametro
            Tile t = _tiles[coord.y, coord.x];

            // direccion en la que tenemos que activar el trace
            Utils.Coord direction = _currentTilePress - coord;

            if (!t.IsEnd())
                t.SetColor(_currentTraceColor);

            t.ActiveTrace(new Vector2(direction.x, direction.y).normalized);

            _isMove = true;
        }

        /// <summary>
        ///  Retrocede hasta la casilla correspondiente
        /// </summary>
        /// <param name="coord"></param>
        private void BackToTile(Utils.Coord coord, Color currentColor)
        {
            Tile t = _tiles[coord.y, coord.x];
            int indexTraceStack = GetColorIndex(t.GetColor());

            _traceEnds[indexTraceStack] = false;

            // Eliminamos todos los rastros hasta la posicion pasada por parametro (coord)
            Utils.Coord position = coord;
            if (_traceStacks[indexTraceStack].Count > 0)
                position = _traceStacks[indexTraceStack].Peek();

            while ((position != coord || t.IsEnd()) && _traceStacks[indexTraceStack].Count > 1)
            {
                _traceStacks[indexTraceStack].Pop();
                _tiles[position.y, position.x].DesactiveTrace();

                if (_traceStacks[indexTraceStack].Count > 0)
                    position = _traceStacks[indexTraceStack].Peek();

                _isMove = true;
            }

            // Si estamos en una colision de colores hay que pintar el nuevo rastro
            if (currentColor != t.GetColor())
            {
                // eliminamos la ultima casilla del color antiguo
                _traceStacks[indexTraceStack].Pop();
                // ponemos el rastro del color nuevo
                PutTraceInTile(coord);

                // a�adimos el nuevo rastro a su nuevo color
                indexTraceStack = GetColorIndex(currentColor);
                _traceStacks[indexTraceStack].Push(coord);
                _currentTilePress = coord;
            }
        }

        /// <summary>
        /// Cuenta las casillas que tienen un color asignado
        /// </summary>
        /// <returns>Devuelve un INT que representa el numero de casillas que tienen color asignado</returns>
        private int CountTileWithColor()
        {
            int count = 0;
            foreach (Stack<Utils.Coord> s in _traceStacks)
                count += s.Count;
            return count;
        }

        /// <summary>
        ///  Metodo que ocurre cuando se pulsa la pantalla
        /// </summary>
        /// <param name="touchPos"></param>
        private void TouchDown(Vector2 touchPos)
        {
            // Redondeamos y pasamos a int para crear las coordenadas del array de tiles (la Y esta invertida)
            Utils.Coord indexTile = new Utils.Coord((int)Mathf.Round(touchPos.x), (int)Mathf.Round(-touchPos.y));
            if (!ValidCoords(indexTile))
                return;

            _countTileWithColor = CountTileWithColor();

            Tile tile = _tiles[indexTile.y, indexTile.x];

            // Si pulsamos en una casilla con color
            if (tile.IsTraceActive() || tile.IsEnd())
            {
                _changeColor = _currentTraceColor != tile.GetColor();
                // Asigno el color 
                _currentTraceColor = tile.GetColor();

                _tiles[_currentTilePress.y, _currentTilePress.x].SetCircleTrace(false);
                _isDiffEnd = false;

                Color c = tile.GetColor();
                int indexTraceStack = GetColorIndex(c);

                // Activamos el circulo en la posicion pulsada con el color correspondiente
                circleFinger.enabled = true;
                circleFinger.color = new Color(c.r, c.g, c.b, 0.5f);
                circleFinger.transform.position = new Vector3(touchPos.x, touchPos.y, 0);

                _currentTilePress = indexTile;

                // Desactivo las casillas que estan despues de esta
                BackToTile(indexTile, c);

                if (tile.IsEnd() && _traceStacks[indexTraceStack].Count > 0)
                    _traceStacks[indexTraceStack].Pop();

                // Si la casilla que hemos pulsado no esta en la pila, la metemos
                if (!_traceStacks[indexTraceStack].Contains(indexTile))
                    _traceStacks[indexTraceStack].Push(indexTile);
            }
        }

        /// <summary>
        ///  Metodo que ocurre cuando se mueve la pulsacion por la pantalla
        /// </summary>
        /// <param name="dragPos"></param>
        private void TouchDrag(Vector2 dragPos)
        {
            // si no hemos pulsado en ningun color no hacemos nada
            if (!circleFinger.enabled)
                return;

            // ponemos el circulo grande en la posicion de contacto
            circleFinger.transform.position = new Vector3(dragPos.x * _scaleFactor, dragPos.y * _scaleFactor, 0);

            // Redondeamos y pasamos a int para crear las coordenadas del array de tiles (la Y esta invertida)
            Utils.Coord indexTile = new Utils.Coord((int)Mathf.Round(dragPos.x), (int)Mathf.Round(-dragPos.y));
            if (!ValidCoords(indexTile))
            {
                _isDiffEnd = true;
                return;
            }

            // obtenemos el tile pulsado y le asignamos el color
            Tile tile = _tiles[indexTile.y, indexTile.x];
            int indexTraceStack = GetColorIndex(_currentTraceColor);

            // Si todavia estamos en el mismo tile no pintamos nada
            if (indexTile == _currentTilePress)
                return;

            // Si es un final de color distinto al que tenemos, no pintamos rastro
            if ((tile.GetColor() != _currentTraceColor && tile.IsEnd()) || _isDiffEnd)
            {
                _isDiffEnd = true;
                return;
            }

            // Si estamos en un tile por el que ya hemos pasado o el camino contiene esa casilla eliminamos los trace posteriores a ese tile
            if ((tile.IsTraceActive()) || _traceStacks[indexTraceStack].Contains(indexTile))
            {
                _isDiffEnd = false;
                BackToTile(indexTile, _currentTraceColor);
                _currentTilePress = indexTile;
                return;
            }

            // pintamos el tile y lo metemos en la pila correspondiente
            PutTraceInTile(indexTile);
            _traceStacks[indexTraceStack].Push(indexTile);
            _currentTilePress = indexTile;

            // Si hemos llegado al final lo notificamos para no seguir pintando 
            if (tile.IsEnd())
            {
                _isDiffEnd = true;
                _isEndPath = true;
            }
        }

        /// <summary>
        ///  Metodo que ocurre cuando se deja de pulsar la pantalla
        /// </summary>
        /// <param name="dragPos"></param>
        private void TouchRelease(Vector2 dragPos)
        {

            if (_changeColor)
            //if (_countTileWithColor != CountTileWithColor() && _isMove && _changeColor)
            {
                _isMove = false;
                _changeColor = false;
                _movementsCount++;
            }

            // quitamos el circulo grande
            circleFinger.enabled = false;
            // ponemos el circulo peque�o al ultimo tile presionado
            if (_isEndPath)
            {
                // animacion correspondiente
                _isEndPath = false;

                // comprobamos si hemos ganado
                _traceEnds[GetColorIndex(_currentTraceColor)] = true;
                if (AllColorsEnd())
                    Win();
            }
            else
            {
                _tiles[_currentTilePress.y, _currentTilePress.x].SetCircleTrace(true);
            }
        }

        /// <summary>
        /// Metodo que comprueba si todas las tuberias estan completas
        /// </summary>
        /// <returns>TRUE si todas las tuberias estan completas. FALSE en caso contrario</returns>
        private bool AllColorsEnd()
        {
            foreach (bool b in _traceEnds)
                if (!b)
                    return false;
            return true;
        }

        /// <summary>
        /// Metodo que se llama cuando se ha ganado el nivel. Muestra la ventana de victoria con los botones correspondientes
        /// </summary>
        private void Win()
        {
            SetActiveUpdate(false);
            GameManager.Instance().Win();
        }

        /// <summary>
        /// Metodo que procesa el input en funcion de si se esta ejecutando en el editor de unity o en un movil android
        /// </summary>
        private void ProcessInput()
        {
            bool press = false, drag = false, release = false;
            Vector2 pos = new Vector2(0, 0);

#if !UNITY_EDITOR && UNITY_ANDROID
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                press = touch.phase == TouchPhase.Began;
                drag = touch.phase == TouchPhase.Moved;
                release = touch.phase == TouchPhase.Ended;

                pos = touch.position;
            }
#else
            //Lo acabamos de pulsar
            press = Input.GetMouseButtonDown(0);
            //Lo estamos pulsando
            drag = Input.GetMouseButton(0);
            //Lo acabamos de soltar
            release = Input.GetMouseButtonUp(0);

            pos = Input.mousePosition;
#endif

            Vector2 unityPos = Camera.main.ScreenToWorldPoint(pos);
            unityPos /= _scaleFactor;
            if (press)
                TouchDown(unityPos);
            //Lo estamos pulsando
            else if (drag)
                TouchDrag(unityPos);
            //Lo acabamos de soltar
            else if (release)
                TouchRelease(unityPos);

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
        /// Metodo para obtener el numeor de flows completos
        /// </summary>
        /// <returns>Devuelve un numero INT que representa de flows completos</returns>
        public int GetNumFlowsEnded()
        {
            int count = 0;
            foreach (bool b in _traceEnds)
                if (b)
                    count++;

            return count;
        }

        /// <summary>
        /// Metodo para obtener el numero de flows que tiene el nivel
        /// </summary>
        /// <returns>Devuelve un INT que representa el numero de flows que tiene el nivel</returns>
        public int GetNumFlows()
        {
            return _traceEnds.Length;
        }

        /// <summary>
        /// Metodo para obtener el numero de moviminetos hechos
        /// </summary>
        /// <returns>Devuelve un numero INT que representa el numero de movimientos hechos</returns>
        public int GetNumMovements()
        {
            return _movementsCount;
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
            _movementsCount = 0;
            _countTileWithColor = 0;
            _isMove = false;
            _changeColor = false;
            _currentTraceColor = Color.clear;

            ResetBoard();

            _height = map.getAlto();
            _width = map.getAncho();

            _traceEnds = new bool[map.getFlujos()];
            for (int i = 0; i < _traceEnds.Length; i++)
                _traceEnds[i] = false;

            _traceStacks = new Stack<Utils.Coord>[map.getFlujos()];
            for (int i = 0; i < _traceStacks.Length; i++)
                _traceStacks[i] = new Stack<Utils.Coord>();

            _tiles = new Tile[_height, _width];

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _tiles[i, j] = Instantiate(tilePrefab);
                    _tiles[i, j].gameObject.transform.SetParent(gameObject.transform);
                    // El Tile (0,0) esta en la esquina superior-izquierda del "grid"
                    _tiles[i, j].gameObject.transform.localPosition = new Vector2(j, -i);

                    _tiles[i, j].id = IsEndOrStart(map.getTuberias(), i, j);

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

            MapRescaling();
        }
        #endregion



    }

}