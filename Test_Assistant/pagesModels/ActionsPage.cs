using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Test_Assistant.Models;

namespace Test_Assistant.pagesModels
{
    public class ActionsPage : FlowLayoutPanel
    {
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        /// <MOUSE_EVENT_VARIABLES>

        private const uint InputMouse = 0;
        private const uint MouseEventLeftDown = 0x0002;
        private const uint MouseEventLeftUp = 0x0004;
        private const uint MouseEventMove = 0x0001;
        private const uint MouseEventAbsolute = 0x8000;

        /// </MOUSE_EVENT_VARIABLES>
        /// 
        private Form1 _instanceForm1;
        private FileData _fileData;
        private static ImageProcessor _ImageProcessor;
        public ActionsPage(Form1 instanceForm1, FileData fileData)
        {
            _instanceForm1 = instanceForm1;
            _fileData = fileData;
            _ImageProcessor = new ImageProcessor();

            Width = 550;
            FlowDirection = FlowDirection.LeftToRight;
            WrapContents = true; // Дозволяємо перенесення елементів на наступний рядок
            AllowDrop = true;
            AutoScroll = true; // Додаємо прокрутку, якщо елементів багато
            Padding = new Padding(20);


            var StartRecordButton = new Button(); StartRecordButton.Size = new Size(183, 52); StartRecordButton.Location = new Point(19, 229); StartRecordButton.Text = "Record";// StartRecordButton.Click += buttonStartRecording_Click;
            var StartOrderButton = new Button(); StartOrderButton.Size = new Size(183, 52); StartOrderButton.Location = new Point(588, 229); StartOrderButton.Text = "Order"; StartOrderButton.Click += buttonStartOrder_Click;
            Controls.Add(StartRecordButton);
            Controls.Add(StartOrderButton);
        }
        private void buttonStartOrder_Click(object sender, EventArgs e)
        {
            _instanceForm1.WindowState = FormWindowState.Minimized;


            if (_fileData.Testcases != null)
            {
                PerformOrderClicksAsync(_fileData.Testcases[0]);
            }
        }
        public async void PerformOrderClicksAsync(TestcaseData testcase)
        {
            if (testcase == null)
            {
                MessageBox.Show("Invalid data");
                return;
            }
            var specialActions = _fileData.SpecialActions.Where(p => p.testCaseId == testcase.id).ToList();
            List<int> specialActionsIds = new List<int>();
            if (specialActions != null)
                specialActionsIds = specialActions.Select(p => p.testCaseActionId).ToList();

            for (int i = 0; i < testcase.actions.Count; i++)
            {
                MouseClickAt(testcase.actions[i].x, testcase.actions[i].y);
                await Task.Delay(testcase.actions[i].t * 1000);

                if (specialActionsIds.Contains(testcase.id))
                {
                    var specialAction = specialActions.FirstOrDefault(p => p.testCaseActionId == testcase.id);
                    if (specialAction != null)
                    {
                        switch (specialAction.actionName)
                        {
                            case "Photo":
                                _ImageProcessor.TakeScreenshot(specialAction.xAreaStart, specialAction.yAreaStart, specialAction.xAreaEnd, specialAction.yAreaEnd);
                                break;
                            case "Parse":
                                var tempPath = _ImageProcessor.TakeScreenshot(specialAction.xAreaStart, specialAction.yAreaStart, specialAction.xAreaEnd, specialAction.yAreaEnd);
                                _ImageProcessor.ParseImage(tempPath);
                                break;
                            default: break;
                        }
                    }
                }
            }

            //TakeScreenshot();
            _instanceForm1.WindowState = FormWindowState.Normal;
        }
        static void MouseClickAt(int x, int y)
        {
            var inputs = new Input[3];

            // Move the mouse to the specified coordinates
            inputs[0] = new Input
            {
                Type = InputMouse,
                Data = new MouseInput
                {
                    X = x * 65535 / GetSystemMetrics(0), // Scale X to absolute coordinates
                    Y = y * 65535 / GetSystemMetrics(1), // Scale Y to absolute coordinates
                    Flags = MouseEventMove | MouseEventAbsolute
                }
            };

            // Mouse down
            inputs[1] = new Input
            {
                Type = InputMouse,
                Data = new MouseInput
                {
                    Flags = MouseEventLeftDown
                }
            };

            // Mouse up
            inputs[2] = new Input
            {
                Type = InputMouse,
                Data = new MouseInput
                {
                    Flags = MouseEventLeftUp
                }
            };

            // Send the inputs
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
    }
}
