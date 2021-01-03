using System.ComponentModel.Design;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header ("Attack animation names")]
    public string [] attackAnimations;

    public AudioClip attackSound;
    public AudioClip kickSound;

    public bool active = true;

    // Update is called once per frame
    void Update()
    {
        if(active) { 
            if(Input.GetButtonDown("Fire1"))
            {
                int animCount = attackAnimations.Length;
                int animationIndex = Random.Range(0, animCount);
          
                GetComponent<Animator>().SetTrigger(attackAnimations[animationIndex]);

            }
        }
    }

    public void PlayMoveSound()
    {
        transform.parent.parent.GetComponent<AudioSource>().PlayOneShot(attackSound);
    }

    public void PlayKickSound()
    {
        transform.parent.parent.GetComponent<AudioSource>().PlayOneShot(kickSound);
    }
}
