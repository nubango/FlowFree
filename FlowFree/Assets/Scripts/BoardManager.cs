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
        private Stack<Coord>[] _traceStacks;

        // Tile actual por donde esta pasando el dedo/raton
        private Coord _currentTilePress;

        // Flag para no pintar despues de los extremos
        private bool _isDiffEnd = false;
        private bool _isEndPath = false;

        private void Awake()
        {
            // TODO: Inicializacion debe depender de cuantos colores tenga el nivel
            _traceStacks = new Stack<Coord>[5];
            for (int i = 0; i < _traceStacks.Length; i++)
                _traceStacks[i] = new Stack<Coord>();
        }
        void Start()
        {
            transform.position = new Vector3(0, 0, 0);
        }

        public struct Coord
        {
            public Coord(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x { get; }
            public int y { get; }

            public override bool Equals(object obj)
            {
                return obj is Coord coord &&
                       x == coord.x &&
                       y == coord.y;
            }

            public static bool operator ==(Coord a, Coord b) => (a.x == b.x && a.y == b.y);
            public static bool operator !=(Coord a, Coord b) => (a.x != b.x || a.y != b.y);
            public static Coord operator -(Coord a, Coord b) => new Coord(a.x - b.x, a.y - b.y);
        }
        private bool ValidCoords(Coord coord)
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
            //for(int i = 0; i < _numeroDeColores.length; i++)
            //{
            //    if (color == _numeroDeColores[i])
            //        return i;
            //}

            // ifs provisionales hasta que este implementado los colores de forma generica
            if (color == Color.red)
                return 0;
            else if (color == Color.green)
                return 1;
            else if (color == Color.yellow)
                return 2;
            else if (color == Color.blue)
                return 3;
            else if (color == Color.magenta)
                return 4;

            return -1;
        }

        // Metodo que activa un rastro en la posicion indicada
        private void PutTraceInTile(Coord coord)
        {
            // Accedo al tile correspondiente a las coordenadas pasadas por parametro
            Tile t = _tiles[coord.y, coord.x];

            // direccion en la que tenemos que activar el trace
            Coord direction = _currentTilePress - coord;

            if (!t.IsEnd())
                t.SetColor(_currentTraceColor);

            t.ActiveTrace(new Vector2(direction.x, direction.y).normalized);
        }

        /// <summary>
        ///  Retrocede hasta la casilla correspondiente
        /// </summary>
        /// <param name="coord"></param>
        private void BackToTile(Coord coord)
        {
            Tile t = _tiles[coord.y, coord.x];
            int indexTraceStack = GetColorIndex(t.GetColor());

            // Eliminamos todos los rastros hasta la posicion pasada por parametro (coord)
            Coord position = coord;
            if (_traceStacks[indexTraceStack].Count > 0)
                position = _traceStacks[indexTraceStack].Peek();

            while ((position != coord || t.IsEnd()) && _traceStacks[indexTraceStack].Count > 1)
            {
                _traceStacks[indexTraceStack].Pop();
                _tiles[position.y, position.x].DesactiveTrace();

                if (_traceStacks[indexTraceStack].Count > 0)
                    position = _traceStacks[indexTraceStack].Peek();
            }

            // Si estamos en una colision de colores hay que pintar el nuevo rastro
            if (_currentTraceColor != t.GetColor())
            {
                // eliminamos la ultima casilla del color antiguo
                _traceStacks[indexTraceStack].Pop();
                // ponemos el rastro del color nuevo
                PutTraceInTile(coord);

                // a�adimos el nuevo rastro a su nuevo color
                indexTraceStack = GetColorIndex(_currentTraceColor);
                _traceStacks[indexTraceStack].Push(coord);
                _currentTilePress = coord;
            }
        }

        /// <summary>
        ///  Metodo que ocurre cuando se pulsa la pantalla
        /// </summary>
        /// <param name="touchPos"></param>
        private void TouchDown(Vector2 touchPos)
        {
            // Redondeamos y pasamos a int para crear las coordenadas del array de tiles (la Y esta invertida)
            Coord indexTile = new Coord((int)Mathf.Round(touchPos.x), (int)Mathf.Round(-touchPos.y));
            if (!ValidCoords(indexTile))
                return;

            Tile tile = _tiles[indexTile.y, indexTile.x];
            // Si pulsamos en una casilla con color
            if (tile.IsTraceActive() || tile.IsEnd())
            {
                _tiles[_currentTilePress.y, _currentTilePress.x].SetCircleTrace(false);
                _isDiffEnd = false;
                // Asigno el color 
                _currentTraceColor = tile.GetColor();
                int indexTraceStack = GetColorIndex(_currentTraceColor);

                // Activamos el circulo en la posicion pulsada con el color correspondiente
                circleFinger.enabled = true;
                circleFinger.color = new Color(_currentTraceColor.r, _currentTraceColor.g, _currentTraceColor.b, 0.5f);
                circleFinger.transform.position = new Vector3(touchPos.x, touchPos.y, 0);

                _currentTilePress = indexTile;

                // Desactivo las casillas que estan despues de esta
                BackToTile(indexTile);

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
            circleFinger.transform.position = new Vector3(dragPos.x, dragPos.y, 0);

            // Redondeamos y pasamos a int para crear las coordenadas del array de tiles (la Y esta invertida)
            Coord indexTile = new Coord((int)Mathf.Round(dragPos.x), (int)Mathf.Round(-dragPos.y));
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
                BackToTile(indexTile);
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
            // quitamos el circulo grande
            circleFinger.enabled = false;
            // ponemos el circulo peque�o al ultimo tile presionado
            if (_isEndPath)
            {
                Debug.Log("Camino " + _currentTraceColor + " acabado");
                // animacion correspondiente
                _isEndPath = false;

                // comprobamos si hemos ganado
            }
            else
            {
                _tiles[_currentTilePress.y, _currentTilePress.x].SetCircleTrace(true);
            }

        }

        private void Update()
        {
            ProcessInput();
        }

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
            if (press)
                TouchDown(unityPos);
            //Lo estamos pulsando
            else if (drag)
                TouchDrag(unityPos);
            //Lo acabamos de soltar
            else if (release)
                TouchRelease(unityPos);

        }

        private int CompareTile(List<List<pos>> tuberias, int i, int j)
        {
            for (int h = 0; h < tuberias.Count; h++)
            {
                if (tuberias[h][0].x == j && tuberias[h][0].y == i ||
                    tuberias[h][tuberias[h].Count - 1].x == j && tuberias[h][tuberias[h].Count - 1].y == i)
                    return h + 1;
            }

            return 0;
        }

        public void SetMap(Logic.Level map)
        {
            int alto = map.getAlto();
            int ancho = map.getAncho();

            _tiles = new Tile[alto, ancho];

            for (int i = 0; i < alto; i++)
            {
                for (int j = 0; j < ancho; j++)
                {
                    _tiles[i, j] = Instantiate(tilePrefab);
                    _tiles[i, j].gameObject.transform.SetParent(gameObject.transform);
                    // El Tile (0,0) esta en la esquina superior-izquierda del "grid"
                    _tiles[i, j].gameObject.transform.localPosition = new Vector2(j, -i);

                    _tiles[i, j].id = CompareTile(map.getTuberias(), i, j);

                    if (_tiles[i, j].id != 0)
                    {
                        _tiles[i, j].SetCircleEnd(true);
                        _tiles[i, j].SetTick(true);
                    }
                    _tiles[i, j].SetThinWalls(true, true, true, true);

                    //_tiles[i, j].SetThickWalls(false, false, true, true);

                    switch (_tiles[i, j].id)
                    {
                        case 1:
                            _tiles[i, j].SetColor(Color.red);
                            _tiles[i, j].SetColor(Color.red);
                            break;
                        case 2:
                            _tiles[i, j].SetColor(Color.green);
                            _tiles[i, j].SetColor(Color.green);
                            break;
                        case 3:
                            _tiles[i, j].SetColor(Color.yellow);
                            _tiles[i, j].SetColor(Color.yellow);
                            break;
                        case 4:
                            _tiles[i, j].SetColor(Color.blue);
                            _tiles[i, j].SetColor(Color.blue);
                            break;
                        case 5:
                            _tiles[i, j].SetColor(Color.magenta);
                            _tiles[i, j].SetColor(Color.magenta);
                            break;
                    }
                }
            }

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
                        _tiles[i, j].SetCircleEnd(true);
                        _tiles[i, j].SetTick(true);
                    }
                    _tiles[i, j].SetThinWalls(true, true, true, true);

                    //_tiles[i, j].SetThickWalls(false, false, true, true);

                    switch (_tiles[i, j].id)
                    {
                        case 1:
                            _tiles[i, j].SetColor(Color.red);
                            _tiles[i, j].SetColor(Color.red);
                            break;
                        case 2:
                            _tiles[i, j].SetColor(Color.green);
                            _tiles[i, j].SetColor(Color.green);
                            break;
                        case 3:
                            _tiles[i, j].SetColor(Color.yellow);
                            _tiles[i, j].SetColor(Color.yellow);
                            break;
                        case 4:
                            _tiles[i, j].SetColor(Color.blue);
                            _tiles[i, j].SetColor(Color.blue);
                            break;
                        case 5:
                            _tiles[i, j].SetColor(Color.magenta);
                            _tiles[i, j].SetColor(Color.magenta);
                            break;
                    }
                }
            }
        }
    }

}