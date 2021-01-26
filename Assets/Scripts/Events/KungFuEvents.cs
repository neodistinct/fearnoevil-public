using UnityEngine;

public class KungFuEvents : MonoBehaviour
{
    private CharacterStats _playerCharacterStats;
    
    public GameObject characterRoot;
    

    // Start is called before the first frame update
    void Awake()
    {
        if(characterRoot)
            _playerCharacterStats = characterRoot.GetComponent<CharacterStats>();
    }

    public void DecreaseEnergy()
    {
        _playerCharacterStats.ChangeEnergy(-CharacterStats.KICK_ENERGY);
    }
}
