using Test_Assistant.Enums;
using Test_Assistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_Assistant.pagesModels
{
    public class EditSpecialActionPageForm : Form
    {
        private TextBox pathTextBox;
        private TextBox compareToTextBox;
        private Button saveButton;
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
            Height = 400;
            pathLabel = new Label { Text = "Photo path", Dock = DockStyle.Top };
            pathTextBox = new TextBox { Text = action.path, Dock = DockStyle.Top };

            areaStartLabel = new Label { Text = "Area Start", Dock = DockStyle.Top };
            xAreaStartField = new NumericUpDown {Margin=new Padding(20), Value = action.xAreaStart, Dock = DockStyle.Top };
            yAreaStartField = new NumericUpDown { Margin = new Padding(20), Value = action.yAreaStart, Dock = DockStyle.Top };
            areaEndLabel = new Label { Text = "Area End", Dock = DockStyle.Top };
            xAreaEndField = new NumericUpDown {Margin = new Padding(20), Value = action.xAreaEnd, Dock = DockStyle.Top };
            yAreaEndField = new NumericUpDown { Margin = new Padding(20), Value = action.yAreaEnd, Dock = DockStyle.Top };

            compareToLabel = new Label { Text = "Text compare to", Dock = DockStyle.Bottom };
            compareToTextBox = new TextBox { Text = action.path, Dock = DockStyle.Bottom };

            saveButton = new Button { Text = "Save", Dock = DockStyle.Bottom };

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
    }
}
