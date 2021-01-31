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
    [SerializeField]
    private Material deadTexture;

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
        }
        
    }

    public void SetMoving(int value)
    {
        if(_navMeshAgent)
        {
            bool isMoving = Convert.ToBoolean(value);

            if (isMoving) SetFaceTexture(FaceTexture.Default);

            float newSpeed = isMoving ? _speed : 0.1f;

            _navMeshAgent.speed = newSpeed;
        }
    }


    public void SetFaceTexture(FaceTexture faceTextureId = FaceTexture.Default)
    {

        switch (faceTextureId) { 
            case FaceTexture.Default:
                if (defaultTexture) faceMeshRenderer.materials = new Material[] { defaultTexture };
                break;
            case FaceTexture.Grin:
                if (grinTexture) faceMeshRenderer.materials = new Material[] { grinTexture };
                break;
            case FaceTexture.Dead:
                if (deadTexture) faceMeshRenderer.materials = new Material[] { deadTexture };
                break;
            default:
                break;
        }

        
    }
}

public enum FaceTexture
{
    Idle,
    Default,
    Grin,
    Dead,
}