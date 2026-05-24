using UnityEngine;
using Utilities.Bases;

namespace Managers
{
    public class VFXManager : SimpleSingleton<VFXManager>
    {
        public void PlayVFX(GameObject vfxPrefab, Vector3 position)
        {
            if (vfxPrefab == null)
            {
                Debug.LogWarning("VFX prefab is null");
                return;
            }

            GameObject vfxInstance = Instantiate(vfxPrefab, position, Quaternion.identity);

            // Try to find the duration from ParticleSystem
            ParticleSystem ps = vfxInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                float duration = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(vfxInstance, duration);
            }
            else
            {
                // Fallback duration if it's not a ParticleSystem
                Destroy(vfxInstance, 2f); 
            }
        }

        public void PlayVFX(GameObject vfxPrefab, Transform spawnPoint)
        {
            PlayVFX(vfxPrefab, spawnPoint.position);
        }
    }
}
