using System;
using UnityEngine;
using UnityEngine.AI;

public class CharacterEvents : MonoBehaviour
{
    [SerializeField]
    private GameObject attackPoint;
    [SerializeField]
    private Material defaultTexture;
    [SerializeField]
    private Material grinTexture;

    private NavMeshAgent _navMeshAgent;
    private float _speed = 0;

    private Transform face;
    SkinnedMeshRenderer faceMeshRenderer;


    public void Awake()
    {
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();       
        _speed = _navMeshAgent.speed;

        face = transform.Find("Face");
        faceMeshRenderer = face.GetComponent<SkinnedMeshRenderer>();

    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            var b = new Material[] { defaultTexture };
            if (defaultTexture) faceMeshRenderer.materials = b;
        }
    }

    public void SetDamaging(int value)
    {
        bool isActive = Convert.ToBoolean(value);

        if (attackPoint)
        {
            attackPoint.GetComponent<AttackProjectile>().isActive = isActive;
            
            if (isActive)
            {
                if (grinTexture) faceMeshRenderer.materials = new Material[] { grinTexture };
                Debug.Log("Setting grin texture");
            }
            // Setting default texture in else statement here - purely doesnt work, must be Unity Engine bug. That's why we set it in SetMoving event.
        }
        
    }

    public void SetMoving(int value)
    {
        if(_navMeshAgent)
        {
            bool isMoving = Convert.ToBoolean(value);

            if (isMoving)
            {
                if (defaultTexture) faceMeshRenderer.materials = new Material[] { defaultTexture };
            }

            float newSpeed = isMoving ? _speed : 0.1f;

            _navMeshAgent.speed = newSpeed;
        }
    }

    public void SetDefaultFaceTexture()
    {
        if (defaultTexture) faceMeshRenderer.materials = new Material[] { defaultTexture };
    }
}
