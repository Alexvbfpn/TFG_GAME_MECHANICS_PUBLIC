using DG.Tweening;
using GameMechanics.EntitiesSystem;
using Items.Weapons;
using Items.Weapons.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CrosshairManager : MonoBehaviour
    {
        public Image crosshairImage;
        public Sprite nullCrosshairSprite;
        public float crosshairUpdateSharpness = 5f;
        
        protected PlayerWeaponsManager m_weaponsManager;
        protected bool m_wasPointingAtEnemy;
        
        protected RectTransform m_crosshairRectTransform;
        protected CrosshairData m_crosshairDataDefault;
        protected CrosshairData m_crosshairDataTarget;
        protected CrosshairData m_currentCrosshair;

        protected float reloadTime;
        
        protected WeaponController m_lastWeapon;

        protected void OnWeaponChanged(WeaponController newWeapon)
        {
            if (newWeapon != null)
            {
                crosshairImage.enabled = true;
                m_crosshairDataDefault = newWeapon.crosshairDataDefault;
                m_crosshairDataTarget = newWeapon.crosshairDataTargetInSight;
                m_crosshairRectTransform = crosshairImage.GetComponent<RectTransform>();
                reloadTime = newWeapon.delayBetweenShots;
                m_lastWeapon = newWeapon;
                m_lastWeapon.OnShoot += ReloadCrosshairAnimation;
            }
            else
            {
                Debug.Log("No weapon found");
                if(nullCrosshairSprite)
                    crosshairImage.sprite = nullCrosshairSprite;
                else
                    crosshairImage.enabled = false;
                if (m_lastWeapon != null)
                {
                    m_lastWeapon.OnShoot -= ReloadCrosshairAnimation;
                    m_lastWeapon = null;
                }
            }
            
            UpdateCrosshairPointingAtEnemy(true);
        }
        
        protected void UpdateCrosshairPointingAtEnemy(bool force)
        {
            if (m_crosshairDataDefault.CrosshairSprite == null)
                return;

            if ((force || !m_wasPointingAtEnemy) && m_weaponsManager.isPointingAtEnemy)
            {
                m_currentCrosshair = m_crosshairDataTarget;
                crosshairImage.sprite = m_currentCrosshair.CrosshairSprite;
                m_crosshairRectTransform.sizeDelta = m_currentCrosshair.CrosshairSize * Vector2.one;
                m_crosshairRectTransform.rotation = Quaternion.Euler(m_currentCrosshair.CrosshairRotation);
                // m_crosshairRectTransform.rotation = Quaternion.Euler(Vector3.Lerp(m_crosshairRectTransform.rotation.eulerAngles, m_currentCrosshair.CrosshairRotation, 
                //     Time.deltaTime * crosshairUpdateSharpness)); // Cuidado con esto con la mecanica de tiempo
            }
            else if ((force || m_wasPointingAtEnemy) && !m_weaponsManager.isPointingAtEnemy)
            {
                m_currentCrosshair = m_crosshairDataDefault;
                crosshairImage.sprite = m_currentCrosshair.CrosshairSprite;
                m_crosshairRectTransform.sizeDelta = m_currentCrosshair.CrosshairSize * Vector2.one;
                m_crosshairRectTransform.rotation = Quaternion.Euler(m_currentCrosshair.CrosshairRotation);
                // m_crosshairRectTransform.rotation = Quaternion.Euler(Vector3.Lerp(m_crosshairRectTransform.rotation.eulerAngles, m_currentCrosshair.CrosshairRotation, 
                //     Time.deltaTime * crosshairUpdateSharpness)); // Cuidado con esto con la mecanica de tiempo
            }
           
            crosshairImage.color = Color.Lerp(crosshairImage.color, m_currentCrosshair.CrosshairColor, 
                Time.deltaTime * crosshairUpdateSharpness); // Cuidado con esto con la mecanica de tiempo
            
            m_crosshairRectTransform.sizeDelta = Mathf.Lerp(m_crosshairRectTransform.sizeDelta.x, 
                m_currentCrosshair.CrosshairSize, Time.deltaTime * crosshairUpdateSharpness) * Vector2.one; // Cuidado con esto con la mecanica de tiempo
            
            // m_crosshairRectTransform.rotation = Quaternion.Euler(Vector3.Lerp(m_crosshairRectTransform.rotation.eulerAngles, m_currentCrosshair.CrosshairRotation, 
            //     Time.deltaTime * crosshairUpdateSharpness)); // Cuidado con esto con la mecanica de tiempo
        }

        protected void ReloadCrosshairAnimation()
        {
            crosshairImage.transform.DORotate(new Vector3(0, 0, 90), reloadTime, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
                .OnComplete(() => crosshairImage.transform.DOPunchScale(Vector3.one, 0.2f, 10, 1).SetUpdate(true));
        }
        
        protected void Start()
        {
            m_weaponsManager = FindObjectOfType<PlayerWeaponsManager>();
            
            OnWeaponChanged(m_weaponsManager.GetActiveWeapon());
            
            m_weaponsManager.OnSwitchedToWeapon += OnWeaponChanged;
        }

        protected void Update()
        {
            //UpdateCrosshairPointingAtEnemy(false);
            m_wasPointingAtEnemy = m_weaponsManager.isPointingAtEnemy;
        }
    }
}