using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    /*
     Para los muros hay que hacer un metodo en tile que le pasas una direccion (tileactual- anterior) para saber si en esa direccion hay un muro. Devuelve un bool, TRUE si hay muro y FALSE si no hay muro
     */
    public class TraceInput
    {
        #region ATRIBUTTES
        private BoardManager _boardManager;

        // Color actual del trazo
        private Color _currentTraceColor;

        private Color _lastTraceColor;

        // Array de pilas que contienen los caminos actuales de cada color
        private Stack<Utils.Coord>[] _traceStacks;

        // Array que lleva la cuenta de los cambios en los diferentes caminos
        private int[] _traceStacksLengthCount;

        // Array que guarda TRUE si un flujo esta completo y FALSE en caso contrario
        private bool[] _traceEnds;

        // Ultimo tile que tiene color
        private Utils.Coord _lastColorTile;

        // Tile actual por donde esta pasando el dedo/raton
        private Utils.Coord _currentTilePress;

        // Flag para no pintar despues de los extremos
        private bool _isEndPath = false;

        // Flag para saber si hemos pulsado fuera de la pantalla y asi no contar un movimiento
        private bool _pressOutBounds = false;

        // cuenta de los movimientos que llevamos en el nivel
        private int _movementsCount = 0;

        private SpriteRenderer _circleFinger;

        private Tile[,] _tiles;

        private List<List<Utils.Coord>> _paths;

        private int _countShowPaths;
        #endregion

        #region PUBLIC_METHODS
        public void Init(BoardManager bm, Tile[,] tiles, List<List<Utils.Coord>> paths, SpriteRenderer circleFinger, int numFlujos)
        {
            _boardManager = bm;
            _tiles = tiles;
            _paths = paths;
            _circleFinger = circleFinger;
            _movementsCount = 0;
            _currentTraceColor = Color.clear;
            _lastTraceColor = Color.clear;
            _countShowPaths = 0;

            _traceEnds = new bool[numFlujos];
            for (int i = 0; i < _traceEnds.Length; i++)
                _traceEnds[i] = false;

            _traceStacksLengthCount = new int[numFlujos];
            for (int i = 0; i < numFlujos; i++)
                _traceStacksLengthCount[i] = 0;

            _traceStacks = new Stack<Utils.Coord>[numFlujos];
            for (int i = 0; i < numFlujos; i++)
                _traceStacks[i] = new Stack<Utils.Coord>();
        }

        /// <summary>
        /// Metodo que procesa el input en funcion de si se esta ejecutando en el editor de unity o en un movil android
        /// </summary>
        public void ProcessInput()
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
            unityPos /= _boardManager.GetScaleFactor();
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
        /// Metodo para obtener el numero de moviminetos hechos
        /// </summary>
        /// <returns>Devuelve un numero INT que representa el numero de movimientos hechos</returns>
        public int GetNumMovements()
        {
            return _movementsCount;
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
        /// Metodo que muestra un path completo
        /// </summary>
        public void ShowPath()
        {
            if (_countShowPaths + 1 == _paths.Count)
                return;
            _countShowPaths++;

            for (int i = 0; i < _paths[_countShowPaths].Count - 1; i++)
            {
                Utils.Coord p = _paths[_countShowPaths][i];

                Color c;
                if (_tiles[p.y, p.x].IsEnd())
                    c = _tiles[p.y, p.x].GetColor();
                else
                {
                    Utils.Coord lastPos = _paths[_countShowPaths][i + 1];
                    c = _tiles[lastPos.y, lastPos.x].GetColor();
                }
                PutTraceInTile(p, _paths[_countShowPaths][i + 1] - p, c);
            }

        }
        #endregion

        #region PRIVATE_METHODS
        /// <summary>
        ///  Metodo que ocurre cuando se pulsa la pantalla
        /// </summary>
        /// <param name="touchPos"></param>
        private void TouchDown(Vector2 touchPos)
        {
            // Redondeamos y pasamos a int para crear las coordenadas del array de tiles (la Y esta invertida)
            Utils.Coord indexTile = new Utils.Coord((int)Mathf.Round(touchPos.x), (int)Mathf.Round(-touchPos.y));
            if (!_boardManager.ValidCoords(indexTile))
            {
                _pressOutBounds = true;
                return;
            }
            Tile tile = _tiles[indexTile.y, indexTile.x];

            // Si pulsamos en una casilla con color
            if (tile.IsTraceActive() || tile.IsEnd())
            {
                // actualiza la longitud de cada path
                for (int i = 0; i < _traceStacksLengthCount.Length; i++)
                    _traceStacksLengthCount[i] = _traceStacks[i].Count;

                _tiles[_lastColorTile.y, _lastColorTile.x].SetCircleTrace(false);

                Color c = tile.GetColor();
                _currentTraceColor = c;
                int indexTraceStack = GetColorIndex(c);

                // Activamos el circulo en la posicion pulsada con el color correspondiente
                _circleFinger.enabled = true;
                _circleFinger.color = new Color(c.r, c.g, c.b, 0.5f);
                _circleFinger.transform.position = new Vector3(touchPos.x, touchPos.y, 0);

                _lastColorTile = indexTile;

                // Desactivo las casillas que estan despues de esta
                BackToTile(indexTile);

                if (tile.IsEnd() && _traceStacks[indexTraceStack].Count > 0)
                    _traceStacks[indexTraceStack].Pop();

                // Si la casilla que hemos pulsado no esta en la pila, la metemos
                if (!_traceStacks[indexTraceStack].Contains(indexTile))
                    _traceStacks[indexTraceStack].Push(indexTile);

                // vemos si hemos pulsado en un inicio y no hemos avanzado, entonces actualizamos para que no cuente como movimiento
                for (int i = 0; i < _traceStacksLengthCount.Length; i++)
                {
                    if (_traceStacksLengthCount[i] == _traceStacks[i].Count || _traceStacksLengthCount[i] + 1 == _traceStacks[i].Count)
                    {
                        _traceStacksLengthCount[i] = _traceStacks[i].Count;
                    }
                }
            }
        }

        /// <summary>
        ///  Metodo que ocurre cuando se mueve la pulsacion por la pantalla
        /// </summary>
        /// <param name="dragPos"></param>
        private void TouchDrag(Vector2 dragPos)
        {
            // si no hemos pulsado en ningun color no hacemos nada
            if (!_circleFinger.enabled)
                return;

            // ponemos el circulo grande en la posicion de contacto
            _circleFinger.transform.position = new Vector3(dragPos.x * _boardManager.GetScaleFactor(), dragPos.y * _boardManager.GetScaleFactor(), 0);

            // Redondeamos y pasamos a int para crear las coordenadas del array de tiles (la Y esta invertida)
            Utils.Coord indexTile = new Utils.Coord((int)Mathf.Round(dragPos.x), (int)Mathf.Round(-dragPos.y));
            if (!_boardManager.ValidCoords(indexTile))
                return;

            _currentTilePress = indexTile;

            // Calcula la tangente del angulo formado por el triangulo rectangulo que define el segmento que une los dos puntos. 
            Utils.Coord segment = _lastColorTile - _currentTilePress;

            segment = Normalize(segment);
            Vector2 d = new Vector2(-segment.x, -segment.y);
            Utils.Coord direction = new Utils.Coord((int)d.x, (int)d.y);

            //Debug.Log(direction.x + " " + direction.y);
            //Debug.Log(_currentTilePress.x + " " + _currentTilePress.y);

            GoToDirection(direction);

        }

        /// <summary>
        ///  Metodo que ocurre cuando se deja de pulsar la pantalla
        /// </summary>
        /// <param name="dragPos"></param>
        private void TouchRelease(Vector2 dragPos)
        {
            if (_pressOutBounds)
            {
                _pressOutBounds = false;
                return;
            }

            if (_lastTraceColor != _currentTraceColor && HasPathsChanged())
            {
                _movementsCount++;
                _lastTraceColor = _currentTraceColor;
            }

            // quitamos el circulo grande
            _circleFinger.enabled = false;
            // ponemos el circulo peque�o al ultimo tile presionado
            if (_isEndPath)
            {
                // animacion correspondiente
                _isEndPath = false;

                // comprobamos si hemos ganado
                _traceEnds[GetColorIndex(_currentTraceColor)] = true;
                if (AllColorsEnd())
                {
                    _lastTraceColor = Color.clear;
                    _boardManager.Win();
                }
            }
            else
            {
                _tiles[_lastColorTile.y, _lastColorTile.x].SetCircleTrace(true);
            }

            _currentTraceColor = Color.clear;
        }

        private bool HasPathsChanged()
        {
            bool change = false;

            for (int i = 0; i < _traceStacksLengthCount.Length && !change; i++)
                change = _traceStacksLengthCount[i] != _traceStacks[i].Count;

            return change;
        }

        private Utils.Coord Normalize(Utils.Coord v)
        {
            if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            {
                if (v.x > 0)
                    v.x = 1;
                else if (v.x < 0)
                    v.x = -1;
                v.y = 0;
            }
            else if (Mathf.Abs(v.x) < Mathf.Abs(v.y))
            {
                v.x = 0;
                if (v.y > 0)
                    v.y = 1;
                else if (v.y < 0)
                    v.y = -1;
            }

            return v;
        }

        private void GoToDirection(Utils.Coord direction)
        {
            // si vamos en diagonal no hacemos nada
            if (Mathf.Abs(direction.x) == Mathf.Abs(direction.y))
                return;

            Utils.Coord nextPos = _lastColorTile + direction;
            if (!_boardManager.ValidCoords(nextPos))
                return;

            Tile t = _tiles[nextPos.y, nextPos.x];
            int indexTraceStack = GetColorIndex(_currentTraceColor);

            // si la casilla siguiente es el inicio o si la casilla siguiente es un trazo del mismo color 
            if ((t.IsEnd() && _traceStacks[indexTraceStack].Contains(nextPos) && !t.IsEmpty()) || (t.GetColor() == _currentTraceColor && !t.IsEnd() && !t.IsEmpty()))
            {
                // vuelvo atras en esa casilla
                BackToTile(nextPos);
            }
            // si la casilla siguiente esta vacia y la anterior no es un final o
            // si la casilla siguiente esta vacia y la anterior es el inicio o 
            // si la casilla siguiente es el final
            else if ((GetColorIndex(t.GetColor()) == -1 && !_tiles[_lastColorTile.y, _lastColorTile.x].IsEnd() &&
                _traceStacks[indexTraceStack].Count > 1 && !t.IsEmpty()) ||
                (GetColorIndex(t.GetColor()) == -1 && _tiles[_lastColorTile.y, _lastColorTile.x].IsEnd() &&
                _traceStacks[indexTraceStack].Count == 1 && !t.IsEmpty()) ||
                (t.IsEnd() && !_traceStacks[indexTraceStack].Contains(nextPos) &&
                t.GetColor() == _currentTraceColor && !t.IsEmpty()))
            {

                // pinto el trazo
                PutTraceInTile(nextPos, _lastColorTile - nextPos, _currentTraceColor);

                _isEndPath = t.IsEnd() && t.GetColor() == _currentTraceColor;
            }
            // si la casilla siguiente es un trazo de otro color y no un final
            else if ((GetColorIndex(t.GetColor()) != -1 && t.GetColor() != _currentTraceColor &&
                !t.IsEnd() && !_tiles[_lastColorTile.y, _lastColorTile.x].IsEnd() && !t.IsEmpty()) ||
                (GetColorIndex(t.GetColor()) != -1 && t.GetColor() != _currentTraceColor &&
                !t.IsEnd() && _tiles[_lastColorTile.y, _lastColorTile.x].IsEnd() && !t.IsEmpty()))
            {
                // vuelvo atras en esa casilla
                BackToTile(nextPos);
                // pinto el trazo nuevo
                PutTraceInTile(nextPos, _lastColorTile - nextPos, _currentTraceColor);
            }

            if (t.GetColor() == _currentTraceColor)
                _lastColorTile = nextPos;
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
        private void PutTraceInTile(Utils.Coord coord, Utils.Coord direction, Color color)
        {
            Tile t = _tiles[coord.y, coord.x];
            int indexTraceStack = GetColorIndex(t.GetColor());
            if (indexTraceStack != -1 && _traceStacks[indexTraceStack].Count > 0 && t.GetColor() != color)
                // eliminamos la ultima casilla del color antiguo
                _traceStacks[indexTraceStack].Pop();

            if (!t.IsEnd())
                t.SetColorTrace(color);

            t.ActiveTrace(new Vector2(direction.x, direction.y).normalized);

            // añadimos el nuevo rastro a su nuevo color
            indexTraceStack = GetColorIndex(t.GetColor());
            _traceStacks[indexTraceStack].Push(coord);
        }

        /// <summary>
        ///  Retrocede hasta la casilla correspondiente
        /// </summary>
        /// <param name="coord"></param>
        private void BackToTile(Utils.Coord coord)
        {
            Tile t = _tiles[coord.y, coord.x];
            int indexTraceStack = GetColorIndex(t.GetColor());
            if (indexTraceStack == -1)
                return;

            _traceEnds[indexTraceStack] = false;

            // Eliminamos todos los rastros hasta la posicion pasada por parametro (coord)
            Utils.Coord position = coord;
            if (_traceStacks[indexTraceStack].Count > 0)
                position = _traceStacks[indexTraceStack].Peek();

            while ((position != coord || t.IsEnd()) && _traceStacks[indexTraceStack].Count > 1)
            {
                Debug.Log(position.x + " " + position.y);
                _traceStacks[indexTraceStack].Pop();
                _tiles[position.y, position.x].DesactiveTrace();


                if (_traceStacks[indexTraceStack].Count > 0)
                    position = _traceStacks[indexTraceStack].Peek();
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
        #endregion
    }
}