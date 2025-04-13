using Test_Assistant.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Models
{
    public class SpecialActionListElement : ComboBox
    {
        public SpecialActionListElement(string selectedItem = "")
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            Width = 70;
            Height = 40;
            Margin = new Padding(5);
            BackColor = Color.LightGray;
            Items.Add("");
            Items.AddRange(Enum.GetNames(typeof(SpecialActionsEnum)));
            SelectedItem = selectedItem;
        }
    }
}
