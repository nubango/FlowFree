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
            Logic.Map x = new Logic.Map();
            x.Parse("9:11+B,0,1,6,,45:46:47:48:50:51:52:53;79,88,87,86,85,84,83,82,73,64,65,66,67,68,59,60,61,62,71;11,12,13,14,15,16,25,34,33,32,31,30,39,38,37,36,27,18,9;24,23,22,21,20,29;0,1,10,19,28;2,3,4,5,6,7,8,17,26,35,44,43,42,41,40,49,58,57,56,55,54,63,72,81,90,91,92,93,94,95,96,97,98,89,80;70,69,78,77,76,75,74");
            
        }

        private void Update()
        {
            
        }
    }
}