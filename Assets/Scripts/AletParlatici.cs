using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Yeni XRI sürümleri için gerekli olabilir

public class AletParlatici : MonoBehaviour
{
    [Header("Görsel Efektler")]
    public Material sariParlamaMateryali; // Buraya sarý materyalini sürükle

    private Material orijinalMateryal;
    private Renderer aletRenderer;
    private XRGrabInteractable grabInteractable;

    void Start()
    {
        aletRenderer = GetComponent<Renderer>();
        if (aletRenderer != null)
        {
            orijinalMateryal = aletRenderer.material; // Aletin kendi rengini kaydet
        }

        // Objede zaten var olan XR tutma kodunu bul
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Elimiz alete deðdiðinde Parlat, elimizi çektiðimizde Söndür
        if (grabInteractable != null)
        {
            grabInteractable.hoverEntered.AddListener(Parlat);
            grabInteractable.hoverExited.AddListener(Sondur);
        }
    }

    void Parlat(HoverEnterEventArgs args)
    {
        if (aletRenderer != null && sariParlamaMateryali != null)
        {
            aletRenderer.material = sariParlamaMateryali;
        }
    }

    void Sondur(HoverExitEventArgs args)
    {
        if (aletRenderer != null)
        {
            aletRenderer.material = orijinalMateryal;
        }
    }
}