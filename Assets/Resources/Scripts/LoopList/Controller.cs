using UnityEngine;
using System.Collections;
using EnhancedUI;
using EnhancedUI.EnhancedScroller;
using UnityEngine.UI;

namespace Xiaoku
{
    /// <summary>
    /// This example shows how to simulate a grid with a fixed number of cells per row
    /// The data is stored as normal, but the differences in this example are:
    /// 
    /// 1) The scroller is told the data count is the number of data elements divided by the number of cells per row
    /// 2) The cell view is passed a reference to the data set with the offset index of the first cell in the row
    public class Controller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Internal representation of our data. Note that the scroller will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        private SmallList<Data> _data;

        /// <summary>
        /// This is our scroller we will be a delegate for
        /// </summary>
        public EnhancedScroller scroller;

        float viewWidth;
        float contentWitdh;
        RectTransform content;

        private void Update()
        {
            //横向滚动条
            if (content)
            {
                float x = -(contentWitdh - viewWidth + 20) * hScrollbar.value;
                float y = content.localPosition.y;
                float z = content.localPosition.z;
                content.localPosition = new Vector3(x, y, z);
            }
        }

        /// <summary>
        /// This will be the prefab of each cell in our scroller. The cell view will
        /// hold references to each row sub cell
        /// </summary>
        public EnhancedScrollerCellView cellViewPrefab;

        int numberOfCellsPerRow = 21;

        public Scrollbar hScrollbar;//自写横向滚调条

        /// <summary>
        /// Be sure to set up your references to the scroller after the Awake function. The 
        /// scroller does some internal configuration in its own Awake function. If you need to
        /// do this in the Awake function, you can set up the script order through the Unity editor.
        /// In this case, be sure to set the EnhancedScroller's script before your delegate.
        /// 
        /// In this example, we are calling our initializations in the delegate's Start function,
        /// but it could have been done later, perhaps in the Update function.
        /// </summary>
        void Start()
        {
            // tell the scroller that this script will be its delegate
            scroller.Delegate = this;
            _data = new SmallList<Data>();
            // load in a large set of data
            //LoadData();
            //InitHScrollbar();

        }

        //初始化横向滚动条
        void InitHScrollbar()
        {
            var container = scroller.transform.Find("Container").GetComponent<RectTransform>();
            var child2 = container.transform.GetChild(1).GetComponent<RectTransform>();
            //Debug.Log("container pos:" + container.localPosition);
            this.viewWidth = container.rect.width;
            this.contentWitdh = child2.rect.width;
            this.content = container;
            hScrollbar.size = this.viewWidth / this.contentWitdh;
            //hScrollbar.value = 0;
        }

        /// <summary>
        /// Populates the data with a lot of records
        /// </summary>
        private void LoadData()
        {
            // set up some simple data
            _data = new SmallList<Data>();
            for (var i = 0; i < 1000; i ++)
            {
                _data.Add(new Data() { someText = i.ToString() });
            }

            // tell the scroller to reload now that we have the data
            scroller.ReloadData();
            
        }

        public void SetData(int id, string str)
        {
           for(int i = 0; i < _data.Count; i++)
            {
                if(_data[i].id == id)
                {
                    _data[i] = new Data() { id = id, someText = str, isEnable = _data[i].isEnable };
                }
            }
        }

        //把string数组转成循环列表可读格式
        public void LoadData(SmallList<Data> data, int colCount)
        {
            numberOfCellsPerRow = colCount;
            _data.Clear();
            for (int i = 0; i < data.Count; i++)
            {
                _data.Add(data[i]);
            }
            scroller.ReloadData();
            InitHScrollbar();
        }

        //返回数据
        public SmallList<Data> GetData()
        {
            return _data;
        }

        #region EnhancedScroller Handlers

        /// <summary>
        /// This tells the scroller the number of cells that should have room allocated.
        /// For this example, the count is the number of data elements divided by the number of cells per row (rounded up using Mathf.CeilToInt)
        /// </summary>
        /// <param name="scroller">The scroller that is requesting the data size</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return Mathf.CeilToInt((float)_data.Count / (float)numberOfCellsPerRow);
        }

        /// <summary>
        /// This tells the scroller what the size of a given cell will be. Cells can be any size and do not have
        /// to be uniform. For vertical scrollers the cell size will be the height. For horizontal scrollers the
        /// cell size will be the width.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell size</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <returns>The size of the cell</returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return 30f;
        }

        /// <summary>
        /// Gets the cell to be displayed. You can have numerous cell types, allowing variety in your list.
        /// Some examples of this would be headers, footers, and other grouping cells.
        /// </summary>
        /// <param name="scroller">The scroller requesting the cell</param>
        /// <param name="dataIndex">The index of the data that the scroller is requesting</param>
        /// <param name="cellIndex">The index of the list. This will likely be different from the dataIndex if the scroller is looping</param>
        /// <returns>The cell for the scroller to use</returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            // first, we get a cell from the scroller by passing a prefab.
            // if the scroller finds one it can recycle it will do so, otherwise
            // it will create a new cell.
            CellView cellView = scroller.GetCellView(cellViewPrefab) as CellView;

            cellView.name = "Cell " + (dataIndex * numberOfCellsPerRow).ToString() + " to " + ((dataIndex * numberOfCellsPerRow) + numberOfCellsPerRow - 1).ToString();

            // pass in a reference to our data set with the offset for this cell
            cellView.SetData(ref _data, dataIndex * numberOfCellsPerRow, numberOfCellsPerRow);

            // return the cell to the scroller
            return cellView;
        }

        #endregion
    }
}
