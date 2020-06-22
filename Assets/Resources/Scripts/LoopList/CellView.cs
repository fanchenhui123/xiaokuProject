using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace Xiaoku
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// It stores references to sub cells
    /// </summary>
    public class CellView : EnhancedScrollerCellView
    {
        public GameObject rowCellViewPrefab;

        int[] widths = new int[21]
        {80, 100, 50, 180, 100, 180, 100,
        100, 50, /**批注**/100, 100, /**质损**/100, 100,
        100, 100, 100, 100, 100,
        100, 100, 100};

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(ref SmallList<Data> data, int startingIndex,int colCount = 21)
        {
            //初始化条件
            if (gameObject.transform.childCount <= 0)
            {
                for (int i = 0; i < colCount; i++)
                {
                    GameObject temp = GameObject.Instantiate(rowCellViewPrefab);
                    temp.name = "row" + i;
                    temp.transform.SetParent(gameObject.transform);
                }
            }
            // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                // if the sub cell is outside the bounds of the data, we pass null to the sub cell
                var rowCellView = gameObject.transform.GetChild(i).GetComponent<RowCellView>();
                rowCellView.SetData(startingIndex + i < data.Count ? data[startingIndex + i] : null);
                rowCellView.SetWidth(widths[i]);
                if (i == 9 || i == gameObject.transform.childCount - 1)
                {
                    rowCellView.SetClickDialog(data[startingIndex + i].someText);
                }
            }
        }

    }
}