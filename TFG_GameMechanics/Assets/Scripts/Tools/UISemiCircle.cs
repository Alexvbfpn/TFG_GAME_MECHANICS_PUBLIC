using UnityEngine;

namespace Tools
{
    [ExecuteInEditMode]
    public class UISemiCircle : MonoBehaviour
    {
        public RectTransform[] uiElements; // Los elementos UI a organizar
        public Transform target; // El objeto al que deben apuntar los elementos
        public float radius = 200f; // Radio del semicirculo
        public float startAngle = 0f; // Ángulo de inicio del semicirculo en grados
        public float endAngle = 180f; // Ángulo de fin del semicirculo en grados
        public bool clockwise = true; // Dirección del semicirculo (horaria o antihoraria)
        public float spriteRotationOffset = -45f; // Offset de rotación del sprite en grados

        void Update()
        {
            ArrangeUIElements();
        }

        void ArrangeUIElements()
        {
            if (uiElements == null || uiElements.Length == 0)
                return;

            // Calcular la posición del centro de la pantalla en coordenadas del Canvas
            Vector3 canvasCenter = new Vector2(Screen.width / 2f, Screen.height / 1.75f);

            // Determinar si la dirección es en sentido horario o antihorario
            int direction = clockwise ? 1 : -1;

            // Calcular el ángulo total a lo largo del semicírculo
            float totalAngle = Mathf.Abs(endAngle - startAngle);

            for (int i = 0; i < uiElements.Length; i++)
            {
                // Calcular el ángulo para este elemento
                float angle = startAngle + (totalAngle * i / (uiElements.Length - 1) * direction);

                // Convertir el ángulo a radianes
                float angleRad = angle * Mathf.Deg2Rad;

                // Calcular la posición del elemento de la interfaz de usuario en el semicírculo
                Vector3 pos = new Vector2(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius);
                uiElements[i].anchoredPosition = pos;

                // Calcular la dirección desde el elemento hacia el centro de la pantalla
                Vector2 directionToCenter = canvasCenter - uiElements[i].position;

                // Calcular el ángulo de rotación basado en la dirección hacia el centro de la pantalla
                float angleToCenter = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;

                // Aplicar rotación al elemento de la interfaz de usuario para que mire hacia el centro de la pantalla
                uiElements[i].rotation = Quaternion.Euler(0, 0, angleToCenter + spriteRotationOffset);
            }
        }
    }
}