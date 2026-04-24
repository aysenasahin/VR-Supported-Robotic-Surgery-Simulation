using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class YuvaParlatici : MonoBehaviour
{
    [Header("G�rsel Ayarlar")]
    public MeshRenderer parlayacakZemin; // Tepsinin veya masan�n g�rselini buraya ataca��z
    public Material sariParlamaMateryali; // Senin o fosforlu sar� materyalin

    private Material orijinalMateryal;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor yuva;

    void Start()
    {
        yuva = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

        if (parlayacakZemin != null)
        {
            orijinalMateryal = parlayacakZemin.material; // Tepsinin orijinal �elik rengini kaydet
        }

        if (yuva != null)
        {
            // Do�ru alet yuvaya YAKLA�TI�INDA (Hover) parlat
            yuva.hoverEntered.AddListener(Parlat);

            // Alet yuvadan uzakla��rsa S�ND�R
            yuva.hoverExited.AddListener(Sondur);

            // Alet yuvaya tam OTURDU�UNDA parlamay� bitir, orijinal renge d�n
            yuva.selectEntered.AddListener(Sondur);
        }
    }

    void Parlat(HoverEnterEventArgs args)
    {
        if (parlayacakZemin != null && sariParlamaMateryali != null)
        {
            parlayacakZemin.material = sariParlamaMateryali;
        }
    }

    void Sondur(HoverExitEventArgs args)
    {
        EskiRengeDon();
    }

    void Sondur(SelectEnterEventArgs args)
    {
        EskiRengeDon();
    }

    void EskiRengeDon()
    {
        if (parlayacakZemin != null)
        {
            parlayacakZemin.material = orijinalMateryal;
        }
    }
}