using Test_Assistant.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant.Processors
{
    public class FileDataProcessor
    {
        private readonly string debugFile = ".\\debug.json";
        private readonly string orderFilePath = ".\\order.json";
        private string lastTestCasefilePath = ".\\lastTestCaseFile.json";
        public FileDataProcessor()
        {
        }
        public FileData LoadDataFromFile()
        {
            if (File.Exists(orderFilePath))
            {
                try
                {
                    return JsonConvert.DeserializeObject<FileData>(File.ReadAllText(orderFilePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}");
                    return new FileData();
                }
            }
            return new FileData();
        }
        public void SaveDataToFile(FileData data)
        {
            try
            {
                //string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(orderFilePath, JsonConvert.SerializeObject(data, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving data: {ex.Message}");
            }
        }

        public void AddNewActionToLastTestCaseFile(TestCaseAction newAction)
        {
            try
            {
                var lastTestCase = GetLastTestCase();
                if (lastTestCase.actions == null)
                {
                    lastTestCase.actions = new List<TestCaseAction>();
                }
                lastTestCase.actions.Add(newAction);
                File.WriteAllText(lastTestCasefilePath, JsonConvert.SerializeObject(lastTestCase, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving last test case: {ex.Message}");
            }
        }

        public void AddSpecialActionToLastTestCaseFile(SpecialAction specialAction)
        {
            try
            {
                var lastTestCase = GetLastTestCase();
                if (lastTestCase.actions == null)
                    lastTestCase.actions = new List<TestCaseAction>();

                if (!lastTestCase.actions.Any())
                    lastTestCase.actions.Add(new TestCaseAction());

                lastTestCase.actions.Last().specialActionId = specialAction.id;
                File.WriteAllText(lastTestCasefilePath, JsonConvert.SerializeObject(lastTestCase, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving last test case: {ex.Message}");
            }
        }
        public TestCaseData GetLastTestCase()
        {
            if (File.Exists(lastTestCasefilePath))
            {
                try
                {
                    return JsonConvert.DeserializeObject<TestCaseData>(File.ReadAllText(lastTestCasefilePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading data: {ex.Message}");
                    return new TestCaseData();
                }
            }
            return new TestCaseData();
        }

        public void ClearLastTestCaseFile()
        {
            try
            {
                File.WriteAllText(lastTestCasefilePath, JsonConvert.SerializeObject(new TestCaseData(), Formatting.Indented));
                File.WriteAllText(lastTestCasefilePath, JsonConvert.SerializeObject(new TestCaseData(), Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving last test case: {ex.Message}");
            }
        }
    }
}
