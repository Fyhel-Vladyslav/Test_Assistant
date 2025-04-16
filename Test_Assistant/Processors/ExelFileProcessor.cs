using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_Assistant.Enums;
using Test_Assistant.Models;

namespace Test_Assistant.Processors
{
    public class ExelFileProcessor
    {
        private string _exelFilePath;
        private string _exelFileName;
        private FileData _fileData; 

        private XLWorkbook _workbook;
        private IXLWorksheet _worksheet;
        public ExelFileProcessor(FileData fileData, string checkListName, string exelFileName=null, string exelFilePath = null)
        {
            //_exelData = new ExelFileData();
            _exelFilePath = exelFilePath ?? $".\\checkLists";
            _exelFileName = exelFileName ?? $"{checkListName}.xlsx" ?? $"CheckList_{DateTime.Now:MM-dd_HH-mm-ss}.xlsx";
            _fileData = fileData;

            _workbook = new XLWorkbook();
            _worksheet = _workbook.Worksheets.Add("Sheet1");

            _worksheet.Cell(1, 1).Value = checkListName;
            _worksheet.Cell(2, 1).Value = "№";
            _worksheet.Cell(2, 2).Value = "name";
            _worksheet.Cell(2, 3).Value = "expected result";
            _worksheet.Cell(2, 4).Value = "actual result";
            _worksheet.Cell(2, 5).Value = "status";
            _worksheet.Cell(2, 6).Value = "comments";
            _worksheet.Cell(2, 7).Value = "image";
            _worksheet.Column(1).Width = 5;
            _worksheet.Column(2).Width = 20;
            _worksheet.Column(3).Width = 10;
            _worksheet.Column(6).Width = 40;

        }

        public string SaveExelFile()
        {
            string fullPath = $"{_exelFilePath}\\{_exelFileName}";
            _workbook.SaveAs(fullPath); // Save the workbook instead of worksheet
            return fullPath;
        }

        public void AddTestCaseLine(TestCaseData testCase, SpecialAction specialAction, string actualResult, string pathToImage=null)
        {
            if (testCase != null)
            {
                if (_worksheet == null)
                    return;

                bool isSuccess = false;
                string comments = string.Empty;

                var lastRow = _worksheet.LastRowUsed().RowNumber() + 1;
                // Add test case data to the next row
                _worksheet.Cell(lastRow, 1).Value = testCase.id;
                _worksheet.Cell(lastRow, 2).Value = testCase.name;//_worksheet.Cell(lastRow, 2).Value = testCase.description;
                _worksheet.Cell(lastRow, 4).Value = actualResult;

                if (specialAction != null)
                {
                    comments += $"Special action: {specialAction.actionName}; ";

                    if(specialAction.comparedTo != null)
                    {
                        _worksheet.Cell(lastRow, 3).Value = specialAction.comparedTo;

                        if(string.Equals(specialAction.comparedTo ,actualResult) )
                        {
                            _worksheet.Cell(lastRow, 5).Value = "success";
                            _worksheet.Cell(lastRow, 5).Style.Fill.BackgroundColor = XLColor.LightGreen;
                            isSuccess = true;
                        }
                        else
                        {
                            _worksheet.Cell(lastRow, 5).Value = "failed"; 
                            _worksheet.Cell(lastRow, 5).Style.Fill.BackgroundColor = XLColor.LightCoral;
                            isSuccess = false;
                        }
                    }
                    
                    if (!String.IsNullOrEmpty(pathToImage))
                    {
                        if(!isSuccess || specialAction.actionName == SpecialActionsEnum.Photo.ToString())
                        _worksheet.AddPicture(pathToImage)
                            .MoveTo(_worksheet.Cell(lastRow, 7))
                            .Scale(0.5);
                    }
                }

                _worksheet.Cell(lastRow, 6).Value = comments;
            }
        }
    }
}
