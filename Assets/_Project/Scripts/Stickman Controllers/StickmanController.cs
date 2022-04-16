using UnityEngine;
using UnityEngine.AI;

public class StickmanController : MonoBehaviour
{

    private bool fight;
    private Transform stickman;
    private Transform enemy;
    private bool justOne = true;
    private bool justOneAgain = true;

    private void OnEnable()
    {
        EventManager.startRunAnim += GameStart;
    }

    private void OnDisable()
    {
        EventManager.startRunAnim -= GameStart;
    }
    private void Start()
    {
        stickman = this.transform;
        fight = false;
    }

    private void Update()
    {
        transform.LookAt(Vector3.forward);
    }

    private void Fight()
    {

        NavMeshAgent[] navMeshAgents;
        navMeshAgents = this.transform.parent.GetComponentsInChildren<NavMeshAgent>();

        foreach (NavMeshAgent child in navMeshAgents)
        {
            if (child.isOnNavMesh)
                child.SetDestination(stickman.position);
        }
        EventManager.stopWalk.Invoke();
    }

    public void KillEachOther(Collider other)
    {

        other.enabled = false;
        EventManager.DecreaseNumber.Invoke();

        if (stickman.transform.parent.childCount <= 2)
        {

        }

        if (other.transform.parent.childCount <= 2)
        {
            other.transform.parent.gameObject.SetActive(false);
            EventManager.continueWalk.Invoke();
        }
        Destroy(other.gameObject);
        //var _instance = Object.Instantiate(deadVfx, stickman.position, Quaternion.identity) as GameObject;
        //var _enemyInstance = Object.Instantiate(enemyDeadVfx, other.transform.position, Quaternion.identity) as GameObject;
        stickman.transform.parent = null;
        EventManager.playerPool.Push(stickman.gameObject);
        //Destroy(stickman.gameObject);

    }

    private void Dead()
    {
        EventManager.DecreaseNumber.Invoke();
        //var _instance = Object.Instantiate(deadVfx, stickman.position, Quaternion.identity) as GameObject;
        stickman.transform.parent = null;
        EventManager.playerPool.Push(stickman.gameObject);
        //Destroy(stickman.gameObject);
    }

    private void GameStart()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {

            KillEachOther(other);
            justOne = false;

        }
        if (other.CompareTag("fight"))
        {

            Fight();
            other.GetComponent<EnemyController>().Fight(stickman);
            justOneAgain = false;

        }
        else if (other.CompareTag("barrier"))
        {

            Dead();
            justOne = false;


        }
        else if (other.CompareTag("panel"))
        {
            if (other.GetComponent<PanelInformation>().GetType() == panelType.Mult)
            {
                EventManager.IncreaseNumberWithMult.Invoke(other.GetComponent<PanelInformation>().GetNumber());
            }
            else if (other.GetComponent<PanelInformation>().GetType() == panelType.Add)
            {
                EventManager.IncreaseNumberWithAdd.Invoke(other.GetComponent<PanelInformation>().GetNumber());
            }

            other.transform.parent.parent.gameObject.SetActive(false);
        }
        else if (other.CompareTag("finish"))
        {
            EventManager.triggerFinish.Invoke();
        }
        if (other.CompareTag("navmeshOffTrigger"))
        {
            this.GetComponent<NavMeshAgent>().enabled = false;
            this.GetComponent<Rigidbody>().drag = 0;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("navmeshOffTrigger"))
        {
            this.GetComponent<NavMeshAgent>().enabled = true;
            this.GetComponent<Rigidbody>().drag = 30;
        }
    }
}
