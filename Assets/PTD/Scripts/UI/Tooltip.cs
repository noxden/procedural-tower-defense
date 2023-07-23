//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TooltipSystemScriptableObject tooltipSystem;
    [SerializeField] private GameObject tooltipObject;
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField;
    [SerializeField] private int characterWrapLimit;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private LayoutElement layoutElement;

    [SerializeField] private float offsetFromMouse;

    private void Awake()
    {
        tooltipSystem.showTooltipEvent.AddListener(Show);
        tooltipSystem.hideTooltipEvent.AddListener(Hide);
    }

    //private void OnEnable()
    //{
    //    tooltipSystem.showTooltipEvent.AddListener(Show);
    //    tooltipSystem.hideTooltipEvent.AddListener(Hide);
    //}

    //private void OnDisable()
    //{
    //    tooltipSystem.showTooltipEvent.RemoveListener(Show);
    //    tooltipSystem.hideTooltipEvent.RemoveListener(Hide);
    //}

    private void Update()
    {
        if (Application.isEditor)
        {
            CheckLayoutElement();
        }

        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;

        float offset = pivotY < 0 ? offsetFromMouse : -offsetFromMouse;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        tooltipObject.transform.position = position + new Vector2(0, offset);
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text = content;

        CheckLayoutElement();
    }

    private void CheckLayoutElement()
    {
        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    public void Show(string content, string header = "")
    {
        SetText(content, header);
        tooltipObject.SetActive(true);
    }

    public void Hide()
    {
        tooltipObject.SetActive(false);
    }
}
