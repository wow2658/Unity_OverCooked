using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFail;
    public static DeliveryManager Instance { get; private set; }

    // 모든 가능한 레시피 목록을 포함하는 List<List<KitchenObjectSO>>
    [SerializeField] private RecipeListSO recipeListSO;

    // 현재 대기 중인 레시피를 추적하는 List<KitchenObjectSO>
    private List<RecipeSO> waitingRecipeSOList;

    // 새로운 레시피를 생성하기 위한 타이머
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;

    // 동시에 대기할 수 있는 레시피의 최대 개수
    private int waitingRecipesMax = 4;

    // GameOverUI에 띄울 성공 횟수
    private int successfulRecipesAmount;

    private void Awake()
    {
        // 싱글톤 get set하면 초기에 해주는 작업
        Instance = this;

        // List는 선언하고 끝이아니라 꼭 여기에서 초기화도 해준다.
        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            // 대기 중인 레시피를 더 추가할 수 있는지 확인
            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax)
            {
                // 사용 가능한 레시피 목록에서 무작위로 레시피를 선택
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];

                // 무작위로 선택한 개체를 대기 중인 레시피 리스트 RecipeSO에 추가
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        // 대기 중인 레시피 목록을 순회
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {

            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            // Count. 접시의 재료 수가 레시피의 재료 수와 같은지 확인
            if (waitingRecipeSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                // 이번에 i번째로 검사하는 대기 중인 레시피에 들어가는 재료의 Count와 지금 접시에 올려져있는 재료의 Count가 동일한 경우에만 검사를 진행
                // Flag. 접시 내용물이 레시피와 일치하는지 여부를 나타내는 플래그
                // 로직이 끝났을때도 plateContentsMatchesRecipe가 여전히 true면 Player delivered the correct recipe!
                bool plateContentsMatchesRecipe = true;

                // 대기중인 레시피들을 순회 ex) { Salad, cheeseBuger 등 }
                foreach (KitchenObjectSO recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectSOList)
                {
                    // Flag. 현재 레시피 재료가 접시에서 발견되었는지 여부를 나타내는 플래그
                    // 발견 안되면 Player did not deliver a correct recipe이 되게함
                    bool ingredientFound = false;

                    // 접시의 모든 재료를 순회 ex) { Bread, TomatoSlices 등 }
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        // 레시피 재료와 접시에 올려 놓은 재료가 일치하는지 확인
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            // 재료(ingredient)가 일치하는 경우 플래그를 true로 설정하고 루프를 종료
                            ingredientFound = true;
                            break;
                        }
                    }
                    // 만약 레시피 재료가 접시에서 발견되지 않으면
                    if (!ingredientFound)
                    {
                        // 플래그를 false로 설정하고 루프를 종료
                        plateContentsMatchesRecipe = false;
                    }
                }

                // 접시 내용물이 레시피와 일치하는 경우
                if (plateContentsMatchesRecipe)
                {
                    // 플레이어가 올바른 레시피를 전달한 경우
                    Debug.Log("Player delivered the correct recipe!");
                    // GameOverUI에 올라갈 성공횟수 +1
                    successfulRecipesAmount++;

                    // 일치하는 레시피를 대기 목록에서 제거
                    waitingRecipeSOList.RemoveAt(i);

                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        // plateKitchenObjectSO == recipeKitchenObjectSO를 못찾고 ingredientFound가 false여서
        // 일치하는 레시피를 찾지 못한 경우
        Debug.Log("Player did not deliver a correct recipe");
        OnRecipeFail?.Invoke(this, EventArgs.Empty);
    }
    public List<RecipeSO> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }


    public int GetSuccessfulRecipesAmount()
    {
        return successfulRecipesAmount;
    }
}
