using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemData : MonoBehaviour, IInteracted
{
    public List<Items> dropItems; // Reference to the Item scriptable object
    public Collider pouchCollider;
    public Transform contentInfo;
    public GameObject itemdispprefab;
    public GameObject canvas;
    public bool lookingAtPouch;
    public int itemDropCount;
    public int goldAmount;
    private void Start()
    {
        itemDropCount = Random.Range(1, 4);
        goldAmount = Random.Range(0, 21);
        for (int i = 0; i < itemDropCount; i++)
        {
            int itemDrop = Random.Range(0, GameManager.instance.everyItem.Count);
            dropItems.Add(GameManager.instance.everyItem[itemDrop]);
            GameObject obj = Instantiate(itemdispprefab, contentInfo);
            Image itemimg = obj.GetComponent<Image>();
            itemimg.sprite = dropItems[i].itemIcon;
        }
        SetPositionToNearestGround();
    }
    private void Update()
    {
        Mathf.Cos( Time.deltaTime );
        lookingAtPouch = false;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 5f))
        {
            if (hit.collider == pouchCollider) lookingAtPouch = true;
        }

        canvas.SetActive(lookingAtPouch);
        if (lookingAtPouch)
            canvas.transform.LookAt(Camera.main.transform);
    }
    public void SetPositionToNearestGround()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Scenario"), QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(Vector3.forward,hit.normal));
            }
            else
            {
                Debug.LogError("SetPositionToNearestGround did not found a ground", transform);
            }
        }
    }
    public void Interacted() 
    {
        Inventory inventory = GameObject.FindAnyObjectByType<Inventory>();
        if (inventory != null && dropItems.Count != 0)
        {
            foreach (var i in dropItems)
            {
                inventory.AddItem(i);
            }
            inventory.ListItems();
        }
        if (!(goldAmount <= 0))
        {
            GameManager.instance.ChangeCurrencyAmount(goldAmount);
        }
        Destroy(gameObject);
    }

}
