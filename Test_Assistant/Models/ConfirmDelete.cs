﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Models
{
    public class ConfirmDelete
    {
        /// <summary>
        /// Displays a confirmation dialog to the user asking if they want to delete an item.
        /// </summary>
        /// <returns>True if the user confirms deletion; otherwise, false.</returns>
        public bool CallWindow(string message = "Are you sure you want to delete this item?")
        {
            return MessageBox.Show(message,
                                   "Confirm Page",
                                   MessageBoxButtons.YesNo,
                                   MessageBoxIcon.Warning) == DialogResult.Yes;
        }
    }
}
