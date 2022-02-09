using System.ComponentModel;
using System.Globalization;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(Item))]
public class FireArm : MonoBehaviour
{

    [Header("General")]
    [SerializeField] KeyCode FireKey;
    [SerializeField] int FireMouse = 0;
    [SerializeField] float Damage = 10f;
    [SerializeField] float Range = 100f;
    [SerializeField] float FireRate = .5f;
    [SerializeField] float ImpactForce = 10f;
    [Header("Aiming")]
    [SerializeField] float AimFOV = 30f;
    [SerializeField] float AimFOVNorm = 60f;
    [SerializeField] int AimMouse = 1;
    [SerializeField] float AimSensitivity = 5f;
    private Transform AimPos;
    [Header("Ammo")]
    [Range(0, Mathf.Infinity)][SerializeField] int MaxAmmo = 10;
    [Range(0, Mathf.Infinity)] public int CurrentAmmo = 0;
    [Header("Reload")]
    [SerializeField] bool bReloadHole = false;
    [SerializeField] KeyCode ReloadKey = KeyCode.R;
    [SerializeField] float ReloadTime = 1f;
    [Header("Fire")]
    [SerializeField] Transform FirePoint;
    [SerializeField] bool FullAuto = false;
    private Vector3 DirToFire;
    [Header("UI")]
    [SerializeField] Canvas PlayerCanvas;

    [Header("VFX")]
    [SerializeField] ParticleSystem MuzzleFlash;
    [SerializeField] GameObject ImpactEffect;

    //private shit
    private Text AmmmoText;
    private bool bCanShoot = true;
    private bool bIsReloading = false;
    private Transform GunHolster;
    private Player _Player;
    private Animator _Animator;
    private AudioSource _AudioSource;
    private Camera _Camera;
    private ParticleSystem _Bullet;
    private Item _Item;


    void Start()
    {

        AmmmoText = PlayerCanvas.GetComponentInChildren<Text>();
        //get the player from the tag "Player"
        _Animator = GetComponent<Animator>();
        _AudioSource = GetComponent<AudioSource>();
        _Camera = Camera.main;
        _Bullet = GetComponentInChildren<ParticleSystem>();
        _Item = GetComponent<Item>();
    }



    void Update()
    {
        DirToFire = FirePoint.transform.forward;

        if (_Item.IsPickedUp)
        {
            PlayerChange(_Item.player);
        }

        if (_Item.IsPickedUp && _Item != null)
        {
            //fire the gun
            if ((Input.GetMouseButtonDown(FireMouse) || Input.GetKeyDown(FireKey)) && (CurrentAmmo > 0) && bCanShoot)
            {
                PlayMuzzleFlash();
                StopAllCoroutines();
                Fire();
            }
            else if (FullAuto && (Input.GetMouseButton(FireMouse) || Input.GetKey(FireKey)) && (CurrentAmmo > 0) && bCanShoot)
            {
                PlayMuzzleFlash();
                StopAllCoroutines();
                FireFullAuto();
            }
            //relod the gun
            else if (Input.GetKeyDown(ReloadKey) && !bIsReloading)
            {
                bIsReloading = true;
                if (bReloadHole)
                {
                    StartCoroutine(ReloadHole());
                }
                else
                {
                    StartCoroutine(ReloadSingle());
                }
            }
            AimPlayer();
        }
    }

    public void PlayerChange(Player player)
    {
        _Player = player;
        GunHolster = _Player.transform.GetChild(0).Find("WeaponHolster");
        AimPos = GunHolster.Find("AimPos");
    }

    void FireFullAuto()
    {
        bIsReloading = false;
        CurrentAmmo--;
        //update the ammo text
        UpdateAmmoText(CurrentAmmo);
        RaycastHit hit;
        StartCoroutine(ShootDelay());
        //apply force to the shot bullet
        Debug.DrawRay(FirePoint.position, DirToFire * Range, Color.red);
        //shoot raycast
        if (Physics.Raycast(FirePoint.position, DirToFire, out hit, Range))
        {
            CreateHitImpact(hit);
            //check if an enemy
            if (hit.transform.GetComponent<Enemy>())
            {
                hit.transform.GetComponent<Enemy>().TakeDamage(Damage);
            }
            //apply force to the object
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(DirToFire * (ImpactForce));
            }
            //spawn the impact effect

        }


        bCanShoot = false;

    }

    void Fire()
    {
        bIsReloading = false;
        CurrentAmmo--;
        //update the ammo text
        UpdateAmmoText(CurrentAmmo);
        RaycastHit hit;
        StartCoroutine(ShootDelay());
        //apply force to the shot bullet
        Debug.DrawRay(FirePoint.position, DirToFire * Range, Color.red);
        //shoot raycast
        if (Physics.Raycast(FirePoint.position, DirToFire, out hit, Range))
        {
            CreateHitImpact(hit);
            //check if an enemy
            if (hit.transform.GetComponent<Enemy>())
            {
                hit.transform.GetComponent<Enemy>().TakeDamage(Damage);
            }
            //apply force to the object
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(DirToFire * (ImpactForce));
            }
            //spawn the impact effect

        }


        bCanShoot = false;

    }

    private void PlayMuzzleFlash()
    {
        MuzzleFlash.Play();
    }
    private void CreateHitImpact(RaycastHit hit)
    {
        GameObject Impact = Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(Impact, .1F);
    }


    void AimPlayer()
    {
        //zoom the camera to the guns scope if the mouse is held down
        //move the gun to the center of the screen
        if (Input.GetMouseButton(AimMouse))
        {
            //turn the cross Hair off
            PlayerCanvas.GetComponentInChildren<Image>().enabled = false;
            _Camera.fieldOfView = Mathf.Lerp(_Camera.fieldOfView, AimFOV, Time.deltaTime * AimSensitivity);
            transform.position = Vector3.Lerp(transform.position, AimPos.position, Time.deltaTime * AimSensitivity);
        }
        else
        {
            //turn the cross Hair on
            PlayerCanvas.GetComponentInChildren<Image>().enabled = true;

            _Camera.fieldOfView = Mathf.Lerp(_Camera.fieldOfView, AimFOVNorm, Time.deltaTime * AimSensitivity);
            transform.position = Vector3.Lerp(transform.position, GunHolster.position, Time.deltaTime * AimSensitivity);
        }


    }


    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(FireRate);
        bCanShoot = true;
    }

    IEnumerator ReloadSingle()
    {
        while (CurrentAmmo < MaxAmmo && (CurrentAmmo > -1))
        {
            CurrentAmmo++;
            //update the ammo text
            UpdateAmmoText(CurrentAmmo);

            yield return new WaitForSeconds(ReloadTime);

        }
        bIsReloading = false;
    }

    IEnumerator ReloadHole()
    {
        CurrentAmmo = MaxAmmo;
        UpdateAmmoText(CurrentAmmo);
        yield return new WaitForSeconds(ReloadTime);

    }
    public void UpdateAmmoText(int Ammount)
    {
        AmmmoText.text = "Ammo: " + Ammount.ToString();
    }

};
