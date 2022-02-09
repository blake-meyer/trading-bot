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
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Program>()
                .Build();

            string apiKey = config["apiKey"];
            string secretKey = config["secretKey"];
            decimal coreNumber = System.Convert.ToDecimal(config["coreNumber"]);
            decimal marginPercent = System.Convert.ToDecimal(config["marginPercent"]);
            string action = "None";

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

            decimal currentBTCHolding = 0;
            decimal changePercent = 0;
            decimal tradeAmount = 0;
            foreach (Balance balance in accountInformation?.balances)
            {
                if (balance.asset == "BTC")
                {
                    currentBTCHolding = System.Convert.ToDecimal(balance.free) * System.Convert.ToDecimal(priceTickerObj.price);
                    changePercent = (currentBTCHolding - coreNumber) / coreNumber * 100;
                    tradeAmount = Math.Abs(currentBTCHolding - coreNumber);
                    //trade_amount    = round(abs(current_holding - my_core_number), my_round_off)
                    
                    Console.WriteLine(balance.asset + " Balance: " + balance.free);
                }
            }

            Console.WriteLine("Current BTC Holding: " + currentBTCHolding);
            Console.WriteLine("Change Percent: " + changePercent);
            
            if (changePercent > marginPercent)
            {
                action = "Sell";
            }
            else if (changePercent < marginPercent)
            {
                action = "Buy";
            }
            Console.WriteLine("ACTION: " + action);
            Console.WriteLine("Trade Amount: " + tradeAmount);

        }
    }
}


