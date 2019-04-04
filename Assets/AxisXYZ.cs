using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AxisXYZ : MonoBehaviour
{
    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    public void OnRenderObject()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
        GL.Color(Color.red); //x
        GL.Vertex3(-0.5f, -0.5f, -0.5f);
        GL.Vertex3(0.5f, -0.5f, -0.5f);
        GL.Vertex3(-0.5f, -0.5f, 0.5f);
        GL.Vertex3(0.5f, -0.5f, 0.5f);
        GL.Vertex3(-0.5f, 0.5f, -0.5f);
        GL.Vertex3(0.5f, 0.5f, -0.5f);
        GL.Vertex3(-0.5f, 0.5f, 0.5f);
        GL.Vertex3(0.5f, 0.5f, 0.5f);
        GL.Color(Color.green);//y
        GL.Vertex3(-0.5f, -0.5f, -0.5f);
        GL.Vertex3(-0.5f, 0.5f, -0.5f);
        GL.Vertex3(-0.5f, -0.5f, 0.5f);
        GL.Vertex3(-0.5f, 0.5f, 0.5f);
        GL.Vertex3(0.5f, -0.5f, -0.5f);
        GL.Vertex3(0.5f, 0.5f, -0.5f);
        GL.Vertex3(0.5f, -0.5f, 0.5f);
        GL.Vertex3(0.5f, 0.5f, 0.5f);
        GL.Color(Color.blue);//z
        GL.Vertex3(-0.5f, -0.5f, -0.5f);
        GL.Vertex3(-0.5f, -0.5f, 0.5f);
        GL.Vertex3(-0.5f, 0.5f, -0.5f);
        GL.Vertex3(-0.5f, 0.5f, 0.5f);
        GL.Vertex3(0.5f, -0.5f, -0.5f);
        GL.Vertex3(0.5f, -0.5f, 0.5f);
        GL.Vertex3(0.5f, 0.5f, -0.5f);
        GL.Vertex3(0.5f, 0.5f, 0.5f);

        GL.End();
        GL.PopMatrix();
    }
}