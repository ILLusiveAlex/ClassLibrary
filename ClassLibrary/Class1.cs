using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace EpamAutomationTests
{
    [TestClass]
    public class EpamTests
    {
        private IWebDriver? driver;
        private WebDriverWait? wait;

        [TestInitialize]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TestMethod]
        [DataRow("Java")]
        [DataRow("C#")]
        public void ValidateJobSearch(string keyword)
        {
            driver.Navigate().GoToUrl("https://www.epam.com/");

            wait.Until(d => d.FindElement(By.LinkText("Careers"))).Click();

            var keywordField = wait.Until(driver => driver.FindElement(By.Id("new_form_job_search-keyword")));
            keywordField.Clear();
            keywordField.SendKeys(keyword);

            driver.FindElement(By.CssSelector("div.recruiting-search__location")).Click();
            driver.FindElement(By.XPath("//li[contains(text(), 'All Locations')]")).Click();

            driver.FindElement(By.CssSelector("label.recruiting-search__filter-label-23")).Click();

            driver.FindElement(By.ClassName("job-search-button-transparent-23"))?.Click();

            var latestJob = wait.Until(d => d.FindElements(By.ClassName("search-result__item"))).Last();
            latestJob.FindElement(By.CssSelector("div.search-result__item-controls a.search-result__item-apply-23")).Click();

            Assert.IsTrue(driver.PageSource.Contains(keyword), "The job description does not contain the expected keyword.");
        }

        [TestMethod]
        [DataRow("BLOCKCHAIN")]
        [DataRow("Cloud")]
        [DataRow("Automation")]
        public void ValidateGlobalSearch(string searchTerm)
        {
            driver.Navigate().GoToUrl("https://www.epam.com/");

            driver.FindElement(By.ClassName("header-search__button"))?.Click();

            var searchInput = driver.FindElement(By.TagName("input"));
            searchInput.Clear();
            searchInput.SendKeys(searchTerm);

            driver.FindElement(By.CssSelector("span.bth-text-layer"))?.Click();

            var results = wait.Until(d => d.FindElements(By.XPath("//li[@class='search-results__item']/a")))
                            .Select(e => e.Text.ToLower())
                            .ToList();

            Assert.IsTrue(results.All(text => text.Contains(searchTerm.ToLower())), "Not all results contain the expected term.");
        }

        [TestCleanup]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}