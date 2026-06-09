using UnityEngine;
using UnityEngine.UIElements;
using System;
public class pistaLibro: MonoBehaviour
{
    public GameObject pistaLibroCanvas;
    private bool puedeInteractuar;

    private void Start()
    {
        pistaLibroCanvas.SetActive(false);
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
            pistaLibroCanvas.SetActive(false);

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
            GameManager.Instance.EncontrarPista2();
            pistaLibroCanvas.SetActive(!pistaLibroCanvas.gameObject.activeSelf);
        }
    }
}