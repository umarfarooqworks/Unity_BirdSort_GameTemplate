using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;
    
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            if (instance == null)
                Debug.Log("instance is null");
            else
                Debug.Log("instance is NOT null");
        }
    }

    public event Action<int> onBranchClick; // gets called when we click on a branch
    public void OnBranchClicked(int ID)
    {
        onBranchClick.Invoke(ID);
    }


    //List<Item>
    public event Action<int, List<Item>> onBranchItemsSelected;
    public void OnBranchItemsSelected(int branchID, List<Item> itmes)
    {
        onBranchItemsSelected.Invoke(branchID, itmes);
    }
}
