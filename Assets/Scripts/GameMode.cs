using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class GameMode : MonoBehaviour
{
    [SerializeField]
    private float interactionDistance = 3;
    [SerializeField]
    private AudioClip winSound;
    [SerializeField]
    private AudioClip looseSound;
    [SerializeField]
    private AudioClip fightSound;
    [SerializeField]
    private GameObject playerDummy;

    private Transform _playerWeaponModel;
    private Animator _playerWeaponModelAnimator;

    private Transform _kungFuManeken;
    private Animator _kungFuAnimator;

    private Rigidbody _playerRigidBody;
    private BoxCollider _playerBoxCollider;

    private CharacterController _playerCharacterController;
    private FirstPersonController _playerFirstPersonController;
    private Transform _playerFirstPersonCharacter;
    private CharacterStats _playerStats;

    private Canvas _gameCanvas;
    private GameObject _menuEventSystem;

    private AudioSource _audioPlayer;

    // Scene List
    private List<string> _levels = new List<string> { "Level1", "Level2" };

    // List of enemies in scene
    private GameObject[] _enemies;
    private GameObject[] _blockerObjects;

    // Access variables
    private NavMeshAgent _navAgent;
    private Animator _enemyAnimator;
    private CharacterStats _enemyHealth;
    
    private Text _matchStatusText;
    private Text _enemiesPawnedCountText;
    private Text _startFightText;
    private Text _pressAnyKeyText;
    private Text _playerNameText;

    // Mode switchers 
    private bool _followMode = false;
    private bool _matchOver = false;

    // Gameplay affect values
    private const int ROTATION_SPEED = 4;

    private void Awake()
    {
        _blockerObjects = GameObject.FindGameObjectsWithTag("Blocker");

        try { 

            _menuEventSystem = SceneManager.GetSceneByName("MainMenu").GetRootGameObjects()[1];

            // Debug.Log(menuEventSystem);
        }
        catch (Exception)
        {

        }

        _gameCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _audioPlayer = gameObject.GetComponent<AudioSource>();

        if (LevelHub.menuCanvas) LevelHub.menuCanvas.enabled = false;
        if (_menuEventSystem) _menuEventSystem.SetActive(false);

        // Player section        
        
        _playerFirstPersonCharacter = transform.Find("FirstPersonCharacter");
        _playerWeaponModel = _playerFirstPersonCharacter.Find("WeaponModel");
        _kungFuManeken = _playerFirstPersonCharacter.Find("KungFuModel");
        _playerWeaponModelAnimator = _playerWeaponModel.GetComponent<Animator>();

        if (_kungFuManeken) { 
            _kungFuAnimator = _kungFuManeken.GetComponent<Animator>();
        }

        _playerRigidBody = gameObject.GetComponent<Rigidbody>();
        _playerBoxCollider = gameObject.GetComponentInChildren<BoxCollider>();
        _playerCharacterController = gameObject.GetComponent<CharacterController>();
        _playerFirstPersonController = gameObject.GetComponent<FirstPersonController>();
        _playerStats = gameObject.GetComponent<CharacterStats>();

        // Enemies section
        _enemies = GameObject.FindGameObjectsWithTag("Character");
        _matchStatusText = GameObject.Find("MatchStatusText").GetComponent<Text>();
        _enemiesPawnedCountText = GameObject.Find("EnemiesPawnedCountText").GetComponent<Text>();
        _startFightText = GameObject.Find("StartFightText").GetComponent<Text>();
        _pressAnyKeyText = GameObject.Find("ReturnToMenuText").GetComponent<Text>();
        _playerNameText = GameObject.Find("PlayerNameText").GetComponent<Text>();
        _playerNameText.text = Library.GetPlayerName();
    }

    // Update is called once per frame
    void Update()
    {

        ProcessPlayerInput();

        // Now lets get moving info 

        _playerWeaponModelAnimator.SetFloat("BlendTest", Math.Abs(Input.GetAxis("Vertical")));
        

        int pawnedEnemyCount = 0;
        foreach (GameObject testEnemy in _enemies)
        {
            _navAgent = testEnemy.GetComponent<NavMeshAgent>();
            _enemyAnimator = testEnemy.GetComponent<Animator>();
            _enemyHealth = testEnemy.GetComponent<CharacterStats>();

            if (_followMode)
            {
                if (testEnemy && _enemyHealth.health > 0)
                {
                    // Process distance
                    float distance = (testEnemy.transform.position - transform.position).sqrMagnitude;
//                    //Debug.Log("Distance is: " + distance);

                    if (distance <= interactionDistance) // STOP AND ATTACK
                    {

                        _navAgent.velocity = Vector3.zero;

                        if (_enemyAnimator)
                        {
                            _enemyAnimator.SetBool("IsRunning", false);
                            _enemyAnimator.SetBool("IsAttacking", true);
                        }

                        Library.RotateTowards(transform, testEnemy.transform, ROTATION_SPEED);
                        //testEnemy.GetComponentInChildren<NavMeshObstacle>().carving = true;

                    }
                    else
                    { // FOLLOW THE PLAYER
                        
                        _navAgent.enabled = true;

                        //testEnemy.GetComponentInChildren<NavMeshObstacle>().carving = false;

                        _navAgent.SetDestination(transform.position);

                        if (_enemyAnimator) {
                            _enemyAnimator.SetBool("IsRunning", true);
                            _enemyAnimator.SetBool("IsAttacking", false);
                        }

                    }
                }

            }
            else // STOP FOLLOWING FORCELY
            {

                if (testEnemy)
                {                    
                    _navAgent.enabled = false;

                    if(_enemyAnimator)
                    {
                        _enemyAnimator.SetBool("IsRunning", false);
                        _enemyAnimator.SetBool("IsAttacking", false);
                    }

                }
            }

            if (_enemyHealth.pawned) pawnedEnemyCount++;
        }


        ProcessMatchEvents(pawnedEnemyCount);

    }

    private void ProcessPlayerInput()
    {
        if (Input.GetButtonDown("Fire2") && _kungFuAnimator && _playerStats.energy >= CharacterStats.KICK_ENERGY) {
            Debug.Log("Anim name:" + _kungFuAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name);

            if (_playerWeaponModelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && 
                !_kungFuAnimator.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
            {
                _playerWeaponModelAnimator.SetTrigger("HolsterFast");
                _kungFuAnimator.SetTrigger("Kick");
            }

            
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            _followMode = !_followMode;

            if (_followMode)
            {
                _audioPlayer.PlayOneShot(fightSound);
                
                foreach (GameObject testEnemy in _enemies)
                {

                    CharacterStats enemyHealthItem = testEnemy.GetComponent<CharacterStats>();
                    Animator enemyAnimatorItem = testEnemy.GetComponent<Animator>();

                    enemyHealthItem.pawned = false;
                    enemyHealthItem.health = 100;

                    enemyAnimatorItem.SetBool("IsRunning", true);

                }

            }

            _startFightText.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale != 0)
            {

                _playerFirstPersonController.enabled = false;

                Time.timeScale = 0;
                if (LevelHub.menuCanvas) LevelHub.menuCanvas.enabled = true;
                if(_menuEventSystem) _menuEventSystem.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                _playerFirstPersonController.enabled = true;

                Time.timeScale = 1;
                if (LevelHub.menuCanvas) LevelHub.menuCanvas.enabled = false;
                if (_menuEventSystem) _menuEventSystem.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

            }
        }
    }

    public void StartFight()
    {
        _followMode = true;
        //Debug.Log("Follow mode is:" + followMode);

        if (_followMode) _audioPlayer.PlayOneShot(fightSound);

        foreach(GameObject blocker in _blockerObjects) {
            blocker.SetActive(false);
        }
        
    }

    private void ProcessMatchEvents(int pawnedEnemyCount)
    {
        if(_enemiesPawnedCountText) { 
            _enemiesPawnedCountText.text = "ENEMIES PAWNED: " + pawnedEnemyCount + "/" + _enemies.Length;
            // Display victory message if all are pawned
            if (_enemies.Length != 0 && pawnedEnemyCount == _enemies.Length && !_matchOver)
            {
                _matchOver = true;
                Invoke("DoVictoryActions", 3);
            }
        }
    }

    private void DoVictoryActions()
    {
        int currentSceneIndex = Library.GetInstance().GetCurrentSceneIndex(_levels);

        _matchStatusText.text = "YOU WIN!";
        _audioPlayer.PlayOneShot(winSound);
        _playerWeaponModelAnimator.SetTrigger("Holster");

        // If this is the last level - show player character and do camera animations
        if (currentSceneIndex == _levels.Count - 1)
        {
            if(Camera.current) Camera.current.enabled = false;
            
            playerDummy.SetActive(true);
            playerDummy.GetComponent<Animator>().SetTrigger("Win");
            playerDummy.transform.Find("CameraRoot").gameObject.GetComponent<Rotator>().enabled = true;
            _playerFirstPersonCharacter.gameObject.SetActive(false);

            _playerCharacterController.enabled = false;
            _playerFirstPersonController.enabled = false;

            // Hide Game canvas
            _gameCanvas.enabled = false;

            // Show Titles
            if(LevelHub.titlesCanvas) LevelHub.titlesCanvas.enabled = true;
            
        }
        else // Load next scene to play
        {
            Invoke("LoadNextScene", 3);            
        }

    }

    private void LoadNextScene()
    {
        int currentSceneIndex = Library.GetInstance().GetCurrentSceneIndex(_levels);
        int nextSceneIndex = currentSceneIndex + 2;

        // Load next level if it is not last level
        
        if (currentSceneIndex != _levels.Count - 1) {
            LevelHub.LoadLevel("Level" + nextSceneIndex);
        }
        
    }

    // Shall be used in singleton class actually
    public void DeactivateAllEnemies()
    {
        // Will loop through all enemies later

        _followMode = false;

    }

    public void DeactivatePlayer()
    {
        _playerRigidBody.isKinematic = false;
        _playerBoxCollider.enabled = true;

        _playerCharacterController.enabled = false;
        _playerFirstPersonController.enabled = false;

        _playerWeaponModel.gameObject.SetActive(false);

        _matchStatusText.text = "YOU LOOSE!";
        _audioPlayer.PlayOneShot(looseSound);
        _pressAnyKeyText.enabled = true;

        DeactivateAllEnemies();

    }

}
