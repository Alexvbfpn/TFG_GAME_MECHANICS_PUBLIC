using System;
using System.Collections.Generic;
using Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.CharacterSelector
{
    public class CursorDetection : MonoBehaviour
    {
        public Transform detectionOrigin;
        
        protected CursorLogic m_cursorLogic;
        protected GraphicRaycaster m_raycaster;
        protected PointerEventData m_pointerEventData = new PointerEventData(null);
        
        
        protected void InitializeCursorLogic() => m_cursorLogic = GetComponent<CursorLogic>();
        protected void InitializeRaycaster() => m_raycaster = GetComponentInParent<GraphicRaycaster>();

        protected void SelectableElementCast()
        {
            m_pointerEventData.position = detectionOrigin.position;
            var results = new List<RaycastResult>();
            m_raycaster.Raycast(m_pointerEventData, results);
            if (results.Count > 0)
            {
                if (results[0].gameObject.TryGetComponent(out CellLogic cell))
                {
                    //SI ALGO FALLA O SE COMPORTA DE MANERA RARA REVISAR ESTE IF
                    if (cell != m_cursorLogic.currentCell)
                    {
                        if(m_cursorLogic.currentCell != null)
                            m_cursorLogic.currentCell.StopBorderTween();
                        m_cursorLogic.SetCurrentCell(cell);
                    }
                    m_cursorLogic.Interact();
                    
                }
            }
            else
            {
                if (m_cursorLogic.currentCell != null)
                {
                    m_cursorLogic.currentCell.StopBorderTween();
                    m_cursorLogic.SetCurrentCell(null);
                }
                    
            }
            
        }

        protected void InteractionCast()
        {
            m_pointerEventData.position = detectionOrigin.position;
            var results = new List<RaycastResult>();
            m_raycaster.Raycast(m_pointerEventData, results);
            if (results.Count > 0)
            {
                if (results[0].gameObject.TryGetComponent(out Token token))
                {
                    m_cursorLogic.Interact();
                    print("Token:" + results[0].gameObject.name);
                    // if (m_cursorLogic.hasToken)
                    // {
                    //     m_cursorLogic.ReleaseCurrentToken();
                    // }
                    // m_cursorLogic.SetCurrentToken(token);
                    // m_cursorLogic.GrabToken();
                }
            }
        }

        protected void Awake()
        {
            InitializeCursorLogic();
            InitializeRaycaster();
        }

        protected void Update()
        {
            if (!m_cursorLogic.hasToken)
            {
                InteractionCast();
            }
            else
            {
                SelectableElementCast();
            }
            
        }
    }
}