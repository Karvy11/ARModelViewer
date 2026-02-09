using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewModelData", menuName = "AR/Model Data")]
public class ARModelData : ScriptableObject
{
    public string id;
    public GameObject prefab;
    public Sprite icon;
    public int maxInstances = 1;

    [Header("Animation")]

    public List<AnimationClip> clips;
}
