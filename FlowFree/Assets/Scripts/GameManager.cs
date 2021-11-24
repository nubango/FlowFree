using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flow
{
    public class GameManager : MonoBehaviour
    {
        public LevelManager levelManager;

        [Header("Categories")]
        public LevelPack.CategoryPackage intro;
        public LevelPack.CategoryPackage manias;
        public LevelPack.CategoryPackage rectangles;

        [Header("Current Skin")]
        public LevelPack.SkinPackage currentSkin;

        [Header("DEBUG")]
        public int currentLevel = 1;
        // Asset *GOLPE* de Texto *GOLPE*
        public TextAsset currentLevelTxt;

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

        private void Start()
        {
            //levels.ToString();
            //levels[0].packName;
        }

        private void Update()
        {
            
        }
    }
}