using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    public class InputManager : MonoBehaviour
    {
        [Tooltip("Texto en pantalla para el debug de la posicion, especialmente para Android")]
        public Text positionText;

        void Update()
        {
            // Boton izquierdo del raton
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                // La esquina abajo-izquierda es (0,0), la esquina arriba-derecha es (Screen.Width, Screen.Height)
                positionText.text = "Position(x,y): " + mousePos.x + " " + mousePos.y;

            }

            // Input tactil
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // La esquina abajo-izquierda es (0,0), la esquina arriba-derecha es (Screen.Width, Screen.Height)
                positionText.text += " " + touch.position.x + " " + touch.position.y;
            }
        }
    }
}