using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathIllustrator : MonoBehaviour
{
    private const float LineHeightOffset = 0.5f;
    LineRenderer line;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    public void IllustratePath(Path path)
    {
        line.positionCount = path.waypoints.Length;

        for (int i = 0; i < path.waypoints.Length; i++)
        {
            Transform nodeTransform = path.waypoints[i].transform;
            line.SetPosition(i, nodeTransform.position.With(y: nodeTransform.position.y + LineHeightOffset));
        }
    }

    public void DissapiredIllustration()
    {
        line.positionCount = 0;
    }
}
