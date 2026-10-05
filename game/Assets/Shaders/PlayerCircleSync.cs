using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerCircleSync : MonoBehaviour
{
    public static int PosID = Shader.PropertyToID("Position");
    public static int SizeID = Shader.PropertyToID("Size");

    public Material WallMaterial;
    public Camera Camera;
    public LayerMask Mask;

    void Update()
    {
        var dir = Camera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        if (Physics.Raycast(ray, 30000, Mask))
            WallMaterial.SetFloat(SizeID, 2);
        else
            WallMaterial.SetFloat (SizeID, 0);

            var veiw = Camera.WorldToViewportPoint(transform.position);
        WallMaterial.SetVector(PosID, veiw);
    }

}
