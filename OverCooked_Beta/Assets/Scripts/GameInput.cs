using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    // 델리게이트랑 비슷한 개념인것 같은데, Player.cs의 Start에서 구독하고 호출한다.
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    // package가 알아서 이어주는건지?? 에디터에서 딱히 등록하지 않았는데 알아서 인식되었다.
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        // 이 코드내에서 쓸수 있도록 class 생성자 호출하고 그릇에 담기
        playerInputActions = new PlayerInputActions();
        // Enable이라는 함수 발동시켜서 쓸 수 있도록 만드는 코드같다.
        playerInputActions.Player.Enable();

        // 구독 Add와 Remove의 개념으로 동작하는것 같다.
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed;

    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //if(OnInteractAction != null)
        //{
        //    OnInteractAction(this, EventArgs.Empty);
        //}
        OnInteractAction?.Invoke(this, EventArgs.Empty); // 위의 코드를 줄인것이라고 한다.
    }

    // Player.cs의 Update에서 호출한다.
    public Vector2 GetMovementVectorNormalized()
    {
        // Vector2 inputVector = new Vector2(0, 0); 이제 안쓰는 코드.

        // 에디터에서 Apply해서 편리하게 생성된 복잡한 코드로 이루어진 playerInputActions.cs가 있다.
        // 그 친구가 Player->Move값을 키보드를 통해 인식해서 Vector2를 만들어낸것을
        // Read해서 이쪽에서 쓸 수 있게 담아내는 코드인것 같다.
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();


        //if (Input.GetKey(KeyCode.W)) 이제 안쓰는 코드.
        //{
        //    inputVector.y = +1;
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    inputVector.y = -1;
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    inputVector.x = -1;
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    inputVector.x = +1;
        //}

        inputVector = inputVector.normalized;
        return inputVector;

    }
}
