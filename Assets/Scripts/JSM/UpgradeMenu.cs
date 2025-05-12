using System.Collections.Generic;
using UnityEngine;
using static WeaponUpgrade;

public class UpgradeMenu : MonoBehaviour
{
    public static UpgradeMenu Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private GameObject menuRoot;
    [SerializeField] private Transform optionsParent;
    [SerializeField] private GameObject optionUIPrefab;
    [SerializeField] private UpgradeDefinitions definitions;

    private HashSet<string> _chosenTitles = new HashSet<string>();
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        menuRoot.SetActive(false);
    }

    public void ShowOptions(List<UpgradeOption> options)
    {
        foreach (Transform t in optionsParent) Destroy(t.gameObject);
        foreach (var opt in options)
        {
            var uiObj = Instantiate(optionUIPrefab, optionsParent);
            var ui = uiObj.GetComponent<UpgradeOptionUI>();

            System.Action markChosen = () =>
            {
                _chosenTitles.Add(opt.Title);
            };

            ui.Initialize(opt, markChosen);
        }
        menuRoot.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        menuRoot.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OnBossDefeated()
    {
        // 1) 무기 카테고리 확인
        var handler = FindObjectOfType<RangeWeaponHandler>();
        int category = handler != null ? handler.WeaponId : 0;

        // 2) 전용 풀과 공통 풀을 ‘아직 선택되지 않은 것’으로 필터링
        var specificPool = definitions
            .GetOptionsByCategory(category)
            .FindAll(o => !_chosenTitles.Contains(o.Title));

        var commonPool = definitions
            .GetAllOptions()
            .FindAll(o => !_chosenTitles.Contains(o.Title));

        var picks = new List<UpgradeOption>();

        // 3) 전용이 남아 있으면 확정 1개
        if (specificPool.Count > 0)
        {
            int idx = Random.Range(0, specificPool.Count);
            picks.Add(specificPool[idx]);
        }

        // 4) 공통 풀에서 나머지 슬롯(총 3개) 채우기
        while (picks.Count < 3 && commonPool.Count > 0)
        {
            int idx = Random.Range(0, commonPool.Count);
            picks.Add(commonPool[idx]);
            commonPool.RemoveAt(idx);
        }

        // 5) UI 표시
        ShowOptions(picks);
    }

}
