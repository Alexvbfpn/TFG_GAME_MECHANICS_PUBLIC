using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI.CharacterSelector
{
    public class AutomaticSlotSelector : MonoBehaviour
    {
        public void InitializeSlotCharacter(CellData elementData)
        {
            ElementSelectionManager.instance.ShowElementInSlot(transform.GetSiblingIndex(), elementData);
        }

        protected void Start()
        {
            //InitializeSlotCharacter();
        }
    }
}