using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    public enum Mode
    {
        semiauto,
        auto
    }
    public bool DelayOver => timeSinceShot > shotDelay;
    public Mode FireMode => fireMode;

    [SerializeField] private Mode fireMode;
    [SerializeField] private float shotDelay;

    private float timeSinceShot;

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
        if (!DelayOver)
        {
            Debug.LogWarning("Tried shooting " + this + ", but timesinceshot is " + timeSinceShot + "; less than " + shotDelay);
            return;
        }
        timeSinceShot = 0f;

        GetComponent<Gun>()?.Shoot();
        GetComponent<Melee>()?.Shoot();
    }
}
