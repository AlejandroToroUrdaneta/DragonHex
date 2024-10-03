using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamLigth : MonoBehaviour
{
    public Material material;

    Dictionary<int, Color> teamColor = new Dictionary<int, Color>()
    {
        {-1, new Color(1.0f, 1.0f, 1.0f,0.0f) }, //White and transparent
        {0, new Color(0.0f,0.0f, 1.0f,1.0f) },//Blue And Not Transparent
        {1, new Color(1.0f, 0.0f, 0.0f,1.0f) }, // Red  And Not Transparent
        {2, new Color(0.78f,0.54f,0.23f) } // Ligth Brown
    };

    private void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        material = renderer.material;
        ChangeColor(-1);
    }

    public void ChangeColor(int color)
    {
        if (teamColor.TryGetValue(color, out Color col))
        {
            material.SetColor("_AuraColor", col);
        }
        else
        {
            Debug.Log("Invalid terraincost or mapping" + color);
        }
    }
}
