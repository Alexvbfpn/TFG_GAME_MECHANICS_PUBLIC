using System.Collections;
using Misc;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace GameMechanics
{
    [AddComponentMenu("Mechanics/Game/Game Scene Loader")]
    public class GameSceneLoader : Singleton<GameSceneLoader>
    {
        /// <summary>
        /// Called when any loading process starts.
        /// </summary>
        public UnityEvent onLoadingStart;
        
        /// <summary>
        /// Called when any loading process finishes.
        /// </summary>
        public UnityEvent onLoadingFinish;
        
        public UIAnimator loadingScreen;
        
        [Header("Minimum Time")]
        public float startDelay = 1f;
        public float finishDelay = 1f;
        
        /// <summary>
        /// Returns true if there's any loading in process.
        /// </summary>
        public bool isLoading { get; protected set; }
        
        /// <summary>
        /// Returns the loading percentage.
        /// </summary>
        public float loadingProgress { get; protected set; }
        
        
        /// <summary>
        /// Returns the name of the current scene.
        /// </summary>
        public string currentScene => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        
        /// <summary>
        /// Loads a given scene based on its name.
        /// </summary>
        /// <param name="scene">Scene to we want to switch</param>
        public virtual void Load(string scene)
        {
            if (!isLoading && (currentScene != scene))
            {
                StartCoroutine(LoadRoutine(scene));
            }
        }
        
        /// <summary>
        /// Reloads the current scene.
        /// </summary>
        public virtual void Reload()
        {
            StartCoroutine(LoadRoutine(currentScene));
        }

        protected virtual IEnumerator LoadRoutine(string scene)
        {
            onLoadingStart?.Invoke();
            isLoading = true;
            loadingScreen.SetActive(true);
            loadingScreen.Show();
            
            yield return new WaitForSeconds(startDelay);
            
            var asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scene);
            //asyncOperation.allowSceneActivation = false;
            loadingProgress = 0;

            while (!asyncOperation.isDone)
            {
                loadingProgress = asyncOperation.progress;
                yield return null;
            }
            loadingProgress = 1;
            
            yield return new WaitForSeconds(finishDelay);
            
            isLoading = false;
            loadingScreen.Hide();
            onLoadingFinish?.Invoke();
        }
        
    }
}