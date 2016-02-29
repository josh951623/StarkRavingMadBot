namespace StarkRavingMadBot
{
    class Program
    {
        private const string BOT_EMAIL = "USERNAME";
        private const string BOT_PASS = "PASSWORD";

        static void Main(string[] args)
        {
            var bot = new StarkRavingMadBot(BOT_EMAIL, BOT_PASS);
            bot.Start();
        }
    }
}
