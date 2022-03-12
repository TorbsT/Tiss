using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pools;
using UnityEngine.Rendering.Universal;
using EzPools;

public class Gun : MonoBehaviour
{
    public enum Mode
    {
        semiauto,
        auto
    }
    public bool CanShoot => timeSinceShot > gunSO.ShotDelay;
    public Mode FireMode => fireMode;

    [SerializeField] private GameObject flashPrefab;
    [SerializeField] private GunSO gunSO;
    [SerializeField] private BulletSO bulletSO;
    [SerializeField] private Transform shotOrigin;
    [SerializeField] private FlashObject flashObject;
    [SerializeField] private Mode fireMode;
    private float timeSinceShot;


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
        timeSinceShot += Time.deltaTime;
    }

    public void Shoot()
    {
        if (!CanShoot)
        {
            Debug.LogWarning("Tried shooting " + this + ", but timesinceshot is " + timeSinceShot + " - less than " + gunSO.ShotDelay);
            return;
        }
        timeSinceShot = 0f;

        GameObject flash = Manager.Instance.Depool(flashPrefab);
        flash.transform.position = shotOrigin.position;
        flash.GetComponent<Flash>().FlashObject = flashObject;

        // Factory pattern?
        for (int i = 0; i < gunSO.ShotCount; i++)
        {
            Bullet bullet = BulletPool.Instance.Depool();
            bullet.SO = bulletSO;
            bullet.transform.position = shotOrigin.position;
            bullet.transform.rotation = shotOrigin.rotation;
            float angle = Random.Range(-gunSO.ShotSpread, gunSO.ShotSpread);
            bullet.transform.RotateAround(shotOrigin.position, shotOrigin.forward, angle);
        }
    }
}
