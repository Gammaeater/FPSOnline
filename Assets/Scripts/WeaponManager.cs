﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [Header("PickUpWeapon")]
    public float pickupRange;
    public float picupRadius;

    public int weaponLayer;
    public float swaySize;
    public float swaySmooth;


    public float defaultFov;
    public float scopedFov;
    public float fovSmooth;



    public Transform weaponHolder;
    public Transform playerCamera;
    public Transform swayHolder;



    public bool _isWeaponHeld;
    private Weapon _heldWeapon;
    public TMP_Text ammoText;
    public Camera[] playerCams;
    public Image crosshairImage;


    private void Update()
    {
        if (crosshairImage != null)
        {
            crosshairImage.gameObject.SetActive(!_isWeaponHeld || !_heldWeapon.Scoping); //<- nullrE
        }
        else
        {
            return;
}

        foreach (var cam in playerCams)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _isWeaponHeld && _heldWeapon.Scoping ? scopedFov : defaultFov, fovSmooth * Time.deltaTime);
        }


        //if (_isWeaponHeld)
        //{
        var mouseDelta = -new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        swayHolder.localPosition = Vector3.Lerp(swayHolder.localPosition, Vector3.zero, swaySmooth * Time.deltaTime);
        swayHolder.localPosition += (Vector3)mouseDelta * swaySize;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_heldWeapon != null)
            {
                _heldWeapon.Drop(playerCamera);
                _heldWeapon = null;
                _isWeaponHeld = false;
            }
            else
            {
                return;
            }
        }
        // }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            var hitList = new RaycastHit[256];
            var hitNumber = Physics.CapsuleCastNonAlloc(playerCamera.position, playerCamera.position + playerCamera.forward * pickupRange,
                picupRadius, playerCamera.forward, hitList);


            var realList = new List<RaycastHit>();
            for (int i = 0; i < hitNumber; i++)
            {
                var hit = hitList[i];
                if (hit.transform.gameObject.layer != weaponLayer)
                {
                    continue;
                }

                if (hit.point == Vector3.zero)
                {
                    realList.Add(hit);
                    Debug.Log(hit + "<color=green>WeaponManager Added realList hit</color>");
                    Debug.Log(hit + "<color=green> <--added</color>");
                }
                else if (Physics.Raycast(playerCamera.position, hit.point = playerCamera.position, out var hitInfo, hit.distance + 0.1f) && hitInfo.transform == hit.transform)
                {
                    realList.Add(hit);
                    Debug.Log(hit + "<color=red>WeaponManager else if Added realList hit</color>");
                }


            }
            if (realList.Count == 0)
            {
                return;
            }

            realList.Sort((hit1, hit2) =>
            {

                var dist1 = GetDistanceTo(hit1);
                var dist2 = GetDistanceTo(hit2);
                return Mathf.Abs(dist1 - dist2) < 0.001f ? 0 : dist1 < dist2 ? -1 : 1;

            });

            _isWeaponHeld = true;
            _heldWeapon = realList[0].transform.GetComponent<Weapon>();
            _heldWeapon.Pickup(weaponHolder, playerCamera, ammoText);
            //weaponHolder


        }

    }
    private float GetDistanceTo(RaycastHit hit)
    {
        return Vector3.Distance(playerCamera.position, hit.point == Vector3.zero ? hit.transform.position : hit.point);
    }
}
