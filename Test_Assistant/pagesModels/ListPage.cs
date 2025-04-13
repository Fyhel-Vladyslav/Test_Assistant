using Test_Assistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using Test_Assistant.Enums;

namespace Test_Assistant.pages
{
    public class ListPage: FlowLayoutPanel
    {
        private ListPage _thisLink;
        private FileData _fileData;
        private ConfirmDelete _confirmDelete = new ConfirmDelete();
        private int deleteButtonsWidth = 220;
        public ListPage(FileData fileData)
        {
            _thisLink = this;
            _fileData = fileData;
            Width = (int)WindowParamethers.TotalWidth;
            FlowDirection = FlowDirection.LeftToRight;
            WrapContents = true; // Дозволяємо перенесення елементів на наступний рядок
            AllowDrop = true; 
            AutoScroll = true; // Додаємо прокрутку, якщо елементів багато
            Padding = new Padding(10);
            Dock = DockStyle.Left;

            CreateComponents();
        }

        private void CreateComponents()
        {
            int flowLayoutPanelHeinght = 0;
            int ChecklistsAmount = _fileData.OrderLists.Count();

            if (ChecklistsAmount > 0)
            {
                for (int i = 0; i < _fileData.OrderLists.Count(); i++)
                {
                    var checklist = _fileData.OrderLists[i];
                    List<string> labelsNames = new List<string>();

                    foreach (var caseId in checklist.caseIds)
                        labelsNames.Add(_fileData.Testcases.FirstOrDefault(x => x.id == caseId)?.name ?? "deleted");

                    var dragAndDropElement = new DragAndDropElement<int>(checklist.caseIds, labelsNames);
                    dragAndDropElement.BorderStyle = BorderStyle.FixedSingle;
                    dragAndDropElement.Width = (int)WindowParamethers.TotalWidth- deleteButtonsWidth;


                    _thisLink.Controls.Add(dragAndDropElement);

                    // -------------------------------------------------------

                    var _comboBox = new ComboBox()
                    {
                        Location = new Point(50, 50),
                        Text = "Add item",
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        Dock = DockStyle.Right,
                        Width = 100,
                        Tag = checklist.id,
                    };
                    //_comboBox.Click += _deleteButton_Click;

                    _comboBox.Items.AddRange(_fileData.Testcases.Select(x => $"{x.name}_{x.id}").ToArray());
                    _comboBox.SelectionChangeCommitted += _comboBox_SelectionChangeCommitted;




                    _thisLink.Controls.Add(_comboBox);



                    var _deleteButton = new Button
                    {
                        Location = new Point(50, 50),
                        Text = "Drop",
                        BackColor = Color.Red,
                        ForeColor = Color.White,
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Dock = DockStyle.Right,
                        Height = 50,
                        Margin = new Padding(5),
                        AllowDrop = true,
                        Width = 60,
                        Tag = checklist.id
                    };
                    _deleteButton.Click += _deleteButton_Click;

                    _thisLink.Controls.Add(_deleteButton);


                    flowLayoutPanelHeinght += dragAndDropElement.Height;

                }
            }



            var _addButton = new Button
            {
                Text = "Create new testcase order",
                BackColor = Color.LightGray,
                ForeColor = Color.DarkSlateGray,
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 40,
                Margin = new Padding(5),
                Width = (int)WindowParamethers.TotalWidth- deleteButtonsWidth
            };
            _addButton.Click += _addButton_Click;

            _thisLink.Controls.Add(_addButton);

        }

        private void RefreshPage()
        {
            _thisLink.Controls.Clear();
            CreateComponents();
        }
        private void _comboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var combobox = (ComboBox)sender;
            string selected = combobox.SelectedItem.ToString();

            if (selected == null)
                return;

            var orderListId = (int)combobox.Tag;
            // Extract ID after the last underscore
            int lastUnderscore = selected.LastIndexOf('_');
            if (lastUnderscore != -1 && int.TryParse(selected.Substring(lastUnderscore + 1), out int testCaseId))
            {
                _fileData.OrderLists.FirstOrDefault(p => p.id == orderListId).caseIds.Add(testCaseId);

                // Remove the selected item from the ComboBox
                combobox.SelectedIndex = -1;
                combobox.Text = "Add item";

                RefreshPage();
            }
        }
        private void _deleteButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int id = (int)clickedButton.Tag;

            if (_confirmDelete.CallWindow())
            {
                _fileData.OrderLists.Remove(_fileData.OrderLists.First(p => p.id == id));

                RefreshPage();
            }
        }
        private void _addButton_Click(object sender, EventArgs e)
        {
                var orderList = new OrderList
                {
                    id = 0,
                    caseIds = new List<int>()
                };
                if (_fileData.OrderLists.Count() > 0)
                    orderList.id = _fileData.OrderLists.Last().id + 1;

                orderList.name = $"New OrderList{orderList.id}";

                _fileData.OrderLists.Add(orderList);

                RefreshPage();
        }
    }
}
