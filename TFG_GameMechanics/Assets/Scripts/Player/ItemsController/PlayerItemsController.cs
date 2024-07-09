using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    public abstract class PlayerItemsController<T> : MonoBehaviour where T : ItemController
    {
        public List<T> items = new List<T>();

        public bool needAimToShoot = true;
        public bool isAiming { get; protected set; }
        public int activeItemIndex { get; protected set; }

        public virtual ItemController GetActiveItem() => items[activeItemIndex] as ItemController;

        protected Player m_player;
        
        protected virtual void InitializePlayer() => m_player = FindObjectOfType<Player>();
        
        protected void SetupItems()
        {
            activeItemIndex = 0;
            
            if (items.Count > 0)
            {
                HideAllItems();
                GetActiveItem().ChangeActiveState(true);
                GetActiveItem().MoveToDefaultPosition();
            }
        }
        
        public void ChangeAimingState(bool state) => isAiming = state;
        
        protected virtual void HideAllItems()
        {
            foreach (var item in items)
            {
                item.ItemInitializations();
                item.ChangeActiveState(false);
            }
        }
        
        public virtual IEnumerator SwitchActiveItemCoroutine()
        {
            items[activeItemIndex + 1 >= items.Count ? 0 : activeItemIndex + 1].ChangeActiveState(true);
            yield return new WaitUntil(() => !GetActiveItem().isUsing);
            SwitchActiveItem();
        }
        
        public virtual void SwitchActiveItem()
        {
            if(!isAiming ) return;//|| !GetActiveItem().TryUse()) return;

            ChangeAimingState(false);
            //isAiming = false;
            
            //ChangeParentSocketPosition(inactiveItemPosition);
            GetActiveItem().ChangeActiveState(false);
            
            // If we reach the end of the list, we start from the beginning, circular path
            int nextIndex = activeItemIndex + 1 >= items.Count ? 0 : activeItemIndex + 1;
            activeItemIndex = nextIndex;
            
            GetActiveItem().ChangeActiveState(true);
        }

        protected void AimLogic(bool state)
        {
            if (state)
            {
                StartAimItem();
            }
            else
            {
                StopAimItem();
            }
        }
        
        protected void StartAimItem()
        {
            Debug.Log("Cuando se llama a StartAimItem() aiming es " + isAiming);
            if(!isAiming)
            {
                Debug.Log("StartAimItem() called");
                ChangeAimingState(!isAiming);
                GetActiveItem().MoveToAimingPosition();
            }
        }
        
        protected void StopAimItem()
        {
            if (!GetActiveItem().isUsing)
            {
                ChangeAimingState(false);
                GetActiveItem().MoveToDefaultPosition();
            }
        }
        
        protected void ShootCurrentItem()
        {
            if(needAimToShoot && !isAiming) return;
            
            if (GetActiveItem().TryUse())
            {
                ShootLogic();
            }
        }
        
        protected abstract void ShootLogic();
        
        protected virtual void Awake()
        {
            InitializePlayer();
            // m_player.events.onAim.AddListener((x) => AimLogic(x));
            // //m_player.events.onStopAim.AddListener(() => ChangeAimingState(false));
            // m_player.events.onShoot.AddListener(() => ShootCurrentItem());
        }

        protected virtual void Start()
        {
            SetupItems();
        }
    }
}