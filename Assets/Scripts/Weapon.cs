using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Throwing")]
    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;

    [Header("Shooting")]
    public int maxAmmo;
    public int shotsPerSecond;
    public float relodSpeed;
    public float hitForce;
    public float range;
    public bool tapable;
    public float kickbackForce;
    public float resetSmooth;
    public Vector3 scopePos;
    public int weaponOfKind;
    public ParticleSystem muzzFlash;
    public GameObject impactEffect;

    [Header("Data")]
    public int weaponGfxLayer;
    public GameObject[] weaponGfxs;
    public Collider[] gfxColliders;
    public Transform primeHolder, secondHolder, meleeHolder;
   






    private float _rotationTime;
    private float _time;
    private bool _held;
    private bool _scoping;
    private Rigidbody _rb;
    private bool _reloading;
    private int _ammo;
    private bool _shooting;
    private Transform _playerCamera;
    public TMP_Text _ammoText;
    private float anyTime;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private float animTime;

    private void Start()
    {
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 0.1f;
        _ammo = maxAmmo;
    }

    private void Update()
    {




        if (!_held)
        {
            return;
        }

        if (_time < animTime)
        {
            _time += Time.deltaTime;
            _time = Mathf.Clamp(_time, 0f, animTime);
            var delta = -(Mathf.Cos(Mathf.PI * (_time / animTime)) - 1f) / 2f;
            transform.localPosition = Vector3.Lerp(_startPosition, Vector3.zero, delta);
            transform.localRotation = Quaternion.Lerp(_startRotation, Quaternion.identity, delta);
        }
        else
        {
            _scoping = Input.GetMouseButton(1) && !_reloading;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _scoping ? scopePos : Vector3.zero, resetSmooth * Time.deltaTime);
        }





        if (_reloading)
        {
            _rotationTime += Time.deltaTime;
            var spinDelta = -(Mathf.Cos(Mathf.PI * (_rotationTime / relodSpeed)) - 1f) / 2f;
            transform.localRotation = Quaternion.Euler(new Vector3(spinDelta * 360f, 0, 0));
        }



        if (Input.GetKeyDown(KeyCode.R) && !_reloading && _ammo < maxAmmo)
        {
            StartCoroutine(RelodCooldown());
        }

        if ((tapable ? Input.GetMouseButton(0) : Input.GetMouseButton(0)) && !_shooting && !_reloading)
        {
            _ammo--;
            _ammoText.text = _ammo + " / " + maxAmmo;
            Shoot();
            StartCoroutine(_ammo <= 0 ? RelodCooldown() : ShootingCooldown());
        }

    }

    private void Shoot()
    {
        transform.localPosition -= new Vector3(0, 0, kickbackForce);
        if (!Physics.Raycast(_playerCamera.position, _playerCamera.forward, out var hitinfo, range))
        {
            return;
        }

        var rb = hitinfo.transform.GetComponent<Rigidbody>();
        if (rb == null)
        {
            return;
        }

        muzzFlash.Play();
        rb.velocity += _playerCamera.forward * hitForce;
    }

    private IEnumerator ShootingCooldown()
    {
        _shooting = true;
        yield return new WaitForSeconds(1f / shotsPerSecond);
        _shooting = false;

    }
    private IEnumerator RelodCooldown()
    {
        _reloading = true;
        _ammoText.text = "RELOADING";
        _rotationTime = 0f;
        yield return new WaitForSeconds(relodSpeed);
        _ammo = maxAmmo;
        _ammoText.text = _ammo + " / " + maxAmmo;
        _reloading = false;
    }


    public void Pickup(Transform weaponHolder, Transform plaerCamera, TMP_Text ammoText)
    {
        if (_held)
        {
            return;
        }

        Destroy(_rb);
        //transform.parent = weaponHolder;
        WeaponKindGrab();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        foreach (var col in gfxColliders)
        {
            col.enabled = false;
        }
        foreach (var gfx in weaponGfxs)
        {
            gfx.layer = weaponGfxLayer;
        }

        _held = true;
        _playerCamera = plaerCamera;
        _ammoText = ammoText;

        _ammoText.text = _ammo + " / " + maxAmmo;
        _scoping = false;

    }

    public void Drop(Transform plaerCamera)
    {
        if (!_held)
        {
            return;
        }

        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 0.1f;
        var forward = plaerCamera.forward;
        forward.y = 0f;
        _rb.velocity = forward * throwForce;
        _rb.velocity += Vector3.up * throwExtraForce;
        _rb.angularVelocity = Random.onUnitSphere * rotationForce;

        foreach (var col in gfxColliders)
        {
            col.enabled = true;
        }

        foreach (var gfx in weaponGfxs)
        {
            gfx.layer = 0;
        }

        _ammoText.text = "";
        transform.parent = null;
        _held = false;

    }

    public void WeaponKindGrab()
    {
        switch (weaponOfKind)

        {
            case 1:
                transform.parent = primeHolder;
                break;
            case 2:
                transform.parent = secondHolder;
                break;
            default:
                transform.parent = meleeHolder;
                break;


        }
    }

    public bool Scoping => _scoping;
}
