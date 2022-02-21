using System;
using System.Threading.Tasks;
using Binance.Spot;
using System.Text.Json;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Binance.Common;

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
            int minutesLoop = System.Convert.ToInt32(config["minutesLoop"]);
            string binanceUrl = config["binanceUrl"];

            while (true)
            {
                
                string action = "None";
                decimal btcPrice = 0;

                // Get BTC Price and print to console
                Market market = new Market(binanceUrl, apiKey, secretKey);
                var priceTickerString = await market.SymbolPriceTicker("BTCUSDT");
                PriceTicker? priceTickerObj = JsonSerializer.Deserialize<PriceTicker>(priceTickerString);

                if (priceTickerObj?.price != null)
                {
                    btcPrice = System.Convert.ToDecimal(priceTickerObj.price);
                }

                var account = new SpotAccountTrade(binanceUrl, apiKey, secretKey);
                var accountInformationString = await account.AccountInformation();
                AccountInformation? accountInformation = JsonSerializer.Deserialize<AccountInformation>(accountInformationString);

                decimal currentBTCHolding = 0;
                decimal changePercent = 0;
                decimal tradeAmount = 0;
                decimal btcBalance = 0;
                decimal usdBalance = 0;
                foreach (Balance balance in accountInformation?.balances)
                {
                    if (balance.asset == "BTC")
                    {
                        currentBTCHolding = System.Convert.ToDecimal(balance.free) * System.Convert.ToDecimal(priceTickerObj.price);
                        changePercent = (currentBTCHolding - coreNumber) / coreNumber * 100;
                        tradeAmount = Decimal.Round(Math.Abs(currentBTCHolding - coreNumber), 2);
                        btcBalance = System.Convert.ToDecimal(balance.free);
                    }

                    if (balance.asset == "USDT")
                    {
                        usdBalance = System.Convert.ToDecimal(balance.free);
                    }
                }


                // ACTION
            
                ConsoleColor color = ConsoleColor.White;
                if (changePercent > marginPercent)
                {
                    action = "Sell " + tradeAmount;
                    await account.NewOrder(symbol: "BTCUSDT", side: Binance.Spot.Models.Side.SELL, type: Binance.Spot.Models.OrderType.MARKET, quoteOrderQty: tradeAmount);
                    color = ConsoleColor.Green;
                }
                else if (changePercent < -marginPercent)
                {
                    action = "Buy " + tradeAmount;
                    await account.NewOrder(symbol: "BTCUSDT", side: Binance.Spot.Models.Side.BUY, type: Binance.Spot.Models.OrderType.MARKET, quoteOrderQty: tradeAmount);
                    color = ConsoleColor.Red;
                }
                DisplayInfo(btcPrice, btcBalance, coreNumber, currentBTCHolding, usdBalance, changePercent, action, color);
            
                System.Threading.Thread.Sleep(minutesLoop * 60 * 1000);
            }
        }

        public static void DisplayInfo(decimal btcPrice, decimal btcBalance, decimal coreNumber, decimal currentBTCHolding, decimal usdBalance, decimal changePercent, string action, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("\n");
            Console.WriteLine("BTC Price: " + btcPrice);
            Console.WriteLine("BTC Balance: " + btcBalance);
            Console.WriteLine("My Core Number: " + coreNumber);
            Console.WriteLine("Current BTC Holding (USD): " + currentBTCHolding);
            Console.WriteLine("Current USD Holding: " + usdBalance);
            Console.WriteLine("Total Value Held: " + (currentBTCHolding + usdBalance));
            Console.WriteLine("Change Percent: " + changePercent);
            Console.WriteLine("\n");
            Console.WriteLine("ACTION: " + action);
            Console.WriteLine("\n");
            Console.ResetColor();
        }
    }
}


