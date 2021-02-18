using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seoulTrobl
{
    class GradeAge
    {
        protected ChromeDriverService _driverService = null;
        protected ChromeOptions _options = null;
        protected ChromeDriver _driver = null;
        protected string[] eSigungu = { "서울시", "종로구", "중구", "용산구", "성동구", "광진구", "동대문구", "중랑구", "성북구", "강북구", "도봉구", "노원구", "은평구", "서대문구", "마포구", "양천구", "강서구", "구로구", "금천구", "영등포구", "동작구", "관악구", "서초구", "강남구", "송파구", "강동구" };
        protected string[] eAgeS = { "0", "10", "20", "30", "40", "50", "60", "70", "80" };
        protected string[] eAgeE = { "9", "19", "29", "39", "49", "59", "69", "79", "999" };

        public void getGradeAge()
        {
            try
            {
                string dbBaseYear = Program.selectDS("SELECT MAX(base_year) FROM seoul_grade_age ").Tables[0].Rows[0][0].ToString();
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
                _driver.Navigate().GoToUrl("https://data.seoul.go.kr/dataList/10476/S/2/datasetView.do");
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                _driver.SwitchTo().Frame("IframeRequest");

                Console.WriteLine("시작");

                string baseYear = _driver.FindElementByXPath("//*[@id='OctagonGrid']/tbody/tr[1]/td[1]/nobr").Text;

                if (int.Parse(dbBaseYear) < int.Parse(baseYear))
                {
                    for (int i = 0; i < eSigungu.Length; i++)
                    {
                        Console.WriteLine(eSigungu[i] + " 진행중....");

                        int valCnt = 0;
                        for (int j = 0; j < eAgeS.Length; j++)
                        {
                            for (int k = 0; k < 2; k++)
                            {
                                seoulGradeAge model = new seoulGradeAge();

                                model.base_year = baseYear;
                                model.sigungu = eSigungu[i];
                                model.age_s = eAgeS[j];
                                model.age_e = eAgeE[j];

                                if (k == 0)
                                {
                                    model.sexdstn = "남자";
                                }
                                else
                                {
                                    model.sexdstn = "여자";
                                }

                                IList<IWebElement> eVal = _driver.FindElements(By.Id("dataSession"))[0].FindElement(By.TagName("tbody")).FindElements(By.TagName("tr"));
                                model.val = eVal[i].FindElements(By.TagName("td"))[k + valCnt + 10].Text;

                                StringBuilder sb = new StringBuilder();
                                sb.Append(" insert into seoul_grade_age values(");
                                sb.Append(" '" + model.base_year.Replace("'", "") + "',");
                                sb.Append(" '" + model.sigungu.Replace("'", "") + "',");
                                sb.Append(" '" + model.age_s.Replace("'", "") + "',");
                                sb.Append(" '" + model.age_e.Replace("'", "") + "',");
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

    internal class seoulGradeAge
    {
        public string base_year { get; set; }
        public string sigungu { get; set; }
        public string age_s { get; set; }
        public string age_e { get; set; }
        public string sexdstn { get; set; }
        public string val { get; set; }
    }
}
