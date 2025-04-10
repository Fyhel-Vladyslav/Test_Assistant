using Test_Assistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Test_Assistant.pages
{
    public class ListPage: FlowLayoutPanel
    {
        private ListPage _thisLink;
        private FileData _fileData;
        private ConfirmDelete _confirmDelete = new ConfirmDelete();
        public ListPage(FileData fileData)
        {
            _thisLink = this;
            _fileData = fileData;
            Width = 800;
            Height = 400;
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
                    List<string> labelsNames = new List<string>();

                    foreach (var order in _fileData.OrderLists[i].caseIds)
                        labelsNames.Add(_fileData.Testcases.FirstOrDefault(x => x.id == order).name);

                    var dragAndDropElement = new DragAndDropElement<int>(_fileData.OrderLists[i].caseIds, labelsNames);
                    dragAndDropElement.BorderStyle = BorderStyle.FixedSingle;
                    dragAndDropElement.Width = 700;


                    _thisLink.Controls.Add(dragAndDropElement);

                    // -------------------------------------------------------

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
                        Tag = i
                    };
                    _deleteButton.Click += _deleteButton_Click;

                    _thisLink.Controls.Add(_deleteButton);


                    flowLayoutPanelHeinght += dragAndDropElement.Height;

                }
            }
        }
        private void _deleteButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int id = (int)clickedButton.Tag;

            if (_confirmDelete.CallWindow())
            {
                _fileData.OrderLists.RemoveAt(id);
                // Call your actual delete logic here with id

                _thisLink.Controls.Clear();
                CreateComponents();
            }
        }
    }
}
