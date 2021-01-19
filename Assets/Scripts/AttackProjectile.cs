using System.Collections;
using UnityEngine;

[AddComponentMenu ("SchoolFX/Attack Projectile")]
[RequireComponent (typeof(BoxCollider))]
public class AttackProjectile : MonoBehaviour
{
    [SerializeField]
    private int damage = 5;
    [SerializeField]
    private bool playerProjectile = false;
    [SerializeField]
    private GameObject bloodPrefab;
    
    public bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isActive || playerProjectile)
        {
            if ((other.tag == "Player" && !playerProjectile)
                || other.tag == "Character")
            {
                CharacterHealth healthModule = other.GetComponent<CharacterHealth>();

                if(healthModule) healthModule.CauseDamage(damage);

                if (other.tag != "Player")
                {
                    if (bloodPrefab) { 
                        // Also can be used ClosestPointOnBounds
                        Vector3 collisionPoint = other.gameObject.GetComponent<Collider>().ClosestPoint(transform.position);
                        GameObject bloodPrefabInstance = Instantiate(bloodPrefab, collisionPoint, Quaternion.identity);

                        Destroy(bloodPrefabInstance, 2);                       

                    }
                }
            }
        }

    }

}
