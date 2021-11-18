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
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _tiles[i, j] = Instantiate(tilePrefab);
                    _tiles[i, j].gameObject.transform.SetParent(this.gameObject.transform);
                    _tiles[i, j].gameObject.transform.localPosition = new Vector2(i, j);

                    if ((i % 2) == 0)
                        _tiles[i, j].SetColor(Color.cyan);
                }
            }
        }
    }

}