using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class OyunMotoru : MonoBehaviour
{
    [Header("Görsel Yönlendirme")]
    public GameObject okIsareti; // Sahnede yarattýðýn ok objesini buraya sürükle

    [Header("Arayüz")]
    public GameObject oyunSonuPaneli; // Canvas'ý buraya sürükle

    [Header("Oyun Kurallarý")]
    public int toplamAletSayisi = 5; // Makas, Penset, Kase, Gazlý Bez, Kanlý Bez
    private int yerlesenAletSayisi = 0;

    void Start()
    {
        // Oyun baþlarken oku ve bitiþ ekranýný gizle
        if (okIsareti != null) okIsareti.SetActive(false);
        if (oyunSonuPaneli != null) oyunSonuPaneli.SetActive(false);
    }

    void Update()
    {
        // Ok görünürse, fiyakalý bir þekilde kendi etrafýnda dönsün
        if (okIsareti != null && okIsareti.activeSelf)
        {
            okIsareti.transform.Rotate(0, 90 * Time.deltaTime, 0);
        }
    }

    // ALETÝ ELÝMÝZE ALDIÐIMIZDA ÇALIÞACAK
    public void OkuGoster(Transform hedefYuva)
    {
        okIsareti.SetActive(true);
        // Oku, tam hedefin 20 santim üstüne ýþýnla
        okIsareti.transform.position = hedefYuva.position + new Vector3(0, 0.2f, 0);
    }

    // ALETÝ BIRAKTIÐIMIZDA (VE YA YUVAYA GÝRDÝÐÝNDE) ÇALIÞACAK
    public void OkuGizle()
    {
        okIsareti.SetActive(false);
    }

    // ALET DOÐRU YUVAYA OTURDUÐUNDA ÇALIÞACAK
    public void AletYuvayaOturdu()
    {
        yerlesenAletSayisi++;
        OkuGizle();

        // Bütün aletler yerleþti mi kontrolü
        if (yerlesenAletSayisi >= toplamAletSayisi)
        {
            OyunBitti();
        }
    }

    // YUVADAN ALET GERÝ ÇIKARILIRSA
    public void AletYuvadanCikti()
    {
        yerlesenAletSayisi--;
    }

    void OyunBitti()
    {
        oyunSonuPaneli.SetActive(true);
    }

    // BUTONLARA BAÐLANACAK FONKSÝYONLAR
    public void OyunuYenidenBaslat()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OyunuKapat()
    {
        Application.Quit();
    }
}