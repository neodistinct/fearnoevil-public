using UnityEngine;

public class FightStartTrigger : MonoBehaviour
{

    GameMode gameMode;
    public GameObject playerStatusText;
    public float rotationSpeed = 32;

    // Start is called before the first frame update
    private void Start()
    {
        gameMode = GameObject.Find("PlayerController").GetComponent<GameMode>();
    }

    private void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameMode.StartFight();

        Destroy(gameObject);

        if (playerStatusText) playerStatusText.SetActive(false);
    }
}
