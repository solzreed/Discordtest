using Discord;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using System.Linq;
using System.Net;
using System.Threading;
using System.Text;

namespace Discordtest
{
    class Program
    {
        static void Main(string[] args)
                    => new Program().MainAsync().GetAwaiter().GetResult();

        private readonly DiscordSocketClient _client;
        static string program = Thread.GetDomain().BaseDirectory + "Tesseract-OCR";
        TesseractService service = new TesseractService(program, "kor", program + @"\tessdata");
        public Program()
        {
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
        }
        public async Task MainAsync()
        {
            string token = File.ReadAllText("token.txt");
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} 연결됨!");

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content == "?")  await message.Channel.SendMessageAsync("뭐요");
            if (message.Content == "돈삽이") await message.Channel.SendMessageAsync("하주형");
            if(message.Attachments.Count != 0)
            {
                var path = message.Attachments.FirstOrDefault().Url;
                await message.Channel.SendMessageAsync(path);
                WebClient webClient = new WebClient();
                webClient.DownloadFile(path, @"test.jpg");
                byte[] byteArray = Encoding.ASCII.GetBytes(@"test.jpg");
                MemoryStream stream = new MemoryStream(byteArray);
                var text = service.GetText(stream);
                await message.Channel.SendMessageAsync(text);


            }

        }



    }
}
