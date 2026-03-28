using UnityEngine;

public class BlueBlinkHighlightBharani : MonoBehaviour
{
    public Color glowColor = new Color(0.2f, 0.7f, 1f);
    public float intensity = 2f;
    public float speed = 3f;

    [Header("Prevent Original Mesh From Showing")]
    public bool preventOriginalShow = false;

    Material mat;
    Color originalEmission;
    Color originalColor;
    bool stored = false;

    // 🔑 Controls when glow is active
    bool glowEnabled = false;

    void Start()
    {
        Renderer r = GetComponent<Renderer>();
        mat = r.material;   // instance material

        mat.EnableKeyword("_EMISSION");

        // Store original values once
        originalEmission = mat.GetColor("_EmissionColor");
        originalColor = mat.GetColor("_Color");
        stored = true;

        // Start without glow
        mat.SetColor("_EmissionColor", originalEmission);
    }

    void Update()
    {
        // ❌ Do nothing unless GetShader() is called
        if (!glowEnabled || mat == null)
            return;

        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float glow = t * intensity;

        Color final = glowColor * glow;
        mat.SetColor("_EmissionColor", final);

        // 🔒 Prevent original mesh showing if enabled
        if (preventOriginalShow)
        {
            mat.SetColor("_Color", Color.black);
        }
        else
        {
            mat.SetColor("_Color", originalColor);
        }
    }

    // ✅ CALL THIS FUNCTION TO START GLOW
    public void GetShader()
    {
        if (mat == null) return;

        glowEnabled = true;
        mat.EnableKeyword("_EMISSION");
    }

    // ✅ CALL THIS TO STOP GLOW AND RESTORE ORIGINAL
    public void ResetToOriginal()
    {
        if (!stored || mat == null) return;

        glowEnabled = false;
        mat.SetColor("_EmissionColor", originalEmission);
        mat.SetColor("_Color", originalColor);
        mat.DisableKeyword("_EMISSION");
    }
}