using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Flow
{
    public class GameManager : MonoBehaviour
    {
        #region PUBLIC_ATTRIBUTES
        public LevelManager levelManager;

        [Header("Categories")]
        public LevelPack.CategoryPackage[] categories;

        [Header("Current Skin")]
        public LevelPack.SkinPackage currentSkin;

        [Header("DEBUG")]
        public int currentLevel = 1;
        // Asset *GOLPE* de Texto *GOLPE*
        public TextAsset currentLevelTxt;
        #endregion



        #region PRIVATE_ATTRIBUTES
        // Game Manager es un Singleton
        private static GameManager _instance;

        private Logic.Category[] logicCategories;

        // Nivel actual
        private int _currentLevel;
        private int _currentPackage;
        private int _currentCategory;
        #endregion



        #region PUBLIC_METHODS
        public static GameManager Instance()
        {
            return _instance;
        }


        #region Methods for loading scenes
        public void LoadCategoryScene()
        {
            SceneManager.LoadScene("CategoryScene");
        }

        public void LoadLevelsScene()
        {
            SceneManager.LoadScene("LevelsScene");
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene("Game");
        }
        #endregion


        /// <summary>
        /// Metodo que devuelve el siguiente nivel a jugar
        /// </summary>
        /// <returns></returns>
        public void NextLevel()
        {
            // Si nos hemos pasado el ultimo nivel, volvemos al menu de las categorias
            if (_currentLevel + 1 >= logicCategories[_currentCategory].GetPackages()[_currentPackage].GetLevels().Length)
            {
                LoadCategoryScene();
                return;
            }

            _currentLevel++;
        }

        /// <summary>
        /// Metodo que Devuelve el nivel actual
        /// </summary>
        /// <returns></returns>
        public Logic.Level GetCurrentLevel()
        {
            return logicCategories[_currentCategory].GetPackages()[_currentPackage].GetLevels()[_currentLevel];
        }


        /// <summary>
        /// Cambia el nivel actual
        /// </summary>
        /// <returns></returns>
        public Logic.Level AssignCurrentLevel(int category, int package, int level)
        {
            _currentCategory = category;
            _currentPackage = package;
            _currentLevel = level;
            return logicCategories[category].GetPackages()[package].GetLevels()[level];
        }


        // DEBUG
        public Logic.Level GetDebugLevel()
        {
            return logicCategories[0].GetPackages()[0].GetLevels()[0];
        }
        // DEBUG
        #endregion



        #region PRIVATE_METHODS

        #region Unity
        private void Awake()
        {
            if (_instance != null)
            {
                _instance.levelManager = levelManager;
                InitCategories();
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;

            InitCategories();

            DontDestroyOnLoad(gameObject);
        }

        #endregion


        /// <summary>
        /// Metodo que inicializa, si puede, las categorias con sus paquetes y niveles paseando los ficheros correspondientes
        /// </summary>
        private void InitCategories()
        {
            // si ya hemos inicializado las categorias no hacemos nada
            if (_instance.logicCategories != null)
                return;

            // Comprobamos si se han asignado en el editor las Categorias
            foreach (LevelPack.CategoryPackage c in _instance.categories)
                if (c == null)
                {
                    Debug.Log("INFORMACION: No se han asignado las categorias en el editor");
                    return;
                }

            _instance.logicCategories = new Logic.Category[_instance.categories.Length];

            // Parseamos las categorias
            for (int i = 0; i < _instance.categories.Length; i++)
                _instance.logicCategories[i] = ParseCategoyPackage.Parse(_instance.categories[i]);

        }
        #endregion

    }
}