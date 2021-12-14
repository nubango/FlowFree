using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Flow
{
    /*
     * Script para llamar a los metodos referidos a los botones de la interfaz
     * Llama a métodos de los managers Singleton (AdsManager y GameManager)
     */
    public class Callbacks : MonoBehaviour
    {
        public void OnClickedPlay(GameObject go)
        {
            // El color del texto del boton jugar cambia cada entre los 5 primeros del paquete de color
            ColorBlock color = go.GetComponent<Button>().colors;
            // #FF0000, #008D00, #0C29FE, #EAE000, #FB8900
            Color[] colors = { new Color32(255, 0, 0, 255), new Color32(0, 141, 0, 255), new Color32(12, 41, 254, 255), new Color32(234, 224, 0, 255), new Color32(251, 137, 0, 255) };
            int rnd = Random.Range(0, colors.Length);
            color.pressedColor = colors[rnd];
            go.GetComponent<Button>().colors = color;

            //GameManager.Instance().LoadCategoryScene();
            GameManager.Instance().LoadGameScene();
        }

        public void OnClickedOptions()
        {
            Debug.Log("OPCIONES!");
        }

        public void OnClickedRewardedVideo()
        {
            //AdsManager.Instance().ShowRewardedAd();
        }

        public void OnClickedLevelPackage(int levelPackage)
        {
            //GameManager.Instance().LoadPackage();
        }

        public void OnClickedBack()
        {
            //GameManager.Instance().LoadScene();
        }

        public void OnClickNextLevel()
        {
            GameManager.Instance().levelManager.NextLevel();
        }

        public void OnClickPreviousLevel()
        {
            GameManager.Instance().levelManager.PreviousLevel();
        }

        public void OnClickDisableWinMenu()
        {
            GameManager.Instance().DisableWinMenu();
        }
        public void OnClickTakeHint()
        {
            GameManager.Instance().TakeHint();
        }
    }

}