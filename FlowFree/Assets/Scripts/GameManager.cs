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

        // Game Manager es un Singleton
        static GameManager _instance;

        public static GameManager Instance()
        {
            return _instance;
        }

        void Awake()
        {
            if (_instance != null)
            {
                _instance.levelManager = levelManager;
                DestroyImmediate(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}