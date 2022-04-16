using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    private float firstDistance, currentDistance;

    [Header("Variables")]
    [SerializeField] private Slider filledBar;
    [SerializeField] private Transform finishPoint;
    [SerializeField] private Transform playerPoint;

    private void Start()
    {
        firstDistance = Mathf.Abs(finishPoint.position.z - playerPoint.position.z);
    }

    private void Update()
    {
        OpenProgressBar();
    }

    private void OpenProgressBar()
    {
        if (checkIsFinish())
        {
            currentDistance = Mathf.Abs(finishPoint.position.z - playerPoint.position.z);
        }
        filledBar.value = (firstDistance - currentDistance) / firstDistance;
    }

    private bool checkIsFinish()
    {
        if (finishPoint.position.z <= playerPoint.position.z)
        {
            return false;
        }
        return true;
    }
}