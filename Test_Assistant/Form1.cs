using Test_Assistant.ImageProcessorModels;
using Test_Assistant.Models;
using Test_Assistant.pages;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;
using Test_Assistant.ImageProcessorModels;
using Test_Assistant.Models;
using Test_Assistant.pages;
using Test_Assistant;
using static System.Windows.Forms.LinkLabel;

namespace Test_Assistant
{
    public partial class Form1 : Form
    {
        /// <HOOK_IMPORTS>

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        ///</HOOK_IMPORTS>

        /// <FILE_PATCHES>

        private static string debugFilePath = ".\\..\\..\\..\\..\\MouseClicks.txt";// Debug file location
        private static string orderFilePath = ".\\..\\..\\..\\..\\order.json";
        private static string screenshotsFilePath = ".\\..\\..\\..\\..\\..\\screenshots";


        //private static string debugFilePath = ".\\MouseClicks.txt";// Debug file location
        //private static string orderFilePath = ".\\order.txt";
        //private static string screenshotsFilePath = ".\\screenshots";

        /// </FILE_PATCHES>

        /// <KEYBOARD_EVENT_VARIABLES>

        private static IntPtr _keyboardHookID = IntPtr.Zero;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_RETURN = 0x0D; // Virtual key code for Enter

        /// </KEYBOARD_EVENT_VARIABLES>

        /// <MOUSE_EVENT_VARIABLES>

        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const uint InputMouse = 0;
        private const uint MouseEventLeftDown = 0x0002;
        private const uint MouseEventLeftUp = 0x0004;
        private const uint MouseEventMove = 0x0001;
        private const uint MouseEventAbsolute = 0x8000;

        /// </MOUSE_EVENT_VARIABLES>

        /// <MOUSE_AND_KEYBOARD_HOOK_VARIABLES>

        private static LowLevelMouseProc _mouseProc = MouseHookCallback;
        private static IntPtr _mouseHookID = IntPtr.Zero;
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        /// </MOUSE_AND_KEYBOARD_HOOK_VARIABLES>

        /// <LOGISTIC_VARIABLES> 

        private Panel ActionsPanel;
        private Panel CasesPanel;
        private Panel ListPanel;
        private Button DismissButton;
        private Button SaveButton;
        private static Form1 _instanceForm1;
        private FileDataProcessor fileDataProcessor = new FileDataProcessor(orderFilePath);
        public static System.Windows.Forms.Timer _globalTimer;
        private POINT[] points = new POINT[4];
        private static bool isRecordingClicks = false;
        private static int elapsedPartialSeconds = 0;
        private static FileData prevFileData;
        private static FileData fileData;
        private static ParseImageProcessor _parseImageProcessor;
        private static ScreenshotProcessor _screenshotProcessor;
        private CasesPage casesPage;

        /// </LOGISTIC_VARIABLES>

