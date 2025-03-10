using Test_Assistant.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Assistant
{
    public class FileDataProcessor
    {
        private readonly string orderFilePath = "order.json";
        public FileDataProcessor(string filePath)
        {
            this.orderFilePath = filePath;

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
    }
}
