using Test_Assistant.Enums;
using Test_Assistant.Models;
using Test_Assistant.pagesModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.Design.AxImporter;
using Test_Assistant.Processors;

namespace Test_Assistant.pages
{
    public class CasesPage : FlowLayoutPanel
    {
        private MouseAndKeyboardProcessor _mouseAndKeyboardProcessor;
        private FileData _fileData;
        private CasesPage _thisLink;
        private ConfirmDelete _confirmDelete = new ConfirmDelete();
        private int deleteButtonsWidth = 140;

        private class ActionPanelElemnt : FlowLayoutPanel
        {
            public ActionPanelElemnt(TestCaseAction action = null)
            {

                WrapContents = true;
                AutoSize = true;
                Margin = new Padding(5);
                BackColor = Color.LightSkyBlue;
                Dock = DockStyle.Bottom;

                Controls.Add(new TextBox { Width = 40, Margin = new Padding(3), Text = action?.x.ToString() });
                Controls.Add(new TextBox { Width = 40, Margin = new Padding(3), Text = action?.y.ToString() });
                Controls.Add(new TextBox { Width = 40, Margin = new Padding(12, 3, 3, 3), Text = action?.t.ToString() });
            }
        }

        public CasesPage(FileData fileData, MouseAndKeyboardProcessor mouseAndKeyboardProcessor)
        {
            _mouseAndKeyboardProcessor = mouseAndKeyboardProcessor;
            _fileData = fileData;
            _thisLink = this;
            Width = (int)WindowParamethers.TotalWidth;
            Padding = new Padding(20);
            AutoScroll = true;
            Dock = DockStyle.Left;

            CreateComponents();
        }

        private void RefreshPage()
        {
            _thisLink.Controls.Clear();
            CreateComponents();
        }
        private void CreateComponents()
        {
            int flowLayoutPanelHeinght = 0;
            int testCasesAmount = _fileData.Testcases.Count();

            if (testCasesAmount > 0)
            {
                for (int i = 0; i < testCasesAmount; i++)
                {
                    var testCase = _fileData.Testcases[i];

                    var NameInput = new TextBox();
                    NameInput.Tag = testCase.id;
                    NameInput.Width = 330;
                    NameInput.Text = testCase.name ?? $"Testcase_{i}";
                    NameInput.Margin = new Padding(10);
                    NameInput.Dock = DockStyle.Top;


                    var testCaseElement = new FlowLayoutPanel
                    {
                        Tag = testCase.id,
                        Width = (int)WindowParamethers.TotalWidth - deleteButtonsWidth,
                        Height = 100,
                        TabIndex = testCase.id,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        AutoScroll = true,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White,
                    };

                    int j = 0;
                    foreach (var action in testCase.actions)
                    {
                        ActionPanelElemnt actionPanel = new ActionPanelElemnt(action);

                        var fileSpecialAction = _fileData.SpecialActions.FirstOrDefault(p => p.id == action.specialActionId);
                        var newSpecAction = new SpecialActionListElement(fileSpecialAction != null ? fileSpecialAction.actionName : null);
                        newSpecAction.TabIndex = _fileData.Testcases[i].id;
                        newSpecAction.MouseDown += (sender, e) =>
                        {
                            if (e.Button == MouseButtons.Right)
                            {
                                if (newSpecAction.SelectedItem != null && newSpecAction.SelectedItem != "")
                                {
                                    string selectedText = newSpecAction.SelectedItem.ToString();
                                    EditSpecialActionPageForm editForm = new EditSpecialActionPageForm(fileSpecialAction);

                                    if (editForm.ShowDialog() != DialogResult.OK)
                                    {
                                        MessageBox.Show("Entered Data were lost");
                                    }
                                }
                            }
                        };

                        actionPanel.TabIndex = j++;
                        testCaseElement.Controls.Add(actionPanel);
                        testCaseElement.Controls.Add(newSpecAction);
                    }
                    Button addButton = new Button
                    {
                        Font = new Font("Arial", 8),
                        Text = "new action",
                        Width = 85,
                        Height = 35,
                        Margin = new Padding(3),
                    };

                    CreateTestCaseButtonClick(i, _fileData, testCaseElement, addButton);

                    _thisLink.Controls.Add(NameInput);

                    testCaseElement.Controls.Add(addButton);
                    _thisLink.Controls.Add(testCaseElement);



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
                        Tag = testCase.id
                    };
                    _deleteButton.Click += _deleteButton_Click;

                    _thisLink.Controls.Add(_deleteButton);

                    flowLayoutPanelHeinght += testCaseElement.Height;
                }
                Height = flowLayoutPanelHeinght + 70;
            }