        /// <HOOK_LOGISTIC>
        private static IntPtr SetMouseHook(LowLevelMouseProc proc)
        {
            using (var currentProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var currentModule = currentProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
            }
        }
        private static IntPtr SetKeyboardHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        private static IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (isRecordingClicks)
            {
                if (nCode >= 0 && (wParam == (IntPtr)WM_LBUTTONDOWN || wParam == (IntPtr)WM_RBUTTONDOWN))
                {
                    MSLLHOOKSTRUCT mouseStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                    if (mouseStruct.mouseData != null)
                    {
                        File.AppendAllText(debugFilePath, $"X: {mouseStruct.pt.x}, Y: {mouseStruct.pt.y}, Button: {(wParam == (IntPtr)WM_LBUTTONDOWN ? "Left" : "Right")}" + Environment.NewLine);
                        File.AppendAllText(orderFilePath, $"{mouseStruct.pt.x},{mouseStruct.pt.y},{elapsedPartialSeconds}\n");
                        elapsedPartialSeconds = 0;
                    }
                }
            }
            return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
        }
        private static IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (isRecordingClicks)
            {
                if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
                {
                    if (Marshal.ReadInt32(lParam) == VK_RETURN) // Enter key
                    {
                        isRecordingClicks = false;
                        _instanceForm1.WindowState = FormWindowState.Normal;
                        _globalTimer.Enabled = false;
                        elapsedPartialSeconds = 0;
                    }
                }
            }
            return CallNextHookEx(_keyboardHookID, nCode, wParam, lParam);

        }

        /// </HOOK_LOGISTIC>

        /// <WINFORM_LOGISTIC>
        public Form1()
        {

            _instanceForm1 = this;
            _globalTimer = new System.Windows.Forms.Timer();
            fileData = fileDataProcessor.LoadDataFromFile();
            prevFileData = fileDataProcessor.LoadDataFromFile();
            _parseImageProcessor = new ParseImageProcessor();
            _screenshotProcessor = new ScreenshotProcessor();

            CreateActionsPanel();
            CreateChecklistPanel();
            CreateCasesPanel();
            CreateLayoutElements();

            InitializeComponent();

            File.WriteAllText(debugFilePath, "Mouse Click Positions:\n");

            _mouseHookID = SetMouseHook(_mouseProc);
            _keyboardHookID = SetKeyboardHook(KeyboardHookCallback);



            ActionsPanel.Visible = false;
            ListPanel.Visible = false;
            CasesPanel.Visible = true;


            //fileDataProcessor.SaveDataToFile(new FileData { OrderLists = new List<OrderList> { new OrderList { id = 1, name = "da", caseIds = new List<int> { 1, 3 } } }, Testcases = new List<TestcaseData> { new TestcaseData { id = 1, name = "DA", actions = new List<TestCaseAction> { new TestCaseAction { x = 1, y = 2, t = 3 }, new TestCaseAction { x = 100, y = 200, t = 300 }, new TestCaseAction { x = 10, y = 20, t = 30 } } } } });
        }

        private void CreateActionsPanel()
        {
            ActionsPanel = new Panel(); ActionsPanel.Size = new Size(800, 395); ActionsPanel.Location = new Point(0, 30);
            Controls.Add(ActionsPanel);
        }

        private void CreateCasesPanel()
        {
            CasesPanel = new Panel(); CasesPanel.Size = new Size(800, 395); CasesPanel.Location = new Point(0, 30);
            Controls.Add(CasesPanel);

            if (fileData.Testcases == null)
            {
                return;
            }

            // Initialize the FlowLayoutPanel
            casesPage = new CasesPage(fileData);


            CasesPanel.Controls.Add(casesPage);
        }

        private void CreateChecklistPanel()
        {
            ListPanel = new Panel(); ListPanel.Size = new Size(800, 395); ListPanel.Location = new Point(0, 30);
            Controls.Add(ListPanel);

            if (fileData.Testcases == null)
            {
                return;
            }

            // Initialize the FlowLayoutPanel
            ListPage listPage = new ListPage(fileData);

            ListPanel.Controls.Add(listPage);
        }

        private void CreateLayoutElements()
        {
            DismissButton = new Button(); DismissButton.Size = new Size(183, 52); DismissButton.Location = new Point(19, 429); DismissButton.Text = "Dismiss"; DismissButton.Click += DismissButton_Click;
            SaveButton = new Button(); SaveButton.Size = new Size(183, 52); SaveButton.Location = new Point(588, 429); SaveButton.Text = "Save"; DismissButton.Click += DismissButton_Click;
            Controls.Add(DismissButton);
            Controls.Add(SaveButton);
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookWindowsHookEx(_keyboardHookID);
            UnhookWindowsHookEx(_mouseHookID);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            points[0] = new POINT { x = 0, y = 0 };
            points[1] = new POINT { x = 0, y = 500 };
            points[2] = new POINT { x = 500, y = 500 };
            points[3] = new POINT { x = 500, y = 0 };
        }
        private void buttonStartRecording_Click(object sender, EventArgs e)
        {
            isRecordingClicks = true;
            _instanceForm1.WindowState = FormWindowState.Minimized;
            StartPartialTimer();
            File.WriteAllText(orderFilePath, "");
        }

        private void buttonStartOrder_Click(object sender, EventArgs e)
        {
            _instanceForm1.WindowState = FormWindowState.Minimized;


            if (fileData.Testcases != null)
            {
                PerformOrderClicksAsync(fileData.Testcases[0]);
            }
        }



        // Блокуємо натискання Enter
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                return true; // Перехоплюємо і запобігаємо обробці
            }
            return base.ProcessCmdKey(ref msg, keyData); // Пропускаємо обробку
        }

        private void StartPartialTimer()
        {
            _globalTimer.Enabled = true;
            _globalTimer.Interval = 1000;
            _globalTimer.Tick += (sender, e) => { elapsedPartialSeconds++; };// Calculate elapsed seconds beetwen mouse clicks
        }



        private void SaveButton_Click(object sender, EventArgs e)
        {
            casesPage.SaveAllToLocalData();

            fileDataProcessor.SaveDataToFile(fileData);
        }

        private void DismissButton_Click(object sender, EventArgs e)
        {
            fileDataProcessor.SaveDataToFile(prevFileData);
            DismissButton.Enabled = false;
        }

        /// </WINFORM_LOGISTIC>

        /// <summary>
        /// 


        public async void PerformOrderClicksAsync(TestcaseData testcase)
        {
            if (testcase == null)
            {
                MessageBox.Show("Invalid data");
                return;
            }
            if (_screenshotProcessor == null)
            {
                _screenshotProcessor = new ScreenshotProcessor();
            }
            var specialActions = fileData.SpecialActions.Where(p => p.testCaseId == testcase.id).ToList();
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
                                _screenshotProcessor.TakeScreenshot(0, 0, 200, 100);
                                break;
                            case "Parse":
                                var tempPath = _screenshotProcessor.TakeScreenshot(0, 0, 200, 100);
                                _parseImageProcessor.ParseImage(tempPath);
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

        private void CasesPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void testBtn_Click(object sender, EventArgs e)
        {
            var bitmap = _screenshotProcessor.TakeScreenshot(0, 0, 200, 100);
            string text = _parseImageProcessor.ParseImage(bitmap);
            MessageBox.Show(text);

        }

        private void actionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActionsPanel.Visible = true;
            ListPanel.Visible = false;
            CasesPanel.Visible = false;
        }

        private void orderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActionsPanel.Visible = false;
            ListPanel.Visible = true;
            CasesPanel.Visible = false;
        }

        private void casesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActionsPanel.Visible = false;
            ListPanel.Visible = false;
            CasesPanel.Visible = true;
        }
    }
}
