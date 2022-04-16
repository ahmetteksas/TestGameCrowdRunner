using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDisplay : MonoBehaviour
{
    [SerializeField] private GameObject winGamePanel;

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
            heal.value++;
            other.gameObject.SetActive(false);
            if (heal.value == 1)
            {
                animBase.SetTrigger("Death");
                winGamePanel.SetActive(true);
            }
        }
    }
}
