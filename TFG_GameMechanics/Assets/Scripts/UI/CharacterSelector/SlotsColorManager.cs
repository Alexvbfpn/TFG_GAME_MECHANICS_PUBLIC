using System;
using System.Collections.Generic;
using System.Linq;
using UI.CharacterSelector.ClassHelpers;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.CharacterSelector
{
    public class SlotsColorManager : MonoBehaviour
    {
        public Color[] slotColors;

        protected List<CellData> randomIndexes;
        
        protected void InitializeSlotColorComponents()
        {
            //GetComponent<Image>().color = slotColors[transform.GetSiblingIndex()];

            foreach (SlotManager s in transform.GetComponentsInChildren<SlotManager>())
            {
                s.SetSlotColor(slotColors[s.transform.GetSiblingIndex()]);
                if (s.TryGetComponent(out AutomaticSlotSelector a))
                {
                    CellData random = randomIndexes[Random.Range(0, randomIndexes.Count - 1)];
                    randomIndexes.Remove(random);
                    a.InitializeSlotCharacter(random);
                }
                else
                {
                    ElementSelectionManager.instance.ShowElementInSlot(0, null);
                }
            }
        }

        protected void InitializeRandomIndexes()
        {
            randomIndexes = FindObjectOfType<GridElementsManager>().elements.ToList();
           
        }

        protected void Awake()
        {
            InitializeRandomIndexes();
        }

        protected void Start()
        {
            InitializeSlotColorComponents();
        }
    }
}