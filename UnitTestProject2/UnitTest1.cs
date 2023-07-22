using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Windows.Forms;

namespace UnitTestProject2


{
    [TestClass]
    public class UnitTest1
    {
        
        IWebDriver driver;

        [TestInitialize]
        public void Setup()
        {
            // подготовка системы к тестированию
            
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService(@".", "geckodriver.exe");
            driver = new FirefoxDriver(service);
            driver.Navigate().GoToUrl("https://elba.kontur.ru/Outsourcer/SelectOrganization.aspx");

            // данные для входа в систему 
            var email = driver.FindElement(By.Id("Email"));
            var password = driver.FindElement(By.Id("Password"));
            var btnInput = driver.FindElement(By.Id("LoginButton"));

            // вход в систему 
            email.SendKeys("kontur@testers.ru");
            password.SendKeys("111111");
            btnInput.Click();

            // выбор организации
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementExists(By.Id("OrganizationTable_row14_Name")));
            var chooseCompany = driver.FindElement(By.Id("OrganizationTable_row15_Name"));
            chooseCompany.Click();

            // Переход в реквизиты
            wait.Until(ExpectedConditions.ElementExists(By.Id("MainMenu_Requisites_LinkText")));
            var requisites = driver.FindElement(By.Id("MainMenu_Requisites_LinkText"));
            requisites.Click();

            wait.Until(ExpectedConditions.ElementExists(By.Id("MainMenu_Requisites_BankRequisites_LinkText")));
            var bankAccount = driver.FindElement(By.Id("MainMenu_Requisites_BankRequisites_LinkText"));
            bankAccount.Click();

            // открытие формы банковских реквизитов
            System.Threading.Thread.Sleep(1000);
            wait.Until(ExpectedConditions.ElementExists(By.Id("Accounts_AddAccountLink")));
            var btnAddAccount = driver.FindElement(By.Id("Accounts_AddAccountLink"));
            btnAddAccount.Click();

            // переменные банковского счета
            var bankAccountLightboxAccountInput = driver.FindElement(By.Id("BankAccountLightbox_AccountInput"));
            var bankAccountLightboxBankEditorName = driver.FindElement(By.Id("BankAccountLightbox_BankEditor_Name"));
            var bankAccountLightboxAcceptButton = driver.FindElement(By.Id("BankAccountLightbox_AcceptButton"));

            // добавление банковских реквизитов
            bankAccountLightboxAccountInput.SendKeys("40702810400420000530");
            bankAccountLightboxBankEditorName.SendKeys("УРАЛЬСКИЙ БАНК ПАО СБЕРБАНК");
            bankAccountLightboxAcceptButton.Click();

            // переход в меню деньги
            System.Threading.Thread.Sleep(1000);
            var mainMenuPaymentsLinkText = driver.FindElement(By.Id("MainMenu_Payments_LinkText"));
            mainMenuPaymentsLinkText.Click();

        }
        
  
        [TestMethod]
        public void TestMethod1()
        {

            // загрузка выписки
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementExists(By.Id("PaymentUpload_PaymentFileUpload_Input")));
            var uploadFile = driver.FindElement(By.Id("PaymentUpload_PaymentFileUpload_Input"));
            string path = Directory.GetCurrentDirectory()+ "\\excerpts\\1.txt";
            uploadFile.SendKeys(path);
            
            // проверка сообщения об успешной загрузке выписки
            wait.Until(ExpectedConditions.ElementExists(By.Id("ImportMessage")));
            var message = driver.FindElement(By.Id("ImportMessage")).Text;
            Assert.AreEqual("Принята 1 запись, поступлений: 1 200,00 , списаний: 0,00 . Отменить", message);

            // проверка, что добавился нужный контрагент
            var counterparty = driver.FindElement(By.CssSelector("span.t-readable")).Text;
            Assert.AreEqual(@"ООО ""Рабочая лошадка""", counterparty);
          
            // проверка суммы
            var amount = driver.FindElement(By.CssSelector("span.c-currencySymbol_unobtrusive")).Text;
            Assert.AreEqual("1 200,00", amount);
            
            // проверка суммы учтенной в налогах
            var amountTax = driver.FindElement(By.CssSelector("div.c-currencyStaticText_positive")).Text;
            Assert.AreEqual("1 200,00", amountTax);
            
