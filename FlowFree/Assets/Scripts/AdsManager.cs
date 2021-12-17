using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Flow
{
    public class AdsManager : MonoBehaviour, IUnityAdsListener
    {
        // Dashboard
#if UNITY_IOS
    private string gameId = "4173828";
    //private string gameId = "4455936";

#elif UNITY_ANDROID
        private string gameId = "4173829";
        //private string gameId = "4455937";
#endif
        [Tooltip("Atajo para deshabilitar anuncios")]
        public bool disableAds;

        public bool testMode = true;

        public Button hintButton;

        private string bannerId = "bannerPlacement";
        private string rewardedId = "rewardedVideo";
        private string videoId = "video";

        private void Awake()
        {
            if (_instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            // Initializes the ads service, with a specified Game ID and test mode status
            if (!disableAds)
            {
                Advertisement.Initialize(gameId, testMode);
                StartCoroutine(ShowBannerWhenReady());
                Advertisement.AddListener(this);
            }
        }

        public void DisplayVideoAd()
        {
            Advertisement.Show(videoId);
        }

        IEnumerator ShowBannerWhenReady()
        {
            while (!Advertisement.IsReady(bannerId))
            {
                yield return new WaitForSeconds(0.5f);
            }
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(bannerId);
        }

        public void ShowRewardedVideo()
        {
            // Check if UnityAds ready to be shown for the specified Placement
            if (Advertisement.IsReady(rewardedId))
            {
                Advertisement.Banner.Hide();

                // Shows content in the specified Placement
                Advertisement.Show(rewardedId);
            }
            else
            {
                Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
            }
        }

        // IUnityAdsListener interface methods

        public void OnUnityAdsReady(string placementId)
        {
            // If the ready Placement is rewarded, activate the button: 
            if (placementId == rewardedId)
            {
                hintButton.interactable = true;
            }
        }

        /*
         * Metodo que se llama cuando el anuncio acaba, de salta o acaba por error
         * Si venimos de un anuncio remunerado, sumamos una pista
         * **/
        public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult)
        {
            // Define conditional logic for each ad completion status:
            if (showResult == ShowResult.Finished)
            {
                // Reward the user for watching the ad to completion.
                Debug.Log("Video completed - Offer a reward to the player");

                if (surfacingId == rewardedId)
                {
                    GameManager.Instance().AddHint();
                    // guardar el progreso
                }
            }
            else if (showResult == ShowResult.Skipped)
            {
                // Do not reward the user for skipping the ad.
                Debug.LogWarning("Video was skipped - Do NOT reward the player");
            }
            else if (showResult == ShowResult.Failed)
            {
                Debug.LogWarning("The ad did not finish due to an error.");
            }
        }

        public void OnUnityAdsDidError(string message)
        {
            // Log the error.
        }

        public void OnUnityAdsDidStart(string placementId)
        {
            // Optional actions to take when the end-users triggers an ad.
        }

        /*
         * persistente
         * **/

        static AdsManager _instance;
        public static AdsManager Instance()
        {
            return _instance;
        }
    }
}