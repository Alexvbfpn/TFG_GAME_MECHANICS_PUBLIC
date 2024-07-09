using Items;

namespace GameMechanics.EntitiesSystem
{
    public class CameraItemsController : PlayerItemsController<ItemController>
    {
        protected override void ShootLogic()
        {
            StopAllCoroutines();
            StartCoroutine(SwitchActiveItemCoroutine());
        }

        protected override void Awake()
        {
            base.Awake();
            m_player.events.onAim.AddListener((x) => AimLogic(x));
            //m_player.events.onStopAim.AddListener(() => ChangeAimingState(false));
            m_player.events.onShoot.AddListener(() => ShootCurrentItem());
        }
    }
}