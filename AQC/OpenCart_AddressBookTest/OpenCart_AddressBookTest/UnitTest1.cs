﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace OpenCart_AddressBookTest
{
    [TestFixture]
    public class UnitTest1
    {
        //const string TEST_SITE_URL = "http://atqc-shop.epizy.com";
        const string TEST_SITE_URL = "http://localhost";
        const string PAGE_NAME = "Account";
        const string EMAIL = "zinko@mail.com";
        const string PASSWORD = "querty";
        const string LOGIN_PAGE_NAME = "Login";
        const string ACCOUNT_PAGE_NAME = "Account";
        const string ADDRESS_BOOK_PAGE_NAME = "Address Book";
        const string ADD_ADDRESS_PAGE_NAME = "Add Address";
        const string EDIT_ADDRESS_PAGE_NAME = "Edit Address";
        public string newAddress;
        
        IWebDriver driver;
        

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl(TEST_SITE_URL);

            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            

            //LogIn to the site
            //My Acount link
            driver.FindElement(By.CssSelector("#top-links a.dropdown-toggle")).Click();
            //Login link
            driver.FindElement(By.CssSelector("#top-links a[href$='/login']")).Click();
            
            //Verify that opened Account page
            Assert.AreEqual(breadcrumbLastPage(driver).Text, LOGIN_PAGE_NAME, 
                        "\"Login page\" hasn't been opened!");

            
            driver.FindElement(By.Id("input-email")).Clear();
            driver.FindElement(By.Id("input-email")).SendKeys(EMAIL);//Type Login
            driver.FindElement(By.Id("input-password")).Clear();
            driver.FindElement(By.Id("input-password")).SendKeys(PASSWORD);//Type Password
            driver.FindElement(By.CssSelector("form input[type ='submit']")).Click();//Login button

            //Verify that opened Account page
            Assert.AreEqual(breadcrumbLastPage(driver).Text, ACCOUNT_PAGE_NAME,
                       "\"Account page\" hasn't been opened!");            
        }        

        [OneTimeTearDown]
        public void TearDown()
        {            
            //driver.FindElement(By.CssSelector("a[href$='/logout']"));
            //driver.Quit();
        }

        [TestCase("Roman", "Zinko", "Grinchenko, 7", "Lviv", "Ukraine", "L'vivs'ka Oblast'",
                    "Roman Zinko\r\nGrinchenko, 7\r\nLviv\r\nL\'vivs\'ka Oblast\'\r\nUkraine")]
        public void Test1CreateNewAddressTest(string firstName, string lastName, string address1,
                                            string city, string country, string region, string expected)
        {
            //Arrange              
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            //Click on "Address Book" link
            driver.FindElement(By.CssSelector("#column-right a[href$='/address']")).Click();

            //Verify that "Address Book" page is opened 
            Assert.AreEqual(breadcrumbLastPage(driver).Text, ADDRESS_BOOK_PAGE_NAME,
                       "\"Address Book page\" hasn't been opened!");

            //New Address button
            driver.FindElement(By.CssSelector("#content .buttons .pull-right>a")).Click();

            //Verify that "New Address" page is opened
            Assert.AreEqual(breadcrumbLastPage(driver).Text, ADD_ADDRESS_PAGE_NAME,
                       "\"New Address page\" hasn't been opened!");

            //Fill all required fields with data
            FillField(driver.FindElement(By.Id("input-firstname")), firstName);
            FillField(driver.FindElement(By.Id("input-lastname")), lastName);
            FillField(driver.FindElement(By.Id("input-address-1")), address1);
            FillField(driver.FindElement(By.Id("input-city")), city);
           
            //Select Country
            new SelectElement(driver.FindElement(By.Id("input-country"))).SelectByText(country);
            
            //Loading regions
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(
                By.CssSelector(".fa.fa-circle-o-notch.fa-spin")));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //Select Region
            new SelectElement(driver.FindElement(By.Id("input-zone"))).SelectByText(region);

            //Click on "Continue" button
            driver.FindElement(By.CssSelector("form input[type ='submit']")).Click();

            //Verify that Address Book page is opened 
            Assert.AreEqual(breadcrumbLastPage(driver).Text, ADDRESS_BOOK_PAGE_NAME,
                       "\"Address Book page\" hasn't been opened!");

            //Act
            //Get text from Address table
            IWebElement productTable = driver.FindElement(
                    By.CssSelector(".table.table-bordered.table-hover"));

            string actual = productTable.FindElement(
                    By.XPath("//tbody/tr[last()]/td[contains(@class, 'text-left')]")).Text;
            
            //Save new Address
            newAddress = expected;
            
            //Assert
            Assert.AreEqual(expected, actual, "TEST CASE FAILED");//Verify that correct text displayed in the table
        }

        [TestCase("Khreshchatyk, 1", "Kyiv", "Kyiv",
                "Roman Zinko\r\nKhreshchatyk, 1\r\nKyiv\r\nKyiv\r\nUkraine")]
        public void Test2EditAddressTest(string address, string city, string region, string expected)
        {
            //Arrange 
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
            //Verify that Address Book page is opened 
            Assert.AreEqual(breadcrumbLastPage(driver).Text, ADDRESS_BOOK_PAGE_NAME,
                       "\"Address Book page\" hasn't been opened!");

            //Find address table
            IWebElement addressTable = driver.FindElement(
                    By.CssSelector(".table.table-bordered.table-hover"));
            //Get address table lenght            
            int addressTableLenght = addressTable.FindElements(By.XPath("//tbody/tr")).Count;
            int i = 1;
            int newAddressPossition = 0;
            string temp;
            do
            {
                temp = addressTable.FindElement(By.XPath($"//tbody/tr[{i}]/td[contains(@class, 'text-left')]")).Text;
                if (temp.Equals(newAddress))
                {
                   newAddressPossition = i;
                } 
                i++;                
            }
            while (newAddressPossition < 1 && i < addressTableLenght);

            IWebElement editAddressInTable = addressTable.FindElement(By.XPath($"//tbody/tr[{newAddressPossition}]"));

            //"Edit Address" button in first table row
            editAddressInTable.FindElement(By.LinkText("Edit")).Click();

            //Verify that "Edit Address" page is opened            
            Assert.AreEqual(breadcrumbLastPage(driver).Text, EDIT_ADDRESS_PAGE_NAME,
                      "\"Address Book page\" hasn't been opened!");

            //Set new data
            FillField(driver.FindElement(By.Id("input-address-1")),address);
            FillField(driver.FindElement(By.Id("input-city")), city);

            //Loading regions
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            //Wait, while loading indicator is visible
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(
                            By.CssSelector(".fa.fa-circle-o-notch.fa-spin")));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //Select Region
            SelectElement selectRegion = new SelectElement(driver.FindElement(By.Id("input-zone")));
            selectRegion.SelectByText(region);            

            //Click on "Continue" button
            driver.FindElement(By.CssSelector("form input[type ='submit']")).Click();

            //Verify that Address Book page is opened 
            Assert.AreEqual(breadcrumbLastPage(driver).Text, ADDRESS_BOOK_PAGE_NAME,
                       "\"Address Book page\" hasn't been opened!");

            //Act
            //Get text from Address table            
            string actual = editAddressInTable.FindElement(By.ClassName("text-left")).Text;

            //Assert
            Assert.AreEqual(expected, actual, "TEST 2 FAILED!");//Verify that new text displayed in the table
        }

        //[TestCase("Roman", "Zinko", "Grinchenko, 7", "Lviv", "220"/*Ukraine*/, "3493"/*L'vivs'ka Oblast'*/)]
        public void Test3DeleteLastAddressTest(string firstName, string lastName, string address1,
                                            string city, string country, string region)
        {
            /***///CreateNewAddressTest(firstName, lastName, address1, city, country, region, "ffff");

            //Arrange   

            //Click on "Address Book" link
            driver.FindElement(By.CssSelector("#column-right a[href$='/address']")).Click();
            //Verify that "Address Book" page is opened 
            driver.FindElement(By.CssSelector(".breadcrumb a[href$='/address']"));

            //Add new Address
            //Save primary "Address Book" table length
            int expected = driver.FindElements(By.CssSelector("#content tbody")).Count;

            driver.FindElement(By.CssSelector("#content .buttons .pull-right>a")).Click();
            //Verify that "New Address" page is opened            
            driver.FindElement(By.CssSelector(".breadcrumb a[href*='/address/add']"));
            //Fill all required fields with data
            driver.FindElement(By.Id("input-firstname")).SendKeys(firstName);
            driver.FindElement(By.Id("input-lastname")).SendKeys(lastName);
            driver.FindElement(By.Id("input-address-1")).SendKeys(address1);
            driver.FindElement(By.Id("input-city")).SendKeys(city);
            //Select Country
            new SelectElement(driver.FindElement(By.Id("input-country"))).SelectByValue(country);
            //Loading regions
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(
                By.CssSelector(".fa.fa-circle-o-notch.fa-spin")));
            //Select Region
            new SelectElement(driver.FindElement(By.Id("input-zone"))).SelectByValue(region);
            //Click on "Continue" button
            driver.FindElement(By.CssSelector("form input[type ='submit']")).Click();
            //Verify that Address Book page is opened 
            driver.FindElement(By.CssSelector(".breadcrumb a[href$='/address']"));

            int tbodyRowsEnd = driver.FindElements(By.CssSelector("#content tbody")).Count;

            if (tbodyRowsEnd > expected)
            {
                driver.FindElement(By.XPath("//tbody/tr[last()]]")).FindElement(By.ClassName("#btn-danger")).Click();
            }
            else
            {
                throw new Exception();
            }

            //Act
            //Save "Address Book" table length, after deleting added address
            int actual = driver.FindElements(By.CssSelector("#content tbody")).Count;

            //Assert
            Assert.AreEqual(expected, actual);//Verify that the address deleted from the table

        }

        public static IWebElement breadcrumbLastPage(IWebDriver driver)
        {
            return driver.FindElement(By.CssSelector("ul.breadcrumb :last-of-type a"));
        }
        
        public static void FillField(IWebElement element, string value)
        {
            element.Click();
            element.Clear();
            element.SendKeys(value);
        }
    }
}
