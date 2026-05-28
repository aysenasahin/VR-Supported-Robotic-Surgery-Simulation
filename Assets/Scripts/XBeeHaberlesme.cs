using UnityEngine;
using System;
using System.IO.Ports;

public class XBeeHaberlesme : MonoBehaviour
{
    [Header("Baglanti")]
    public string portAdi = "COM3";   // Inspector'dan gir
    public int baudRate = 9600;

    [Header("Timeout")]
    public int readTimeoutMs = 1000;
    public int writeTimeoutMs = 1000;

    [Header("Debug")]
    public bool debug = true;

    // ---------------------------------------------------
    private static SerialPort seriPort;
    private static bool portHazir = false;
    private static bool baglantiAcildi = false;   // sadece 1 kez baglan
    // ---------------------------------------------------

    void Start()
    {
        if (baglantiAcildi) return;   // birden fazla obje varsa sadece 1 kez
        baglantiAcildi = true;
        ElleBaglan(portAdi);
    }

    // -----------------------------------------------------------------------
    void ElleBaglan(string port)
    {
        try
        {
            seriPort = new SerialPort(port, baudRate)
            {
                ReadTimeout = readTimeoutMs,
                WriteTimeout = writeTimeoutMs,
                NewLine = "\n",
                DtrEnable = false,
                RtsEnable = false
            };

            seriPort.Open();
            portHazir = true;
            Debug.Log("✅ XBee baglandi -> " + port);
        }
        catch (Exception e)
        {
            portHazir = false;
            Debug.LogWarning("❌ Baglanamadi [" + port + "] -> " + e.Message);
        }
    }

    // -----------------------------------------------------------------------
    public void MesajGonder(string mesaj)
    {
        if (seriPort == null || !seriPort.IsOpen || !portHazir)
        {
            if (debug) Debug.Log("🧪 SIM MODE -> " + mesaj);
            return;
        }

        try
        {
            seriPort.WriteLine(mesaj);
            if (debug) Debug.Log("📡 TX -> " + mesaj);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Mesaj gonderilemedi -> " + e.Message);
        }
    }

    // -----------------------------------------------------------------------
    // VR'da alet ELE ALINDI  →  <HEDEF:ALETISMI>
    public void AletSecildiMesajiGonder()
    {
        string isim = AletIsmiAl();
        MesajGonder("<HEDEF:" + isim + ">");
    }

    // VR'da alet SOKETE BIRAKILD  →  <SOKET:ALETISMI>
    public void AletSoketeKonduMesajiGonder()
    {
        string isim = AletIsmiAl();
        MesajGonder("<SOKET:" + isim + ">");
    }

    string AletIsmiAl()
    {
        return gameObject.name
            .Replace("(Clone)", "")
            .Trim()
            .ToUpperInvariant();
    }

    // -----------------------------------------------------------------------
    void OnDestroy() => PortKapat();
    void OnApplicationQuit() => PortKapat();

    static void PortKapat()
    {
        if (seriPort != null && seriPort.IsOpen)
        {
            seriPort.Close();
            portHazir = false;
            baglantiAcildi = false;
            Debug.Log("XBee port kapatildi.");
        }
    }
}