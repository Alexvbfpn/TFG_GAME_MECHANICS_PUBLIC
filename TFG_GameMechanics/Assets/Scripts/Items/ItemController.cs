using System.Collections;
using DG.Tweening;
using GameMechanics;
using Tools;
using UnityEngine;

namespace Items
{
    // This class is intended for first-person game items.
    [System.Serializable]
    public abstract class ItemController : MonoBehaviour, IItem
    {
        [Header("Internal References")]
        [Tooltip("Translation to apply to weapon arm when aiming with this item/weapon")]
        public Vector3 aimOffset;

        public GameObject skin;
        
        public SerializableDictionary<ItemPositionType, ItemPositionInfo> itemPositions = new SerializableDictionary<ItemPositionType, ItemPositionInfo>()
            {
                {ItemPositionType.Default, null},
                {ItemPositionType.Aim, null},
                {ItemPositionType.Inactive, null},
                {ItemPositionType.Special, null}
            };
        
        [Header("Item Events")]
        public ItemEvents itemEvents;
        
        public GameObject owner { get; protected set; }
        public GameObject sourcePrefab { get; protected set; }
        
        protected Tween _currentRotationTween;
        protected Tween _currentScaleTween;
        protected Tween _currentMovementTween;
        
        protected string _defaultPositionLayer = GameLayers.FirstViewer;
        protected string _inactivePositionLayer = GameLayers.Interactable;
        
        public bool isActive { get; protected set; }
        
        public bool isUsing { get; protected set; }

        public virtual bool IsReadyToUse() => isActive && !isUsing;
        
        protected virtual void OnUse() => print("Using");

        public virtual void ChangeActiveState(bool state)
        {
            if (state)
                Show();
            else
                Hide();
            isActive = state;
        }
        
        public virtual bool TryUse()
        {
            //Debug.Log("TryUse" +IsReadyToUse());
            if (!IsReadyToUse()) return false;
            
            isUsing = true;
            OnUse();
            return true;
        }
        
        public virtual void Show()
        {
            //gameObject.SetActive(true);
            //this.enabled = true;
            skin.SetActive(true);
        }

        public void Hide()
        {
            skin.SetActive(false);
            //gameObject.SetActive(false);
            isUsing = false;
            //this.enabled = false;
        }

        protected abstract IEnumerator SpecialAction();

        public abstract void ItemInitializations();
        
        protected Tween MoveItem(ItemPositionInfo info)
        {
            //DOTween.KillAll(false);
            if (_currentRotationTween != null)
            {
                _currentRotationTween.Kill(false);
                _currentScaleTween.Kill(false);
                _currentMovementTween.Kill(false);
            }

            _currentRotationTween = transform.DOLocalRotate(info.transform.localRotation.eulerAngles, info.movementDuration).SetEase(info.movementEase);
            _currentScaleTween = transform.DOScale(info.transform.localScale, info.movementDuration).SetEase(info.movementEase);
            _currentMovementTween = transform.DOLocalMove(info.transform.localPosition, info.movementDuration).SetEase(info.movementEase)
                .OnComplete
                (() => transform.localPosition = info.transform.localPosition

                );
            return _currentMovementTween;
        }

        #region --- DEBUG MOVEMENT SECTION ---
        [ContextMenu("Move to Default Position")]
        public virtual void MoveToDefaultPosition()
        {
            itemEvents.onMoveDefaultPosition?.Invoke();
            
            GameLayers.SetLayerToGameObject(skin, _defaultPositionLayer);
            MoveItem(itemPositions.GetValueByKey(ItemPositionType.Default));
        }
        [ContextMenu("Move to Aiming Position")]
        public virtual void MoveToAimingPosition()
        {
            itemEvents.onMoveAimPosition?.Invoke();
            
            MoveItem(itemPositions.GetValueByKey(ItemPositionType.Aim));
        }
        [ContextMenu("Move to Inactive Position")]
        public virtual void MoveToInactivePosition()
        {
            itemEvents.onMoveInactivePosition?.Invoke();
            
            GameLayers.SetLayerToGameObject(skin, _inactivePositionLayer);
            MoveItem(itemPositions.GetValueByKey(ItemPositionType.Inactive)).OnComplete(() => isUsing = false);
            
        }
        [ContextMenu("Move to Special Position")]
        public virtual void MoveToSpecialPosition()
        {
            itemEvents.onMoveSpecialPosition?.Invoke();
            
            MoveItem(itemPositions.GetValueByKey(ItemPositionType.Special));
        }
        
        #endregion
    }
}