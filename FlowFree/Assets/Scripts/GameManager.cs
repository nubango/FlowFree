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
            //levels.ToString();
            //levels[0].packName;
            Logic.Level x = new Logic.Level("9:12+B,0,31,9,,,45|54:46|55:47|56:48|57:50|59:51|60:52|61:53|62;60,61,62,71,80,89,98,107,106,105,104,103,102,101,100,99,90,81,72,63,54,55,56,57,58,49,50,51,52,53,44,35,26,17;59,68,77,76,75,74,83;67,66,65,64,73,82,91,92,93,84,85,86,87,78,69;13,22,21,30,39,40,41,42,43,34,25,16,7,8;18,9,0,1,2,3,4,5,6,15,24,33;37,38,29,20;48,47,46,45,36,27,28,19,10,11,12;70,79,88,97,96,95,94;14,23,32,31");
            //x.Map();


            /*Aqui las lineas del read txt
            Flow.GameManager _GMInstance = Flow.GameManager.Instance();
            //_GMInstance.intro.categoryPackages[0].packName;
            TextAsset txt = _GMInstance.intro.categoryPackages[0].maps;

            string h = txt.text;
            string[] levels = h.Split('\n');
            Debug.Log(levels[1]);
            Flow.Logic.Map map1;
            map1 = new Logic.Map(levels[1]);
            Debug.Log(map1.getAlto() +" "+ map1.getAncho() + " "+ map1.getFlujos() + " "+ map1.getNumLevel() + " ;");*/
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