using System;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using System.Threading;

namespace BotWPF



{
    class TelegramMessageClient

    {
         MainWindow w;

        private TelegramBotClient bot;
        public  ObservableCollection<MessageLog> BotMessageLog { get; set; }

        
       
        public void MessageListener ( Message message)
        {
            string text=$"{DateTime.Now.ToLongTimeString()}:{message.Chat.FirstName}{message.Chat.Id} {message.Text}";
            Debug.WriteLine($"{text} TypeMessage: {message.Type.ToString()}");
           

            if (message.Text == null) return;

            var messageText = message.Text;

           
            w.Dispatcher.Invoke(() =>
            {
                BotMessageLog.Add(
                new MessageLog(
                    DateTime.Now.ToLongTimeString(), messageText,message.Chat.FirstName, message.Chat.Id));
            });

            
            
        }
        public void SaveJson()
        {
       
          
            string serialized = JsonConvert.SerializeObject(BotMessageLog);
       
            if (System.IO.File.Exists(@"savemessage.json"))
                  System.IO.File.Create(@"savemessage.json").Close();
            System.IO.File.AppendAllText (@"savemessage.json", serialized);

            

        }

        public TelegramMessageClient(MainWindow W, string PathToken = @"C:\Dowload\token.txt")
        {
            this.BotMessageLog = new ObservableCollection<MessageLog>();
            this.w = W;

            bot = new TelegramBotClient(System.IO.File.ReadAllText(PathToken));
            var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };
            bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
            );
           

          
        }
        public void SendMessage(string Text, string Id)
        {
            long id = Convert.ToInt64(Id);
            bot.SendTextMessageAsync(id, Text);
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
           
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)

            {
                  
                 await HandleMessage(botClient, update.Message);
                  
                return;
            }

        }
       async Task HandleMessage(ITelegramBotClient botClient, Message message)
        {

            if (message.Text == "/start")
            
            {
                
                await botClient.SendTextMessageAsync(message.Chat.Id, "Привествую, я готов перекинуть твои данные)", replyToMessageId: message.MessageId);
                return;
            }


            if (message.Text == "Привет")
                await botClient.SendTextMessageAsync(message.Chat.Id, "Привет, привет давай кидай чего нибудь)", replyToMessageId: message.MessageId);

             MessageListener(message);
            

        }
        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
          
            return Task.CompletedTask;
        }

    }
}
