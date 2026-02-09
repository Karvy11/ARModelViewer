using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.ARStarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

public class ARSpawnManager : MonoBehaviour
{
    [SerializeField] private ModelLibrary library;
    [SerializeField] private ARInteractorSpawnTrigger _arSpawnTrigger;
    [SerializeField] WorldMenuController worldMenueUIPrefab;

    private ARModelData _selectedModel;
    public ARModelData selectedModel => _selectedModel;
    private Dictionary<string, int> _spawnCounts = new Dictionary<string, int>();
    private bool _spawnAsChildren = true;
    private Camera _cameraToFace;

    public event Action<GameObject> OnObjectSpawned;


    void OnEnable()
    {
        _arSpawnTrigger.objectSpawnTriggered.AddListener(TrySpawnAt);
    }



    // Called by Main UI
    public void SelectModelByID(string id)
    {
        _selectedModel = library.models.Find(m => m.id == id);
    }
    public void DecreaseCount(string id)
    {
        if (_spawnCounts.ContainsKey(id) && _spawnCounts[id] > 0)
        {
            _spawnCounts[id]--;
        }
    }

    public void TrySpawnAt(Vector3 position, Vector3 spawnNormal)
    {
        if (_selectedModel == null) return;

        // Check Limits
        int currentCount = _spawnCounts.ContainsKey(_selectedModel.id) ? _spawnCounts[_selectedModel.id] : 0;
        if (currentCount >= _selectedModel.maxInstances)
        {
            Debug.Log($"Limit reached for {_selectedModel.id}");
            return;
        }

        Spawn(_selectedModel, position, spawnNormal);
    }

    private void Spawn(ARModelData data, Vector3 spawnPoint, Vector3 spawnNormal)
    {

        var newObject = Instantiate(data.prefab);
        if (_spawnAsChildren)
            newObject.transform.parent = transform;

        newObject.transform.position = spawnPoint;
        InitializeCameraVar();
        var facePosition = _cameraToFace.transform.position;
        var forward = facePosition - spawnPoint;
        BurstMathUtility.ProjectOnPlane(forward, spawnNormal, out var projectedForward);
        newObject.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);
        newObject.transform.parent = transform;


        // Initialize Object Logic
        var controller = newObject.GetComponent<ARObjectController>();
        if (controller)
        {
            controller.Initialize(data);
            var worldUi = Instantiate(worldMenueUIPrefab, controller.WorldUISpawnLocation.position, Quaternion.identity, controller.WorldUISpawnLocation);

            controller.WorldMenuController = worldUi;
            worldUi.Controller = controller;
            worldUi.InitializeUI();

        }



        // Track Count
        if (!_spawnCounts.ContainsKey(data.id)) _spawnCounts[data.id] = 0;
        _spawnCounts[data.id]++;

        OnObjectSpawned?.Invoke(newObject);

    }

    private void InitializeCameraVar()
    {
        if (_cameraToFace == null)
            _cameraToFace = Camera.main;
    }
}