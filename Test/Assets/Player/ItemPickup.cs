using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{

    //player
    [Header("General")]
    [SerializeField] KeyCode PickUpKey = KeyCode.E;
    [SerializeField] float PickUpRange = 4f;
    [SerializeField] float ThrowForce = 10f;
    [SerializeField] Canvas PlayerUI;
    private Text AmmoText;
    public bool bIsPickedUp = false;
    private Item _CurrentItem;
    private GameObject _CurrentItemObject;
    private Player _Player;
    private Transform _WeaponHolster;
    //camera
    private Camera _Camera;
    private Vector3 _AimDir;
    private Vector3 _AimPos;


    void Start()
    {
        _Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _WeaponHolster = _Player.transform.GetChild(0).Find("WeaponHolster");
        //find the main camera
        _Camera = Camera.main;
        AmmoText = PlayerUI.transform.Find("AmmoText").GetComponent<Text>();
        AmmoText.enabled = false;
    }

    void FixedUpdate()
    {

        //set the player direction to the camera direction
        _AimDir = _Camera.transform.forward;
        _AimPos = _Camera.transform.position;
    }
    void Update()
    {

        //if Pickup is pressed then PickupItem but if pressed again then DropItem
        if (Input.GetKeyDown(PickUpKey) && !bIsPickedUp)
        {
            PickupItem();
        }
        else if (Input.GetKeyDown(PickUpKey) && bIsPickedUp)
        {
            ThrowItem();
        }
    }

    //throw the item
    void ThrowItem()
    {
        //throw the current item 

        //if the item was a gun then disable the ammo text
        if (_CurrentItem.GetComponent<FireArm>())
        {
            AmmoText.enabled = false;
        }


        //give the current item no parent 
        _CurrentItem.transform.parent = null;

        //set the current item to not picked up
        bIsPickedUp = false;
        //set the current item to have rigidbody
        _CurrentItem.IsPickedUp = false;
        _CurrentItem.rb.isKinematic = false;
        _CurrentItem.col.enabled = true;
        _CurrentItem.player = null;
        //throw the item in the direction of AimDir with the force of ThrowForce
        _CurrentItem.rb.AddForce(_AimDir * ThrowForce, ForceMode.Impulse);

        _CurrentItem = null;
        _CurrentItemObject = null;


    }
    //pick up item
    void PickupItem()
    {
        RaycastHit hit;
        Vector3 Pos = _AimPos;

        Debug.DrawRay(_AimPos, _AimDir * PickUpRange, Color.red, 2f);

        if (Physics.Raycast(_AimPos, _AimDir, out hit, PickUpRange))
        {

            if (hit.collider.gameObject.GetComponent<Item>() && hit.collider.gameObject.GetComponent<Item>().IsPickedUp == false)
            {
                print("hit " + hit.collider.gameObject.name);

                _CurrentItemObject = hit.collider.gameObject;
                _CurrentItem = hit.collider.gameObject.GetComponent<Item>();
                _CurrentItem.player = _Player;


                _CurrentItem.rb.isKinematic = true;
                _CurrentItem.col.enabled = false;
                bIsPickedUp = true;
                _CurrentItem.IsPickedUp = true;
                _CurrentItem.transform.SetParent(_WeaponHolster);
                //set the position of the item to the weapon holster
                _CurrentItem.transform.position = _WeaponHolster.position;
                //set the rotation of the item to the weapon holster
                _CurrentItem.transform.rotation = _WeaponHolster.rotation;

                //if the item is a gun then update the ammo text and show it
                if (_CurrentItem.GetComponent<FireArm>())
                {
                    AmmoText.enabled = true;

                    _CurrentItem.GetComponent<FireArm>().UpdateAmmoText(_CurrentItem.GetComponent<FireArm>().CurrentAmmo);

                }
            }

        }
    }
}


