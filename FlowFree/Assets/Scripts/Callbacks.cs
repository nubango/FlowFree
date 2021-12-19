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
        public void OnClickedOptions()
        {
        }

        public void OnClickedRewardedVideo()
        {
            AdsManager.Instance().ShowRewardedVideo();
        }

        public void OnClickedLevelPackage(PackageButton pb)
        {
            GameManager.Instance().LoadPackage(pb.GetCategory(), pb.GetPackage());
        }

        public void OnClickedLevel(LevelButton level)
        {
            if (!level.locked.enabled)
                GameManager.Instance().LoadLevel(level.GetId());
        }

        public void OnClickedGoToLevelsScene()
        {
            GameManager.Instance().LoadLevelsScene();
        }
        public void OnClickedGoToCategoriesScene()
        {
            GameManager.Instance().LoadCategoryScene();
        }
        public void OnClickedGoToIntroScene()
        {
            GameManager.Instance().LoadIntroScene();
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