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

        [Tooltip("Menu que sale al pasarte el nivel")]
        public GameObject endLevelMenu;



        void Start()
        {
            DisableWinMenu();
            boardManager.SetMap(GameManager.Instance().GetDebugLevel());
        }

        private void UpdateUI(Logic.Level level)
        {
            levelText.text = "Nivel " + level.getNumLevel();
            dimensionText.text = level.getAlto() + "x" + level.getAncho();
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

            UpdateUI(level);
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

            UpdateUI(level);
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

            UpdateUI(logicLevel);
            boardManager.SetMap(logicLevel);
        }

        /// <summary>
        /// Metodo activa el menu de nivel ganado
        /// </summary>
        public void Win()
        {
            endLevelMenu.SetActive(true);
        }

        /// <summary>
        /// Deshabilita el menu que sale al ganar un nivel
        /// </summary>
        public void DisableWinMenu()
        {
            boardManager.SetActiveUpdate(true);
            endLevelMenu.SetActive(false);
        }
    }
}