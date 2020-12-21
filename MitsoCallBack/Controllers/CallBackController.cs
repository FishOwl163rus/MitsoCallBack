using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

namespace MitsoCallBack.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class CallBackController : ControllerBase
    {
        private IConfiguration Cfg { get; }

        public CallBackController(IConfiguration configuration)
        {
            Cfg = configuration;
            VkHelper.AuthWithToken(Cfg["AccessToken"]);
        }

        public async Task<IActionResult> CallBack([FromBody] Updates updates)
        {
            switch (updates.Type)
            {
                case "confirmation":
                    return Ok(Cfg["ConfirmCode"]);
                case "message_new":
                    var message = MessageNew.FromJson(new VkResponse(updates.Object)).Message;

                    if (message.Text == "/start")
                    {
                        await VkHelper.SendMsg(message.PeerId, "Сова инициализирована.", VkHelper.DefaultKeyboard);
                    }

                    if (string.IsNullOrEmpty(message.Payload))
                    {
                        return Ok("ok");
                    }

                    var payload = (string) JObject.Parse(message.Payload)["button"] ?? string.Empty;
                    string week;

                    switch (payload)
                    {
                        case "lessons_today":
                            week = await Parser.ParseWeek(0);
                            await VkHelper.SendMsg(message.PeerId, week.ExtractDay((byte) DateTime.Now.Day));

                            break;
                        case "lessons_current":
                            week = await Parser.ParseWeek(0);
                            await VkHelper.SendMsg(message.PeerId, week);

                            break;
                        case "lessons_next":
                            week = await Parser.ParseWeek(1);
                            await VkHelper.SendMsg(message.PeerId, week);

                            break;
                        case "lessons_all":

                            week = await Parser.ParseAll();
                            await VkHelper.SendMsg(message.PeerId, week);
                            break;
                    }
                    break;
            }
            return Ok("ok");
        }
    }
}