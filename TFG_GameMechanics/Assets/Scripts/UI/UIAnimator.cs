using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    [AddComponentMenu("Mechanics/UI/UI Animator")]
    public class UIAnimator : MonoBehaviour
    {
        /// <summary>
        /// Called when the Show action is invoked.
        /// </summary>
        public UnityEvent onShow;

        /// <summary>
        /// Called when the Hide action is invoked.
        /// </summary>
        public UnityEvent onHide;

        public bool hiddenOnAwake;
        public float hideShowDuration = 0.0f;
        
        protected CanvasGroup m_canvasGroup;
        
        protected void InitializeCanvasGroup() => m_canvasGroup = GetComponent<CanvasGroup>();
        
        public virtual void Show()
        {
            //m_canvasGroup.DOFade(1, hideShowDuration);
            m_canvasGroup.alpha = 1;
            //Debug.Log("Showing UI");
            onShow?.Invoke();
        }
        
        public virtual void Hide()
        {
            //m_canvasGroup.DOFade(0, hideShowDuration);
            m_canvasGroup.alpha = 0;
            onHide?.Invoke();
        }
        
        /// <summary>
        /// Calls the Game Object Set Active passing a given value.
        /// </summary>
        /// <param name="value">The value you want to pass.</param>
        public virtual void SetActive(bool value) => gameObject.SetActive(value);
        
        protected virtual void Awake()
        {
            InitializeCanvasGroup();
            
            if (hiddenOnAwake)
            {
                m_canvasGroup.alpha = 0;
            }
        }
    }
}