using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class LevelManager : MonoBehaviour
    {
        [Tooltip("Board Manager")]
        public BoardManager boardManager;

        void Start()
        {
            boardManager.SetMap(GameManager.Instance().GetDebugLevel());
        }

        /// <summary>
        /// Metodo se llama para cambiar al siguiente nivel (lo llama el boardmanager)
        /// </summary>
        public void NextLevel()
        {
            // cambia el tablero con el siguiente nivel 
            boardManager.SetMap(GameManager.Instance().NextLevel());
        }
    }
}