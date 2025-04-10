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
using Test_Assistant.Models;
using Test_Assistant.pages;
using static System.Windows.Forms.LinkLabel;
using Test_Assistant.pagesModels;

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

        ///</HOOK_IMPORTS>

        /// <FILE_PATCHES>

        private static string debugFilePath = ".\\MouseClicks.txt";// Debug file location

        private static string orderFilePath = ".\\order.json";
        private static string screenshotsFilePath = ".\\screenshots";

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
        private static bool isRecordingClicks = false;
        private static int elapsedPartialSeconds = 0;
        private static FileData prevFileData;
        private static FileData fileData;
        private static ImageProcessor _ImageProcessor;
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
            _ImageProcessor = new ImageProcessor();

            CreateActionsPanel();
            CreateChecklistPanel();
            CreateCasesPanel();
            CreateLayoutElements();

            InitializeComponent();

            File.WriteAllText(debugFilePath, "Mouse Click Positions:\n");

            _mouseHookID = SetMouseHook(_mouseProc);
            _keyboardHookID = SetKeyboardHook(KeyboardHookCallback);

            ActionsPanel.Visible = false; //// TEST TEST TEST
            ListPanel.Visible = true;
            CasesPanel.Visible = false;


            //fileDataProcessor.SaveDataToFile(new FileData { OrderLists = new List<OrderList> { new OrderList { id = 1, name = "da", caseIds = new List<int> { 1, 3 } } }, Testcases = new List<TestcaseData> { new TestcaseData { id = 1, name = "DA", actions = new List<TestCaseAction> { new TestCaseAction { x = 1, y = 2, t = 3 }, new TestCaseAction { x = 100, y = 200, t = 300 }, new TestCaseAction { x = 10, y = 20, t = 30 } } } } });
        }

        private void CreateActionsPanel()
        {
            ActionsPanel = new Panel(); ActionsPanel.Size = new Size(800, 395); ActionsPanel.Location = new Point(0, 30);
            Controls.Add(ActionsPanel);

            ActionsPanel.Controls.Add(new ActionsPage(_instanceForm1, fileData));
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
            SaveButton = new Button(); SaveButton.Size = new Size(183, 52); SaveButton.Location = new Point(588, 429); SaveButton.Text = "Save"; SaveButton.Click += SaveButton_Click;
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
        }

        private void buttonStartRecording_Click(object sender, EventArgs e)
        {
            isRecordingClicks = true;
            _instanceForm1.WindowState = FormWindowState.Minimized;
            StartPartialTimer();
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



        

        
        private void testBtn_Click(object sender, EventArgs e) // TODO : Remove this method
        {
            var path = _ImageProcessor.TakeScreenshot(0, 0, 200, 100);
            string text = _ImageProcessor.ParseImage(path);
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
