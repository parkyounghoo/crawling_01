using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace seoulTrobl
{
    internal class TroblType
    {
        protected ChromeDriverService _driverService = null;
        protected ChromeOptions _options = null;
        protected ChromeDriver _driver = null;
        protected string[] eSigungu = { "서울시", "종로구", "중구", "용산구", "성동구", "광진구", "동대문구", "중랑구", "성북구", "강북구", "도봉구", "노원구", "은평구", "서대문구", "마포구", "양천구", "강서구", "구로구", "금천구", "영등포구", "동작구", "관악구", "서초구", "강남구", "송파구", "강동구" };
        protected string[] eTroblType = { "지체", "뇌병변", "시각", "청각", "언어", "지적장애", "자폐성", "정신장애", "신장장애", "심장장애", "호흡기", "간", "안면", "장루요루", "뇌전증" };
        public void getTroblType()
        {
            try
            {
                string dbBaseYear = Program.selectDS("SELECT MAX(base_year) from seoul_trobl_type").Tables[0].Rows[0][0].ToString();
                if (dbBaseYear == "")
                {
                    dbBaseYear = "0000";
                }
                _driverService = ChromeDriverService.CreateDefaultService();
                _driverService.HideCommandPromptWindow = true;

                _options = new ChromeOptions();
                _options.AddArgument("headless"); 
                _options.AddArgument("disable-gpu");

                _driver = new ChromeDriver(_driverService, _options);
                _driver.Navigate().GoToUrl("https://data.seoul.go.kr/dataList/18/S/2/datasetView.do");
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                _driver.SwitchTo().Frame("IframeRequest");

                //string html = _driver.PageSource;

                Console.WriteLine("시작");

                string baseYear = _driver.FindElementByXPath("//*[@id='OctagonGrid']/tbody/tr[1]/td[1]/nobr").Text;

                if (int.Parse(dbBaseYear) < int.Parse(baseYear))
                {
                    //스크롤에 가려진 화면의 텍스트는 못가져옴.
                    //IList<IWebElement> eSigungu = _driver.FindElements(By.Id("OctagonGrid"))[0].FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));

                    int cnt = eSigungu.Length;
                    for (int i = 0; i < cnt; i++)
                    {
                        //스크롤에 가려진 화면의 텍스트는 못가져옴.
                        //IList<IWebElement> eTroblType = _driver.FindElements(By.Id("colSession"))[0].FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"))[1].FindElements(By.TagName("td"));
                        Console.WriteLine(eSigungu[i] + " 진행중....");

                        int valCnt = 0;
                        for (int j = 0; j < eTroblType.Length; j++)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                seoulTroblType model = new seoulTroblType();

                                model.base_year = baseYear;
                                model.sigungu = eSigungu[i];
                                model.type = eTroblType[j];

                                if (k == 0)
                                {
                                    model.sexdstn = "남자";
                                }
                                else
                                {
                                    model.sexdstn = "여자";
                                }

                                IList<IWebElement> eVal = _driver.FindElements(By.Id("dataSession"))[0].FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                                model.val = eVal[i].FindElements(By.TagName("td"))[k + valCnt + 4].Text;

                                StringBuilder sb = new StringBuilder();
                                sb.Append(" insert into seoul_trobl_type values(");
                                sb.Append(" '" + model.base_year.Replace("'", "") + "',");
                                sb.Append(" '" + model.sigungu.Replace("'", "") + "',");
                                sb.Append(" '" + model.type.Replace("'", "") + "',");
                                sb.Append(" '" + model.sexdstn.Replace("'", "") + "',");
                                sb.Append(" '" + model.val.Replace("'", "") + "',");
                                sb.Append(" getdate() )");

                                Program.insert(sb.ToString());
                            }

                            valCnt += 3;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                throw;
            }
            finally
            {
                _driverService.Dispose();
            }
        }
    }

    internal class seoulTroblType
    {                               
        public string base_year { get; set; }
        public string sigungu { get; set; }
        public string type { get; set; }
        public string sexdstn { get; set; }
        public string val { get; set; }
    }
}