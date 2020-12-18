/*using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [HideInInspector] public int Damage = 10;

    [SerializeField] float _travelSpeed = 10f;
    [SerializeField] float _maxLifeTime = 5f;

    float _lifeTime;

    Rigidbody _rb;
    IDamageable<inputDamage, HitResult> _owner;

    private void Awake()
    {
        _lifeTime = _maxLifeTime;
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.AddForce(transform.forward * _travelSpeed, ForceMode.Force);

        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0) Destroy(gameObject);
    }

    public void SetOwner(IDamageable<inputDamage, HitResult> owner)
    {
        _owner = owner;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != _owner.gameObject)
        {
            //print(string.Format("Hitted target {0}", other.gameObject));
            var target = other.GetComponentInParent<IDamageable<inputDamage, HitResult>>();

            if (target != null)
                _owner.OnHiConnected(target.Hit(new inputDamage() { Damage = this.Damage }));

            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != _owner.gameObject)
        {
            print(string.Format("Colisioné con {0}", collision.gameObject.name));
            Destroy(gameObject);
        }
    }
}
*/