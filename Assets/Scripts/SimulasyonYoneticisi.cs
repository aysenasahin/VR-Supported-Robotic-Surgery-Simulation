using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class SimulasyonYoneticisi : MonoBehaviour
{
    public static SimulasyonYoneticisi instance;

    [Header("Alet Sayma Ayarlarý")]
    public int toplamAletSayisi = 5;
    private int yerlesenAletSayisi = 0;

    [Header("UI Panelleri")]
    public GameObject durdurmaPaneli;
    public GameObject bitisPaneli;

    [Header("Menü Açýkken Kapanacak Hareket Sistemleri")]
    public Behaviour[] hareketSistemleri;

    [Header("Menü Açýkken Kapanacak Görseller")]
    public GameObject[] menuAcikkenKapanacakGorseller;

    [Header("Menü Açýkken Kilitlenecek Hareket Kökü")]
    public Transform hareketKilitlenecekKok; // Buraya XR Origin (XR Rig) verilecek

    private bool oyunDurduMu = false;
    private bool oyunBittiMi = false;

    private bool oncekiVRPauseButonu = false;

    private bool hareketKilitliMi = false;
    private Vector3 kilitliPozisyon;
    private Quaternion kilitliRotasyon;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;

        if (durdurmaPaneli != null)
            durdurmaPaneli.SetActive(false);

        if (bitisPaneli != null)
            bitisPaneli.SetActive(false);

        HareketiAyarla(true);
        MenuGorselleriniAyarla(true);
        HareketKilidiniKaldir();
    }

    void Update()
    {
        if (oyunBittiMi)
            return;

        if (Input.GetKeyDown(KeyCode.P) || VRPauseButonunaBasildiMi())
        {
            if (oyunDurduMu)
                DevamEt();
            else
                Durdur();
        }
    }

    void LateUpdate()
    {
        if (hareketKilitliMi && hareketKilitlenecekKok != null)
        {
            hareketKilitlenecekKok.SetPositionAndRotation(kilitliPozisyon, kilitliRotasyon);
        }
    }

    private bool VRPauseButonunaBasildiMi()
    {
        InputDevice sagKontrolcu = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool suAnBasili = false;

        if (sagKontrolcu.isValid)
        {
            sagKontrolcu.TryGetFeatureValue(CommonUsages.secondaryButton, out suAnBasili);
        }

        bool buFrameBasildi = suAnBasili && !oncekiVRPauseButonu;
        oncekiVRPauseButonu = suAnBasili;

        return buFrameBasildi;
    }

    public void Durdur()
    {
        oyunDurduMu = true;

        if (bitisPaneli != null)
            bitisPaneli.SetActive(false);

        if (durdurmaPaneli != null)
        {
            durdurmaPaneli.SetActive(true);
            durdurmaPaneli.transform.SetAsLastSibling();
        }

        Time.timeScale = 0f;

        HareketiAyarla(false);
        MenuGorselleriniAyarla(false);
        HareketKilidiniBaslat();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void AnaMenuyeDon()
    {
        Time.timeScale = 1f;
        HareketiAyarla(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("GirisEkrani");
    }
    public void DevamEt()
    {
        oyunDurduMu = false;

        if (durdurmaPaneli != null)
            durdurmaPaneli.SetActive(false);

        Time.timeScale = 1f;

        HareketiAyarla(true);
        MenuGorselleriniAyarla(true);
        HareketKilidiniKaldir();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void YenidenBaslat()
    {
        Time.timeScale = 1f;

        HareketiAyarla(true);
        MenuGorselleriniAyarla(true);
        HareketKilidiniKaldir();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Cikis()
    {
        Debug.Log("Oyundan çýkýlýyor...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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

    private void OyunBitti()
    {
        oyunBittiMi = true;
        oyunDurduMu = false;

        Debug.Log("Tebrikler! Tüm cerrahi aletler doðru yuvalara yerleþtirildi.");

        if (durdurmaPaneli != null)
            durdurmaPaneli.SetActive(false);

        if (bitisPaneli != null)
        {
            bitisPaneli.SetActive(true);
            bitisPaneli.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("BitisPaneli GameManager üzerindeki SimulasyonYoneticisi scriptine baðlanmamýþ!");
        }

        Time.timeScale = 0f;

        HareketiAyarla(false);
        MenuGorselleriniAyarla(false);
        HareketKilidiniBaslat();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HareketiAyarla(bool aktifMi)
    {
        if (hareketSistemleri == null)
            return;

        foreach (Behaviour sistem in hareketSistemleri)
        {
            if (sistem != null)
                sistem.enabled = aktifMi;
        }
    }

    private void MenuGorselleriniAyarla(bool aktifMi)
    {
        if (menuAcikkenKapanacakGorseller == null)
            return;

        foreach (GameObject obje in menuAcikkenKapanacakGorseller)
        {
            if (obje != null)
                obje.SetActive(aktifMi);
        }
    }

    private void HareketKilidiniBaslat()
    {
        if (hareketKilitlenecekKok == null)
            return;

        kilitliPozisyon = hareketKilitlenecekKok.position;
        kilitliRotasyon = hareketKilitlenecekKok.rotation;
        hareketKilitliMi = true;
    }

    private void HareketKilidiniKaldir()
    {
        hareketKilitliMi = false;
    }
}