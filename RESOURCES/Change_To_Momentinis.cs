using SB_Payments_Change;
using Microsoft.VisualBasic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace SB_Momentiniai_Mokejimai_Script.RESOURCES
{
    public class Change_To_Momentinis : OpenAndCloseDriver
    {
        [TestCase(Description = "Selects each file package then changes payment status")]
        [Category("Status change")]
        public async Task ChangePaymentStatus()
        {
            WebDriverWait wait = new WebDriverWait(OpenAndCloseDriver.driver, TimeSpan.FromSeconds(180));

            await Task.Delay(2000);

            ReadOnlyCollection<IWebElement> packages = OpenAndCloseDriver.driver.FindElements(By.XPath("//div[@class='import-select-row-info']"));

            int tableCount = packages.Count;

            Console.WriteLine("Number of packages found: " + packages.Count);

            for (int tableIndex = 0; tableIndex < tableCount; tableIndex++)
            {
                try
                {
                    packages = OpenAndCloseDriver.driver.FindElements(By.XPath("//div[@class='import-select-row-info']")); // Refreshes package collection
                    IWebElement currentTable = packages[tableIndex];

                    currentTable.Click();

                    await Task.Delay(3000);

                    ReadOnlyCollection<IWebElement> rows = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='payment-select-row-info']")));

                    Console.WriteLine("Total number of rows found in package " + (tableIndex + 1) + ": " + rows.Count);

                    int rowsWith718InCurrentTable = 0;

                    for (int i = 0; i < rows.Count; i++)
                    {
                        try
                        {
                            IWebElement currentRow = rows[i];

                            IWebElement accountElement = currentRow.FindElement(By.XPath(".//div[@class='payment-select-row-info__account']"));
                            string beneficiaryAccountText = accountElement.FindElement(By.TagName("span")).Text;

                            string cleanedText = new string(beneficiaryAccountText.Where(char.IsDigit).ToArray());

                            if (cleanedText.Length >= 5 && cleanedText.Substring(2, 3).Equals("718"))
                            {
                                rowsWith718InCurrentTable++;
                                continue;
                            }

                            currentRow.Click();

                            await Task.Delay(2000);

                            await Task.Delay(1200);

                            WebDriverWait waitInner = new WebDriverWait(OpenAndCloseDriver.driver, TimeSpan.FromSeconds(10));

                            IWebElement mainBlock = driver.FindElement(By.Id("block-id-mainBlock"));

                            string elementText = mainBlock.Text;

                            if (elementText.Contains("Momentinis"))
                            {
                                IWebElement returnButton = OpenAndCloseDriver.driver.FindElement(By.CssSelector("button[aria-label='Grįžti']"));
                                returnButton.Click();

                                rows = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='payment-select-row-info']")));

                                await Task.Delay(1000);

                                continue;
                            }
                            else if (!elementText.Contains("Mokėjimo prioritetas"))
                            {
                                IWebElement returnButton = OpenAndCloseDriver.driver.FindElement(By.CssSelector("button[aria-label='Grįžti']"));
                                ((IJavaScriptExecutor)OpenAndCloseDriver.driver).ExecuteScript("arguments[0].click();", returnButton);

                                rows = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='payment-select-row-info']")));

                                await Task.Delay(1000);

                                continue;
                            }

                            IWebElement changeButton = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Keisti']")));

                            await Task.Delay(2000);

                            changeButton.Click();

                            await Task.Delay(1000);

                            bool containsMomentinisCheckbox = OpenAndCloseDriver.driver.FindElements(By.XPath("//span[text()='Momentinis']")).Count > 0;

                            if (containsMomentinisCheckbox)
                            {
                                try
                                {
                                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                                    IWebElement momentinisCheckbox = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("label[for='priority-value-I']")));

                                    await Task.Delay(500);

                                    momentinisCheckbox.Click();

                                    await Task.Delay(200);

                                    js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

                                    await Task.Delay(3000);

                                    IWebElement saveButton = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Išsaugoti']")));

                                    await Task.Delay(1000);

                                    saveButton.Click();

                                    await Task.Delay(3000);

                                    IWebElement returnButton = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Grįžti']")));

                                    await Task.Delay(1000);

                                    returnButton.Click();

                                    await Task.Delay(500);

                                    rows = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='payment-select-row-info']")));
                                }
                                catch (NoSuchElementException)
                                {
                                    await Task.Delay(500);

                                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                    js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

                                    await Task.Delay(2000);

                                    IWebElement saveButton = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Išsaugoti']")));

                                    saveButton.Click();

                                    await Task.Delay(3000);

                                    rows = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='payment-select-row-info']")));
                                }
                            }
                            else
                            {
                                try
                                {
                                    await Task.Delay(3000);

                                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                    js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

                                    IWebElement saveButton = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Išsaugoti']")));

                                    await Task.Delay(1000);

                                    saveButton.Click();

                                    await Task.Delay(3000);

                                    IWebElement returnButton = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Grįžti']")));

                                    await Task.Delay(1000);

                                    returnButton.Click();

                                    await Task.Delay(1000);

                                    rows = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='payment-select-row-info']")));
                                }
                                catch (NoSuchElementException)
                                {
                                    await Task.Delay(300);

                                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                                    js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

                                    IWebElement saveButton = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Išsaugoti']")));

                                    await Task.Delay(1000);

                                    saveButton.Click();

                                    await Task.Delay(3000);

                                    IWebElement returnButton = waitInner.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button[aria-label='Grįžti']")));

                                    await Task.Delay(1000);

                                    returnButton.Click();

                                    await Task.Delay(1000);

                                    rows = wait.Until(driver => driver.FindElements(By.XPath("//div[@class='payment-select-row-info']")));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception occurred in row actions: " + ex.Message);
                            Console.WriteLine($"Exception occurred: {ex.Message}");
                            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                        }
                }
                    Console.WriteLine("Number of rows with '718' in package " + (tableIndex + 1) + ": " + rowsWith718InCurrentTable);

                    int initialRowCount = rows.Count;

                    if (rows.Count == 0 || rows.Count < initialRowCount)
                    {
                        Console.WriteLine("All rows in the table have been processed. Navigating back to select another table.");

                        await Task.Delay(2000);
                        continue; // Proceeds to the next table
                    }

                    // Navigate back to select the next table
                    OpenAndCloseDriver.driver.Navigate().Back();

                    await Task.Delay(5000); //Waiting for all the elements to load fully

                    packages = OpenAndCloseDriver.driver.FindElements(By.XPath("//div[@class='import-select-row-info']"));
                    if (packages.Count == 0)
                    {
                        Console.WriteLine("No more tables to process. Test finished successfully.");
                        OpenAndCloseDriver.driver.Quit();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occurred in table iteration: " + ex.Message);
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                }
            }
        }
    }
}