using UnityEngine;

namespace GameMechanics
{
    [AddComponentMenu("Mechanics/Game/Game Controller")]
    public class GameController : MonoBehaviour
    {
        protected GameSceneLoader m_loader => GameSceneLoader.instance;
        
        public virtual void LoadScene(string scene) => m_loader.Load(scene);
    }
}