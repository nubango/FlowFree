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
        private int _currentLevel = 0;
        private int _currentPackage = 0;
        private int _currentCategory = 0;

        // Array de niveles que guarda la informacion que se va a guardar
        private List<SaveLevel> _saveLevels;

        // Flag que gestiona si se muestran o no los anuncios
        private bool _ads = true;
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
        /// Metodo suma uno al nivel actual
        /// </summary>
        public bool NextLevel()
        {
            // Si nos hemos pasado el ultimo nivel, volvemos al menu de las categorias
            // (es Length - 1 porque esta cogiendo un nivel 150 null)
            if (_currentLevel + 1 >= logicCategories[_currentCategory].GetPackages()[_currentPackage].GetLevels().Length - 1)
            {
                LoadCategoryScene();
                return false;
            }

            Logic.Level l = logicCategories[_currentCategory].GetPackages()[_currentPackage].GetLevels()[_currentLevel];

            int record = levelManager.GetNumMovements();

            record = l.GetRecord() == 0 ? record : l.GetRecord() > record ? record : l.GetRecord();

            l.SetRecord(record);

            _currentLevel++;
            return true;
        }

        /// <summary>
        /// Metodo que resta uno al nivel actual
        /// </summary>
        public bool PreviousLevel()
        {
            // Si nos hemos pasado el ultimo nivel, volvemos al menu de las categorias
            if (_currentLevel - 1 < 0)
            {
                // invalidar el boton?
                return false;
            }

            _currentLevel--;
            return true;
        }

        /// <summary>
        /// Metodo que Devuelve el nivel actual
        /// </summary>
        /// <returns>El nivel actual</returns>
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

        /// <summary>
        /// Metodo que se llama cuando nos hemos pasado un nivel
        /// </summary>
        public void Win(int record)
        {
            Logic.Level l = logicCategories[_currentCategory].GetPackages()[_currentPackage].GetLevels()[_currentLevel];
            SaveLevel saveLevel = new SaveLevel();

            saveLevel.category = _currentCategory;
            saveLevel.package = _currentPackage;
            saveLevel.level = _currentLevel;
            saveLevel.record = l.GetRecord() == 0 ? record : l.GetRecord() > record ? record : l.GetRecord();
            saveLevel.active = true;
            _saveLevels.Add(saveLevel);

            SaveSystem.Instance().Save(_saveLevels, levelManager.GetNumHints(), _ads);

            bool nextPackage = _currentLevel == logicCategories[_currentCategory].GetPackages()[_currentPackage].GetLevels().Length - 2;
            levelManager.Win(nextPackage);
        }

        public void DisableWinMenu()
        {
            levelManager.DisableWinMenu();
        }

        // DEBUG
        public Logic.Level GetDebugLevel()
        {
            return logicCategories[_currentCategory].GetPackages()[_currentPackage].GetLevels()[_currentLevel];
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
                _instance._saveLevels = _saveLevels;
                _instance.InitCategories();
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            _saveLevels = new List<SaveLevel>();

            InitCategories();

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // todo: cambiar de sitio este if
            if (SaveSystem.Instance().Load())
            {
                levelManager.SetNumHints(SaveSystem.Instance().GetGame().hints);
                _ads = SaveSystem.Instance().GetGame().ads;
                List<SaveLevel> l = SaveSystem.Instance().GetGame().levels;

                for (int i = 0; i < l.Count; i++)
                {
                    logicCategories[l[i].category].GetPackages()[l[i].package].GetLevels()[l[i].level].SetRecord(l[i].record);
                }
            }
        }

        #endregion


        /// <summary>
        /// Metodo que inicializa, si puede, las categorias con sus paquetes y niveles paseando los ficheros correspondientes
        /// </summary>
        private void InitCategories()
        {
            // si ya hemos inicializado las categorias no hacemos nada
            if (logicCategories != null)
                return;

            // Comprobamos si se han asignado en el editor las Categorias
            foreach (LevelPack.CategoryPackage c in categories)
                if (c == null)
                {
                    Debug.Log("INFORMACION: No se han asignado las categorias en el editor");
                    return;
                }

            logicCategories = new Logic.Category[categories.Length];

            // Parseamos las categorias
            for (int i = 0; i < categories.Length; i++)
                logicCategories[i] = ParseCategoyPackage.Parse(categories[i]);
        }

        #endregion

    }
}