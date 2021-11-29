using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class BoardManager : MonoBehaviour
    {
        [Tooltip("Prefab de la casilla")]
        public Tile tilePrefab;

        public SpriteRenderer circleFinguer;

        private Tile[,] _tiles;

        private int _width;
        private int _height;

        private Vector2 _currentInputPoint;

        // Contiene la posicion de la ultima pulsacion detectada
        private Vector2 _lastIndex;

        // Color actual del trazo
        private Color _traceColor;

        private Stack<Coord>[] _traceStacks;


        private Coord _firstPress;


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
            _currentInputPoint = new Vector2(0, 0);
            _lastIndex = new Vector2(-1, -1);
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
            public static bool operator !=(Coord a, Coord b) => (a.x != b.x && a.y != b.y);
            public static Coord operator -(Coord a, Coord b) => new Coord(a.x - b.x, a.y - b.y);
        }
        private bool ValidCoords(Coord coord)
        {
            return coord.x >= 0 && coord.y >= 0 && coord.x < _width && coord.y < _height;
        }

        // Metodo que devuelve el indice de la pila de moviminetos correspondiente al color que se le pasa por parametro
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

        // Metodo que ocurre cuando se pulsa la pantalla
        private void TouchDown(Vector2 touchPos)
        {
            // Transformamos la coordenada de la pantalla a coordenadas de unity
            Vector2 unityPos = Camera.main.ScreenToWorldPoint(touchPos);

            // Este redondeo se esta haciendo regular por que las pulsaciones no coinciden bien con los tiles
            Coord indexTile = new Coord((int)Mathf.Round(unityPos.x), (int)Mathf.Round(-unityPos.y));
            if (!ValidCoords(indexTile))
                return;

            Tile tile = _tiles[indexTile.y, indexTile.x];
            // Si pulsamos en una casilla con color
            if (!tile.IsEmpty())
            {
                // Asigno el color 
                _traceColor = tile.GetCircleColor();
                int indexColor = GetColorIndex(_traceColor);

                // Activamos el circulo en la posicion pulsada con el color correspondiente
                circleFinguer.enabled = true;
                circleFinguer.color = new Color(_traceColor.r, _traceColor.g, _traceColor.b, 0.5f);
                circleFinguer.transform.position = new Vector3(unityPos.x, unityPos.y, 0);

                _firstPress = indexTile;

                // Desactivo las casillas que estan despues de esta
                while (_traceStacks[indexColor].Count > 0 && _traceStacks[indexColor].Peek() != indexTile)
                {
                    Coord c = _traceStacks[indexColor].Pop();
                    _tiles[c.y, c.x].DesactiveTrace();
                }
            }
        }

        private void PutTraceInTile(Coord coord)
        {
            // Accedo al tile correspondiente a las coordenadas pasadas por parametro
            Tile t = _tiles[coord.y, coord.x];
            int index = GetColorIndex(t.GetTraceColor());

            Coord lastIndex = _firstPress;

            if (_traceStacks[index].Count > 0)
                // calculo la direccion en la que tengo que activar el rastro, la contraria a la del movimiento porque se activa el rastro de la casilla nueva
                lastIndex = _traceStacks[index].Peek();

            Coord direction = lastIndex - coord;

            // Asigno el color y activo el rastro
            _tiles[coord.y, coord.x].SetTraceColor(_traceColor);
            _tiles[coord.y, coord.x].ActiveTrace(new Vector2(direction.x, direction.y));

            // Introduzco a la pila de posiciones la nueva coordenada
            _traceStacks[index].Push(coord);
        }

        private void BackToTile(Coord coord)
        {
            Tile t = _tiles[coord.y, coord.x];
            int index = GetColorIndex(t.GetTraceColor());

            // Eliminamos todos los rastros hasta la posicion pasada por parametro (coord)
            Coord position = _traceStacks[index].Pop();
            while (position != coord)
            {
                _tiles[position.y, position.x].DesactiveTrace();
                position = _traceStacks[index].Pop();
            }

            _tiles[position.y, position.x].DesactiveTrace();
        }

        // Metodo que ocurre cuando se mueve la pulsacion por la pantalla
        private void TouchDrag(Vector2 dragPos)
        {
            // si no hemos pulsado en ningun color no hacemos nada
            if (!circleFinguer.enabled)
                return;

            // Transformamos la coordenada de la pantalla a coordenadas de unity
            Vector2 unityPos = Camera.main.ScreenToWorldPoint(dragPos);
            // ponemos el circulo grande en la posicion de contacto
            circleFinguer.transform.position = new Vector3(unityPos.x, unityPos.y, 0);

            // estamos dentro de los limites del tablero?
            Coord indexTile = new Coord((int)unityPos.x, (int)-unityPos.y);
            if (indexTile.x < 0 || indexTile.y < 0 || indexTile.x >= _width || indexTile.y >= _height)
                return;

            // obtenemos el tile pulsado
            Tile tile = _tiles[indexTile.y, indexTile.x];
            int indexTraceStack = GetColorIndex(tile.GetCircleColor());

            // comprobamos si el tile es el final del path
            if (tile.IsExtreme() && _traceStacks[indexTraceStack].Count > 0 && !_traceStacks[indexTraceStack].Contains(indexTile))
            {
                // animacion de los circulitos
            }
            // si tiene activado un trace (ya hemos pasado por aqui)
            else if (tile.IsTraceActive())
            {
                // recolocamos el path del antiguo color
                BackToTile(indexTile);
            }

            // cambiamos de color
            tile.SetTraceColor(_traceColor);

            // "pulsamos" el tile (pintamos rastro correspondiente)
            PutTraceInTile(indexTile);
        }

        // Metodo que ocurre cuando se deja de pulsar la pantalla
        private void TouchRelease(Vector2 dragPos)
        {
            // quitamos el circulo grande
            circleFinguer.enabled = false;
            // ponemos el circulo peque�o al ultimo tile presionado
            // comprobamos si hemos ganado
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        TouchDown(touch.position);
                        break;
                    case TouchPhase.Moved:
                        TouchDrag(touch.position);
                        break;
                    case TouchPhase.Ended:
                        TouchRelease(touch.position);
                        break;
                    default:
                        break;
                }
            }
        }
        /*
        una pila donde se guardan las posiciones de las casillas por las que vas pasando (una por cada color) cada vez que se avanza se mete en la pila y si vuelve a una
        posicion por la que ya se ha pasado, lo unico que hay que hacer es quitar posiciones de la pila correspondiente hasta que aparezca la casilla en cuestion
         */

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
                            _tiles[i, j].SetCircleColor(Color.red);
                            break;
                        case 2:
                            _tiles[i, j].SetCircleColor(Color.green);
                            break;
                        case 3:
                            _tiles[i, j].SetCircleColor(Color.yellow);
                            break;
                        case 4:
                            _tiles[i, j].SetCircleColor(Color.blue);
                            break;
                        case 5:
                            _tiles[i, j].SetCircleColor(Color.magenta);
                            break;
                    }
                }
            }
        }
    }

}