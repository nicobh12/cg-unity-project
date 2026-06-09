using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement;
public class PuertaBloqueada: MonoBehaviour
{
    private bool puedeInteractuar;
    public GameObject puertaBloqueada;

    private void Start()
    {
        puertaBloqueada.SetActive(false);
    }
    
    private void Update()
    {
        if (puedeInteractuar)
        {
            if (!GameManager.Instance.keyFound)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    puertaBloqueada.SetActive(true);
                    GameManager.Instance.vioPuerta = true;
                }
            
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    puertaBloqueada.SetActive(false);
                }
            }else if (GameManager.Instance.keyFound)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    SceneManager.LoadScene("nv2");
                }
            }
        
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            puedeInteractuar = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        puertaBloqueada.SetActive(false);
        puedeInteractuar = false;
    }
}