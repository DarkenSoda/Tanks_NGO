using UnityEngine;

namespace Utilities.Bases
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T _instance;
        public static T Instance { get => _instance; }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(transform.root);
            }
        }

        // private void OnDestroy()
        // {
        //     if (Instance == this)
        //     {
        //         Instance = null;
        //     }
        // }
    }
}
