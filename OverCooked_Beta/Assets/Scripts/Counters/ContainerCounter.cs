using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
  

    public override void Interact(Player player)
    {
        #region ClearCounter보다 더 이전의 고쳐지기 전 코드
        //if (kitchenObject == null) // counter에 올려져 있는 음식이 없을 때
        //{
        //    Debug.Log("Interact!");
        //    // 프리팹으로 등록한 음식을 나(카운터)의 counterTopPoint 좌표에 소환(인스턴스화)한다. Instantiate는 부모도 정해준다.
        //    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);

        //    // 프리팹으로 등록한 음식에 등록된 컴포넌트중 KitchenObject.cs의 SetClearCounter 함수를 이용하여 나(카운터)를 기억하라고 던져준다.
        //    // kitchenObject != null 일때는 아래 코드보다 훨씬 간단하게 kitchenObject.SetClearCounter(위치할 카운터) 쓰면 끝이다.
        //    // // 아래의 this에서 오류가 발생했었는데 컨테이너카운터가 아직 IKitchenObjectParent을 상속받지 않아서 발생한 오류였다.
        //    kitchenObjectTransform.GetComponent<KitchenObject>().SetkitchenObjectParent(this);

        //    /* 아래의 코드는 kitchenObjectTransform.GetComponent<KitchenObject>().SetClearCounter(this);이 한번에 대체한다.
        //     * 
        //    // offset없이 어긋나지 않게 정자세로 놓는다. 기준은 parent일텐데 Instantiate에서 counterTopPoint을 parent로 정해준것이다.
        //    kitchenObjectTransform.localPosition = Vector3.zero;

        //    // kitchenObjectTransform는 프리팹인것같다. 그 프리팹은 KitchenObject 스크립트를 컴포넌트로 가지고 있다.
        //    kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();

        //    // 이제 kitchenObject != null 이다.
        //    kitchenObject.SetClearCounter(this);
        //    */

        //}
        //else
        //{
        //    /* // kitchenObject != null 라고 했으니까 어떤 kitchenObject인건지 확인
        //    Debug.Log(kitchenObject.GetClearCounter());
        //    */
        //    // kitchenObject에 인터페이스로 구현된 SetkitchenObjectParent(인터페이스) 활용
        //    // ** pickup하는 핵심코드. secondClearCounter로 test후 interface를 통해 player에 붙이는 것으로 활용되었다.
        //    kitchenObject.SetkitchenObjectParent(player);
        //}
        #endregion
        if(!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);

            OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        }
       
    }
}
