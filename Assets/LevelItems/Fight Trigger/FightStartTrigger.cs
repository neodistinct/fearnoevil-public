using UnityEngine;

public class FightStartTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject playerStatusText;
    [SerializeField]
    private float rotationSpeed = 32;

    private GameMode gameMode;

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
