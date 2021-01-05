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

    private Transform playerWeaponModel;
    private Animator playerWeaponModelAnimator;

    private Transform kungFuManeken;
    private Animator kungFuAnimator;

    private Canvas gameCanvas;
    private GameObject menuEventSystem;

    // Gameplay affect values
    private const int ROTATION_SPEED = 4;

    // Scene List
    private List<string> levels = new List<string> { "Level1", "Level2" };

    // List of enemies in scene
    private GameObject[] enemies;
    private GameObject[] blockerObjects;

    // Access variables
    private NavMeshAgent navAgent;
    private Animator enemyAnimator;
    private CharacterHealth enemyHealth;
    
    private Text matchStatusText;
    private Text enemiesPawnedCountText;
    private Text startFightText;
    private Text playerNameText;

    // Mode switchers 
    private bool followMode = false;
    private bool matchOver = false;

    private void Start()
    {
        blockerObjects = GameObject.FindGameObjectsWithTag("Blocker");

        try { 

            menuEventSystem = SceneManager.GetSceneByName("MainMenu").GetRootGameObjects()[1];

            // Debug.Log(menuEventSystem);
        }
        catch (Exception)
        {

        }

        gameCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        if (LevelHub.menuCanvas) LevelHub.menuCanvas.enabled = false;
        if (menuEventSystem) menuEventSystem.SetActive(false);

        playerWeaponModel = gameObject.transform.Find("FirstPersonCharacter").Find("WeaponModel");
        playerWeaponModelAnimator = playerWeaponModel.GetComponent<Animator>();

        kungFuManeken = gameObject.transform.Find("FirstPersonCharacter").Find("KungFuModel");
        
        if (kungFuManeken) { 
            kungFuAnimator = kungFuManeken.GetComponent<Animator>();
        }

        enemies = GameObject.FindGameObjectsWithTag("Character");
        matchStatusText = GameObject.Find("MatchStatusText").GetComponent<Text>();
        enemiesPawnedCountText = GameObject.Find("EnemiesPawnedCountText").GetComponent<Text>();
        startFightText = GameObject.Find("StartFightText").GetComponent<Text>();
        playerNameText = GameObject.Find("PlayerNameText").GetComponent<Text>();
        playerNameText.text = Library.GetPlayerName();
    }

    // Update is called once per frame
    void Update()
    {

        ProcessPlayerInput();

        // Now lets get moving info 

        playerWeaponModelAnimator.SetFloat("BlendTest", Math.Abs(Input.GetAxis("Vertical")));
        

        int pawnedEnemyCount = 0;
        foreach (GameObject testEnemy in enemies)
        {
            navAgent = testEnemy.GetComponent<NavMeshAgent>();
            enemyAnimator = testEnemy.GetComponent<Animator>();
            enemyHealth = testEnemy.GetComponent<CharacterHealth>();

            if (followMode)
            {
                if (testEnemy && enemyHealth.health > 0)
                {
                    // Process distance
                    float distance = (testEnemy.transform.position - transform.position).sqrMagnitude;
//                    //Debug.Log("Distance is: " + distance);

                    if (distance <= interactionDistance) // STOP AND ATTACK
                    {

                        navAgent.velocity = Vector3.zero;

                        if (enemyAnimator)
                        {
                            enemyAnimator.SetBool("IsRunning", false);
                            enemyAnimator.SetBool("IsAttacking", true);
                        }

                        Library.RotateTowards(transform, testEnemy.transform, ROTATION_SPEED);
                        //testEnemy.GetComponentInChildren<NavMeshObstacle>().carving = true;

                    }
                    else
                    { // FOLLOW THE PLAYER
                        
                        navAgent.enabled = true;

                        //testEnemy.GetComponentInChildren<NavMeshObstacle>().carving = false;

                        navAgent.SetDestination(transform.position);

                        if (enemyAnimator) {
                            enemyAnimator.SetBool("IsRunning", true);
                            enemyAnimator.SetBool("IsAttacking", false);
                        }

                    }
                }

            }
            else // STOP FOLLOWING FORCELY
            {

                if (testEnemy)
                {                    
                    navAgent.enabled = false;

                    if(enemyAnimator)
                    {
                        enemyAnimator.SetBool("IsRunning", false);
                        enemyAnimator.SetBool("IsAttacking", false);
                    }

                }
            }

            if (enemyHealth.pawned) pawnedEnemyCount++;
        }


        ProcessMatchEvents(pawnedEnemyCount);

    }

    private void ProcessPlayerInput()
    {
        if (Input.GetButtonDown("Fire2") && kungFuAnimator) {

            if(playerWeaponModelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                playerWeaponModelAnimator.SetTrigger("HolsterFast");
                kungFuAnimator.SetTrigger("Kick");
            }


        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            followMode = !followMode;

            if (followMode)
            {
                gameObject.GetComponent<AudioSource>().PlayOneShot(fightSound);
                
                foreach (GameObject testEnemy in enemies)
                {

                    CharacterHealth enemyHealthItem = testEnemy.GetComponent<CharacterHealth>();
                    Animator enemyAnimatorItem = testEnemy.GetComponent<Animator>();

                    enemyHealthItem.pawned = false;
                    enemyHealthItem.health = 100;

                    enemyAnimatorItem.SetBool("IsRunning", true);

                }

            }

            startFightText.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale != 0)
            {

                gameObject.GetComponent<FirstPersonController>().enabled = false;

                Time.timeScale = 0;
                if (LevelHub.menuCanvas) LevelHub.menuCanvas.enabled = true;
                if(menuEventSystem) menuEventSystem.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                gameObject.GetComponent<FirstPersonController>().enabled = true;

                Time.timeScale = 1;
                if (LevelHub.menuCanvas) LevelHub.menuCanvas.enabled = false;
                if (menuEventSystem) menuEventSystem.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

            }
        }
    }

    public void StartFight()
    {
        followMode = true;
        //Debug.Log("Follow mode is:" + followMode);

        if (followMode) gameObject.GetComponent<AudioSource>().PlayOneShot(fightSound);

        foreach(GameObject blocker in blockerObjects) {
            blocker.SetActive(false);
        }
        
    }

    private void ProcessMatchEvents(int pawnedEnemyCount)
    {
        if(enemiesPawnedCountText) { 
            enemiesPawnedCountText.text = "ENEMIES PAWNED: " + pawnedEnemyCount + "/" + enemies.Length;
            // Display victory message if all are pawned
            if (enemies.Length != 0 && pawnedEnemyCount == enemies.Length && !matchOver)
            {
                matchOver = true;
                Invoke("DoVictoryActions", 3);
            }
        }
    }

    private void DoVictoryActions()
    {
        int currentSceneIndex = Library.GetInstance().GetCurrentSceneIndex(levels);

        matchStatusText.text = "YOU WIN!";
        gameObject.GetComponent<AudioSource>().PlayOneShot(winSound);
        playerWeaponModelAnimator.SetTrigger("Holster");

        // If this is the last level - show player character and do camera animations
        if (currentSceneIndex == levels.Count - 1)
        {
            if(Camera.current) Camera.current.enabled = false;
            
            playerDummy.SetActive(true);
            playerDummy.GetComponent<Animator>().SetTrigger("Win");
            playerDummy.transform.Find("CameraRoot").gameObject.GetComponent<Rotator>().enabled = true;
            gameObject.transform.Find("FirstPersonCharacter").gameObject.SetActive(false);

            gameObject.GetComponent<CharacterController>().enabled = false;
            gameObject.GetComponent<FirstPersonController>().enabled = false;

            // Hide Game canvas
            gameCanvas.enabled = false;

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
        int currentSceneIndex = Library.GetInstance().GetCurrentSceneIndex(levels);
        int nextSceneIndex = currentSceneIndex + 2;

        // Load next level if it is not last level
        
        if (currentSceneIndex != levels.Count - 1) {
            LevelHub.LoadLevel("Level" + nextSceneIndex);
        }
        
    }

    // Shall be used in singleton class actually
    public void DeactivateAllEnemies()
    {
        // Will loop through all enemies later

        followMode = false;

    }

    public void DeactivatePlayer()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponentInChildren<BoxCollider>().enabled = true;

        gameObject.GetComponent<CharacterController>().enabled = false;
        gameObject.GetComponent<FirstPersonController>().enabled = false;

        playerWeaponModel.gameObject.SetActive(false);

        matchStatusText.text = "YOU LOOSE!";
        gameObject.GetComponent<AudioSource>().PlayOneShot(looseSound);

        DeactivateAllEnemies();

    }

}
