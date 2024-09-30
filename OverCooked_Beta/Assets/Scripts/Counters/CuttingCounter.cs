using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter
{
    public static event EventHandler OnAnyCut;

    public event EventHandler OnCut;


    //[SerializeField] private KitchenObjectSO cutKitchenObjectSO;
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        // 빈 카운터일 때
        {
            if (player.HasKitchenObject() && HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) // player가 음식들고 빈 카운터에 E 누른다면
                                                                                                                 // 캐릭터가 들고있는걸 판별했을때 모르는건 놓지 말자
            {
                // 그 카운터에 음식을 놓는다.
                player.GetKitchenObject().SetkitchenObjectParent(this);
            }
            else // player가 빈 손으로 빈 카운터에 E 누른다면
            {
                // Nothing
            }
        }
        else
        // 뭐가 올려져 있는 카운터일 때
        {
            if (player.HasKitchenObject()) // player가 뭔가를 들고 있는채로 음식있는 카운터에 E를 누르면
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) // 플레이어가 들고있는게 접시라면
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();

                    }
                }
            }
            else // player가 빈 손으로 음식이 올려져있는 카운터에 E 누른다면
            {
                // 올려져 있는 음식을 player가 집어든다.
                GetKitchenObject().SetkitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))      // 뭐가 올려져 있는 카운터일 때
                                                                                                    // 도마위에 올려져있는걸 판별했을때 이미 자른건 자르지 말자
        {
            OnAnyCut?.Invoke(this, EventArgs.Empty);
            OnCut?.Invoke(this, EventArgs.Empty);
            // 올려져 있는 음식이 input일때 그것와 짝을 이루는 output을 저장. Destroy하기 전에 찾아야함.
            KitchenObjectSO outputkitchenObjectSO =  GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
            // 올려져 있는 음식을 Destroy.
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(outputkitchenObjectSO, this);
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return true;
            }
        }
        return false;
    }
    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO.output;
            }
        }
        return null;
    }
}
