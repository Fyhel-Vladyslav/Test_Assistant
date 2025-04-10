using System.Collections.Generic;
using System.Linq;

namespace Test_Assistant.Models
{
    public class DragAndDropElement<T> : FlowLayoutPanel
    {
        private List<string> _labelsNames;
        private List<T> _orderList;
        private FlowLayoutPanel _DragAndDropPanel;
        private Label _deleteLabel;
        private static Control _mouseDownElement;
        private int _amountOfItemsInLine;

        public bool isDeleteButtonEnabled { get; set; }
        //isDeleteButtonEnabled = true;
        //isAddButtonEnabled = false;

        public int itemWidth { get; set; }
        public int itemHeight { get; set; }
        public Color itemColor { get; set; }



        public DragAndDropElement()
        {
        }

        public DragAndDropElement(
            List <T> changebleListReference,
            List<string> labelsNames = null,
            int amountOfItemsInLine = 4,
            bool isDeleteButtonEnabled = true)
        {
            this.isDeleteButtonEnabled = isDeleteButtonEnabled;

            Initialize(changebleListReference, amountOfItemsInLine, labelsNames);
        }

        public void Initialize(List<T> changebleListReference, int amountOfItemsInLine = 4, List<string> labelsNames=null)
        {
            _DragAndDropPanel = this;
            this.AllowDrop = true;
            this.DragEnter += DragAndDropElement_DragEnter;
            this.DragDrop += DragAndDropElement_DragDrop;
            this.FlowDirection = FlowDirection.LeftToRight;
            this.Margin = new Padding(1);
            this.AutoScroll = true; // Додаємо прокрутку, якщо елементів багато

            _orderList = changebleListReference;
            _amountOfItemsInLine = amountOfItemsInLine;
            _labelsNames = labelsNames;

            itemColor = Color.LightBlue;
            itemWidth = 120;
            itemHeight = 50;

            this.Width = (itemWidth + 10) * amountOfItemsInLine;
            this.Height = CalculateTestCaseHeight(_orderList.Count() + (isDeleteButtonEnabled ? 1 : 0));

            CreateDragAndDropElements();
        }


        private void CreateDragAndDropElements()
        {
            if (isDeleteButtonEnabled){
                CreateDeleteButton();
            }

            

            if (_orderList == null){
                return;
            }

            for (int i = 0; i < _orderList.Count(); i++)
            {
                
                //if element is empty - replace it to "Empty"
                if (_labelsNames!=null && String.IsNullOrEmpty(_labelsNames[i]))
                    _labelsNames[i] = "Empty";

                Label menuItem = new Label
                {
                    Tag = _orderList[i],
                    BackColor = itemColor,
                    Padding = new Padding(10),
                    Height = itemHeight,
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand,
                    Width = itemWidth,
                    Margin = new Padding(5),
                    Text = _labelsNames != null ? _labelsNames[i] : i.ToString()
                };

                menuItem.MouseDown += MenuItem_MouseDown;
                _DragAndDropPanel.Controls.Add(menuItem);
            }
        }

        private void CreateDeleteButton()
        {
            _deleteLabel = new Label
            {
                Location = new Point(50, 50),
                Text = "Delete",
                BackColor = Color.LightCoral,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = itemHeight,
                Margin = new Padding(5),
                AllowDrop = true,
                Width = itemWidth
            };

            // Attach drag-and-drop events to the delete label
            _deleteLabel.DragEnter += DeleteLabel_DragEnter;
            _deleteLabel.DragDrop += DeleteLabel_DragDrop;

            _DragAndDropPanel.Controls.Add(_deleteLabel);
        }

        private void MenuItem_MouseDown(object sender, MouseEventArgs e)/// On mouse down
        {
            _mouseDownElement = _DragAndDropPanel;
            if (sender is Label label && e.Button == MouseButtons.Left)
            { 
                DoDragDrop(label, DragDropEffects.Move);
            }
        }
        private void DragAndDropElement_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Label))){
                e.Effect = DragDropEffects.Move; // Allow the drag operation
            }
            else{
                e.Effect = DragDropEffects.None; // Disallow other drag type s
            }
        }

        private void DragAndDropElement_DragDrop(object sender, DragEventArgs e)/// On mouse up
        {
            if (e.Data.GetData(typeof(Label)) is Label touchedLabel)
            {
                // Determine the position to insert the item
                Control target = _DragAndDropPanel.GetChildAtPoint(_DragAndDropPanel.PointToClient(new Point(e.X, e.Y)));

                // Check if the parent of draged element is found
                if (_mouseDownElement == null)
                    return;

                // Check if the parent of draged element is the same as the parent of the droped element
                if (_mouseDownElement.Handle != _DragAndDropPanel.Handle)
                    return;

                // Check if draged element is different of the droped element
                if (target != null && target != touchedLabel)
                {
                    int targetPanelIndex = _DragAndDropPanel.Controls.GetChildIndex(target);

                    if (targetPanelIndex >= 0)
                        _DragAndDropPanel.Controls.SetChildIndex(touchedLabel, targetPanelIndex);// Swap the items

                    if (touchedLabel.Tag is T touchedElement)
                    {
                        if (target.Tag is T targetElement)
                        {

                            if (touchedElement == null)
                                return;//TODO : Add error message

                            if (targetElement == null)
                                return;//TODO : Add error message

                            // get the index of the touched and target elements
                            int touchedId = _orderList.IndexOf(touchedElement);
                            int targetId = _orderList.IndexOf(targetElement);

                            // correction depending on whether the element is dragged forward or backward
                            int corectionIndex = targetId > touchedId ? 1 : 0;

                            // inserting an element into the index and delete the old one
                            _orderList.Insert(targetId + corectionIndex, touchedElement);
                            _orderList.RemoveAt(touchedId + 1 - corectionIndex);
                        }

                    }
                }
            }
            _DragAndDropPanel.Refresh();

        }                

        private void DeleteLabel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Label)))
            {
                e.Effect = DragDropEffects.Move; // Allow dropping
            }
            else
            {
                e.Effect = DragDropEffects.None; // Disallow other drag types
            }
        }

        private void DeleteLabel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(Label)) is Label touchedLabel)
            {
                if (_DragAndDropPanel == null)
                    return;
                if (!_DragAndDropPanel.Controls.Contains(touchedLabel))
                    return;

                // Remove the item from the list
                if (touchedLabel.Tag is T element)
                {
                    if (element == null)
                        return;//TODO : Add error message
                    _orderList.Remove(element);
                }

                // Remove the label from the menu panel
                _DragAndDropPanel.Controls.Remove(touchedLabel);

                // Recreate the panel for new height
                _DragAndDropPanel.Height = CalculateTestCaseHeight(_DragAndDropPanel.Controls.Count);
                _DragAndDropPanel.Refresh();
            }
        }
        public virtual int CalculateTestCaseHeight(int amount)/// Calculate the height of the panel depending on the amount of items
        {
            return ((amount + _amountOfItemsInLine - 1) / _amountOfItemsInLine) * (itemHeight + 15) + 10;
        }
    }
}
