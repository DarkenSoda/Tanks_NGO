using Managers;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private GameObject hitVFX;

    private int damage = 20;
    private Vector3 spawnPoint;

    private Rigidbody rb;
    private ulong ownerID;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * speed;
        spawnPoint = transform.position;

        Destroy(gameObject, lifetime);
    }

    public void SetOwner(ulong id, int dmg)
    {
        ownerID = id;
        damage = dmg;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (NetworkingManager.Singleton.IsServer)
        {
            if (collision.gameObject.TryGetComponent(out NetPlayer player))
            {
                player.TakeDamage(damage, ownerID);
            }
        }
        if (hitSound != null)
        {
            AudioManager.Instance.PlaySFXClip(hitSound, transform);
        }
        if (hitVFX != null)
        {
            VFXManager.Instance.PlayVFX(hitVFX, transform.position);
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out TankBarrier _))
        {
            if (!other.bounds.Contains(spawnPoint))
            {
                Destroy(gameObject);
            }
        }
    }
}