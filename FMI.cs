using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleForTesting
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            GetForecast("Helsinki", 5, 2);
        }

        private static void GetForecast(string place, int numOfForecasts, int timeStepHours)
        {
            string nowUtc = CreateDateTimeString(DateTime.UtcNow);
            string endUtc = CreateDateTimeString(DateTime.UtcNow.AddHours(timeStepHours * numOfForecasts));

            var sb = new StringBuilder(256);

            sb.Append("http://opendata.fmi.fi/wfs?service=WFS");
            sb.Append("&request=getFeature");
            sb.Append("&storedquery_id=fmi::forecast::hirlam::surface::point::multipointcoverage");
            sb.Append($"&place={place}");
            sb.Append($"&starttime={nowUtc}");
            sb.Append($"&endtime={endUtc}");
            sb.Append("&parameters=temperature,weatherSymbol3");
            sb.Append($"&timestep={timeStepHours * 60}");

            var response = client.GetAsync(sb.ToString());
            var bodyContent = response.Result.Content.ReadAsByteArrayAsync().Result;

            var xml = Encoding.UTF8.GetString(bodyContent);
            string dataPoints = GetElementValue(xml, "<gml:doubleOrNilReasonTupleList>", "</gml:doubleOrNilReasonTupleList>");
            string beginPosition = GetElementValue(xml, "<gml:beginPosition>", "</gml:beginPosition>");

            dataPoints = CleanDataPoints(dataPoints);

            Console.ReadLine();
        }

        private static string GetElementValue(string xml, string beginTag, string endTag)
        {
            if (string.IsNullOrWhiteSpace(xml)) return "";
            if (string.IsNullOrWhiteSpace(beginTag)) return "";
            if (string.IsNullOrWhiteSpace(endTag)) return "";

            string result;

            try
            {
                int startIndex = xml.IndexOf(beginTag) + beginTag.Length;
                int endIndex = xml.IndexOf(endTag);

                result = xml.Substring(startIndex, endIndex - startIndex).Trim();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// 2019-12-04T12:21:34.668Z
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static string CreateDateTimeString(DateTime date)
        {
            var sb = new StringBuilder(25);

            sb.AppendFormat("{0:0000}", date.Year);
            sb.Append("-");
            sb.AppendFormat("{0:00}", date.Month);
            sb.Append("-");
            sb.AppendFormat("{0:00}", date.Day);
            sb.Append("T");
            sb.AppendFormat("{0:00}", date.Hour);
            sb.Append(":");
            sb.AppendFormat("{0:00}", date.Minute);
            sb.Append(":");
            sb.AppendFormat("{0:00}", date.Second);
            sb.Append(".000Z");

            return sb.ToString();
        }

        private static string CleanDataPoints(string dataPoints)
        {
            var sb = new StringBuilder(100);
            char last = '\0';

            for (int i=0; i<dataPoints.Length; i++)
            {
                if ("0123456789.".Contains(dataPoints[i]))
                {
                    last = dataPoints[i];
                    sb.Append(last);
                }
                else
                {
                    if (last != '\0')
                    {
                        sb.Append(" ");
                    }

                    last = '\0';
                }
            }

            return sb.ToString();
        }
    }
}
