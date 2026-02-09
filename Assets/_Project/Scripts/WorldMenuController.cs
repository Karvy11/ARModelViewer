using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WorldMenuController : MonoBehaviour
{

    private ARObjectController _controller;
    public ARObjectController Controller
    {
        get => _controller;
        set => _controller = value;
    }

    [Header("UI Components")]
    [SerializeField] private TMP_Dropdown clipDropdown;
    [SerializeField] private TMP_Dropdown speedDropdown;
    [SerializeField] private TMP_Dropdown modeDropdown;
    [SerializeField] private Toggle reverseToggle;
    [SerializeField] private Toggle playPauseToggle;



    private readonly List<float> _speedValues = new List<float> { 0.5f, 1.0f, 2.0f, 5.0f };
    private readonly List<string> _speedLabels = new List<string> { "0.5x", "1x", "2x", "5x" };



    public void InitializeUI()
    {
        ;

        clipDropdown.ClearOptions();


        if (_controller.ClipNames != null && _controller.ClipNames.Count > 0)
        {
            clipDropdown.AddOptions(_controller.ClipNames);
            clipDropdown.value = 0;
        }
        else
        {
            clipDropdown.AddOptions(new List<string> { "No Clips Found" });
        }


        speedDropdown.ClearOptions();
        speedDropdown.AddOptions(_speedLabels);
        speedDropdown.value = 1;


        modeDropdown.ClearOptions();
        modeDropdown.AddOptions(new List<string> { "Once", "Loop", "Ping-Pong" });
        modeDropdown.value = 1;


        clipDropdown.onValueChanged.RemoveAllListeners();
        clipDropdown.onValueChanged.AddListener(OnClipChanged);

        speedDropdown.onValueChanged.RemoveAllListeners();
        speedDropdown.onValueChanged.AddListener(OnSpeedChanged);

        modeDropdown.onValueChanged.RemoveAllListeners();
        modeDropdown.onValueChanged.AddListener(OnModeChanged);


        reverseToggle.onValueChanged.AddListener(OnReverseChanged);

        playPauseToggle.isOn = true;

        playPauseToggle.onValueChanged.AddListener(OnPlayPauseToggle);




    }

    private void OnPlayPauseToggle(bool isPlay)
    {
        _controller.TogglePlayStop(isPlay);
    }

    private void OnClipChanged(int index)
    {

        if (_controller.ClipNames == null || index >= _controller.ClipNames.Count) return;

        string selectedClip = _controller.ClipNames[index];
        _controller.PlayClip(selectedClip);
    }

    private void OnSpeedChanged(int index)
    {
        if (index >= 0 && index < _speedValues.Count)
        {
            _controller.SetSpeed(_speedValues[index]);
        }
    }

    private void OnModeChanged(int index)
    {

        _controller.SetPlayMode(index);
    }

    private void OnReverseChanged(bool isReverse)
    {
        _controller.SetDirection(isReverse);
    }
}