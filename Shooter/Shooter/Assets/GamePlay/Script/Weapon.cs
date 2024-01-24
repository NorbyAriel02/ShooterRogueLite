using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform spawn;
    public float shotRate = 1.0f;
    public float impulseforce = 1500f;
    public float delay = 0f;
    private List<GameObject> bullets;
    private void Start()
    {
        int amount = ObjectPreloadingHelper.CalculateAmount(bulletPrefab.GetComponent<Bullet>().timeLife, shotRate);
        bullets = ObjectPreloadingHelper.Load(bulletPrefab, null, spawn.position, spawn.rotation, amount);
    }
    
    public void Shoot()
    {
        if(delay > 0.0f)
            return;

        delay = shotRate;
        GameObject bullet = ObjectPreloadingHelper.GetObject(bullets);
        if(bullet != null)
            SetBullet(bullet);
    }

    private void SetBullet(GameObject bullet)
    {        
        bullet.transform.position = spawn.position;
        bullet.transform.rotation = spawn.rotation;
        bullet.SetActive(true);
        Bullet b = bullet.GetComponent<Bullet>();
        b.GetRB().AddForce(spawn.forward *  impulseforce);
    }

    private void FixedUpdate()
    {
        delay -= Time.deltaTime;
    }
}
