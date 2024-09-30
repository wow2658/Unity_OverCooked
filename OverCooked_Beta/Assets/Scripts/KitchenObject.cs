using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{
    // ScriptableObject는 프리팹에 등록이 안된다. 그래서 MonoBehaviour버전의 KitchenObject을 통해 등록한다.
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    // private ClearCounter clearCounter;
    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    // test용도로 T키를 누르면 ClearCounter.cs에서 SetClearCounter의 clearCounter인자로 세컨드를 넘겨준다.
    // 세컨드는 디버깅용도로 에디터에서 직접 지정해준 것이다.
    // *아주 중요한 코드다. counter도 get set을 이걸 참조해서 사용한다.
    public void SetkitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        if(this.kitchenObjectParent != null)
        {
            // Set하려고 했는데 이미 뭐가 자리차지하고 있는 상태면 Clear한다.
            // 토마토가 counter에서 다른 counter로 이사갈때 parent를 업데이트 해주기 위해서 비워두고 대비하는 것 같다.
            // 그래야 Set 할 수 있으니까
            this.kitchenObjectParent.ClearKitchenObjet();
        }

        // Set. 음식이 카운터를 알게하는 부분. 카운터가 음식을 알게하는 부분은 어디일까? 여기 아래에 있다.
        this.kitchenObjectParent = kitchenObjectParent;

        if(kitchenObjectParent.HasKitchenObject())
        {
            // 이미 다른 음식이 올려져 있는 counter라면 Error
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }

        // Set. 카운터가 음식을 알게하는 부분.
        kitchenObjectParent.SetKitchenObject(this);

        // 최초에는 카운터가 Instantiate으로 counterTopPoint을 parent로 정해주고
        // 그 다음부터는 아래코드가 parent로 업데이트된다.
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        // offset없이 어긋나지 않게 정자세로 놓는다. 기준은 parent이다.
        // 원래 카운터에 있던 코드인데 음식으로 왔다.
        transform.localPosition = Vector3.zero;
    }

    // public ClearCounter GetClearCounter()
    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroySelf()
    {
        kitchenObjectParent.ClearKitchenObjet();
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetkitchenObjectParent(kitchenObjectParent);

        return kitchenObject;
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

}
