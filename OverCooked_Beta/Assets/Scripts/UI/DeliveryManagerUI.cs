using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{

    // 레시피 UI 항목들이 배치될 컨테이너
    [SerializeField] private Transform container;
    // 레시피 UI 항목의 템플릿(원본)
    [SerializeField] private Transform recipeTemplate;


    private void Awake()
    {
        // 게임 시작 시 템플릿을 비활성화
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        // DeliveryManager event Handler 등록
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        // 초기 UI 업데이트 호출
        UpdateVisual();
    }

    // 레시피가 완료될 때 호출
    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    // 레시피가 생성될 때 호출
    private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    // UI를 업데이트하여 현재 대기 중인 레시피를 반영
    private void UpdateVisual()
    {
        // 클린초기화 작업. 컨테이너 내의 모든 자식 오브젝트를 제거 (템플릿 제외)
        foreach (Transform child in container)
        {
            // 백그라운드 검은테두리 컨테이너를 뜻한다
            if (child == recipeTemplate) continue;
            // 템플릿을 제외한 모든 자식 오브젝트를 삭제하고 시작
            Destroy(child.gameObject);
        }

        // 현재 대기 중인 레시피를 순환
        foreach (RecipeSO recipeSO in DeliveryManager.Instance.GetWaitingRecipeSOList())
        {
            // 템플릿을 복제하여 새 레시피 UI 항목을 생성
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            // 새로 생성된 레시피 박스를 활성화
            recipeTransform.gameObject.SetActive(true);
            // 생성된 UI 항목에 레시피 데이터를 설정
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }

}
