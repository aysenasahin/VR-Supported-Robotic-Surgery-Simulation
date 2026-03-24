using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO.Ports; // USB Haberleşme Kütüphanesi

public class XBeeHaberlesme : MonoBehaviour
{
    [Header("XBee Ayarları")]
    public string portAdi = "COM3"; // Gerçekte XBee hangi USB'ye takılıysa o yazılacak
    public int baudRate = 9600;

    // Static yapıyoruz ki sahnede 10 tane alet olsa bile hepsi aynı USB portunu kullansın, çakışmasın
    private static SerialPort seriPort;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Start()
    {
        // 1. Oyun başladığında portu açmayı dene (Sadece bir kere)
        if (seriPort == null)
        {
            seriPort = new SerialPort(portAdi, baudRate);
            try
            {
                seriPort.Open();
                Debug.Log("✅ XBee Bağlantısı Başarılı: " + portAdi);
            }
            catch (System.Exception e)
            {
                // XBee fiziksel olarak takılı değilse oyun çökmez, sadece bu sarı uyarıyı verir
                Debug.LogWarning("⚠️ XBee takılı değil (Test Modu). Detay: " + e.Message);
            }
        }

        // 2. Aletin üzerindeki "Tutulma" bileşenini bul
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            // Alet ELİNE ALINDIĞINDA XBeeHedefYolla fonksiyonunu tetikle
            grabInteractable.selectEntered.AddListener(XBeeHedefYolla);
        }
    }

    void XBeeHedefYolla(SelectEnterEventArgs args)
    {
        // Tuttuğumuz aletin Unity'deki ismini alıp BÜYÜK harfe çeviriyoruz
        string aletIsmi = gameObject.name.ToUpper();

        // Robotun Arduino kodunun beklediği o efsanevi paketi hazırlıyoruz
        string gidenMesaj = "<HEDEF: " + aletIsmi + ">";

        // Eğer fiziksel XBee takılıysa gerçekten yolla
        if (seriPort != null && seriPort.IsOpen)
        {
            seriPort.WriteLine(gidenMesaj);
            Debug.Log("📡 GERÇEK VERİ GÖNDERİLDİ: " + gidenMesaj);
        }
        else // Takılı değilse sadece Unity Konsolunda test et
        {
            Debug.Log("🧪 SİMÜLASYON (Port Kapalı): Robota gidecek emir -> " + gidenMesaj);
        }
    }

    // Oyun kapatıldığında USB portunu serbest bırak (Çok önemli, yoksa PC portu kilitler)
    void OnApplicationQuit()
    {
        if (seriPort != null && seriPort.IsOpen)
        {
            seriPort.Close();
        }
    }
}