using UnityEngine;

public class KungFuEvents : MonoBehaviour
{
    private CharacterStats _playerCharacterStats;
    
    public GameObject characterRoot;

    private delegate void RunTheWorld();
    private RunTheWorld coolMethod;

    // Start is called before the first frame update
    void Awake()
    {
        coolMethod += DecreaseEnergy;

        if (characterRoot)
            _playerCharacterStats = characterRoot.GetComponent<CharacterStats>();
    }

    public void DecreaseEnergy()
    {
        _playerCharacterStats.ChangeEnergy(-CharacterStats.KICK_ENERGY);
    }
}