            var _addButton = new Button
            {
                Location = new Point(50, 50),
                Text = "Create new testcase order",
                BackColor = Color.LightGray,
                ForeColor = Color.DarkSlateGray,
                Font = new Font("Arial", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Height = 40,
                Margin = new Padding(5),
                Width = (int)WindowParamethers.TotalWidth - deleteButtonsWidth
            };
            _addButton.Click += _addButton_Click;

            _thisLink.Controls.Add(_addButton);
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int id = (int)clickedButton.Tag;

            if (_confirmDelete.CallWindow())
            {
                _fileData.Testcases.Remove(_fileData.Testcases.First(p => p.id == id));
                // Call your actual delete logic here with id

                _thisLink.Controls.Clear();
                CreateComponents();
            }
        }
        /* public void SaveAllToLocalData()
         {
             foreach( var testCaseElement in Controls)
             {
                 var testCase = (FlowLayoutPanel)testCaseElement;
                 var testCaseId = (int)testCase.Tag;
                 var actions = testCase.Controls.OfType<ActionPanelElemnt>().ToList();
                 var specialActions = testCase.Controls.OfType<SpecialActionListElement>().ToList();
                 for (int j = 0; j < actions.Count; j++)
                 {
                     var tableAction = actions[j];

                     _fileData.Testcases[testCaseId].actions[j].x = int.Parse(tableAction.Controls[0].Text);
                     _fileData.Testcases[testCaseId].actions[j].y = int.Parse(tableAction.Controls[1].Text);
                     _fileData.Testcases[testCaseId].actions[j].t = int.Parse(tableAction.Controls[2].Text);
                 }

                 foreach ( var tableSpecialAction in specialActions)
                 {
                     if (tableSpecialAction.SelectedItem != null)
                     {
                         if (_fileData.SpecialActions.Count > 0)
                         {
                             var fileAction = _fileData.SpecialActions.FirstOrDefault(p => p.testCaseId == testCaseId && p.testCaseActionId == tableSpecialAction.TabIndex);
                             if (fileAction != null)
                             {
                                 fileAction.actionName = tableSpecialAction.SelectedItem.ToString();
                             }
                             else
                                 _fileData.SpecialActions.Add(new SpecialAction
                                 {
                                     id = _fileData.SpecialActions.Max(x => x.id) + 1,
                                     actionName = tableSpecialAction.SelectedItem.ToString(),
                                     testCaseId = testCaseId,
                                     testCaseActionId = tableSpecialAction.TabIndex,
                                 });
                         }
                         else
                         {
                             _fileData.SpecialActions.Add(
                                 new SpecialAction
                                 {
                                     id = 1,
                                     actionName = tableSpecialAction.SelectedItem.ToString(),
                                     testCaseId = testCaseId,
                                     testCaseActionId = tableSpecialAction.TabIndex,
                                 }
                             );
                         }
                     }
                 }
             }
         }
        */
        private void _addButton_Click(object sender, EventArgs e)
        {
            if (_confirmDelete.CallWindow("Do you want to start recording actions now?"))
            {
                if (_mouseAndKeyboardProcessor != null)
                    _mouseAndKeyboardProcessor.StartRecording();
            }
            else
            {
                var TestcaseLine = new TestCaseData
                {
                    id = 0,
                    actions = new List<TestCaseAction>(),
                };
                if (_fileData.Testcases.Count() > 0)
                    TestcaseLine.id = _fileData.Testcases.Last().id + 1;

                TestcaseLine.name = $"TestCase_{TestcaseLine.id}"; ;

                _fileData.Testcases.Add(TestcaseLine);

                RefreshPage();
            }
        }
        public void SaveAllToLocalData()
        {
            foreach (var testCaseElement in Controls)
            {
                if (testCaseElement is FlowLayoutPanel testCasePanel)
                {
                    var testCasePanelId = (int)testCasePanel.Tag;
                    var fileTestCase = _fileData.Testcases[testCasePanelId];
                    var actions = testCasePanel.Controls.OfType<ActionPanelElemnt>().ToList();
                    var panelSpecialActions = testCasePanel.Controls.OfType<SpecialActionListElement>().ToList();
                    for (int j = 0; j < actions.Count; j++)
                    {
                        var tableAction = actions[j];

                        if (tableAction != null)
                        {
                            var fileAction = fileTestCase.actions[j];
                            fileAction.x = int.Parse(tableAction.Controls[0].Text);

                            fileAction.y = int.Parse(tableAction.Controls[1].Text);
                            fileAction.t = int.Parse(tableAction.Controls[2].Text);

                            var specialActionsElement = panelSpecialActions[j];

                            if (specialActionsElement != null)
                            {
                                var newValue = specialActionsElement.SelectedItem;
                                var fileSpecialAction = _fileData.SpecialActions.FirstOrDefault(p => p.id == fileAction.specialActionId);

                                if (newValue == null || newValue == "")//if newValue is empty
                                {
                                    if (fileSpecialAction != null)//if it really realy exist
                                    {
                                        _fileData.SpecialActions.Remove(fileSpecialAction);
                                        fileAction.specialActionId = 0;
                                    }
                                }
                                else //if newValue == something
                                {
                                    if (fileSpecialAction == null)// wasn't created yet
                                    {
                                        _fileData.SpecialActions.Add(new SpecialAction
                                        {
                                            id = _fileData.SpecialActions.Any() ? _fileData.SpecialActions.Last().id + 1 : 1,
                                            actionName = newValue.ToString(),
                                            //TODO: save initialvalues for Special action fields
                                        });
                                        fileAction.specialActionId = _fileData.SpecialActions.Last().id;
                                    }
                                    else // was already created
                                        fileSpecialAction.actionName = newValue.ToString();
                                }
                            }
                        }
                    }
                }
                if (testCaseElement is TextBox nameElement)
                {
                    var nameFileData = _fileData.Testcases.FirstOrDefault(x => x.id == (int)nameElement.Tag);
                    if (nameFileData != null)
                    {
                        if (!String.IsNullOrEmpty(nameElement.Text))
                        nameFileData.name = nameElement.Text;
                    }
                }
            }

        }


        private void CreateTestCaseButtonClick(int i, FileData fileData, FlowLayoutPanel testCaseElement, Button addButton)
        {
            addButton.Click += (sender, e) =>
            {
                fileData.Testcases[i].actions.Add(new TestCaseAction());

                ActionPanelElemnt actionPanel = new ActionPanelElemnt();
                testCaseElement.Controls.Add(actionPanel);
                //testCaseElement.Controls.SetChildIndex(actionPanel, testCaseElement.Controls.GetChildIndex(addButton));
            };
        }
    }
}
