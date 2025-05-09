using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SB_Payments_Change
{
    public class OpenAndCloseDriver
    {
        public static IWebDriver driver { get; private set; }

        public Stopwatch _stopWatch;

        [SetUp]
        public void StartBrowser()
        {
            PreventSleep();

            if (driver == null)
            {
                try
                {
                    new WebDriverManager.DriverManager().SetUpDriver(new WebDriverManager.DriverConfigs.Impl.ChromeConfig());

                    ChromeOptions option = new ChromeOptions();
                    option.DebuggerAddress = "127.0.0.1:9222";
                    option.AddArgument("--user-data-dir=C:\\ChromeDebug");

                    var service = ChromeDriverService.CreateDefaultService();
                    service.LogPath = "chromedriver.log";
                    service.EnableVerboseLogging = true;

                    driver = new ChromeDriver(service, option, TimeSpan.FromSeconds(120));

                    driver.Manage().Window.Maximize();
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

                    Console.WriteLine("Current URL: " + driver.Url);
                    Console.WriteLine("Browser dimensions: " + driver.Manage().Window.Size);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start browser: {ex.Message}");
                    throw;
                }
            }
        }
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint SetThreadExecutionState(uint esFlags);

        // Method to prevent system sleep during tests
        private void PreventSleep()
        {
            const uint ES_CONTINUOUS = 0x80000000;
            const uint ES_SYSTEM_REQUIRED = 0x00000001;

            // Set ES_SYSTEM_REQUIRED to prevent system sleep
            SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);
        }
        [TearDown]
        public void CloseBrowser()
        {
            int remainingInstances = Process.GetProcessesByName("chromedriver.exe").Length;

            Console.WriteLine("Remaining Chrome instances: " + remainingInstances);

            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }
        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver.Dispose();
                driver = null;
            }

            ProcessStartInfo taskInfo = new ProcessStartInfo();
            taskInfo.FileName = "cmd.exe";
            taskInfo.Arguments = "/C taskkill /F /IM chromedriver.exe /T";
            taskInfo.CreateNoWindow = true;
            taskInfo.UseShellExecute = false;

            using (Process taskkillProcess = new Process())
            {
                taskkillProcess.StartInfo = taskInfo;
                taskkillProcess.Start();
                taskkillProcess.WaitForExit();
            }
        }
    }
}

