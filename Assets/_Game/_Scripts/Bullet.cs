using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    private int damage = 20;

    private Rigidbody rb;
    private ulong ownerID;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifetime);
    }

    public void SetOwner(ulong id, int dmg)
    {
        ownerID = id;
        damage = dmg;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(NetworkingManager.Singleton.IsServer)
        {
            if(collision.gameObject.TryGetComponent<NetPlayer>(out NetPlayer player))
            {
                player.TakeDamage(damage, ownerID);
            }
        }

        Destroy(gameObject);
    }
}