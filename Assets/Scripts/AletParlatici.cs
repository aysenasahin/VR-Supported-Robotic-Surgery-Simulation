using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AletParlatici : MonoBehaviour
{
    [Header("Görsel Efektler")]
    public Material sariParlamaMateryali; // Buraya sarý materyalini sürükle

    private Material orijinalMateryal;
    private Renderer aletRenderer;
    private XRGrabInteractable grabInteractable;

    // Aletin elimizde olup olmadýðýný takip etmek için bir kilit ekliyoruz
    private bool eldeMi = false;

    void Start()
    {
        aletRenderer = GetComponent<Renderer>();
        if (aletRenderer != null)
        {
            orijinalMateryal = aletRenderer.material; // Aletin kendi rengini kaydet
        }

        // Objede zaten var olan XR tutma kodunu bul
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            // Lazerle üstüne gelme (Hover) olaylarý
            grabInteractable.hoverEntered.AddListener(Parlat);
            grabInteractable.hoverExited.AddListener(Sondur);

            // Eline alma ve býrakma (Select) olaylarý
            grabInteractable.selectEntered.AddListener(EleAlindi);
            grabInteractable.selectExited.AddListener(EldenBirakildi);
        }
    }

    void Parlat(HoverEnterEventArgs args)
    {
        // EÐER ALET ELÝMÝZDEYSE PARLAMA YAPMA, FONKSÝYONDAN ÇIK
        if (eldeMi) return;

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

    // ALETÝ ELÝMÝZE ALDIÐIMIZ AN ÇALIÞACAK KISIM
    void EleAlindi(SelectEnterEventArgs args)
    {
        eldeMi = true; // Kilidi kapat, artýk parlama olmasýn

        // Elimize aldýðýmýz an sarý rengi silip orijinal renge zorla döndürüyoruz
        if (aletRenderer != null)
        {
            aletRenderer.material = orijinalMateryal;
        }
    }

    // ALETÝ ELÝMÝZDEN BIRAKTIÐIMIZ AN ÇALIÞACAK KISIM
    void EldenBirakildi(SelectExitEventArgs args)
    {
        eldeMi = false; // Kilidi aç, alet masadayken tekrar parlayabilsin
    }
}