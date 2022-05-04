using UnityEngine;
using UnityEngine.AI;

public class StickmanController : MonoBehaviour
{
    [SerializeField] private GameObject item1;
    [SerializeField] private GameObject item2;
    [SerializeField] private GameObject item3;
    [SerializeField] private Material blueMaterial;

    public static bool changeColor;
    private int itemNumber;
    public float damage;
    private bool fight;
    private Transform stickman;
    private Transform enemy;
    private bool justOne = true;
    private bool justOneAgain = true;
    Spawner spawner;


    private void OnEnable()
    {
        EventManager.startRunAnim += GameStart;
        spawner = FindObjectOfType<Spawner>();
    }

    private void OnDisable()
    {
        EventManager.startRunAnim -= GameStart;
    }
    private void Start()
    {
        stickman = transform;
        fight = false;
    }

    private void Update()
    {
        transform.LookAt(Vector3.forward);
        if (changeColor)
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = blueMaterial;
        }
        if (highDamage)
        {
            damage = .1f;
        }
        else if (mediumDamage)
        {
            damage = .05f;
        }
        else
        {
            damage = .01f;
        }
    }

    private void Fight()
    {

        NavMeshAgent[] navMeshAgents;
        navMeshAgents = transform.parent.GetComponentsInChildren<NavMeshAgent>();

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


    private bool lowDamage;
    private bool mediumDamage;
    private bool highDamage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("item"))
        {
            itemNumber = Random.Range(0, 3);
            if (itemNumber == 0)
            {
                item1.SetActive(true);
                highDamage = true;
                damage = .1f;
            }
            else if (itemNumber == 1)
            {
                item2.SetActive(true);
                mediumDamage = true;
                damage = .05f;
            }
            else
            {
                item3.SetActive(true);
                lowDamage = true;
                damage = .01f;
            }
        }
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
        if (other.CompareTag("add"))
        {
            //Fight();
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
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Rigidbody>().drag = 0;
        }
        if (other.CompareTag("ally"))
        {
            spawner.Addition(1);
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("changecolor"))
        {
            changeColor = true;
            //GetComponentInChildren<SkinnedMeshRenderer>().material = blueMaterial;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("navmeshOffTrigger"))
        {
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<Rigidbody>().drag = 30;
        }
    }
}
