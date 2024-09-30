using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // 이게 추가 되어야 GetComponent<Image>() 사용가능

public class DeliveryManagerSingleUI : MonoBehaviour
{
    // 레시피 이름을 표시할 TMP
    [SerializeField] private TextMeshProUGUI recipeNameText;
    // 아이콘(재료들)을 배치할 컨테이너
    [SerializeField] private Transform iconContainer;
    // 아이콘의 템플릿(원본)
    [SerializeField] private Transform iconTemplate;


    private void Awake()
    {
        // 게임 시작 시 템플릿을 비활성화
        iconTemplate.gameObject.SetActive(false);
    }

    // 레시피 데이터를 UI에 설정하는 메소드
    public void SetRecipeSO(RecipeSO recipeSO)
    {
        
        recipeNameText.text = recipeSO.recipeName;

        // 클린초기화 작업. 아이콘컨테이너 내의 모든 자식 오브젝트를 제거 (템플릿 제외)
        foreach (Transform child in iconContainer)
        {
            // 백그라운드 검은테두리 컨테이너 안에 있는 재료아이콘을 뜻한다
            if (child == iconTemplate) continue;
            // 템플릿을 제외한 모든 자식 오브젝트를 삭제하고 시작
            Destroy(child.gameObject);
        }

        // 레시피에 포함된 재료를 순환
        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            // 템플릿을 복제하여 새 아이콘을 생성
            Transform iconTransform = Instantiate(iconTemplate, iconContainer);
            // 새로 생성된 재료 아이콘을 활성화
            iconTransform.gameObject.SetActive(true);
            // 아이콘에 해당 재료의 스프라이트를 설정
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }
}
