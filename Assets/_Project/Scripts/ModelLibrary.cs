using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModelLibrary", menuName = "AR/Model Library")]
public class ModelLibrary : ScriptableObject
{
    public List<ARModelData> models;
}

