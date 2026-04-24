using UnityEngine;

public class OyunYoneticisi : MonoBehaviour
{
    public static OyunYoneticisi instance;

    [Header("Görev Ayarlarý")]
    public int toplamAletSayisi = 5;
    private int yerlesenAletSayisi = 0;

    [Header("Ýsteðe baðlý bitiþ paneli")]
    public GameObject bitisPaneli;

    private bool oyunBittiMi = false;

    void Awake()
    {
        instance = this;

        if (bitisPaneli != null)
            bitisPaneli.SetActive(false);
    }

    public void AletYerlestirildi()
    {
        if (oyunBittiMi)
            return;

        yerlesenAletSayisi++;

        Debug.Log(yerlesenAletSayisi + " / " + toplamAletSayisi + " alet yerleþtirildi.");

        if (yerlesenAletSayisi >= toplamAletSayisi)
        {
            OyunBitti();
        }
    }

    void OyunBitti()
    {
        oyunBittiMi = true;

        Debug.Log("<color=cyan>TEBRÝKLER! TÜM CERRAHÝ ALETLER YERLEÞTÝRÝLDÝ. SÝMÜLASYON TAMAMLANDI!</color>");

        if (bitisPaneli != null)
            bitisPaneli.SetActive(true);
    }
}