            //проверка даты
            var date = driver.FindElement(By.XPath("//div[2]/div/div/div/div/div[7]/div")).Text;
            Assert.AreEqual("09.02.2011", date);
            
            //проверка номера
            var number = driver.FindElement(By.XPath("//div[7]/span[2]")).Text;
            Assert.AreEqual("№ 342", number);
            
            //проверка назначения платежа
            var purposePayment = driver.FindElement(By.CssSelector("span.paymentsList-table-description-fromUser")).Text;
            Assert.AreEqual("Оплата по счету N 197 от 29 декабря 2010 за монтаж линии ИнтернетСумма 4200-00Без налога (НДС)", purposePayment);

        }

        [TestMethod]
        public void TestMethod2()
        {

            // загрузка выписки
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementExists(By.Id("PaymentUpload_PaymentFileUpload_Input")));
            var uploadFile = driver.FindElement(By.Id("PaymentUpload_PaymentFileUpload_Input"));
            string path = Directory.GetCurrentDirectory() + "\\excerpts\\2.txt";
            uploadFile.SendKeys(path);

            // проверка сообщения об успешной загрузке выписки
            wait.Until(ExpectedConditions.ElementExists(By.Id("ImportMessage")));
            var message = driver.FindElement(By.Id("ImportMessage")).Text;
            Assert.AreEqual("Принята 1 запись, поступлений: 0,00 , списаний: 4 200,00 . Отменить", message);

            // проверка, что добавился нужный контрагент
            var counterparty = driver.FindElement(By.CssSelector("span.t-readable")).Text;
            Assert.AreEqual(@"ООО ""Рабочая лошадка""", counterparty);
        
            // проверка суммы
            var amount = driver.FindElement(By.CssSelector("span.c-currencySymbol_unobtrusive")).Text;
            Assert.AreEqual("4 200,00", amount);
            
            //проверка даты
            var date = driver.FindElement(By.XPath("//div[2]/div/div/div/div/div[7]/div")).Text;
            Assert.AreEqual("09.02.2011", date);
            
            //проверка номера
            var number = driver.FindElement(By.XPath("//div[7]/span[2]")).Text;
            Assert.AreEqual("№ 342", number);
            
            //проверка назначения платежа
            var purposePayment = driver.FindElement(By.CssSelector("span.paymentsList-table-description-fromUser")).Text;
            Assert.AreEqual("Оплата по счету N 197 от 29 декабря 2010 за монтаж линии ИнтернетСумма 4200-00Без налога (НДС)", purposePayment);

        }

        [TestMethod]
        public void TestMethod3()
        {

            // загрузка выписки
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementExists(By.Id("PaymentUpload_PaymentFileUpload_Input")));
            var uploadFile = driver.FindElement(By.Id("PaymentUpload_PaymentFileUpload_Input"));
            string path = Directory.GetCurrentDirectory() + "\\excerpts\\3.txt";
            uploadFile.SendKeys(path);

            // проверка сообщения об успешной загрузке выписки
            wait.Until(ExpectedConditions.ElementExists(By.Id("ImportMessage")));
            var message = driver.FindElement(By.Id("ImportMessage")).Text;
            Assert.AreEqual("Принята 1 запись, поступлений: 1 200,00 , списаний: 0,00 . Отменить", message);

            // проверка, что добавился нужный контрагент
            var counterparty = driver.FindElement(By.CssSelector("span.t-readable")).Text;
            Assert.AreEqual(@"ООО ""Рабочая лошадка""", counterparty);
         
            // проверка суммы
            var amount = driver.FindElement(By.CssSelector("span.c-currencySymbol_unobtrusive")).Text;
            Assert.AreEqual("1 200,00", amount);
            
            // проверка суммы учтенной в налогах
            var amountTax = driver.FindElement(By.CssSelector("div.c-currencyStaticText_positive")).Text;
            Assert.AreEqual("", amountTax);
            
            //проверка даты
            var date = driver.FindElement(By.XPath("//div[2]/div/div/div/div/div[7]/div")).Text;
            Assert.AreEqual("09.02.2012", date);
            
            //проверка номера
            var number = driver.FindElement(By.XPath("//div[7]/span[2]")).Text;
            Assert.AreEqual("№ 342", number);
            
            //проверка назначения платежа
            var purposePayment = driver.FindElement(By.CssSelector("span.paymentsList-table-description-fromUser")).Text;
            Assert.AreEqual("", purposePayment);

        }
        [TestMethod]
        public void TestMethod4()
        {
            // загрузка выписки
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementExists(By.Id("PaymentUpload_PaymentFileUpload_Input")));
            var uploadFile = driver.FindElement(By.Id("PaymentUpload_PaymentFileUpload_Input"));
            string path = Directory.GetCurrentDirectory() + "\\excerpts\\4.txt";
            uploadFile.SendKeys(path);

            // проверка сообщения об успешной загрузке выписки
            wait.Until(ExpectedConditions.ElementExists(By.Id("ImportMessage")));
            var message = driver.FindElement(By.Id("ImportMessage")).Text;
            Assert.AreEqual("Принята 1 запись, поступлений: 0,00 , списаний: 4 200,00 . Отменить", message);

            // проверка, что добавился нужный контрагент
            var counterparty = driver.FindElement(By.CssSelector("span.t-readable")).Text;
            Assert.AreEqual(@"ООО ""Название""", counterparty);
            
            // проверка суммы
            var amount = driver.FindElement(By.CssSelector("span.c-currencySymbol_unobtrusive")).Text;
            Assert.AreEqual("4 200,00", amount);
            
            //проверка даты
            var date = driver.FindElement(By.XPath("//div[2]/div/div/div/div/div[7]/div")).Text;
            Assert.AreEqual("09.02.2011", date);
            
            //проверка номера
            var number = driver.FindElement(By.XPath("//div[7]/span[2]")).Text;
            Assert.AreEqual("№ 342", number);
           
            //проверка назначения платежа
            var purposePayment = driver.FindElement(By.CssSelector("span.paymentsList-table-description-fromUser")).Text;
            Assert.AreEqual("", purposePayment);
        }

        [TestMethod]
        public void TestFormEditing()
        {
            // загрузка выписки
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementExists(By.Id("PaymentUpload_PaymentFileUpload_Input")));
            var uploadFile = driver.FindElement(By.Id("PaymentUpload_PaymentFileUpload_Input"));
            string path = Directory.GetCurrentDirectory() + "\\excerpts\\4.txt";
            uploadFile.SendKeys(path);

            // проверка сообщения об успешной загрузке выписки
            wait.Until(ExpectedConditions.ElementExists(By.Id("ImportMessage")));
            var message = driver.FindElement(By.Id("ImportMessage")).Text;
            Assert.AreEqual("Принята 1 запись, поступлений: 0,00 , списаний: 4 200,00 . Отменить", message);

            //проверка редактирования 
            driver.FindElement(By.CssSelector("div.g-wrap.g-wrap_obtrusive.g-wrap_highlighted")).Click();

            //изменение типа операции
            System.Threading.Thread.Sleep(1000);
            var typeOperation = driver.FindElement(By.Id("ComponentsHost_PaymentEditLightbox_OperationType_Caption"));
            typeOperation.Click();
            wait.Until(ExpectedConditions.ElementExists(By.XPath("//ul[@id='ComponentsHost_PaymentEditLightbox_OperationType_Options']/li[3]")));
            driver.FindElement(By.XPath("//ul[@id='ComponentsHost_PaymentEditLightbox_OperationType_Options']/li[3]")).Click();

            //изменение суммы
            var amount = driver.FindElement(By.Id("ComponentsHost_PaymentEditLightbox_IncomeSum"));
            amount.Clear();
            amount.SendKeys("3200");
            
            //изменение контрагента
            System.Threading.Thread.Sleep(1000);
            var counterparty = driver.FindElement(By.Id("ComponentsHost_PaymentEditLightbox_Contractor_ContractorName"));
            counterparty.Clear();
            counterparty.SendKeys(@"ООО ""Рабочая лошадка""");
            
            // номер платежного поручения
            System.Threading.Thread.Sleep(1000);
            var numberPayment = driver.FindElement(By.Id("ComponentsHost_PaymentEditLightbox_DocumentNumber"));
            numberPayment.Clear();
            numberPayment.SendKeys("522");
            System.Threading.Thread.Sleep(1000);
            var description = driver.FindElement(By.Id("ComponentsHost_PaymentEditLightbox_PaymentDescription"));
            description.Clear();
            description.SendKeys("Привет мир");
            
            // Изменение даты
            System.Threading.Thread.Sleep(1000);
            var dataPayment = driver.FindElement(By.Id("ComponentsHost_PaymentEditLightbox_Date"));
            dataPayment.Clear();
            dataPayment.SendKeys("08.01.1998");
        
            driver.FindElement(By.Id("ComponentsHost_PaymentEditLightbox_AcceptButton")).Click();

            System.Threading.Thread.Sleep(1000);
            
            //проверка изменения типа операции
            var typeOperationNew = driver.FindElement(By.XPath("//div[2]/div/div/div/div/div[3]/div")).Text;
            Assert.AreEqual("Уплата налогов", typeOperationNew);
            
            // проверка изменения суммы
            var amountCounterpartyNew = driver.FindElement(By.CssSelector("span.c-currencySymbol_unobtrusive")).Text;
            Assert.AreEqual("3 200,00", amountCounterpartyNew);
            
            // проверка измененеия описания 
            var descriptionNew = driver.FindElement(By.CssSelector("span.paymentsList-table-description-fromUser")).Text;
            Assert.AreEqual("Привет мир", descriptionNew);
            
            // проверка измененеия даты
            var dataNew = driver.FindElement(By.XPath("//div[2]/div/div/div/div/div[7]/div")).Text;
            Assert.AreEqual("08.01.1998", dataNew);
         
            //номер платежа
            var numberPaymentNew = driver.FindElement(By.XPath("//div[7]/span[2]")).Text;
            Assert.AreEqual("№ 522", numberPaymentNew);
            
            // проверка суммы в налогах
            var amountTax = driver.FindElement(By.CssSelector("div.c-currencyStaticText_negative")).Text;
            Assert.AreEqual("", amountTax);
            
            //Проверка, что контрагент изменился
            var counterpartyChanged = driver.FindElement(By.CssSelector("span.t-readable")).Text;
            Assert.AreEqual(@"ООО ""Рабочая лошадка""", counterpartyChanged);

        }

        public void ActivityAfterTest() {
            //удаление выписок
            var checkAllRows = driver.FindElement(By.Id("ItemsList_CheckAllRows"));
            checkAllRows.Click();
            var deleteAll = driver.FindElement(By.Id("ComponentsHost_PaymentsMultipleActionsDialog_DeletePseudolink"));
            deleteAll.Click();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(ExpectedConditions.ElementExists(By.Id("ComponentsHost_DeleteConfirmationDialog_DeleteCaption")));
            var DeleteCaption = driver.FindElement(By.Id("ComponentsHost_DeleteConfirmationDialog_DeleteCaption"));
            DeleteCaption.Click();

            //переход в реквизиты
            wait.Until(ExpectedConditions.ElementExists(By.Id("MainMenu_Requisites_LinkText")));
            var requisites = driver.FindElement(By.Id("MainMenu_Requisites_LinkText"));
            requisites.Click();
            wait.Until(ExpectedConditions.ElementExists(By.Id("MainMenu_Requisites_BankRequisites_LinkText")));
            var bankAccount = driver.FindElement(By.Id("MainMenu_Requisites_BankRequisites_LinkText"));
            bankAccount.Click();

            // открытие формы банковских реквизит
            wait.Until(ExpectedConditions.ElementExists(By.Id("Accounts_row0_BankLogo"))); //
            var btnAddAccount = driver.FindElement(By.Id("Accounts_row0_BankLogo"));
            btnAddAccount.Click();
            System.Threading.Thread.Sleep(1000);
            var btnDeleteAccount = driver.FindElement(By.Id("BankAccountLightbox_DeleteAccountLink"));
            btnDeleteAccount.Click();
            var btnDeleteAccept = driver.FindElement(By.Id("BankAccountLightbox_DeleteAccountConfirmationPopup_AcceptButton"));
            btnDeleteAccept.Click();

        }

        [TestCleanup]
        public void Clean()
        {
            try {
                ActivityAfterTest();
            } catch (NoSuchElementException e)
            {}
            driver.Close();
            driver.Quit();
        }  
        
    }
}
