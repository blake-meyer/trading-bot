using System;
using System.Threading.Tasks;
using Binance.Spot;
using System.Text.Json;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace BitcoinBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<Program>()
                .Build();

            string apiKey = config["apiKey"];
            string secretKey = config["secretKey"];

            string baseUrl = "https://testnet.binance.vision";


            // Get BTC Price and print to console
            Market market = new Market(baseUrl, apiKey, secretKey);
            var priceTickerString = await market.SymbolPriceTicker("BTCUSDT");
            PriceTicker? priceTickerObj = JsonSerializer.Deserialize<PriceTicker>(priceTickerString);

            if (priceTickerObj?.price != null)
            {
                Console.WriteLine("Symbol: " + priceTickerObj.symbol);
                Console.WriteLine("BTC Price: " + priceTickerObj.price);
            }


            // Check current balances
            var account = new SpotAccountTrade(baseUrl, apiKey, secretKey);
            var accountInformationString = await account.AccountInformation();
            AccountInformation? accountInformation = JsonSerializer.Deserialize<AccountInformation>(accountInformationString);

            foreach (Balance balance in accountInformation?.balances)
            {
                if (balance.asset == "BTC" | balance.asset == "USDT")
                {
                    Console.WriteLine(balance.asset + " Balance: " + balance.free);
                }
            }
        }
    }
}


