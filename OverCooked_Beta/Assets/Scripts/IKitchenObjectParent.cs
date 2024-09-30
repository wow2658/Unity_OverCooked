using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ClearCounter, Player에 등록
public interface IKitchenObjectParent
{
    public Transform GetKitchenObjectFollowTransform();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public KitchenObject GetKitchenObject();

    public void ClearKitchenObjet();

    public bool HasKitchenObject();
}
