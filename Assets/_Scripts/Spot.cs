using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Spot : MonoBehaviour
{
    public Image img;

    public int SpotID;
    public Item item;

    public UnityEvent OnSelectedEvents;
    public UnityEvent OnDesecedEvents;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    public void GetSelected()
    {
        OnSelectedEvents.Invoke();
//        item?.GetSelected();

        img.color = Color.red;
    }
    public void GetDeSelected()
    {
        OnDesecedEvents.Invoke();
        item?.GetDeSelected();

        img.color = Color.white;
    }

    public Item GetItem()
    {
        return item;
    }
}
