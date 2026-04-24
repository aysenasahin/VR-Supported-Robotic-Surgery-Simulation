using UnityEngine;
using System.IO.Ports;

public class XBeeHaberlesme : MonoBehaviour
{
    [Header("XBee Ayarları")]
    public string portAdi = "COM11";
    public int baudRate = 9600;

    private static SerialPort seriPort;
    private static bool portHazir = false;

    void Start()
    {
        if (portHazir)
            return;

        seriPort = new SerialPort(portAdi, baudRate);

        try
        {
            seriPort.Open();
            portHazir = true;
            Debug.Log("✅ XBee Bağlantısı Başarılı: " + portAdi);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("⚠️ XBee takılı değil (Test Modu). Detay: " + e.Message);
        }
    }

    public void AletSecildiMesajiGonder()
    {
        string aletIsmi = gameObject.name.ToUpper();
        string gidenMesaj = "<HEDEF:" + aletIsmi + ">";
        MesajGonder(gidenMesaj);
    }

    public void AletSoketeKonduMesajiGonder()
    {
        string aletIsmi = gameObject.name.ToUpper();

        // İstersen bunu Arduino/XBee tarafındaki protokole göre değiştir
        string gidenMesaj = "<SOKET:" + aletIsmi + ">";
        MesajGonder(gidenMesaj);
    }

    private void MesajGonder(string gidenMesaj)
    {
        if (seriPort != null && seriPort.IsOpen)
        {
            seriPort.WriteLine(gidenMesaj);
            Debug.Log("📡 GERÇEK VERİ GÖNDERİLDİ: " + gidenMesaj);
        }
        else
        {
            Debug.Log("🧪 SİMÜLASYON (Port Kapalı): " + gidenMesaj);
        }
    }

    void OnApplicationQuit()
    {
        if (seriPort != null && seriPort.IsOpen)
        {
            seriPort.Close();
            portHazir = false;
        }
    }
}