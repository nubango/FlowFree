using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class BoardManager : MonoBehaviour
    {
        [Tooltip("Prefab del Tile")]
        public Tile tilePrefab;

        private Tile[,] _tiles;

        private int _width;
        private int _height;

        public void SetMap(Logic.Map map)
        {
        }

        // DEBUG
        public void SetLevel()
        {
            _tiles = new Tile[5, 5];
            int[] nums = { 1, 0, 2, 0, 3, 0, 0, 4, 0, 5, 0, 0, 0, 0, 0, 0, 2, 0, 3, 0, 0, 1, 4, 5, 0 };

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _tiles[i, j] = Instantiate(tilePrefab);
                    _tiles[i, j].gameObject.transform.SetParent(this.gameObject.transform);
                    _tiles[i, j].gameObject.transform.localPosition = new Vector2(j, -i);

                    _tiles[i, j].num = nums[(i * 5) + j];

                    if (_tiles[i, j].num != 0)
                    {
                        _tiles[i, j].EnableCircle();
                        _tiles[i, j].EnableTick();
                    }
                    _tiles[i, j].SetThinWalls(true, true, false, true);

                    _tiles[i, j].SetThickWalls(true, true, false, true);

                    switch (_tiles[i, j].num)
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