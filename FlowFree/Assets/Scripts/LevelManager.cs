using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    public class LevelManager : MonoBehaviour
    {
        [Header("INTRO SCENE OBJECTS")]
        public Text titleIntroText;

        [Header("CATEGORY SCENE OBJECTS")]
        public Text titleCategoryText;

        public GameObject categoryPrefab;
        public CategoryHeader categoryHeaderPrefab;
        public GameObject packagesGroupPrefab;
        public GameObject packagePrefab;
        public GameObject categoryContent;

        [Header("LEVEL SCENE OBJECTS")]
        [Tooltip("Titulo: nombre del paquete")]
        public Text titlePackageText;

        public GameObject gridPrefab;
        public GameObject linePrefab;
        public LevelButton levelButtonPrefab;
        public GameObject levelContent;

        [Header("GAME SCENE OBJECTS")]
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
                TransferInformation();
                Init();
                DestroyImmediate(gameObject);
                return;
            }

            _instance = this;
            _hints = 100;
            _endLevel = false;

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Transfiere la informacion al instance
        /// </summary>
        private void TransferInformation()
        {
            // CATEGORY SCENE OBJECTS
            _instance.titleCategoryText = titleCategoryText;

            _instance.categoryPrefab = categoryPrefab;
            _instance.categoryHeaderPrefab = categoryHeaderPrefab;
            _instance.packagesGroupPrefab = packagesGroupPrefab;
            _instance.packagePrefab = packagePrefab;
            _instance.categoryContent = categoryContent;

            // LEVEL SCENE OBJECTS
            _instance.titlePackageText = titlePackageText;
            _instance.gridPrefab = gridPrefab;
            _instance.linePrefab = linePrefab;
            _instance.levelButtonPrefab = levelButtonPrefab;
            _instance.levelContent = levelContent;

            // GAME SCENE OBJECTS
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
            _instance.canvasScaler = canvasScaler;
            _instance.upperPanel = upperPanel;
            _instance.statsPanel = statsPanel;
            _instance.lowerPanel = lowerPanel;
        }


        private void Start()
        {
            Init();
        }

        /// <summary>
        /// Inicializa los atributos necesarios para la escena en la que esta
        /// </summary>
        private void Init()
        {
            if (boardManager)
            {
                DisableWinMenu();
                boardManager.SetMap(GameManager.Instance().GetCurrentLevel());
                SetUIData(GameManager.Instance().GetCurrentLevel());
                CheckLevelPassed(GameManager.Instance().GetCurrentLevel());
            }
            else if (titlePackageText)
            {
                if (_instance.titlePackageText)
                    _instance.titlePackageText.text = ChangeColorTitle(GameManager.Instance().GetCurrentPackage().GetPackName());
                CreateLevelsGrid();
            }
            else if (titleCategoryText)
            {
                if (_instance.titleCategoryText)
                    _instance.titleCategoryText.text = ChangeColorTitle("niveles");
                CreateCategoryGrid();
            }
            else if (titleIntroText)
            {
                if (_instance.titleIntroText)
                    _instance.titleIntroText.text = ChangeColorTitle("flow");
            }
        }

        /// <summary>
        /// Crea el el tablero de niveles (escena de seleccion de niveles)
        /// </summary>
        private void CreateLevelsGrid()
        {
            Logic.Package pack = GameManager.Instance().GetCurrentPackage();
            Logic.Level[] levels = pack.GetLevels();


            int i = 0;

            GameObject grid = Instantiate(gridPrefab);
            grid.transform.parent = levelContent.transform;
            GameObject line = Instantiate(linePrefab);
            line.transform.parent = grid.transform;

            levelButtonPrefab.text.text = (i + 1).ToString();
            levelButtonPrefab.backgroud.color = levels[i].GetLevelColor();
            GameObject button = Instantiate(levelButtonPrefab.gameObject);
            button.GetComponent<LevelButton>().SetId(i);
            button.transform.parent = line.transform;

            while (++i < levels.Length - 1)
            {
                if (i % 30 == 0)
                {
                    grid = Instantiate(gridPrefab);
                    grid.transform.parent = levelContent.transform;
                }

                if (i % 5 == 0)
                {
                    line = Instantiate(linePrefab);
                    line.transform.parent = grid.transform;
                }

                Logic.Level level = levels[i];

                int l = ((i + 1) % 30) == 0 ? 30 : ((i + 1) % 30);
                levelButtonPrefab.text.text = l.ToString();
                levelButtonPrefab.backgroud.color = level.GetLevelColor();
                button = Instantiate(levelButtonPrefab.gameObject);

                LevelButton lb = button.GetComponent<LevelButton>();
                lb.SetId(i);

                if (level.IsLocked())
                    lb.SetActiveLocked(true);
                else if (level.IsPerfectCompleted())
                    lb.SetActiveStar(true);
                else if (level.IsCompleted())
                    lb.SetActiveTick(true);


                button.transform.parent = line.transform;
            }
        }

        /// <summary>
        /// Crea la lista de paquetes (escena de seleccion de paquetes)
        /// </summary>
        private void CreateCategoryGrid()
        {
            Logic.Category[] categories = GameManager.Instance().GetCategories();

            for (int i = 0; i < categories.Length; i++)
            {
                GameObject category = Instantiate(categoryPrefab);
                category.transform.parent = categoryContent.transform;

                GameObject categoryHeader = Instantiate(categoryHeaderPrefab.gameObject);
                categoryHeader.transform.parent = category.transform;
                CategoryHeader ch = categoryHeader.GetComponent<CategoryHeader>();

                ch.headerText.text = categories[i].GetName();
                ch.headerBackground.color = Color.Lerp(categories[i].GetColor(), Color.black, 0.25f);
                ch.headerLine.color = categories[i].GetColor();


                if (categories[i].IsPerfectCompleted())
                    ch.SetActiveStar(true);
                else if (categories[i].IsCompleted())
                    ch.SetActiveTick(true);
                else
                {
                    ch.SetActiveStar(false);
                    ch.SetActiveTick(false);
                }

                GameObject packageGroup = Instantiate(packagesGroupPrefab);
                packageGroup.transform.parent = category.transform;

                for (int j = 0; j < categories[i].GetPackages().Length; j++)
                {
                    Logic.Package package = categories[i].GetPackages()[j];
                    GameObject packageObject = Instantiate(packagePrefab);
                    packageObject.transform.parent = packageGroup.transform;
                    PackageButton pb = packageObject.GetComponent<PackageButton>();

                    pb.packName.text = "<color=#" + ColorUtility.ToHtmlStringRGBA(categories[i].GetColor()) + ">" + package.GetPackName() + "</color>";
                    pb.completedLevels.text = package.GetNumCompletedLevels() + " / " + package.GetTotalNumLevels();

                    pb.SetCategory(i);
                    pb.SetPackage(j);

                    if (package.IsPerfectCompleted())
                        pb.SetActiveStar(true);
                    else if (package.IsCompleted())
                        pb.SetActiveTick(true);
                    else
                    {
                        pb.SetActiveStar(false);
                        pb.SetActiveTick(false);
                    }
                }
            }
        }

        private void Update()
        {
            if (boardManager)
                UpdateUI();
        }

        /// <summary>
        /// Cambia cada letra a un color del texto pasado por parametro
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string ChangeColorTitle(string text)
        {
            string textAux = "";
            int c = 0;
            Color[] colors = GameManager.Instance().currentSkin.colores;

            for (int i = 0; i < text.Length; i++)
            {
                textAux += "<color=#" + ColorUtility.ToHtmlStringRGBA(colors[c]) + ">" + text[i] + "</color>";
                c = (c + 1) % 5;
            }

            return textAux;
        }

        /// <summary>
        /// Devuelve el tamaño en unidades de unity de la zona central, donde esta el tablero de juego
        /// </summary>
        /// <returns></returns>
        public float GetCenterUnitySize()
        {
            return (GetCenterPixelSize() * Camera.main.orthographicSize * 2) / Screen.height;
        }

        /// <summary>
        /// Devuelve la el tamaño en unidades de unity de la banda superior en la escena de juego
        /// </summary>
        /// <returns></returns>
        public float GetTopUnitySize()
        {
            return (GetTopPixelSize() * Camera.main.orthographicSize * 2) / Screen.height;
        }

        /// <summary>
        /// Devuelve la el tamaño en unidades de unity de la banda superior en la escena de juego
        /// </summary>
        /// <returns></returns>
        public float GetBottomUnitySize()
        {
            return (GetBottomPixelSize() * Camera.main.orthographicSize * 2) / Screen.height;
        }

        /// <summary>
        /// Devuelve el tamaño en pixeles de la zona central, donde esta el tablero de juego
        /// </summary>
        /// <returns></returns>
        public float GetCenterPixelSize()
        {
            float top = GetTopPixelSize();
            float bottom = GetBottomPixelSize();

            return Screen.height - top - bottom;
        }

        /// <summary>
        /// Comprueba si el nivel pasado por parametro está pasado y activa las imagenes correspondientes
        /// </summary>
        /// <param name="l"></param>
        private void CheckLevelPassed(Logic.Level l)
        {
            if (l.IsCompleted())
            {
                if (l.IsPerfectCompleted())
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

        /// <summary>
        /// Devuelve la el tamaño en pixeles de la banda superior en la escena de juego
        /// </summary>
        /// <returns></returns>
        private float GetTopPixelSize()
        {
            Vector2 refResolution = canvasScaler.referenceResolution;

            // calculamos lo que mide en pixeles la zona de arriba de HUD
            float topHeightPixelSize = (Screen.width * (upperPanel.sizeDelta.y + statsPanel.sizeDelta.y)) / refResolution.x;

            // restamos el resultado anterior a la resolucion total para saber cuanto espacio nos queda
            return topHeightPixelSize;
        }

        /// <summary>
        /// Devuelve la el tamaño en pixeles de la banda inferior en la escena de juego
        /// </summary>
        /// <returns></returns>
        private float GetBottomPixelSize()
        {
            Vector2 refResolution = canvasScaler.referenceResolution;

            // calculamos lo que mide en pixeles la zona de arriba de HUD
            float topHeightPixelSize = (Screen.width * lowerPanel.sizeDelta.y) / refResolution.x;

            // restamos el resultado anterior a la resolucion total para saber cuanto espacio nos queda
            return topHeightPixelSize;
        }

        /// <summary>
        /// Actualiza la ui del juego
        /// </summary>
        private void UpdateUI()
        {
            movementsText.text = "pasos: " + boardManager.GetNumMovements();
            flowsText.text = "flujos: " + boardManager.GetNumFlowsEnded() + "/" + boardManager.GetNumFlows();
            hintText.text = _hints + " x";
            pipePercentageText.text = "tuberia " + boardManager.GetPercentage() + "%";
        }

        /// <summary>
        /// Inicializa la ui con los datos del nivel pasado por parametro
        /// </summary>
        /// <param name="level"></param>
        private void SetUIData(Logic.Level level)
        {
            levelText.text = "Nivel " + level.GetNumLevel();
            levelText.color = level.GetLevelColor();
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

            AdsManager.Instance().DisplayVideoAd();

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