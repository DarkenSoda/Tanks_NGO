using UnityEngine;

namespace Utilities.Bases
{
    public class SimpleSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        
        private static T _instance; 
        public static T Instance { get => _instance; }
        
        protected virtual void Awake() {
            if (_instance != null && _instance != this as T)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this as T;
            }
        }
    }
}
