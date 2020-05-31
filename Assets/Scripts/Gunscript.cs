using UnityEngine;

public class Gunscript : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;

    public Camera fpsCam;
    public ParticleSystem muzzFlash;
    public GameObject impactEffect;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shot();
        }

    }
    public void Shot()
    {
        //RaycastHit hit;
        //if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        //{
        //    Debug.Log(hit.transform.name);
        //}

        muzzFlash.Play();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            ITakeDamage damagable = hitInfo.collider.GetComponent<ITakeDamage>();
            if (damagable != null)
            {
                damagable.ITakeDamage(damage);
            }
        }
        Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
    }
}
