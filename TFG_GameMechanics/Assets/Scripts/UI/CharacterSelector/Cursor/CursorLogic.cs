using DG.Tweening;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Events;

namespace UI.CharacterSelector
{
    [RequireComponent(typeof(CursorInputManager))]
    public class CursorLogic : MonoBehaviour
    {
        public float cursorSpeed = 10;

        public bool hasToken;
        
        public UnityEvent<int, CellData> onConfirm;
        public UnityEvent onCancel;
        public UnityEvent onInteract;
        
        public CellLogic currentCell { get; protected set; }
        
        public Token cursorToken { get; protected set; }

        /// <summary>
        /// Returns the Cursor Input Manager instance.
        /// </summary>
        public virtual CursorInputManager inputs { get; protected set; }
        
        public RectTransform rectTransform { get; protected set; }
        
        public Canvas canvas { get; protected set; }
        
        protected virtual void InitializeInputs() => inputs = GetComponent<CursorInputManager>();
        protected virtual void InitializeRectTransform() => rectTransform = GetComponent<RectTransform>();
        protected virtual void InitializeCanvas() => canvas = GetComponentInParent<Canvas>();

        public void SetCurrentCell(CellLogic cell)
        {
            
            if (cell != null)
            {
                cell.StartBorderTween();
            }
            
            currentCell = cell;
            
            //Show character in slot logic
            ElementSelectionManager.instance.ShowElementInSlot(0, cell == null ? null : cell.cellData);
        }
        
        public void SetCurrentToken(Token token)
        {
            cursorToken = token;
        }
        
        public void GrabToken()
        {
            if (cursorToken == null) return;
            //cursorToken.SetGrabbed(true);
            transform.DOMove(cursorToken.transform.position, 0.2f).OnComplete(() =>
            {
                cursorToken.SetGrabbed(true);
                hasToken = true;
            });
            
            //transform.position = cursorToken.transform.position;
        }
        
        public void ReleaseCurrentToken()
        {
            cursorToken.SetGrabbed(false);
            hasToken = false;
        }

        #region --- MOVEMENT ---
        
        protected void Movement()
        {
            var inputDirection = inputs.GetMovementDirection();

            rectTransform.anchoredPosition += new Vector2(inputDirection.x, inputDirection.z) * (Time.deltaTime * cursorSpeed);
        }

        /// <summary>
        /// We control that cursor doesnt go out of screen.
        /// </summary>
        protected void ClampMovementLimits()
        {
            // Obtenemos el tamaño del canvas
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            // Obtenemos el tamaño del elemento
            Vector2 elementSize = rectTransform.sizeDelta;

            // Calculamos los límites
            float minX = (elementSize.x / 2) - (canvasRect.rect.width / 2);
            float maxX = (canvasRect.rect.width / 2) - (elementSize.x / 2);
            float minY = (elementSize.y / 2) - (canvasRect.rect.height / 2);
            float maxY = (canvasRect.rect.height / 2) - (elementSize.y / 2);

            // Clampeamos la posición
            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp(rectTransform.anchoredPosition.x, minX, maxX),
                Mathf.Clamp(rectTransform.anchoredPosition.y, minY, maxY)
            );

            // Aplicamos la posición clamped
            rectTransform.anchoredPosition = clampedPosition;
        }
        
        #endregion

        public void Interact()
        {
            if (inputs.GetInteractDown())
            {
                if (hasToken)
                {
                    ReleaseCurrentToken();
                    onConfirm?.Invoke(0, currentCell.cellData);
                }
                else
                {
                    GrabToken();
                    onInteract?.Invoke();
                    onCancel?.Invoke();
                }   
            }
        }
        
        public void Cancel()
        {
            if (inputs.GetCancelDown() && !hasToken)
            {
                GrabToken();
                onCancel?.Invoke();
            }

        }
        
        protected void Awake()
        {
            InitializeInputs();
            InitializeCanvas();
            InitializeRectTransform();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            //TEMPORAL
            SetCurrentToken(FindObjectOfType<Token>());
        }
        
        protected void Update()
        {
            Movement();
            ClampMovementLimits();
            Cancel();
        }
    }
}