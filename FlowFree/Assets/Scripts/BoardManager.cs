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

        void Start()
        {
            transform.position = new Vector3(-2, 2, 0);
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