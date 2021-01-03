using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[AddComponentMenu ("SchoolFX/CharacterHealth")]
public class CharacterHealth : MonoBehaviour
{
    GameMode gameMode;
    Animator animator;

    public int health = 100;

    public bool pawned = false;

    public Transform customWaypoint;

    public List<AudioClip> hitSounds;

    public List<AudioClip> deathSounds;

    public Image healthSliderImage;
    public Image damageOverlayImage;
    Animator damageOverlayAnimator;

    public void Start()
    {
        gameMode = GameObject.Find("PlayerController").GetComponent<GameMode>();
        animator = gameObject.GetComponent<Animator>();

        if(damageOverlayImage)
        {
            damageOverlayAnimator = damageOverlayImage.GetComponent<Animator>();
        }

    }

    public void CauseDamage(int damage)
    {
        if (!pawned) {

            health -= damage;

            // Play the bloody sound
            if (hitSounds.Count > 0)
            {
                int soundIndex = Random.Range(0, hitSounds.Count);

                gameObject.GetComponent<AudioSource>().PlayOneShot(hitSounds[soundIndex]);
            }

            // Animate damage on screen if one object was attached 
            if(damageOverlayAnimator)
            {
                damageOverlayAnimator.SetTrigger("StopDamage");
                damageOverlayAnimator.SetTrigger("Damage");
            }


            if (health <= 0)
            {
                health = 0;
                pawned = true;

                if (gameObject.tag == "Player") {
                    gameMode.DeactivatePlayer();
                } else if(gameObject.tag == "Character")
                {
                    KillCharacter();
                }
            }
            
            // Visualize health in attached helth slider
            if (healthSliderImage != null)
            {

                float fHealth = health * 0.01f;

                healthSliderImage.fillAmount = fHealth;

            }


        }

    }

    private void KillCharacter()
    {        
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsAttacking", false);
        animator.SetTrigger("DeathTrigger");
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponentInChildren<AttackProjectile>().isActive = false;
        transform.Find("Face").GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = false;


        // Play death sound
        if (deathSounds.Count > 0)
        {
            int soundIndex = Random.Range(0, deathSounds.Count);

            gameObject.GetComponent<AudioSource>().PlayOneShot(deathSounds[soundIndex]);
        }

        //gameObject.GetComponent<NavMeshObstacle>().radius = 0.002f;
    }


}
