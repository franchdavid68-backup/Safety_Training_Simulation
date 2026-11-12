using UnityEngine;

public class Highlighter
{
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private readonly MaterialPropertyBlock block = new MaterialPropertyBlock();
    private Renderer[] renderers = System.Array.Empty<Renderer>();

    public void SetRenderers(Renderer[] newRenderers)
    {
        Off();
        renderers = newRenderers ?? System.Array.Empty<Renderer>();
    }

    public void SetRenderersFrom(GameObject root)
    {
        SetRenderers(root != null
            ? root.GetComponentsInChildren<Renderer>()
            : null);
    }

    public void On(Color color)
    {
        foreach (var r in renderers)
        {
            if (r == null) continue;

            r.GetPropertyBlock(block);
            block.SetColor(EmissionColorID, color);
            r.SetPropertyBlock(block);
        }
    }


    public void Off()
    {
        foreach (var r in renderers)
        {
            if (r != null)
                r.SetPropertyBlock(null);
        }
    }
}
