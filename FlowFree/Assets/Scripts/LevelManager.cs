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
            boardManager.SetLevel();
        }
    }
}