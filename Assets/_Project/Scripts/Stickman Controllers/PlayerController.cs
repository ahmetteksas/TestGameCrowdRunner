using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
#pragma warning disable 0649

    [Header("Variables")]
    [SerializeField] private bool start = false;
    [SerializeField] private float speed;

    private float dragOrigin;
    private float leftThreshold = -5f;
    private float rightThreshold = 5f;
    private bool stop = false;
    private bool finish = false;
    private bool isStunned;

    public float stunDelay;
    public float minCloseOffset;

    [Header("References")]
    [SerializeField] private Transform cam;

#pragma warning restore 0649

    private Rigidbody rigidBodyPlayer;
    private Vector2 currentTouchPosition;
    private float finalX;
    private Vector2 firstPosition;
    private float currentSpeed;


    private void OnEnable()
    {
        EventManager.startGame += GameStart;
        EventManager.stopWalk += stopWalk;
        EventManager.continueWalk += continueWalk;
        EventManager.stopSlide += stopSlide;
        EventManager.triggerFinishCamera += TriggerFinishCamera;
    }

    private void OnDisable()
    {
        EventManager.startGame -= GameStart;
        EventManager.stopWalk -= stopWalk;
        EventManager.continueWalk -= continueWalk;
        EventManager.stopSlide -= stopSlide;
        EventManager.triggerFinishCamera -= TriggerFinishCamera;
    }
    private void Start()
    {
        cam.LookAt(this.transform.position);
        AttachReferences();
        ResetValues();
    }

    private void Update()
    {
        StartGame();
    }

    private void FixedUpdate()
    {
        if (start && !stop)
        {
            EndlessRun();
        }
    }

    private void StartGame()
    {
        if (start && !stop && !finish)
        {
            MovementWithSlide();
        }
    }


    private void AttachReferences()
    {
        rigidBodyPlayer = GetComponent<Rigidbody>();
        currentSpeed = speed;
    }

    private void ResetValues()
    {
        rigidBodyPlayer.velocity = new Vector3(0f, rigidBodyPlayer.velocity.y, rigidBodyPlayer.velocity.z);
        firstPosition = Vector2.zero;
        finalX = 0f;
        currentTouchPosition = Vector2.zero;
    }

    private void EndlessRun()
    {

        rigidBodyPlayer.transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed * 3);
        cam.parent.transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed * 3);
    }

    private void MovementWithSlide()
    {
        NavMeshAgent[] navMeshAgents;
        navMeshAgents = this.GetComponentsInChildren<NavMeshAgent>();

        foreach (NavMeshAgent child in navMeshAgents)
        {
            if (child.isOnNavMesh)
                child.SetDestination(transform.position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition.x;
        }

        if (Input.GetMouseButton(0))
        {
            float moveAmount = (Input.mousePosition.x - dragOrigin);
            transform.localPosition = new Vector3(transform.localPosition.x + moveAmount / 110, transform.localPosition.y, transform.localPosition.z);

            if (transform.localPosition.x >= rightThreshold)
            {
                transform.localPosition = new Vector3(rightThreshold, transform.localPosition.y, transform.localPosition.z);
            }
            else if (transform.localPosition.x <= leftThreshold)
            {
                if (isStunned)
                {
                    transform.localPosition = new Vector3(-2.3f, transform.localPosition.y, transform.localPosition.z);
                }
                else
                {
                    transform.localPosition = new Vector3(leftThreshold, transform.localPosition.y, transform.localPosition.z);
                }
            }

            dragOrigin = Input.mousePosition.x;
        }

        if (Input.GetMouseButtonUp(0))
        {
            ResetValues();
        }

    }
    public void PlayerStunned()
    {
        isStunned = true;
        transform.localPosition = new Vector3(minCloseOffset, transform.localPosition.y, transform.localPosition.z);
        StartCoroutine(SolveStun());
    }
    IEnumerator SolveStun()
    {
        yield return new WaitForSeconds(stunDelay);
        isStunned = false;
    }

    private void GameStart()
    {
        start = true;
    }
    private void stopWalk()
    {
        stop = true;
    }
    private void continueWalk()
    {
        stop = false;
    }
    private void stopSlide()
    {
        finish = true;
    }

    private void TriggerFinishCamera()
    {
        cam.parent.transform.position = new Vector3(cam.parent.transform.position.x + 10, cam.parent.transform.position.y + 15, cam.parent.transform.position.z);
        cam.LookAt(this.transform.position);
    }

}