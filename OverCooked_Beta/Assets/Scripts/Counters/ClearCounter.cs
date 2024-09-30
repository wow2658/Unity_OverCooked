using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    //[SerializeField] private Transform tomatoPrefabs;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    /* [SerializeField] private ClearCounter secondClearCounter;
     * [SerializeField] private bool texting;
    private void Update()
    {
        if (texting && Input.GetKeyDown(KeyCode.T))
        {
            if (kitchenObject != null)
            {
                kitchenObject.SetkitchenObjectParent(secondClearCounter);

                // Debug.Log(kitchenObject.GetClearCounter());
                // 디버그만 찍는게 아니라 실질적으로 움직이게 하려면 어떻게 해야할까?
                // 모든 ClearCount는 counterTopPoint를 갖는다.
                // 그러니 ClearCount에게 자신의 counterTopPoint의 Transform를 알려주는 기능을 구현해서 통신할 수 있게 하자.
                // transform.parent을 세컨드로 최신화하고
                // transform.localPosition = Vector3.zero; 으로 위치를 재설정하게 한다.
                // parent는 Instantiate에서 정해준다.
                
            }
        }
    }
    */
    // Player에서 참조해서 사용한다.
    // 플레이어를 인자를 어디서 어떻게 받아오나 궁금했는데
    // Player.cs의 private void GameInput_OnInteractAction(object sender, System.EventArgs e)에서 
    // selectedCounter.Interact(this); 으로 인식이 가능해진것이었다.

    public override void Interact(Player player)
    {
        #region 그냥 삭제해버리는 부분
        //if (!HasKitchenObject()) // counter에 올려져 있는 음식이 없을 때
        //{
        //    Debug.Log("Interact!");
        //    // 프리팹으로 등록한 음식을 나(카운터)의 counterTopPoint 좌표에 소환(인스턴스화)한다. Instantiate는 부모도 정해준다.
        //    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        //    // 프리팹으로 등록한 음식에 등록된 컴포넌트중 KitchenObject.cs의 SetClearCounter 함수를 이용하여 나(카운터)를 기억하라고 던져준다.
        //    // kitchenObject != null 일때는 아래 코드보다 훨씬 간단하게 kitchenObject.SetClearCounter(위치할 카운터) 쓰면 끝이다.
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
        if (!HasKitchenObject())
        // 빈 카운터일 때
        {
            if (player.HasKitchenObject()) // player가 음식들고 빈 카운터에 E 누른다면
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
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) // 카운터에 올려져있는 음식을 List에 추가하고
                    {
                        GetKitchenObject().DestroySelf(); // 카운터에 올려져있는 것 삭제
                    }
                }
                else // 들고있는게 접시가 아니면 (음식)
                {
                    if(GetKitchenObject().TryGetPlate(out plateKitchenObject)) // 카운터에 올려져있는게 접시일 때
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) // 내 손에 있는 음식을 접시 List에 추가하고
                        {
                            player.GetKitchenObject().DestroySelf(); // 내 손에 있는 것 삭제
                        }
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



}
