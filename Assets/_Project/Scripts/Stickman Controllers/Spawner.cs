using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using ScriptableObjectArchitecture;
using DG.Tweening;

public class Spawner : MonoBehaviour
{
    public GameEventBase finishEvent;

    [SerializeField] private Transform bossEnemy;
    private float radius = 0.5f;
    [SerializeField] private Transform middlePoint;
    private Vector3 merkez;
    private int currentNumber = 5;
    [SerializeField] private GameObject playerPrefab;
    private int count = 1;
    private int kat = 1;
    private int arakat;
    private float high;
    private bool stop = false;
    private bool finish = false;
    private Vector3 point;
    [SerializeField] private TextMeshProUGUI currentNumberText;

    private int poolCount = 20;

    private void OnEnable()
    {
        EventManager.IncreaseNumberWithAdd += Addition;
        EventManager.IncreaseNumberWithMult += Multiplication;
        EventManager.DecreaseNumber += DecreaseNumber;
        EventManager.triggerFinish += FinishAnimation;
    }

    private void OnDisable()
    {
        EventManager.IncreaseNumberWithAdd -= Addition;
        EventManager.IncreaseNumberWithMult -= Multiplication;
        EventManager.DecreaseNumber -= DecreaseNumber;
        EventManager.triggerFinish -= FinishAnimation;
    }
    private void Start()
    {
        FillPool(EventManager.playerPool, playerPrefab, poolCount);
        merkez = new Vector3(middlePoint.position.x, 1.2447f, middlePoint.position.z);
        UpdateCount(currentNumber);
        currentNumberText.text = currentNumber.ToString();
    }

    public void FillPool(ObjectPool pool, GameObject prefab, int count)
    {
        pool.SetObject(prefab);
        pool.Fill(count);
    }

    private void CreateEnemiesAroundPoint(int num, Vector3 point, float radius)
    {

        for (int i = 0; i < num; i++)
        {
            var radians = 2 * Mathf.PI / num * i;

            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);

            var spawnDir = new Vector3(horizontal, 0, vertical);

            var spawnPos = point + spawnDir * radius;

            //var _object = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
            var _object = EventManager.playerPool.Pop();
            _object.transform.position = spawnPos;

            _object.transform.parent = middlePoint;
            if (_object.GetComponent<NavMeshAgent>().isOnNavMesh)
                _object.GetComponent<NavMeshAgent>().SetDestination(middlePoint.position);

        }
    }

    public void Addition(int add)
    {
        currentNumber += add;
        UpdateCount(currentNumber);
    }

    private void Multiplication(int mult)
    {
        currentNumber = currentNumber * mult;
        UpdateCount(currentNumber);
    }

    private void UpdateCount(int _number)
    {
        Debug.Log(_number);
        StickmanController[] _transform;
        _transform = this.GetComponentsInChildren<StickmanController>();

        foreach (StickmanController child in _transform)
        {
            EventManager.playerPool.Push(child.gameObject);
            //Destroy(child.gameObject);
            child.transform.parent = null;
        }
        if (_number % 10 != 0)
        {
            int temporyNumber = _number % 10;
            if (temporyNumber > 0)
            {
                merkez = new Vector3(middlePoint.position.x, 1.2447f, middlePoint.position.z);
                radius += 0.05f;
                CreateEnemiesAroundPoint(temporyNumber, merkez, radius);
            }
            _number = _number - temporyNumber;

        }
        int round = _number / 10;
        for (int i = 0; i < round; i++)
        {
            merkez = new Vector3(middlePoint.position.x, 1.2447f, middlePoint.position.z);
            CreateEnemiesAroundPoint(10, merkez, radius);
            radius += 0.05f;
        }
        currentNumberText.text = currentNumber.ToString();
    }

    private IEnumerator GoAndStop()
    {

        NavMeshAgent[] navMeshAgents;
        navMeshAgents = this.transform.GetComponentsInChildren<NavMeshAgent>();

        foreach (NavMeshAgent child in navMeshAgents)
        {
            child.isStopped = false;
            child.SetDestination(middlePoint.position);
        }
        yield return new WaitForSeconds(0.5f);

        navMeshAgents = this.transform.GetComponentsInChildren<NavMeshAgent>();

        foreach (NavMeshAgent child in navMeshAgents)
        {
            child.isStopped = true;
        }
    }

    private void DecreaseNumber()
    {
        currentNumber--;
        currentNumberText.text = currentNumber.ToString();
        if (currentNumber <= 0)
        {
            EventManager.loseGame.Invoke();

        }
    }

    private void FinishAnimation()
    {
        EventManager.stopSlide.Invoke();
        point = new Vector3(middlePoint.position.x, middlePoint.position.y, middlePoint.position.z);
        StickmanController[] _transform;
        _transform = GetComponentsInChildren<StickmanController>();

        foreach (StickmanController child in _transform)
        {
            EventManager.playerPool.Push(child.gameObject);
            //Destroy(child.gameObject);
        }
        //Destroy(gameObject.transform.GetChild(0).gameObject);

        while (count < currentNumber)
        {
            kat++;
            count += kat;
        }

        if ((count - currentNumber) > 0)
        {
            arakat = count - currentNumber;
            kat--;
        }
        finishEvent.Raise();
        //StartCoroutine(StartFinish());

    }

    private IEnumerator CloseGravity(GameObject _object)
    {
        yield return new WaitForSeconds(0.3f);
        _object.GetComponent<Rigidbody>().useGravity = false;
    }
    Vector3 _offset;
    float _offsetY;
    float _offsetX;
    public void FinalFight()
    {
        for (int i = 0; i < kat; i++)
        {
            var stickman = EventManager.playerPool.Pop();  //Instantiate(finishPrefab, _point, Quaternion.identity) as GameObject;
            _offsetY = Random.Range(-5f, 5f);
            _offsetX = Random.Range(-2f, 2f);
            _offset = new Vector3(_offsetX, _offsetY, 0);
            stickman.transform.DOMove(bossEnemy.position + _offset, 2f);
        }
    }
}
