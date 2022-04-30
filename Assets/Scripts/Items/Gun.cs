using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;
using UnityEngine.Rendering.Universal;

public class Gun : MonoBehaviour
{

    public GunSO GunSO => gunSO;

    [SerializeField] private GameObject flashPrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GunSO gunSO;
    [SerializeField] private BulletSO bulletSO;
    [SerializeField] private Transform shotOrigin;
    [SerializeField] private FlashObject flashObject;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Shoot()
    {
        GameObject flash = EzPools.Instance.Depool(flashPrefab);
        flash.transform.position = shotOrigin.position;
        flash.GetComponent<Flash>().FlashObject = flashObject;

        // Factory pattern?
        for (int i = 0; i < gunSO.ShotCount; i++)
        {
            Bullet bullet = EzPools.Instance.Depool(bulletPrefab).GetComponent<Bullet>();
            bullet.SO = bulletSO;
            bullet.transform.position = shotOrigin.position;
            bullet.transform.rotation = shotOrigin.rotation;
            float angle = Random.Range(-gunSO.ShotSpread, gunSO.ShotSpread);
            bullet.transform.RotateAround(shotOrigin.position, shotOrigin.forward, angle);
        }
    }
    
}
