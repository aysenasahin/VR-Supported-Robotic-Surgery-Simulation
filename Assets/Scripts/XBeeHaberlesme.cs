using UnityEngine;
using System;
using System.IO.Ports;

public class XBeeHaberlesme : MonoBehaviour
{
    [Header("XBee Bağlantı Ayarları")]
    public bool otomatikPortBul = true;

    [Tooltip("Otomatik port bulma kapalıysa bu port kullanılır.")]
    public string ellePortAdi = "COM3";

    public int baudRate = 9600;

    [Header("XBee Tanıma Mesajları")]
    public string tanimaMesaji = "<UNITY_HELLO>";
    public string beklenenCevap = "<XBEE_OK>";

    [Header("Zaman Aşımı")]
    public int readTimeoutMs = 300;
    public int writeTimeoutMs = 300;
    public float portBasinaBeklemeSuresi = 1.0f;

    [Header("Debug")]
    public bool debugMesajlariGoster = true;

    private static SerialPort seriPort;
    private static bool portHazir = false;
    private static bool baglantiDenendi = false;
    private static string aktifPortAdi = "";

    void Start()
    {
        // Bu script her aletin üzerinde olabilir.
        // Ama seri port bağlantısı sadece 1 kere açılmalı.
        if (portHazir || baglantiDenendi)
            return;

        baglantiDenendi = true;

        if (otomatikPortBul)
        {
            XBeePortunuOtomatikBul();
        }
        else
        {
            EllePortaBaglan(ellePortAdi);
        }
    }

    private void XBeePortunuOtomatikBul()
    {
        string[] portlar = SerialPort.GetPortNames();

        if (portlar == null || portlar.Length == 0)
        {
            Debug.LogWarning("Hiç COM port bulunamadı. XBee takılı olmayabilir. Simülasyon modu devam ediyor.");
            return;
        }

        Array.Sort(portlar);

        if (debugMesajlariGoster)
        {
            Debug.Log("Bilgisayarda bulunan COM portları:");
            foreach (string port in portlar)
                Debug.Log(" - " + port);
        }

        foreach (string port in portlar)
        {
            if (debugMesajlariGoster)
                Debug.Log("XBee olabilir diye deneniyor: " + port);

            bool bulundu = PortuDeneVeXBeeMiKontrolEt(port);

            if (bulundu)
            {
                Debug.Log("✅ XBee otomatik bulundu: " + aktifPortAdi);
                return;
            }
        }

        Debug.LogWarning("XBee otomatik bulunamadı. Doğru porttan <XBEE_OK> cevabı gelmedi. Simülasyon modu devam ediyor.");
    }

    private bool PortuDeneVeXBeeMiKontrolEt(string portAdi)
    {
        SerialPort denemePortu = null;

        try
        {
            denemePortu = new SerialPort(portAdi, baudRate);
            denemePortu.ReadTimeout = readTimeoutMs;
            denemePortu.WriteTimeout = writeTimeoutMs;
            denemePortu.NewLine = "\n";

            // Bazı USB-Serial cihazlarda reset/karışıklık yapmaması için kapalı tutuyoruz.
            denemePortu.DtrEnable = false;
            denemePortu.RtsEnable = false;

            denemePortu.Open();

            denemePortu.DiscardInBuffer();
            denemePortu.DiscardOutBuffer();

            // Doğru cihazı tanımak için özel mesaj gönderiyoruz.
            denemePortu.WriteLine(tanimaMesaji);

            float baslangicZamani = Time.realtimeSinceStartup;

            while (Time.realtimeSinceStartup - baslangicZamani < portBasinaBeklemeSuresi)
            {
                try
                {
                    string cevap = denemePortu.ReadLine();
                    cevap = cevap.Trim();

                    if (debugMesajlariGoster)
                        Debug.Log(portAdi + " cevabı: " + cevap);

                    if (cevap == beklenenCevap)
                    {
                        seriPort = denemePortu;
                        portHazir = true;
                        aktifPortAdi = portAdi;

                        return true;
                    }
                }
                catch (TimeoutException)
                {
                    // Bu porttan zamanında cevap gelmedi.
                    break;
                }
            }

            denemePortu.Close();
            return false;
        }
        catch (Exception e)
        {
            if (denemePortu != null && denemePortu.IsOpen)
                denemePortu.Close();

            if (debugMesajlariGoster)
                Debug.LogWarning(portAdi + " denenemedi: " + e.Message);

            return false;
        }
    }

    private void EllePortaBaglan(string portAdi)
    {
        try
        {
            seriPort = new SerialPort(portAdi, baudRate);
            seriPort.ReadTimeout = readTimeoutMs;
            seriPort.WriteTimeout = writeTimeoutMs;
            seriPort.NewLine = "\n";

            seriPort.DtrEnable = false;
            seriPort.RtsEnable = false;

            seriPort.Open();

            portHazir = true;
            aktifPortAdi = portAdi;

            Debug.Log("✅ XBee elle seçilen porta bağlandı: " + aktifPortAdi);
        }
        catch (Exception e)
        {
            portHazir = false;
            Debug.LogWarning("⚠️ Elle seçilen porta bağlanılamadı: " + portAdi + " Detay: " + e.Message);
        }
    }

    public void AletSecildiMesajiGonder()
    {
        string aletIsmi = AletIsminiHazirla();

        // Oyuncu VR'da aleti eline alınca gönderilir.
        // Karşı taraf bu bilgiyle kamera sisteminde hangi aleti takip edeceğini bilir.
        string gidenMesaj = "<HEDEF:" + aletIsmi + ">";

        MesajGonder(gidenMesaj);
    }

    public void AletSoketeKonduMesajiGonder()
    {
        string aletIsmi = AletIsminiHazirla();

        // Oyuncu VR'da aleti doğru sokete koyunca gönderilir.
        string gidenMesaj = "<SOKET:" + aletIsmi + ">";

        MesajGonder(gidenMesaj);
    }

    private string AletIsminiHazirla()
    {
        return gameObject.name
            .Replace("(Clone)", "")
            .Trim()
            .ToUpperInvariant();
    }

    private void MesajGonder(string gidenMesaj)
    {
        if (seriPort != null && seriPort.IsOpen && portHazir)
        {
            try
            {
                seriPort.WriteLine(gidenMesaj);

                if (debugMesajlariGoster)
                    Debug.Log("📡 XBee ile gönderildi [" + aktifPortAdi + "]: " + gidenMesaj);
            }
            catch (Exception e)
            {
                Debug.LogWarning("⚠️ Mesaj gönderilemedi: " + e.Message);
            }
        }
        else
        {
            Debug.Log("🧪 Simülasyon modu / port kapalı: " + gidenMesaj);
        }
    }

    void OnApplicationQuit()
    {
        PortuKapat();
    }

    void OnDestroy()
    {
        // Play moddan çıkarken portun açık kalmaması için.
        PortuKapat();
    }

    private static void PortuKapat()
    {
        if (seriPort != null && seriPort.IsOpen)
        {
            seriPort.Close();
            portHazir = false;
            baglantiDenendi = false;
            aktifPortAdi = "";

            Debug.Log("XBee seri port kapatıldı.");
        }
    }
}