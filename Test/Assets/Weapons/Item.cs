using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] Transform Body;
    public string itemName;
    public string itemDescription;
    public bool IsPickedUp = false;
    public Player player;

    public Rigidbody rb;
    public BoxCollider col;


    void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        col.enabled = true;
        rb.isKinematic = false;
        itemName = transform.name;
        //add the item tag
        gameObject.tag = "Item";
    }
}
