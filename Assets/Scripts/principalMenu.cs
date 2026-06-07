using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("intro");
        Debug.Log("Botón Play funcionando");
    }

    public void Quit()
    {
        Debug.Log("Botón Quit funcionando");
        Application.Quit();
    }
}