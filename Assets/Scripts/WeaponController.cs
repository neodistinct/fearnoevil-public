using System.ComponentModel.Design;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header ("Attack animation names")]
    [SerializeField]
    private string [] attackAnimations;
    [SerializeField]
    private AudioClip attackSound;
    [SerializeField]
    private AudioClip kickSound;
    [SerializeField]
    private bool active = true;

    private Animator _animator;
    private AudioSource _parentAudioSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _parentAudioSource = transform.parent.parent.GetComponent<AudioSource>();
    }

    void Update()
    {
        if(active) { 
            if(Input.GetButtonDown("Fire1"))
            {
                int animCount = attackAnimations.Length;
                int animationIndex = Random.Range(0, animCount);

                _animator.SetTrigger(attackAnimations[animationIndex]);
            }
        }
    }

    // Used as event in animation
    public void PlayMoveSound()
    {
        if(_parentAudioSource) _parentAudioSource.PlayOneShot(attackSound);
    }

    // Used as event in animation
    public void PlayKickSound()
    {
        if (_parentAudioSource) _parentAudioSource.PlayOneShot(kickSound);
    }
}
