using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class GameManager : MonoBehaviour
    {
        public LevelManager levelManager;

        [Header("DEBUG")]
        public int currentLevel = 1;

        void Awake()
        {
            //if (_instance != null)
            //{
            //    _instance.levelManager = levelManager;
            //}
        }
    }
}