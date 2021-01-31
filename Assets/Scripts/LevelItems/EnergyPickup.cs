using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    [SerializeField]
    private int energyRecoveryAmount = 50;
    [SerializeField]
    private AudioClip energyPickupSound;

    private CharacterStats _playerStats;
    private AudioSource _playerAudio;

    private void Awake()
    {
        GameObject player = GameObject.Find("PlayerController");
        if (player) { 
            _playerStats = player.GetComponent<CharacterStats>();
            _playerAudio = player.GetComponent<AudioSource>();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && _playerStats)
        {
            if (_playerStats.energy < 100) {
                _playerStats.ChangeEnergy(energyRecoveryAmount);
                Destroy(gameObject);
            }

            if(_playerAudio && energyPickupSound)
            {
                _playerAudio.PlayOneShot(energyPickupSound);
            }

        }
    }
}
