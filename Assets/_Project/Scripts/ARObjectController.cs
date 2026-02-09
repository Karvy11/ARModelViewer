using System.Collections.Generic;
using UnityEngine;




public class ARObjectController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RuntimeAnimatorController templateController;

    [SerializeField] Transform worldUISpawnLocation;

    public Transform WorldUISpawnLocation => worldUISpawnLocation;

    [SerializeField] private Animator _animator;
    private WorldMenuController worldMenuController;
    public WorldMenuController WorldMenuController
    {
        get => worldMenuController;
        set => worldMenuController = value;
    }
    private AnimatorOverrideController _overrideController;
    private ARModelData _data;


    private bool _isPlaying = true;
    private bool _isReverse = false;

    private float _userSpeed = 1f;


    public enum PlayMode { Once, Loop, PingPong }
    private PlayMode _currentMode = PlayMode.Loop;
    private float _clipDuration;
    private float _timer;


    public List<string> ClipNames { get; private set; } = new List<string>();

    public void Initialize(ARModelData data)
    {
        _data = data;
        // _animator = GetComponent<Animator>();
        _overrideController = new AnimatorOverrideController(templateController);
        _animator.runtimeAnimatorController = _overrideController;


        ClipNames.Clear();
        foreach (var clip in _data.clips)
        {
            ClipNames.Add(clip.name);
        }

        if (ClipNames.Count > 0) PlayClip(ClipNames[0]);
    }

    public void ToggleMenu()
    {
        bool active = !worldMenuController.gameObject.activeSelf;
        worldMenuController.gameObject.SetActive(active);

    }


    private void Update()
    {
        if (!_animator || !_animator.runtimeAnimatorController) return;


        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float time = stateInfo.normalizedTime;


        if (_isPlaying)
        {
            float targetSpeed = _userSpeed;


            if (_isReverse) targetSpeed *= -1f;


            switch (_currentMode)
            {
                case PlayMode.Once:

                    if (!_isReverse && time >= 1.0f)
                    {
                        targetSpeed = 0;
                        _isPlaying = false;
                    }
                    else if (_isReverse && time <= 0.0f)
                    {
                        targetSpeed = 0;
                        _isPlaying = false;
                    }
                    break;

                case PlayMode.Loop:

                    if (!_isReverse && time >= 1.0f)
                    {
                        _animator.Play("DynamicState", 0, 0.0f);
                    }
                    else if (_isReverse && time <= 0.0f)
                    {
                        _animator.Play("DynamicState", 0, 1.0f);
                    }
                    break;

                case PlayMode.PingPong:


                    if (time >= 1.0f)
                    {
                        _isReverse = true;
                    }
                    else if (time <= 0.0f)
                    {
                        _isReverse = false;
                    }


                    targetSpeed = _userSpeed * (_isReverse ? -1f : 1f);
                    break;
            }


            _animator.SetFloat("SpeedMult", targetSpeed);
        }
        else
        {

            _animator.SetFloat("SpeedMult", 0f);
        }
    }

    // --- PUBLIC API FOR UI ---

    public void PlayClip(string name)
    {
        var clip = _data.clips.Find(c => c.name == name);
        if (clip == null) return;

        _overrideController["Empty"] = clip;
        _animator.Play("DynamicState", 0, 0);
        _isPlaying = true;
    }

    public void SetSpeed(float speed)
    {
        _userSpeed = speed;
        _isPlaying = true;
    }

    public void SetDirection(bool isReverse)
    {
        _isReverse = isReverse;


        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (_isReverse && stateInfo.normalizedTime <= 0.01f)
        {
            _animator.Play("DynamicState", 0, 1.0f);
        }
    }

    public void SetPlayMode(int modeIndex)
    {
        _currentMode = (PlayMode)modeIndex;
        if (_currentMode == PlayMode.Once)
        {
            _animator.Play("DynamicState", 0, 0);
        }
        _isPlaying = true; // Auto-resume when mode changes
    }

    public void TogglePlayStop(bool play)
    {
        _isPlaying = play;
    }
}