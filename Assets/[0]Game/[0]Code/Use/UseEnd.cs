using UnityEngine;

namespace Game
{
    public class UseEnd : MonoBehaviour
    {
        [SerializeField]
        private EndEnum _end;
        
        public void Use()
        {
            GameData.EndingsManager.End(_end);
        }
    }
}