using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

[ExecuteInEditMode]
public class TMPRectResizer : MonoBehaviour
{
    public TMP_Text text;
    public RectTransform target;
    public float lineHeight;
    public bool x, y;
    public Vector2 padding;
    private void Update()
    {
        if(text && target)
        {
            float width = x ? 0 : target.sizeDelta.x;
            float height = y ? 0 : target.sizeDelta.y;
            if (x)
            {
                width = padding.x + ((text.textInfo.lineInfo.Length == 0 ? 0 : text.textInfo.lineInfo[0].length) * 1);
            }
            if (y)
            {
                height = padding.y + (text.textInfo.lineCount * lineHeight);
            }
            target.sizeDelta = new Vector2(width, height);
        }
    }
}
