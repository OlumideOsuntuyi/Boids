using UnityEngine;

namespace LostOasis
{
    [ExecuteInEditMode]
    public class TopSafeArea : MonoBehaviour
    {
        public float value;
        public Mode mode;
        private void OnEnable()
        {
            RectTransform trans = GetComponent<RectTransform>();
            Canvas canvas = (FindObjectOfType(typeof(Canvas)) as Canvas);
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            float safeArea = canvasRect.sizeDelta.y - Screen.safeArea.yMax;
            Debug.Log($"Canvas Height: {canvasRect.sizeDelta.y} - SafeArea: {Screen.safeArea.yMax} - Height: {Screen.height}");
            switch (mode)
            {
                case Mode.Top:
                    {
                        trans.offsetMax = new Vector2(trans.offsetMax.x, -(value + safeArea));
                    }
                    break;
                case Mode.Y:
                    {
                        trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, -(value + safeArea));
                    }break;
                default:break;
            }
        }

        public enum Mode
        {
            Top, Y
        }
    }
}