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

namespace Test_Assistant.pages
{
    public class CasesPage: FlowLayoutPanel
    {
        private FileData _fileData;
        private class ActionPanelElemnt : FlowLayoutPanel
        {
            public ActionPanelElemnt(TestCaseAction action=null)
            {

                WrapContents = true;
                AutoSize= true;
                Margin = new Padding(5);
                BackColor = Color.LightSkyBlue;
                


                Controls.Add(new TextBox{Width = 40,Margin = new Padding(3),Text = action?.x.ToString()});
                Controls.Add(new TextBox { Width = 40, Margin = new Padding(3), Text = action?.y.ToString() });
                Controls.Add(new TextBox { Width = 40, Margin = new Padding(12, 3, 3, 3), Text = action?.t.ToString() });
            }
        }

        public CasesPage(FileData fileData)
        {
            _fileData = fileData;
            Width = 800;
            Padding = new Padding(20);

            int flowLayoutPanelHeinght = 0;
            int testCasesAmount = fileData.Testcases.Count();

            if (testCasesAmount > 0)
            {
                for (int i = 0; i < testCasesAmount; i++)
                {
                    var testCaseElement = new FlowLayoutPanel
                    {
                        Tag = fileData.Testcases[i].id,
                        Width = 750,
                        Height = 100,
                        TabIndex = fileData.Testcases[i].id,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        AutoScroll = true,
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.White,
                    };

                    int j = 0;
                    foreach (var action in fileData.Testcases[i].actions)
                    {
                        ActionPanelElemnt actionPanel = new ActionPanelElemnt(action);

                        var fileSpecialAction = fileData.SpecialActions.FirstOrDefault(p => p.testCaseActionId == j && p.testCaseId == i);
                        var newSpecAction = new SpecialActionListElement(fileSpecialAction != null ? fileSpecialAction.actionName : null);
                        newSpecAction.TabIndex = fileData.Testcases[i].id;
                        newSpecAction.DoubleClick += (sender, e) =>
                        {
                            if (newSpecAction.SelectedItem == null) return;

                            string selectedText = newSpecAction.SelectedItem.ToString();
                            EditSpecialActionPageForm editForm = new EditSpecialActionPageForm(fileSpecialAction);

                            if (editForm.ShowDialog() != DialogResult.OK)
                            {
                                MessageBox.Show("Entered Data were lost");
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

                    AddButtonClick(i, fileData, testCaseElement, addButton);

                    testCaseElement.Controls.Add(addButton);
                    Controls.Add(testCaseElement);
                    flowLayoutPanelHeinght += testCaseElement.Height + 10;
                }
                Height = flowLayoutPanelHeinght + 20;
            }
        }


        public void SaveAllToLocalData()
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

        private void AddButtonClick(int i, FileData fileData, FlowLayoutPanel testCaseElement, Button addButton)
        {
            addButton.Click += (sender, e) =>
            {
                fileData.Testcases[i].actions.Add(new TestCaseAction());

                ActionPanelElemnt actionPanel = new ActionPanelElemnt();
                testCaseElement.Controls.Add(actionPanel);
                testCaseElement.Controls.SetChildIndex(actionPanel, testCaseElement.Controls.GetChildIndex(addButton));
            };
        }
    }
}
