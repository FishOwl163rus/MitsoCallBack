using System;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace MitsoCallBack
{
    public static class VkHelper
    {
        private static readonly VkApi Api = new VkApi();
        public static MessageKeyboard DefaultKeyboard =>
            new KeyboardBuilder()
                .AddButton("Пары сегодня", "lessons_today", KeyboardButtonColor.Primary)
                .AddButton("Расписание на неделю", "lessons_current", KeyboardButtonColor.Primary)
                .AddLine()
                .AddButton("Следующая неделя", "lessons_next", KeyboardButtonColor.Primary)
                .AddButton(new MessageKeyboardButtonAction
                {
                    Label = "Физра",
                    Type = KeyboardButtonActionType.OpenLink,
                    Link = new Uri("https://mitso.by/raspisanie-zanyatiy-po-fizkulture"),
                    Payload = "",
                })
                .AddLine()
                .AddButton("Найти все расписание", "lessons_all", KeyboardButtonColor.Positive)
                .Build();
        
        public static void AuthWithToken(string token)
        {
            Api.Authorize(new ApiAuthParams
            {
                AccessToken = token
            });
        }

        public static async Task SendMsg(long? peer, string message)
        {
            await Api.Messages.SendAsync(new MessagesSendParams
            {
                PeerId = peer,
                RandomId = DateTime.Now.Millisecond,
                Message = message,
            });
        }
        
        public static async Task SendMsg(long? peer, string message, MessageKeyboard keyboard)
        {
            await Api.Messages.SendAsync(new MessagesSendParams
            {
                PeerId = peer,
                RandomId = DateTime.Now.Millisecond,
                Message = message,
                Keyboard = keyboard
            });
        }
    }
}