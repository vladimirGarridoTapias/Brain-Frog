using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Corregido: System.CollectionsCollections -> System.Collections

public class AnimacionBateria : MonoBehaviour
{
    [Header("Referencias de UI")]
    public Image contenedorBateria;
    public Sprite[] estadosBateria; // Tus 5 sprites (0 a 4)

    [Header("Configuracion de Animacion")]
    public float tiempoEntreFrames = 0.3f; // Velocidad de cambio
    public bool animarEnBucle = true;

    private void Start()
    {
        // Si quieres que anime sola desde el inicio:
        if (animarEnBucle)
        {
            StartCoroutine(RutinaAnimacionBucle());
        }
    }

    // Corrutina para que la batería se llene y se vacíe constantemente
    private IEnumerator RutinaAnimacionBucle()
    {
        int frameActual = 0;

        while (true)
        {
            if (contenedorBateria != null && estadosBateria.Length > 0)
            {
                contenedorBateria.sprite = estadosBateria[frameActual];
            }

            // Avanza al siguiente sprite (0 -> 1 -> 2 -> 3 -> 4 -> 0...)
            frameActual = (frameActual + 1) % estadosBateria.Length;

            yield return new WaitForSeconds(tiempoEntreFrames);
        }
    }

    // Función pública por si prefieres que solo se llene 1 vez al presionar el botón
    public void AnimarUnaVez()
    {
        StartCoroutine(RutinaAnimacionUnica());
    }

    private IEnumerator RutinaAnimacionUnica()
    {
        for (int i = 0; i < estadosBateria.Length; i++)
        {
            if (contenedorBateria != null)
            {
                contenedorBateria.sprite = estadosBateria[i];
            }
            yield return new WaitForSeconds(tiempoEntreFrames);
        }
    }
}