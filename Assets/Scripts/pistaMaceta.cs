using UnityEngine;
using UnityEngine.UIElements;
using System;
public class pistaMaceta: MonoBehaviour
{
    public GameObject pistaMacetaCanvas;
    private bool puedeInteractuar;

    private void Start()
    {
        pistaMacetaCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Entra a la zona");
            puedeInteractuar = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("sali de la zona");
            puedeInteractuar = false;
            pistaMacetaCanvas.SetActive(false);

        }
    }
    private void Update()
    {
        if(puedeInteractuar && Input.GetKeyDown(KeyCode.E))
        {
            if (!GameManager.Instance.vioMesita)
            {
                Debug.Log("presionar mesa primero");
                return;
            }
            Debug.Log("presione E");
            GameManager.Instance.EncontrarPista1();
            pistaMacetaCanvas.SetActive(!pistaMacetaCanvas.gameObject.activeSelf);
        }
    }
}