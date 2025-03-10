using Test_Assistant.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Models
{
    public class SpecialActionListElement : ListBox
    {
        public SpecialActionListElement(string selectedItem = "")
        {
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
