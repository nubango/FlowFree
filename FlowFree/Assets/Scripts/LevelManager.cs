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

        void Start()
        {
            boardManager.SetMap(GameManager.Instance().GetDebugLevel());
        }

        private void UpdateUI(Logic.Level level)
        {
            levelText.text = "Nivel " + level.getNumLevel();
            dimensionText.text = level.getAlto() + "x" + level.getAncho();
        }

        /// <summary>
        /// Metodo se llama para cambiar al siguiente nivel (lo llama el boardmanager)
        /// </summary>
        public void NextLevel()
        {
            GameManager.Instance().NextLevel();
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
            Logic.Level logicLevel = GameManager.Instance().AssignCurrentLevel(category, package, level);

            UpdateUI(logicLevel);
            boardManager.SetMap(logicLevel);
        }
    }
}