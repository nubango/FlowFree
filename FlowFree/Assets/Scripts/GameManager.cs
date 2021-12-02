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
        public LevelPack.CategoryPackage introCategory;
        public LevelPack.CategoryPackage maniasCategory;
        public LevelPack.CategoryPackage rectanglesCategory;

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

        private Logic.Category intro;
        private Logic.Category manias;
        private Logic.Category rectangles;
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
        public Logic.Level NextLevel()
        {
            // gestionar si estamos en el ultimo nivel de un paquete o no y actuar en consecuencia
            return null;
        }

        // DEBUG
        public Logic.Level GetDebugLevel()
        {
            return rectangles.GetPackages()[2].GetMaps()[149];
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

        private void Start()
        {
            //intro.GetPackages()[0].GetMaps()[0].
        }
        #endregion


        /// <summary>
        /// Metodo que inicializa, si puede, las categorias con sus paquetes y niveles paseando los ficheros correspondientes
        /// </summary>
        private void InitCategories()
        {
            // Comprobamos si se han inicializado
            if (_instance.intro != null && _instance.manias != null && _instance.rectangles != null)
                return;

            // Comprobamos si se han asignado en el editor las Categorias
            if (introCategory == null || maniasCategory == null || rectanglesCategory == null)
            {
                Debug.Log("INFORMACION: No se han asignado las categorias en el editor");
                return;
            }

            _instance.intro = ParseCategoyPackage.Parse(introCategory);
            _instance.manias = ParseCategoyPackage.Parse(maniasCategory);
            _instance.rectangles = ParseCategoyPackage.Parse(rectanglesCategory);
        }
        #endregion

    }
}