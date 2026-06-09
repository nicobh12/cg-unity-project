using UnityEngine;
using UnityEngine.UIElements;

public class MesitaContraseña: MonoBehaviour
{
    public UIDocument uiDocument;
    private bool puedeInteractuar;
    public GameObject llave;

    private void Start()
    {
        uiDocument.gameObject.SetActive(false);
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
            uiDocument.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(puedeInteractuar && Input.GetKeyDown(KeyCode.E) )
        {

            if (!GameManager.Instance.keyFound)
            {
                if (!GameManager.Instance.vioPuerta)
                {
                    Debug.Log("presionar puerta primero");
                    return;
                }
                GameManager.Instance.vioMesita = true;
                uiDocument.gameObject.SetActive(!uiDocument.gameObject.activeSelf);
                VisualElement  root = uiDocument.rootVisualElement;

                root.Q<Label>("primeraPista").text=GameManager.Instance.p1;
                root.Q<Label>("segundaPista").text=GameManager.Instance.p2;
                root.Q<Label>("terceraPista").text=GameManager.Instance.p3;    
            } else if (GameManager.Instance.keyFound)
            {
                llave.SetActive(true);
                GameManager.Instance.key.SetActive(false);
            }
            
        }
    }
}
