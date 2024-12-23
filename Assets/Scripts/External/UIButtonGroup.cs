using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LostOasis
{

    [ExecuteInEditMode]
    public class UIButtonGroup : MonoBehaviour
    {
        public List<Image> buttons;
        public Color activeColor;
        public Color inactiveColor;
        public int Current;

        public UnityEvent onValueChanged;

        private int prev;

        public int MAX => buttons == null ? 1 : buttons.Count - 1;
        private void OnEnable()
        {
            prev = -1;
        }
        private void Update()
        {
            if (buttons.Count == 0) return;
            Current = Mathf.Clamp(Current, 0, MAX);
            if (prev == Current) return;

            prev = Current;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].color = i == Current ? activeColor : inactiveColor;
            }

            onValueChanged.Invoke();
        }
        public void Click(Image image)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] == image)
                {
                    Current = i;
                    break;
                }
            }
        }
        public void Click(int index)
        {
            Current = Mathf.Clamp(index, 0, MAX);
        }
        public void Next()
        {
            Click((Current + 1) % MAX);
        }
        public void Prev()
        {
            int ind = Current - 1;
            if (ind < 0) ind = MAX;
            Click(ind);
        }

    }
}