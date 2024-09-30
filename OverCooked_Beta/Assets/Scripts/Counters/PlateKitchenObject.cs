using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{
    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs { public KitchenObjectSO kitchenObjectSO; }

    [SerializeField] private List<KitchenObjectSO> validkitchenObjectSOList;

    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake()
    {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public void AddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        kitchenObjectSOList.Add(kitchenObjectSO);
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO)
    {
        // 접시에 담을 수 있는 valid한 SO가 아니면 false
        if (!validkitchenObjectSOList.Contains(kitchenObjectSO)) return false;

        // 이미 접시에 올린 List에 포함되어 있으면 false
        if (kitchenObjectSOList.Contains(kitchenObjectSO)) return false;
        else
        {
            // 담을 수 있으면서 처음 접시에 담는것이면 true
            kitchenObjectSOList.Add(kitchenObjectSO);

            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs { kitchenObjectSO = kitchenObjectSO });
            return true;
        }
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() { return kitchenObjectSOList; }

}
