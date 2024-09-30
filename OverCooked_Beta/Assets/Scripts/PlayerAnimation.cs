using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private const string IS_WALKING = "IsWalking";
    /*
    위 코드에서 const를 쓴 이유는 아래과 같다.

    1. 가독성 향상: IS_WALKING이 상수로 정의되어 있기 때문에, 
    이 값이 코드 내에서 변경되지 않을 것임을 명확히 보여준다.
    이를 통해 코드의 의도를 더 쉽게 파악할 수 있다.
    
    2. 유지보수성 향상: 만약 나중에 "IsWalking" 문자열을 변경해야 하는 상황이 온다면, 
    상수로 정의된 IS_WALKING 값을 한 번만 변경해주면 된다.상수를 사용하지 않고 문자열을 직접 사용하면, 
    문자열이 사용된 모든 곳을 찾아서 일일이 변경해야 한다.

    3. 오타 방지: 상수를 사용하면 문자열을 직접 사용할 때 발생할 수 있는 오타를 방지할 수 있다.
    상수로 정의된 값은 컴파일 시점에 검사가 이루어지기 때문에, 오타로 인한 오류를 줄일 수 있다.

    4. 의미 부여: "IsWalking"이라는 문자열이 무엇을 의미하는지 명확하게 알 수 있다. 
    상수 이름을 통해 문자열이 어떤 목적으로 사용되는지 쉽게 이해할 수 있다.

    따라서, const 키워드를 사용하여 상수를 정의하면 코드의 품질을 높이고, 코드 유지보수를 더 쉽게 할 수 있다.
    */

    [SerializeField] private Player player;

    private Animator animator;

    private void Awake()
    {
        // PlayerAnimation이 부착된 GameObject(PlayerVisual 프리펩)에 Animator 컴포넌트가 함께 붙어있다.
        // 그 친구에게 직접 넣어준 Animator Controller를 에디터의 Animator Window창을 통해 마우스로 조작했었는데 
        // 그걸 코드로 접근해서 제어하기위해 Get하는 부분이다.
        animator = GetComponent<Animator>();
        // 나와 동급인 Animator Compoennt를 일단 Get하면 Window창은 알아서 자동으로 딸려오는 느낌

    }

    private void Update()
    {
        // Player.cs로 부터 넘겨받은 isWalking 결과를 에디터에 보이는 Animator상의 IsWalking에 반영시킨다.
        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
