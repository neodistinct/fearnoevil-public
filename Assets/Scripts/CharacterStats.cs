using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngineInternal;

[AddComponentMenu("SCHOOL-FX/CharacterStats")]
public class CharacterStats : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> hitSounds;
    [SerializeField]
    private List<AudioClip> deathSounds;
    [SerializeField]
    private Image healthSliderImage;
    [SerializeField]
    private Image damageOverlayImage;
    [SerializeField]
    private Slider energySlider;

    private Animator _damageOverlayAnimator;
    private GameMode _gameMode;
    private Animator _animator;
    private Collider _characterCollider;
    private AttackProjectile _attackProjectile;
    private Collider _faceCollider;
    private NavMeshAgent _navMeshAgent;
    private AudioSource _audioSource;

    public int health = 100;
    public bool pawned = false;
    [Tooltip("Energy to perform a kick move")]
    public int energy = 0;

    public static int KICK_ENERGY = 50;

    public void Awake()
    {
        Transform face = transform.Find("Face");

        if (face) _faceCollider = face.GetComponent<Collider>();
        _gameMode = GameObject.Find("PlayerController").GetComponent<GameMode>();
        _animator = gameObject.GetComponent<Animator>();
        _characterCollider = GetComponent<Collider>();
        _attackProjectile = GetComponentInChildren<AttackProjectile>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _audioSource = gameObject.GetComponent<AudioSource>();

        if (damageOverlayImage)
        {
            _damageOverlayAnimator = damageOverlayImage.GetComponent<Animator>();
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
            if (_damageOverlayAnimator)
            {
                _damageOverlayAnimator.SetTrigger("StopDamage");
                _damageOverlayAnimator.SetTrigger("Damage");
            }


            if (health <= 0)
            {
                health = 0;
                pawned = true;

                if (gameObject.tag == "Player") {
                    _gameMode.DeactivatePlayer();
                } else if (gameObject.tag == "Character")
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

    public void ChangeEnergy(int energyValue)
    {
        energy += energyValue;

        // Just in case if value somehow decceeds
        if (energy < 0) energy = 0;

        if (energySlider)
        {

                
            StartCoroutine(AnimateSlider(energy / 100f));


        }
    }

    private void KillCharacter()
    {
        if (_animator) { 
            _animator.SetBool("IsRunning", false);
            _animator.SetBool("IsAttacking", false);
            _animator.SetTrigger("DeathTrigger");
        }

        if (_characterCollider) _characterCollider.enabled = false;
        if (_attackProjectile) _attackProjectile.isActive = false;
        if (_faceCollider) _faceCollider.enabled = false;
        if (_navMeshAgent) _navMeshAgent.enabled = false;

        // Play death sound
        if (deathSounds.Count > 0)
        {
            int soundIndex = Random.Range(0, deathSounds.Count);
            
            _audioSource.PlayOneShot(deathSounds[soundIndex]);
        }

        // TODO: Redo or remove if not neccessary
        //gameObject.GetComponent<NavMeshObstacle>().radius = 0.002f;
    }


    private IEnumerator AnimateSlider(float finalValue)
    {
        while (System.Math.Round(energySlider.value, 2) != finalValue) {
            // SmoothStep seems to look more smooth than simple Lerp, huh?
            energySlider.value = Mathf.SmoothStep(energySlider.value, finalValue, Time.deltaTime * 15);
            // Proceeding after next Update
            yield return null; 
        }

    }

   

}
