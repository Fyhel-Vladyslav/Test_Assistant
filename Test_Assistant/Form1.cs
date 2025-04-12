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
using Test_Assistant.Enums;

namespace Test_Assistant
{
    public partial class Form1 : Form
    {
        

        /// <FILE_PATCHES>

        private static string debugFilePath = ".\\MouseClicks.txt";// Debug file location

        private static string orderFilePath = ".\\order.json";
        private static string screenshotsFilePath = ".\\screenshots";

        /// </FILE_PATCHES>

        /// <LOGISTIC_VARIABLES> 

        private Panel ActionsPanel;
        private Panel CasesPanel;
        private Panel ListPanel;
        private Button DismissButton;
        private Button SaveButton;
        private static Form1 _instanceForm1;
        private FileDataProcessor fileDataProcessor = new FileDataProcessor();
        private static FileData prevFileData;
        public static FileData fileData;
        private static ImageProcessor _imageProcessor;
        private CasesPage casesPage;
        private MouseAndKeyboardProcessor _mouseAndKeyboardProcessor;

        /// </LOGISTIC_VARIABLES>

        /// <WINFORM_LOGISTIC>
        public Form1()
        {

            _instanceForm1 = this;
            
            fileData = fileDataProcessor.LoadDataFromFile();
            prevFileData = fileDataProcessor.LoadDataFromFile();
            _imageProcessor = new ImageProcessor();


            InitializeComponent();

            Width = (int)WindowParamethers.TotalWidth+30;
            Height = (int)WindowParamethers.TotalHeight+ (int)WindowParamethers.BottomLayoutHeight;


            this.FormClosing += Form1_FormClosing;

            this.Load += Form1_Load;

            _mouseAndKeyboardProcessor = new MouseAndKeyboardProcessor(this, fileData);

            CreateActionsPanel();
            CreateChecklistPanel();
            CreateCasesPanel();

            CreateLayoutElements();
            ActionsPanel.Visible = false; //// TEST TEST TEST
            ListPanel.Visible = false;
            CasesPanel.Visible = true;

            //Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            //fileDataProcessor.SaveDataToFile(new FileData { OrderLists = new List<OrderList> { new OrderList { id = 1, name = "da", caseIds = new List<int> { 1, 3 } } }, Testcases = new List<TestcaseData> { new TestcaseData { id = 1, name = "DA", actions = new List<TestCaseAction> { new TestCaseAction { x = 1, y = 2, t = 3 }, new TestCaseAction { x = 100, y = 200, t = 300 }, new TestCaseAction { x = 10, y = 20, t = 30 } } } } });
        }

        //private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        //{
        //    _mouseAndKeyboardProcessor.UnhookAll();

        //    // Handle UI thread exceptions here
        //    MessageBox.Show($"Unhandled UI Exception: {e.Exception.Message}, hooks were unhoocked", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    // You can also log the exception, cleanup resources, etc.
        //}

        //private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    _mouseAndKeyboardProcessor.UnhookAll();
        //    // Handle non-UI thread exceptions here
        //    Exception ex = (Exception)e.ExceptionObject;
        //    MessageBox.Show($"Unhandled Non-UI Exception: {ex.Message}, hooks were unhoocked", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    // You can also log the exception, cleanup resources, etc.
        //}

        private void CreateActionsPanel()
        {
            ActionsPanel = new Panel();
            ActionsPanel.Size = new Size((int)WindowParamethers.TotalWidth, (int)WindowParamethers.TotalHeight);
            ActionsPanel.Location = new Point(0, 30);
            Controls.Add(ActionsPanel); 

            ActionsPanel.Controls.Add(new ActionsPage(_instanceForm1, fileData));
        }

        private void CreateCasesPanel()
        {
            CasesPanel = new Panel();
            CasesPanel.Size = new Size((int)WindowParamethers.TotalWidth, (int)WindowParamethers.TotalHeight);
            CasesPanel.Location = new Point(0, 30);
            Controls.Add(CasesPanel);

            if (fileData.Testcases == null)
            {
                return;
            }

            // Initialize the FlowLayoutPanel
            casesPage = new CasesPage(fileData, _mouseAndKeyboardProcessor);

            CasesPanel.Controls.Add(casesPage);
        }

        private void CreateChecklistPanel()
        {
            ListPanel = new Panel();
            ListPanel.Size = new Size((int)WindowParamethers.TotalWidth, (int)WindowParamethers.TotalHeight);
            ListPanel.Location = new Point(0, 30);
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
            DismissButton = new Button(); DismissButton.Size = new Size(183, 52); DismissButton.Location = new Point(19, (int)WindowParamethers.TotalHeight+30); DismissButton.Text = "Dismiss"; DismissButton.Click += DismissButton_Click;
            SaveButton = new Button(); SaveButton.Size = new Size(183, 52); SaveButton.Location = new Point(588, (int)WindowParamethers.TotalHeight+30); SaveButton.Text = "Save"; SaveButton.Click += SaveButton_Click;
            Controls.Add(DismissButton);
            Controls.Add(SaveButton);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _mouseAndKeyboardProcessor.UnhookAll();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            //var path = _imageProcessor.TakeScreenshot(0, 0, 200, 100);
            //string text = _imageProcessor.ParseImage(path);
            //MessageBox.Show(text);

            _mouseAndKeyboardProcessor.StartRecording();
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
