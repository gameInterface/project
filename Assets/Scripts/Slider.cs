using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatAllocation : MonoBehaviour
{
    public Slider attackPowerSlider;
    public Slider attackSpeedSlider;
    public Slider attackRangeSlider;

    public TextMeshProUGUI attackPowerValueText; // 공격력 슬라이더의 값을 표시할 텍스트
    public TextMeshProUGUI attackSpeedValueText; // 공격 속도 슬라이더의 값을 표시할 텍스트
    public TextMeshProUGUI attackRangeValueText; // 공격 범위 슬라이더의 값을 표시할 텍스트
    public TextMeshProUGUI remainingPointsText;

    private int totalPoints = 10;
    private int remainingPoints;

    void Start()
    {
        // 슬라이더가 정수로만 움직이도록 설정
        attackPowerSlider.wholeNumbers = true;
        attackSpeedSlider.wholeNumbers = true;
        attackRangeSlider.wholeNumbers = true;

        remainingPoints = totalPoints;
        UpdateRemainingPointsText();
        UpdateSliderValueTexts();

        attackPowerSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(attackPowerSlider); });
        attackSpeedSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(attackSpeedSlider); });
        attackRangeSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(attackRangeSlider); });

        // 초기 슬라이더 값 설정을 GameData에 반영
        UpdateGameData();
    }

    void OnSliderValueChanged(Slider changedSlider)
    {
        // 사용된 포인트 계산
        int usedPoints = (int)(attackPowerSlider.value + attackSpeedSlider.value + attackRangeSlider.value);
        remainingPoints = totalPoints - usedPoints;

        if (remainingPoints < 0)
        {
            changedSlider.value += remainingPoints; // 초과된 값을 다시 줄임
            remainingPoints = 0;
        }

        UpdateRemainingPointsText();
        UpdateSliderValueTexts();

        // 슬라이더 값이 변경될 때마다 GameData 업데이트
        UpdateGameData();
    }

    void UpdateRemainingPointsText()
    {
        remainingPointsText.text = "Remaining Points: " + remainingPoints.ToString();
    }

    // 각 슬라이더의 값을 텍스트에 업데이트하는 함수
    void UpdateSliderValueTexts()
    {
        attackPowerValueText.text = attackPowerSlider.value.ToString();
        attackSpeedValueText.text = attackSpeedSlider.value.ToString();
        attackRangeValueText.text = attackRangeSlider.value.ToString();
    }

    void UpdateGameData()
    {
        GameData.Instance.attackPower = (int)attackPowerSlider.value;
        GameData.Instance.attackSpeed = (int)attackSpeedSlider.value;
        GameData.Instance.attackRange = (int)attackRangeSlider.value;
    }
}
