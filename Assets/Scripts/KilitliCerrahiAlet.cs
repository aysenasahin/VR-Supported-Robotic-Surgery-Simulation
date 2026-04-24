using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class KilitliCerrahiAlet : XRGrabInteractable
{
    [Header("Bu aletin doðru yuvasý")]
    public XRSocketInteractor hedefYuva;

    [Header("Yuvaya oturunca kapanacak sarý silindir / highlight objesi")]
    public GameObject hedefSilindirGorseli;

    [Header("Yuvaya oturunca verilecek ek rotasyon")]
    public Vector3 yerlestirmeEulerOffset = new Vector3(0f, 0f, -90f);

    private Rigidbody rb;

    private XRBaseInteractor sonEl;
    private Transform eldeTakipNoktasi;

    private bool eldeTakipModu = false;
    private bool kilitlendi = false;
    private bool yoneticiyeBildirdi = false;

    // Ayný anda sadece 1 alet elde olsun
    private static KilitliCerrahiAlet eldekiAlet = null;

    private XBeeHaberlesme xbee;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        xbee = GetComponent<XBeeHaberlesme>();

        // Yanlýþ býrakýnca fýrlama istemiyoruz
        throwOnDetach = false;
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        // Kilitlendikten sonra artýk sadece kendi socket'i seçili tutabilir
        if (kilitlendi)
            return interactor is XRSocketInteractor socket && socket == hedefYuva;

        // Yanlýþ socket'ler bu objeyi hiç seçemesin
        if (interactor is XRSocketInteractor socketInteractor)
            return socketInteractor == hedefYuva && base.IsSelectableBy(interactor);

        // Eðer baþka bir alet zaten eldeyse, bu alet seçilemesin
        if (interactor is not XRSocketInteractor)
        {
            if (eldekiAlet != null && eldekiAlet != this)
                return false;
        }

        return base.IsSelectableBy(interactor);
    }

    private void LateUpdate()
    {
        if (!eldeTakipModu || kilitlendi || isSelected || eldeTakipNoktasi == null)
            return;

        transform.SetPositionAndRotation(
            eldeTakipNoktasi.position,
            eldeTakipNoktasi.rotation
        );
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Elde takip modundan çýk
        eldeTakipModu = false;

        // El aldýysa attach noktasýný kaydet
        if (args.interactorObject is XRBaseInteractor interactor &&
            args.interactorObject is not XRSocketInteractor)
        {
            sonEl = interactor;
            eldeTakipNoktasi = interactor.GetAttachTransform(this);

            // Artýk eldeki aktif alet bu
            eldekiAlet = this;

            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // XBee: alet seçildi
            if (xbee != null)
                xbee.AletSecildiMesajiGonder();
        }

        // Doðru socket aldýysa sonsuz kilit
        if (args.interactorObject is XRSocketInteractor socket &&
            socket == hedefYuva)
        {
            kilitlendi = true;
            eldeTakipModu = false;

            if (eldekiAlet == this)
                eldekiAlet = null;

            Transform attach = hedefYuva.GetAttachTransform(this);

            Vector3 hedefPos = transform.position;
            Quaternion hedefRot = transform.rotation;

            if (attach != null)
            {
                hedefPos = attach.position;
                hedefRot = attach.rotation * Quaternion.Euler(yerlestirmeEulerOffset);
            }

            transform.SetPositionAndRotation(hedefPos, hedefRot);

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            // Sarý silindiri kapat
            if (hedefSilindirGorseli != null)
                hedefSilindirGorseli.SetActive(false);

            // Oyun yöneticisine bir kez bildir
            if (!yoneticiyeBildirdi && OyunYoneticisi.instance != null)
            {
                OyunYoneticisi.instance.AletYerlestirildi();
                yoneticiyeBildirdi = true;
            }

            // XBee: alet sockete kondu
            if (xbee != null)
                xbee.AletSoketeKonduMesajiGonder();

            Debug.Log($"{gameObject.name} doðru yuvaya kilitlendi.");
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (kilitlendi)
            return;

        // Doðru socket býraktýysa izin ver
        if (args.interactorObject is XRSocketInteractor socket &&
            socket == hedefYuva)
            return;

        // Yanlýþ býrakýldýysa elde takip moduna geçir
        StartCoroutine(YanlisBirakildiysaEldeTut());
    }

    private IEnumerator YanlisBirakildiysaEldeTut()
    {
        // Socket'in bu frame objeyi almasýna fýrsat ver
        yield return null;

        if (kilitlendi || isSelected)
            yield break;

        // Doðru socket hemen alacaksa elde takip moduna girme
        if (hedefYuva != null && interactorsHovering.Contains(hedefYuva))
            yield break;

        if (eldeTakipNoktasi == null && sonEl != null)
            eldeTakipNoktasi = sonEl.GetAttachTransform(this);

        if (eldeTakipNoktasi == null)
            yield break;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Hâlâ elde sayýlmaya devam etsin
        eldekiAlet = this;
        eldeTakipModu = true;
    }

    private void OnDisable()
    {
        if (eldekiAlet == this && !kilitlendi)
            eldekiAlet = null;
    }
}