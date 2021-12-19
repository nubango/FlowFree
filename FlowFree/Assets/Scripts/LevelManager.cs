using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    /*
     TODO: el levelmanager tiene que ser un sigleton y estar en el dont destroy on load (y pasarle todos los atributos)
     */
    public class LevelManager : MonoBehaviour
    {
        [Tooltip("Board Manager")]
        public BoardManager boardManager;

        [Tooltip("Texto del nivel")]
        public Text levelText;

        [Tooltip("Texto de la dimension del tablero")]
        public Text dimensionText;

        [Tooltip("Texto del numero de moviminetos realizados")]
        public Text movementsText;

        [Tooltip("Texto del record de numero de moviminetos realizados")]
        public Text recordText;

        [Tooltip("Texto del numero de flojos finalizados")]
        public Text flowsText;

        [Tooltip("Descripcion que aparece en ele menu de ganar un nivel")]
        public Text descriptionText;

        [Tooltip("Texto del boton de siguiente nivel/paquete")]
        public Text nextLevelButtonText;

        [Tooltip("Texto del del numero de pistas")]
        public Text hintText;

        [Tooltip("Texto del porcentaje de completado")]
        public Text pipePercentageText;

        [Tooltip("Menu que sale al pasarte el nivel")]
        public GameObject endLevelMenu;

        public Image star;
        public Image check;

        [Header("IU Objects Scaler")]
        public CanvasScaler canvasScaler;

        [Tooltip("Panel superior")]
        public RectTransform upperPanel;

        [Tooltip("Panle central")]
        public RectTransform statsPanel;

        [Tooltip("Panel inferior")]
        public RectTransform lowerPanel;

        private int _hints;

        private bool _endLevel;

        public static LevelManager _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                _instance.boardManager = boardManager;
                _instance.levelText = levelText;
                _instance.dimensionText = dimensionText;
                _instance.movementsText = movementsText;
                _instance.recordText = recordText;
                _instance.flowsText = flowsText;
                _instance.descriptionText = descriptionText;
                _instance.nextLevelButtonText = nextLevelButtonText;
                _instance.hintText = hintText;
                _instance.pipePercentageText = pipePercentageText;
                _instance.endLevelMenu = endLevelMenu;
                _instance.star = star;
                _instance.check = check;
                _instance.upperPanel = upperPanel;
                _instance.statsPanel = statsPanel;
                _instance.lowerPanel = lowerPanel;

                Flow.Logic.Level l = GameManager.Instance().GetCurrentLevel();
                _instance.SetUIData(l);
                _instance.DisableWinMenu();
                _instance.boardManager.SetMap(l);

                CheckLevelPassed(l);

                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;

            _hints = 100;
            _endLevel = false;

            DontDestroyOnLoad(gameObject);
        }

        public float GetCenterUnitySize()
        {
            return (GetCenterPixelSize() * Camera.main.orthographicSize * 2) / Screen.height;
        }

        public float GetTopUnitySize()
        {
            return (GetTopPixelSize() * Camera.main.orthographicSize * 2) / Screen.height;
        }

        public float GetBottomUnitySize()
        {
            return (GetBottomPixelSize() * Camera.main.orthographicSize * 2) / Screen.height;
        }

        public float GetCenterPixelSize()
        {
            float top = GetTopPixelSize();
            float bottom = GetBottomPixelSize();

            return Screen.height - top - bottom;
        }

        private void CheckLevelPassed(Logic.Level l)
        {
            if (l.IsPassed())
            {
                if (l.IsPerfectPassed())
                {
                    star.enabled = true;
                    check.enabled = false;
                }
                else
                {
                    star.enabled = false;
                    check.enabled = true;
                }
            }
            else
            {
                star.enabled = false;
                check.enabled = false;
            }
        }
        private float GetTopPixelSize()
        {
            Vector2 refResolution = canvasScaler.referenceResolution;

            // calculamos lo que mide en pixeles la zona de arriba de HUD
            float topHeightPixelSize = (Screen.width * (upperPanel.sizeDelta.y + statsPanel.sizeDelta.y)) / refResolution.x;

            // restamos el resultado anterior a la resolucion total para saber cuanto espacio nos queda
            return topHeightPixelSize;
        }

        private float GetBottomPixelSize()
        {
            Vector2 refResolution = canvasScaler.referenceResolution;

            // calculamos lo que mide en pixeles la zona de arriba de HUD
            float topHeightPixelSize = (Screen.width * lowerPanel.sizeDelta.y) / refResolution.x;

            // restamos el resultado anterior a la resolucion total para saber cuanto espacio nos queda
            return topHeightPixelSize;
        }

        private void Start()
        {
            if (boardManager)
            {
                DisableWinMenu();
                boardManager.SetMap(GameManager.Instance().GetCurrentLevel());
                SetUIData(GameManager.Instance().GetCurrentLevel());
            }
        }

        private void Update()
        {
            if (boardManager)
                UpdateUI();
        }

        private void UpdateUI()
        {
            movementsText.text = "pasos: " + boardManager.GetNumMovements();
            flowsText.text = "flujos: " + boardManager.GetNumFlowsEnded() + "/" + boardManager.GetNumFlows();
            hintText.text = _hints + " x";
            pipePercentageText.text = "tuberia " + boardManager.GetPercentage() + "%";
        }

        private void SetUIData(Logic.Level level)
        {
            levelText.text = "Nivel " + level.GetNumLevel();
            dimensionText.text = level.GetAlto() + "x" + level.GetAncho();
            recordText.text = "record: " + level.GetRecord();
        }

        /// <summary>
        /// Metodo se llama para cambiar al siguiente nivel
        /// </summary>
        public void NextLevel()
        {
            DisableWinMenu();
            if (!GameManager.Instance().NextLevel())
                return;

            Logic.Level level = GameManager.Instance().GetCurrentLevel();

            SetUIData(level);
            // cambia el tablero con el siguiente nivel 
            boardManager.SetMap(level);
            CheckLevelPassed(level);
        }

        /// <summary>
        /// Metodo se llama para cambiar al nivel anterior(lo llama el boardmanager)
        /// </summary>
        public void PreviousLevel()
        {
            DisableWinMenu();
            if (!GameManager.Instance().PreviousLevel())
                return;

            Logic.Level level = GameManager.Instance().GetCurrentLevel();

            SetUIData(level);
            // cambia el tablero con el siguiente nivel 
            boardManager.SetMap(level);
            CheckLevelPassed(level);
        }

        /// <summary>
        /// Metodo activa el menu de nivel ganado
        /// </summary>
        public void Win(bool nextPackage)
        {
            if (nextPackage)
                nextLevelButtonText.text = "next package";
            else
                nextLevelButtonText.text = "next level";


            _endLevel = true;
            endLevelMenu.SetActive(true);
            descriptionText.text = "You complete the level in " + boardManager.GetNumMovements() + " moves";
        }

        /// <summary>
        /// Deshabilita el menu que sale al ganar un nivel
        /// </summary>
        public void DisableWinMenu()
        {
            _instance.boardManager.SetActiveUpdate(true);
            _instance.endLevelMenu.SetActive(false);
            _instance._endLevel = false;
        }

        public void AddHint()
        {
            _hints++;
        }

        public int GetNumHints()
        {
            return _hints;
        }

        public void SetNumHints(int hints)
        {
            _hints = hints;
        }

        public int GetNumMovements() { return boardManager.GetNumMovements(); }

        public void TakeHint()
        {
            if (_hints == 0 || _endLevel)
                return;

            _hints--;
            boardManager.ShowPath();
        }
    }
}