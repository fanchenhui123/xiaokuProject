using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace Xiaoku
{
    /// <summary>
    /// This is the sub cell of the row cell
    /// </summary>
    public class RowCellView : MonoBehaviour
    {
        public GameObject container;
        public InputField input;
        public int id;
        /// <summary>
        /// This function just takes the Demo data and displays it 0
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data)
        {
            // this cell was outside the range of the data, so we disable the container.
            // Note: We could have disable the cell gameobject instead of a child container,
            // but that can cause problems if you are trying to get components (disabled objects are ignored).
            container.SetActive(data != null);

            if (data != null)
            {
                // set the text if the cell is inside the data range
                input.text = data.someText;
                input.interactable = data.isEnable;
                id = data.id;
            }
        }

        public void SetWidth(float w)
        {
            var h = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(w, h);
        }

        public void SetClickDialog(string text)
        {
            var cg = input.gameObject.GetComponent<ClickDialogPointer>();
            if (cg == null)
            {
                cg = input.gameObject.AddComponent<ClickDialogPointer>();
            }
            cg.text = text;
        }
    }
}