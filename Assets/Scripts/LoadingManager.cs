using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject loadingScreen; // Panel o Canvas de Carga
    [SerializeField] private Slider progressBar;      // Tu Slider
    [SerializeField] private TMP_Text progressText;    // Texto de porcentaje (Opcional)

    [Header("Configuración")]
    [SerializeField] private string gameSceneName = "EscenaJuego";

    public void LoadGame()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // 1. Activar la pantalla de carga si la tenías oculta
        if (loadingScreen != null) 
            loadingScreen.SetActive(true);

        // 2. Iniciar la carga en segundo plano
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneName);
        operation.allowSceneActivation = false; // Evita que cambie de golpe al llegar al 100%

        // 3. Actualizar la barra suavemente mientras carga
        while (!operation.isDone)
        {
            // Unity carga la escena de 0 a 0.9, el 0.1 final es para la activación
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;

            if (progressText != null)
                progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            // Cuando la escena termina de cargar en memoria
            if (operation.progress >= 0.9f)
            {
                // Espera un pequeño instante para que el jugador vea la barra llena
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true; // Cambia finalmente a la escena
            }

            yield return null;
        }
    }
}