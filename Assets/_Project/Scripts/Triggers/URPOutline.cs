using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Renderer))]
    public class URPOutline : MonoBehaviour
    {
        [Header("Outline Settings")]
        [SerializeField] private Color outlineColor = Color.yellow;
        [SerializeField][Range(0, 10)] private float outlineWidth = 2f;

        private Renderer thisRenderer;
        private Material originalMaterial;
        private Material outlineMaterial;
        private bool isHighlighted;

        private void Awake()
        {
            thisRenderer = GetComponent<Renderer>();
            originalMaterial = thisRenderer.material;

            CreateOutlineMaterial();
        }

        private void CreateOutlineMaterial()
        {
            Shader outlineShader = Shader.Find("Shader Graphs/OutlineURP");

            if (outlineShader == null)
                outlineShader = Shader.Find("Universal Render Pipeline/Unlit");

            if (outlineShader == null)
                outlineShader = Shader.Find("Unlit/Color");

            outlineMaterial = new Material(outlineShader);

            outlineMaterial.SetColor("_Color", outlineColor);

            if (outlineMaterial.HasProperty("_OutlineColor"))
                outlineMaterial.SetColor("_OutlineColor", outlineColor);

            if (outlineMaterial.HasProperty("_OutlineWidth"))
                outlineMaterial.SetFloat("_OutlineWidth", outlineWidth);
        }

        public void SetHighlighted(bool highlighted)
        {
            if (isHighlighted == highlighted) return;

            isHighlighted = highlighted;

            if (highlighted)
            {
                if (originalMaterial.HasProperty("_BaseMap") && outlineMaterial.HasProperty("_BaseMap"))
                {
                    outlineMaterial.SetTexture("_BaseMap", originalMaterial.GetTexture("_BaseMap"));
                }

                thisRenderer.material = outlineMaterial;
            }
            else
            {
                thisRenderer.material = originalMaterial;
            }
        }

        private void OnDestroy()
        {
            if (outlineMaterial != null)
                Destroy(outlineMaterial);
        }
    }
}