using UnityEngine;

public class Screen : MonoBehaviour
{
    [Header("Screen Parameters")]
    public int Width = 800;
    public int Height = 600;

    [Header("References")]
    [SerializeField] Transform displayObject;

    Material displayMaterial;
    Texture2D displayTexture;

    public void Draw(Color[] pixels)
    {
        displayTexture.SetPixels(pixels);
        displayTexture.Apply();
    }

    private void Awake()
    {

        displayTexture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
        displayTexture.filterMode = FilterMode.Point;
        displayTexture.wrapMode = TextureWrapMode.Clamp;
        displayTexture.anisoLevel = 0;

        displayMaterial = displayObject.GetComponent<MeshRenderer>().material;
        displayMaterial.mainTexture = displayTexture;

        displayObject.localScale = new Vector3(Width / 1000f, 1f, Height / 1000f);
    }

    private void OnValidate()
    {
        if (displayObject != null)
        {
            displayObject.localScale = new Vector3(Width / 1000f, 1f, Height / 1000f);
        }
    }
}
