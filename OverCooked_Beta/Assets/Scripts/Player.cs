using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    /* 싱글톤 보일러플레이트
    private static Player instance;
    public static Player Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    */
    public static Player instance { get; private set; } // 위의 코드와 똑같이 작동한다. 싱글톤은 멀티플레이어에서 안쓴다고 한다. 나중에 리팩토링해야함을 인지.
                                                        // SelectedCounterVisual의 Start에서 이걸 가져다가 OnSelectedCounterChanged를 구독해버린다.
    /* 싱글톤관련 필드 좀 쓰나 싶더니 지워버렸다.
     * 
    public static Player instanceField;
    public static Player GetInstanceField()
    {
        return instanceField;
    }
    public static void SetInstanceField(Player instanceField)
    {
        Player.instanceField = instanceField;
    }
    */

    public event EventHandler OnpickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs // 이렇게 시그니처를 업그레이드하면 Interact_performed의 EventArgs.Empty와는 다를 것이다.
    {
        public BaseCounter selectedCouner;
    }

    // private인 변수도 에디터에서 수정할 수 있게 노출시켜주는 [SerializeField]  
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    //[SerializeField] private Transform counterTopPoint;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    // bool을 담는 그릇. 밑의 Update에서 확정되어 IsWalking()함수를 통해 전달될 예정이다.
    private bool isWalking;
    private Vector3 lastInteractDir;

    private BaseCounter selectedCounter;

    // interface
    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }

        instance = this;
    }

    private void Start()
    {
        // +=까지만 작성하고 탭치면 GameInput_OnInteractAction관련된 것들이 자동완성된다.
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
       
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.InteractAlternate(this);
        }
            
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
        /* 기껏 밑에 있는   private void HandleInteractions() 에서 전부 여기로 복사해오더니 지운다
        //Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        //Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        //if (moveDir != Vector3.zero)
        //{
        //    lastInteractDir = moveDir;
        //}

        //float interactionDistance = 2f;
        //RaycastHit raycastHit;

        //if (Physics.Raycast(transform.position, lastInteractDir, out raycastHit, interactionDistance, countersLayerMask))
        //{
        //    if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
        //    {
        //        clearCounter.Interact(); // 이 코드는 여기에만 남겨두고 HandleInteractions 에서는 지운다.
        //    }
        //}

        */

    }

    private void Update()
    {
        HandleMovement();
        HandleInteractions();
    }

    // 만들어질떄 void가 아니라 bool 함수로 만들어졌다.
    // return 값으로 bool 즉, isWalking을 return해준다.
    public bool IsWalking()
    {
        return isWalking;
    }

    // tick
    private void HandleInteractions()
    {
        // Movement에서 복사해왔다.
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDir != Vector3.zero)
        {
            // 멈춘상태면 아래 코드 동작 x, 업데이트 되지 않음
            lastInteractDir = moveDir;
        }

        float interactionDistance = 2f;
        RaycastHit raycastHit;

        // if(Physics.Raycast(transform.position, moveDir, out raycastHit, interactionDistance))
        // 이제 키보드 안누르고 있어도 활성화 상태를 유지한다.
        if (Physics.Raycast(transform.position, lastInteractDir, out raycastHit, interactionDistance, countersLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                // Has ClearCounter
                // clearCounter.Interact();

                // 처음 select될때
                if (baseCounter != selectedCounter)
                {
                    /* selectedCounter = clearCounter;
                    OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
                    {
                        selectedCouner = selectedCounter
                    });
                    */
                    SetSelectedCounter(baseCounter); // 리팩토링 한방에 해결
                }
      
            }
            else // 레이어 감지는 했는데 clearCounter라는 component가 없는 물체일때
            {
                /*selectedCounter = null;
                OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
                {
                    selectedCouner = selectedCounter
                });
                */
                SetSelectedCounter(null);
            }
        }
        else // countersLayerMask 감지 못했을때 
        {
            /*selectedCounter = null;
            OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
            {
                selectedCouner = selectedCounter
            });
            */
            SetSelectedCounter(null);
        }

        //Debug.Log(selectedCounter);
    }

    // tick
    private void HandleMovement()
    {
        // 원래 여기있던 input코드는 GameInput.cs의 GetMovementVectorNormalized()에 옮겼다.
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // 처음부터 input을 vector3로 받았으면 inputVector.y 대신에 inputVector.z를 사용했을텐데
        // 어떠한 제약이 있는지 Vector2를 사용했기 때문에 x(좌우) y(무시) z(앞뒤)에 찢어넣어 우회하는 방법을 선택했다. 
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        // 밑의 transform.position의 가독성을 높여주고, CapsuleCast에 넣어주기 위해 만들어짐.
        // 이게 왜 캡슐캐스트에 들어가는건지 궁금해서 조사해봤다.
        // 충돌예측, 성능최적화, 정확한 충돌정보 제공, 점프&방향전환 등 확장에 도움
        float moveDistance = moveSpeed * Time.deltaTime;

        float playerRadius = .7f;
        float playerHeight = 2f;

        // 검출되면 false, 로직에는 전혀 상관없는 이야기이지만 true를 선호한다고 가정하고 생각할때
        // Raycast가 실패해야 true니까 장애물이 없는걸 선호하는것(참)이다. 그래야 움직일 수 있는 것이다.
        // 결과를 bool로 담고,            시작점              방향      길이로      선을 뿅 쏜다.
        // bool canMove = !Physics.Raycast(transform.position, moveDir, playerSize); 아슬아슬하게 걸치면 버그가 일어나서 다른 코드로 업그레이드.

        // **콜리전 검출의 핵심코드.              캡슐 밑바닥       벡터연산인데 밑바닥시작 -> 머리방향으로 정한 길이 만큼    둘레         방향     여러 좋은 역할
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            // 여기에서 따로 구현을 해주지않으면 착 달라붙어서 좌우로 움직이질 못한다. moveDir이 바뀌면 false돼서 후퇴는 되지만

            // 위아래로 못가면 좌우로 갈 수 있는지 검출
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && (!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance));
            //canMove = (!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance));

            // 좌우로 갈 수 있으면 
            if (canMove)
            {
                // moveDirX방향에 아무것도 없어서 유효하다고 판단된 좌우성질의 moveDirX방향을 진짜 나아가는 방향으로 인정한다.
                moveDir = moveDirX;
            }
            else // 좌우로 못가면
            {
                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);
                //canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    // 세로로만 움직이게 하는 코드
                    moveDir = moveDirZ;
                }
                else
                {
                    // 아무것도 못함
                }
            }

        }

        if (canMove)
        {
            // **Move의 핵심 코드. 여기를 조건문으로 감싼다.
            transform.position += moveDir * moveDistance;
        }


        // 플레이어가 앞으로 바라보는 방향을 키보드가 입력해서 만들어낸 방향 moveDir로 계속 매칭시킨다.
        // 너무 초보적인 방법이다.
        // transform.forward = moveDir; 밑에서 Slerp로 업그레이드 시킨다.


        // 가장 중요한 부분. Vector3가 제로가 아니면, 즉 움직이는 상태면 isWalking을 true해서 지금 진짜 Walking중임을
        // player.IsWalking()을 부른 class에 넘겨준다. 이번 경우에는 PlauerAnimation에서 호출했음.
        isWalking = moveDir != Vector3.zero;



        // Slerp는 lerp중에서도 빙글도는것에 대한 보정에 잘맞는 함수여서 사용된 것 같다.
        // transform.forward이 한프레임만에 moveDir으로 번쩍 변하게 하는것이 아니라
        // 점진적으로 Time.deltaTime* rotateSpeed의 비중으로 가까워지게 보정하고 있다.
        float rotateSpeed = 12f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCouner = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }


    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnpickedSomething?.Invoke(this,EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObjet()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
