using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
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

        [Tooltip("Menu que sale al pasarte el nivel")]
        public GameObject endLevelMenu;

        private int _hints;

        private void Start()
        {
            DisableWinMenu();
            boardManager.SetMap(GameManager.Instance().GetDebugLevel());
            SetUIData(GameManager.Instance().GetDebugLevel());
        }

        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            movementsText.text = "pasos: " + boardManager.GetNumMovements();
            flowsText.text = "flujos: " + boardManager.GetNumFlowsEnded() + "/" + boardManager.GetNumFlows();
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
        }

        /// <summary>
        /// Esteblece el nivel del paquete y la categoria pasadas por parametro
        /// </summary>
        /// <param name="category"></param>
        /// <param name="package"></param>
        /// <param name="level"></param>
        public void SetLevel(int category, int package, int level)
        {
            DisableWinMenu();
            Logic.Level logicLevel = GameManager.Instance().AssignCurrentLevel(category, package, level);

            SetUIData(logicLevel);
            boardManager.SetMap(logicLevel);
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


            endLevelMenu.SetActive(true);
            descriptionText.text = "You complete the level in " + boardManager.GetNumMovements() + " moves";
        }

        /// <summary>
        /// Deshabilita el menu que sale al ganar un nivel
        /// </summary>
        public void DisableWinMenu()
        {
            boardManager.SetActiveUpdate(true);
            endLevelMenu.SetActive(false);
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
    }
}