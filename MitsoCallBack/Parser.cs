using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MitsoCallBack
{
    public static class Parser
    {
        public static Task<string> ParseWeek(byte index)
        {
            var builder = new StringBuilder();

            var url = $"https://mitso.by/schedule/Dnevnaya/ME%60OiM/2%20kurs/1920%20ISIT/{index}#sch";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var footer = doc.DocumentNode.SelectSingleNode("//div[@class='rp-rasp-n']");

            if (footer == null)
                return Task.FromResult("😺 Расписания на эту неделю нет.");

            builder.AppendLine(footer.InnerText);
            builder.AppendLine();
            
            var rootData = doc.DocumentNode.SelectSingleNode("//div[@class='rp-ras']").ChildNodes;
            foreach (var rootNode in rootData)
            {
                var dayNumber = rootNode.SelectSingleNode("div[@class='rp-ras-data']");
                var dayName = rootNode.SelectSingleNode("div[@class='rp-ras-data2']");
                var lessons = rootNode.SelectSingleNode("div[@class='rp-ras-opis']");

                if (dayNumber == null || dayName == null || lessons == null)
                    continue;

                builder.AppendLine($"📅 {dayNumber.InnerText}. {dayName.InnerText}");

                foreach (var lessonNode in lessons.ChildNodes)
                {
                    var time = lessonNode.SelectSingleNode("div[@class='rp-r-time']");
                    var lesson = lessonNode.SelectSingleNode("div[@class='rp-r-op']");

                    if(time == null || lesson == null)
                        continue;
                    
                    builder.AppendLine($"{time.InnerText}");
                    
                    var lessonData = lesson.ChildNodes;
                    
                    if (lessonData != null)
                    {
                        foreach (var lessonItem in lessonData)
                        {
                            builder.AppendLine($"{lessonItem.InnerText}");
                        }
                    }
                    
                    var room = lessonNode.SelectSingleNode("div[@class='rp-r-aud']");
                    if (room != null)
                        builder.AppendLine($"{room.InnerText}");
                }

                builder.AppendLine();
            }
            return Task.FromResult(builder.ToString());
        }

        public static async Task<string> ParseAll()
        {
            var builder = new StringBuilder();
            
            for (byte i = 0; i < 5; i++)
            {
                var data = await ParseWeek(i);
                
                if (!data.StartsWith("😺 Расписания на эту неделю нет."))
                {
                    builder.AppendLine(data);
                }
            }

            var result = builder.ToString();
            
            if (string.IsNullOrWhiteSpace(result)) 
                result = "Ничего не найдено 🐸";

            return result;
        }
        
        public static string ExtractDay(this string parsed, byte day)
        {
            return parsed
                .Split(new[] {Environment.NewLine + Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(s => s.StartsWith($"📅 {day}"));
        }
    }
}