namespace Test_Assistant
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try { 
            Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unhandled Exception: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Perform any necessary cleanup here
                // For example, unhooking keyboard and mouse hooks
                // _mouseAndKeyboardProcessor.UnhookAll();
            }
        }
    }
}