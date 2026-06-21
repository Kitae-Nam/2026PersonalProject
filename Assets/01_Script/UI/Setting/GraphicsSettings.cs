using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace _01_Script.UI.Setting
{
    public class GraphicsSettings : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private TMP_Dropdown refreshRateDropdown;

    private Resolution[] _resolutions;

    void Start()
    {
        SetupResolution();
        SetupScreenMode();
        SetupRefreshRate();
    }

    // ─── 해상도 ───
    private void SetupResolution()
    {
        // 중복 제거 (같은 해상도가 주사율만 다르게 여러 번 나옴)
        _resolutions = Screen.resolutions
            .Select(r => new Resolution { width = r.width, height = r.height })
            .Distinct()
            .ToArray();

        resolutionDropdown.ClearOptions();
        var options = new List<string>();
        int current = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            options.Add($"{_resolutions[i].width} x {_resolutions[i].height}");
            if (_resolutions[i].width == Screen.width &&
                _resolutions[i].height == Screen.height)
                current = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", current);
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        Resolution r = _resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);
        PlayerPrefs.SetInt("Resolution", index);
    }

    // ─── 전체화면 / 창화면 ───
    private void SetupScreenMode()
    {
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(new List<string> { "전체화면", "창화면" });
        screenModeDropdown.value = PlayerPrefs.GetInt("ScreenMode", 0);
        screenModeDropdown.RefreshShownValue();
        screenModeDropdown.onValueChanged.AddListener(SetScreenMode);
    }

    public void SetScreenMode(int index)
    {
        // 0 = 전체화면, 1 = 창화면
        Screen.fullScreenMode = (index == 0)
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;
        PlayerPrefs.SetInt("ScreenMode", index);
    }

    // ─── 주사율 ───
    private void SetupRefreshRate()
    {
        // 지원하는 주사율 목록 추출
        var rates = Screen.resolutions
            .Select(r => Mathf.RoundToInt((float)r.refreshRateRatio.value))
            .Distinct()
            .OrderBy(r => r)
            .ToList();

        refreshRateDropdown.ClearOptions();
        refreshRateDropdown.AddOptions(rates.Select(r => $"{r} Hz").ToList());
        refreshRateDropdown.value = PlayerPrefs.GetInt("RefreshRate", rates.Count - 1);
        refreshRateDropdown.RefreshShownValue();
        refreshRateDropdown.onValueChanged.AddListener(i => SetRefreshRate(rates[i]));
    }

    public void SetRefreshRate(int hz)
    {
        Application.targetFrameRate = hz;
        PlayerPrefs.SetInt("RefreshRate", refreshRateDropdown.value);
    }
    }
}