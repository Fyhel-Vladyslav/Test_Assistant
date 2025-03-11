using Test_Assistant.Enums;
using Test_Assistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Test_Assistant.pagesModels
{
    public class EditSpecialActionPageForm : Form
    {
        private TextBox pathTextBox;
        private TextBox compareToTextBox;
        private Button saveButton;
        private Button setAreaButton;
        private NumericUpDown xAreaStartField;
        private NumericUpDown yAreaStartField;
        private NumericUpDown xAreaEndField;
        private NumericUpDown yAreaEndField;
        private Label areaStartLabel;
        private Label areaEndLabel;
        private Label pathLabel;
        private Label compareToLabel;
        public string UpdatedText { get; private set; }

        public EditSpecialActionPageForm(SpecialAction action)
        {
            if (action == null)
            {
                action = new SpecialAction();
            }
            Height = 400;
            pathLabel = new Label { Text = "Photo path", Dock = DockStyle.Top };
            pathTextBox = new TextBox { Text = action.path, Dock = DockStyle.Top };

            areaStartLabel = new Label { Text = "Start Point of Area", Dock = DockStyle.Top };
            xAreaStartField = new NumericUpDown { Margin = new Padding(20), Dock = DockStyle.Top, Minimum = 0, Maximum = 2000 };
            xAreaStartField.Value = action.xAreaStart;
            yAreaStartField = new NumericUpDown { Margin = new Padding(20), Dock = DockStyle.Top, Minimum = 0, Maximum = 2000 };
            yAreaStartField.Value = action.yAreaStart;
            areaEndLabel = new Label { Text = "End Point of Area", Dock = DockStyle.Top };
            xAreaEndField = new NumericUpDown { Margin = new Padding(20), Dock = DockStyle.Top, Minimum = 0, Maximum = 2000 };
            xAreaEndField.Value = action.xAreaEnd;
            yAreaEndField = new NumericUpDown { Margin = new Padding(20), Dock = DockStyle.Top, Minimum = 0, Maximum = 2000 };
            yAreaEndField.Value = action.yAreaEnd;
            setAreaButton = new Button { Text = "Set area of parsing", Dock = DockStyle.Top, Height = 30 };

            compareToLabel = new Label { Text = "Text compare to", Dock = DockStyle.Bottom };
            compareToTextBox = new TextBox { Text = action.path, Dock = DockStyle.Bottom };

            saveButton = new Button { Text = "Save", Dock = DockStyle.Bottom, Height = 30 };

            saveButton.Click += (s, e) =>
            {
                action.path = pathTextBox.Text;
                action.comparedTo = compareToTextBox.Text;
                action.xAreaStart = (int)xAreaStartField.Value;
                action.yAreaStart = (int)yAreaStartField.Value;
                action.xAreaEnd = (int)xAreaEndField.Value;
                action.yAreaEnd = (int)yAreaEndField.Value;

                DialogResult = DialogResult.OK;
                Close();
            };
            setAreaButton.Click += (s, e) => SetAreaButton_ClickAsync(action, s, e);

            Controls.Add(setAreaButton);
            Controls.Add(xAreaStartField);
            Controls.Add(yAreaStartField);
            Controls.Add(areaEndLabel);
            Controls.Add(xAreaEndField);
            Controls.Add(yAreaEndField);
            Controls.Add(areaStartLabel);

            Controls.Add(pathTextBox);
            Controls.Add(pathLabel);


            Controls.Add(compareToLabel);
            Controls.Add(compareToTextBox);

            Controls.Add(saveButton);
        }

        private async void SetAreaButton_ClickAsync(SpecialAction action, object sender, EventArgs e)
        {
            var selector = new ScreenSelection();
            List<Point> selection = await selector.StartSelection();

            action.xAreaStart = selection[0].X;
            action.yAreaStart = selection[0].Y;
            action.xAreaEnd = selection[1].X;
            action.yAreaEnd = selection[1].Y;

            xAreaStartField.Value = action.xAreaStart;
            yAreaStartField.Value = action.yAreaStart;
            xAreaEndField.Value = action.xAreaEnd;
            yAreaEndField.Value = action.yAreaEnd;
        }
    }
}
