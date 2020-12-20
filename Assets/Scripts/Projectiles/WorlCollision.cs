using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WorlCollision : MonoBehaviour
{
    [SerializeField] int DefaultLayer = 0;
    [SerializeField] int wallLayer = 0;
    [SerializeField] int GroundFloorLayer = 0;

    private void OnTriggerEnter(Collider other)
    {
        int collisionedLayer = other.gameObject.layer;

        if ( collisionedLayer == wallLayer || collisionedLayer == DefaultLayer || 
             collisionedLayer == GroundFloorLayer)
            Destroy(gameObject);
    }
}
