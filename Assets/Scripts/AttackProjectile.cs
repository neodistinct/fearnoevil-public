using UnityEngine;

[AddComponentMenu ("SchoolFX/Attack Projectile")]
[RequireComponent (typeof(BoxCollider))]
public class AttackProjectile : MonoBehaviour
{
    public int damage = 5;
    public bool playerProjectile = false;
    public GameObject bloodPrefab;
    
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
                        Instantiate(bloodPrefab, collisionPoint, Quaternion.identity);
                    }
                }
            }
        }

    }


}
