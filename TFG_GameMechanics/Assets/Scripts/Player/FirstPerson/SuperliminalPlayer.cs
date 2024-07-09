using Misc.PickableObjects;
using UnityEngine;

namespace GameMechanics.EntitiesSystem
{
    public class SuperliminalPlayer : FirstPersonPlayer
    {
        protected float _originalDistance;
        protected float _originalScale;
        protected Vector3 _targetScale;
        protected Quaternion _originalRotation;

        protected override void PickUpLogic()
        {
            if (MHit.transform.TryGetComponent(out APickable pickable))
            {
                SetPickableOriginalParameters(pickable);
                PickUp(pickable);
            }
        }

        protected void SetPickableOriginalParameters(APickable pickable)
        {
            _originalDistance = Vector3.Distance(mainCamera.position, MHit.transform.position);
            _originalScale = pickable.transform.localScale.x;
            _targetScale = pickable.transform.localScale;
            _originalRotation = Quaternion.Inverse(mainCamera.rotation) * pickable.transform.rotation;
        }
        
        public virtual void ResizePickable()
        {
            if (!holding) return;

            HitDetect =
                Physics.Raycast
                (
                    mainCamera.position,
                    mainCamera.transform.TransformDirection(Vector3.forward),
                    out MHit,
                    Mathf.Infinity,
                    ignoreLayer
                );
            //REMOVE AND REPLACE WITH THE PARENT CHANGING
            pickable.pickableGameObject.transform.rotation = new Quaternion(pickable.pickableGameObject.transform.rotation.x, (_originalRotation * mainCamera.rotation).y, pickable.pickableGameObject.transform.rotation.z, pickable.pickableGameObject.transform.rotation.w);

            if (HitDetect)
            {
                // Debug.Log("Hit: " + MHit.transform.name);
                // Debug.DrawRay(mainCamera.position, mainCamera.transform.TransformDirection(Vector3.forward) * MHit.distance, Color.yellow);
                
                pickable.pickableGameObject.transform.position = MHit.point - mainCamera.forward * (offsetFactor * pickable.pickableGameObject.transform.localScale.z);

                float currentDistance = Vector3.Distance(mainCamera.position, pickable.pickableGameObject.transform.position);
                
                float newScale = ScaleFormula(currentDistance, _originalDistance);
                _targetScale.x = _targetScale.y = _targetScale.z = newScale;
                
                pickable.pickableGameObject.transform.localScale = _targetScale * _originalScale;
                
                while 
                (
                    Physics.OverlapBox
                    (
                        pickable.pickableGameObject.transform.position, 
                        pickable.pickableGameObject.GetComponent<Collider>().bounds.extents, 
                        pickable.pickableGameObject.transform.rotation, ignoreLayer, 
                        QueryTriggerInteraction.Ignore
                    ).Length > 0
                )
                {
                    pickable.pickableGameObject.transform.position -= mainCamera.forward * (offsetFactor * pickable.pickableGameObject.transform.localScale.z);
                    currentDistance = Vector3.Distance(mainCamera.position, pickable.pickableGameObject.transform.position);
                
                    newScale = ScaleFormula(currentDistance, _originalDistance);
                    _targetScale.x = _targetScale.y = _targetScale.z = newScale;
                
                    pickable.pickableGameObject.transform.localScale = _targetScale * _originalScale;

                }
            }
            // else
            // {
            //     Debug.DrawRay(mainCamera.position, mainCamera.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            // }
        }

        protected float ScaleFormula(float currentDistance, float originalDistance)
        {
            return currentDistance / originalDistance;
        }
        
        public override void IdleStepSpecificLogic()
        {
            base.IdleStepSpecificLogic();
            ResizePickable();
        }
        
        public override void WalkStepSpecificLogic()
        {
            base.WalkStepSpecificLogic();
            ResizePickable();
        }
    }
}