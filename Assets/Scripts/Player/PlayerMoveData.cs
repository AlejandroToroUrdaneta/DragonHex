using UnityEngine;

[CreateAssetMenu(fileName = "MoveData")]
public class PlayerMoveData : ScriptableObject
{
    public int MaxMove = 8;
    public float MoveSpeed = 0.5f;
}
