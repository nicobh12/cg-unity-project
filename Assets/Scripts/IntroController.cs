using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroController : MonoBehaviour
{
    public TextMeshProUGUI texto;

    private IEnumerator Start()
    {
        texto.text = "Tu casa ha sido invadida por fantasmas. Quieren hacer una fiesta en ella.";

        yield return new WaitForSeconds(3f);

        texto.text = "¡Acaba con su fiesta antes de que destruyan tu casa por completo!";

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("nv1");
    }
}