namespace Client
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
            for(int i = 0; i < 3; i++)
            {
                Form1 form = new Form1();
                form.Show();
            }
            Application.Run();
        }
    }
}