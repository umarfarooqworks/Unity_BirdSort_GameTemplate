using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Image img;

    public enum ItemState
    {
        CanBeSelected,
        CanNotBeSelected
    }

    public ItemState itemState;

    public int ItemID;

    // 0 = parrot
    // 1 = kuwail
    // 2 = sparrow

    public UnityEvent OnSelectedEvents;
    public UnityEvent OnDesecedEvents;

    private void Start()
    {
        UpdateItemImage();
    }

    void UpdateItemImage()
    {
        foreach (Transform x in transform)
            x.gameObject.SetActive(false);

        if(ItemID != -1)
        {
            img = transform.GetChild(ItemID).GetComponent<Image>();
            img.gameObject.SetActive(true);
        }
    }

    public void GetSelected()
    {
        OnSelectedEvents.Invoke();
        SetColor(Color.green);
    }
    public void GetDeSelected()
    {
        OnDesecedEvents.Invoke();
        SetColor(Color.white);
    }

    void SetColor(Color c)
    {
        if(img != null)
            img.color = c;
    }

    public void StartSwap()
    {

    }
    public void EndSwap()
    {

    }


    private void OnValidate()
    {
        if (ItemID > 3)
            ItemID = 3;
        else if (ItemID < -1)
            ItemID = -1;

        gameObject.name = "Item_Bird_" + ItemID;
        if (ItemID != -1)
            img = transform.GetChild(ItemID).GetComponent<Image>();
        else if(ItemID == -1)
        {
            img = null;
        }
        UpdateItemImage();
    }
}

