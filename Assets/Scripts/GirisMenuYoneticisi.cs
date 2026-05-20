using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GirisMenuYoneticisi : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject anaMenuPaneli;
    public GameObject hakkindaPaneli;
    public GameObject nasilOynanirPaneli;

    [Header("Yükleme Yazýsý")]
    public GameObject yuklemeYazisi;

    void Start()
    {
        AnaMenuyuAc();

        if (yuklemeYazisi != null)
            yuklemeYazisi.SetActive(false);
    }

    public void SimulasyonuBaslat()
    {
        StartCoroutine(SahneYukle());
    }

    private IEnumerator SahneYukle()
    {
        if (yuklemeYazisi != null)
            yuklemeYazisi.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        SceneManager.LoadScene("SampleScene");
    }

    public void HakkindaAc()
    {
        if (anaMenuPaneli != null)
            anaMenuPaneli.SetActive(false);

        if (nasilOynanirPaneli != null)
            nasilOynanirPaneli.SetActive(false);

        if (hakkindaPaneli != null)
        {
            hakkindaPaneli.SetActive(true);
            hakkindaPaneli.transform.SetAsLastSibling();
        }
    }

    public void NasilOynanirAc()
    {
        if (anaMenuPaneli != null)
            anaMenuPaneli.SetActive(false);

        if (hakkindaPaneli != null)
            hakkindaPaneli.SetActive(false);

        if (nasilOynanirPaneli != null)
        {
            nasilOynanirPaneli.SetActive(true);
            nasilOynanirPaneli.transform.SetAsLastSibling();
        }
    }

    public void AnaMenuyuAc()
    {
        if (anaMenuPaneli != null)
            anaMenuPaneli.SetActive(true);

        if (hakkindaPaneli != null)
            hakkindaPaneli.SetActive(false);

        if (nasilOynanirPaneli != null)
            nasilOynanirPaneli.SetActive(false);
    }

    public void CikisYap()
    {
        Debug.Log("Oyundan çýkýlýyor...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}