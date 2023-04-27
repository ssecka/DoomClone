using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooting : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bullet;
    public float shootForce, upwardForce;
    
    [Header("Gun Stats")]
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    private int bulletsLeft, bulletsShot;
    private bool shooting, readyToShoot, reloading;
    public Rigidbody playerRb;
    public float recoilForce;
    public int bulletDamage;

    [Header("References")] 
    public Camera camera;
    public Transform GunPoint;
    public TextMeshProUGUI ammoDisplay;
   




    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        
        if(ammoDisplay != null)
            ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
    }

    private void MyInput()
    {
        shooting = Input.GetKey(KeyCode.Mouse0);
        
        //Reloading
        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)Reload();
        
        //Reload automaticly when magazine is empty
        if(readyToShoot && shooting && !reloading && bulletsLeft <= 0)Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {

        readyToShoot = false;

        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Point Far Away from the Player
        
        //Calculate direction

        Vector3 directionWithoutSpread = targetPoint - GunPoint.position;
        
        //calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
        
        //Instantiate bulllet/projectile
        GameObject currentBullet = Instantiate(bullet, GunPoint.position, Quaternion.identity);
        
        //Rotate bulllet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;
        
        //Add Force
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(camera.transform.up * upwardForce, ForceMode.Impulse);
        
        
        
        bulletsLeft--;
        bulletsShot++; 
        
        
            Invoke("ResetShot", timeBetweenShooting);
            
            //Add recoil to player (should only be called once)
            //Shooting with recoil results in grappling hook missing the target 
            //playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
