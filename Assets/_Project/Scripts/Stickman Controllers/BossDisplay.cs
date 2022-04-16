using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDisplay : MonoBehaviour
{
    [SerializeField] private GameObject winGamePanel;
    [SerializeField] private GameObject loseGamePanel;


    private Slider heal;
    private Animator animBase;

    private void Awake()
    {
        animBase = GetComponentInChildren<Animator>();
        heal = GetComponentInChildren<Slider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            heal.value += other.gameObject.GetComponent<StickmanController>().damage;
            if (heal.value == 1)
            {
                animBase.SetTrigger("Death");
                winGamePanel.SetActive(true);
            }
            else
            {
                if (other.gameObject.GetComponent<StickmanController>() == null)
                {
                    loseGamePanel.SetActive(true);
                }
            }
            other.gameObject.SetActive(false);
        }
    }
}
