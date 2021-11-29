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
            Logic.Map x = new Logic.Map("9:12+B,0,31,9,,,45|54:46|55:47|56:48|57:50|59:51|60:52|61:53|62;60,61,62,71,80,89,98,107,106,105,104,103,102,101,100,99,90,81,72,63,54,55,56,57,58,49,50,51,52,53,44,35,26,17;59,68,77,76,75,74,83;67,66,65,64,73,82,91,92,93,84,85,86,87,78,69;13,22,21,30,39,40,41,42,43,34,25,16,7,8;18,9,0,1,2,3,4,5,6,15,24,33;37,38,29,20;48,47,46,45,36,27,28,19,10,11,12;70,79,88,97,96,95,94;14,23,32,31");
            //x.Map();
            
        }

        private void Update()
        {
            
        }
    }
}