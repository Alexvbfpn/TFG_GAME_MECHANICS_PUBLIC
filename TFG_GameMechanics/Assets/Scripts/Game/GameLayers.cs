using UnityEngine;

namespace GameMechanics
{
    public class GameLayers
    {
        public static string Player = "Player";
        public static string Enemy = "Enemy";
        public static string Default = "Default";
        public static string Interactable = "Interactable";
        public static string Ground = "Ground";
        public static string FirstViewer = "FirstViewer";


        public static void SetLayerToGameObject(GameObject gameObject, string layerName)
        {
            foreach (Transform t in gameObject.transform)
            {
                SetLayerToGameObject(t.gameObject, layerName);
            }
            gameObject.layer = LayerMask.NameToLayer(layerName);
        }
        
        public static void SetDefaultLayerToGameObject(GameObject gameObject)
        {
            SetLayerToGameObject(gameObject, Default);
        }
        
        public static int GetLayerMask(string layerName)
        {
            return 1 << LayerMask.NameToLayer(layerName);
        }
    }
}