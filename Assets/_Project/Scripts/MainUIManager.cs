using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class MainUIManager : MonoBehaviour
{
    [SerializeField] private ModelLibrary library;
    [SerializeField] private ARSpawnManager spawnManager;

    [Header("UI")]
    [SerializeField]
    [Tooltip("The menu with all the creatable objects.")] GameObject _optionsMenu;
    [SerializeField] private Button _openOptionsButton;
    [SerializeField] private Button _cancelOptionsButton;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private OptionSelectionButton buttonTemplate;
    private List<OptionSelectionButton> _spawnedButtons = new();
    [SerializeField] private Animator _optionsMenuAnimator;
    // private bool _showOptionsMenu;


    public Animator OptionsMenuAnimator
    {
        get => _optionsMenuAnimator;
        set => _optionsMenuAnimator = value;
    }
    void OnEnable()
    {
        _openOptionsButton.onClick.AddListener(ShowMenu);
        _cancelOptionsButton.onClick.AddListener(HideMenu);
    }


    private void Start()
    {

        GenerateButtons();
        HideMenu();
    }
    void OnDisable()
    {
        _openOptionsButton.onClick.RemoveListener(ShowMenu);
        _cancelOptionsButton.onClick.RemoveListener(HideMenu);
    }

    private void GenerateButtons()
    {
        // Clear existing
        foreach (Transform child in contentContainer) Destroy(child.gameObject);

        foreach (var data in library.models)
        {
            var btnObj = Instantiate(buttonTemplate, contentContainer);
            _spawnedButtons.Add(btnObj);
            if (btnObj.ObjectInfoText != null)
                btnObj.ObjectInfoText.text = data.id;

            if (btnObj.FaceImage != null && data.icon != null)
            {
                btnObj.FaceImage.sprite = data.icon;
            }
            Button btn = btnObj.Button;
            btn.onClick.AddListener(() =>
            {
                ToggleSelectionBox();

                if (spawnManager.selectedModel != data)
                {
                    spawnManager.DecreaseCount(data.id);
                    ClearAllObjects();
                }
                spawnManager.SelectModelByID(data.id);

                HideMenu();

                btnObj.SelectionBox.SetActive(true);


            });
        }
        spawnManager.SelectModelByID(library.models[0].id);
        _spawnedButtons[0].Button.onClick.Invoke();


    }
    void ToggleSelectionBox()
    {
        foreach (var spwnBtn in _spawnedButtons)
        {

            spwnBtn.SelectionBox.SetActive(false);

        }

    }
    public void HideMenu()
    {
        _optionsMenuAnimator.SetBool("Show", false);
        // _showOptionsMenu = false;

    }
    void ShowMenu()
    {
        //  _showOptionsMenu = true;
        _optionsMenu.SetActive(true);
        if (!_optionsMenuAnimator.GetBool("Show"))
        {
            _optionsMenuAnimator.SetBool("Show", true);
        }

    }
    public void ClearAllObjects()
    {
        foreach (Transform child in spawnManager.transform)
        {
            Destroy(child.gameObject);
        }
    }


}
