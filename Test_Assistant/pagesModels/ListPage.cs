﻿using Test_Assistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.pages
{
    public class ListPage: FlowLayoutPanel
    {
        public ListPage(FileData fileData)
        {
            Width = 550;
            FlowDirection = FlowDirection.LeftToRight;
            WrapContents = true; // Дозволяємо перенесення елементів на наступний рядок
            AllowDrop = true; 
            AutoScroll = true; // Додаємо прокрутку, якщо елементів багато
            Padding = new Padding(20);

            int flowLayoutPanelHeinght = 0;
            int ChecklistsAmount = fileData.OrderLists.Count();

            if (ChecklistsAmount > 0)
            {
                for (int i = 0; i < fileData.OrderLists.Count(); i++)
                {
                    List<string> labelsNames = new List<string>();

                    foreach (var order in fileData.OrderLists[i].caseIds)
                        labelsNames.Add(fileData.Testcases.FirstOrDefault(x => x.id == order).name);

                    var dragAndDropElement = new DragAndDropElement<int>(fileData.OrderLists[i].caseIds, labelsNames);

                    Controls.Add(dragAndDropElement);
                    flowLayoutPanelHeinght += dragAndDropElement.Height;
                }
                Height = flowLayoutPanelHeinght + 20;
            }
        }

    }
}
