namespace BitcoinBot
{
    public class AccountInformation
    {
        public int? makerCommission {get; set;}
        public int? takerCommission {get; set;}
        public int? buyerCommission {get; set;}
        public int? sellerCommission {get; set;}
        public bool? canTrade {get; set;}
        public bool? canWithdraw {get; set;}
        public bool? canDeposit {get; set;}
        public long? updateTime {get; set;}
        public string? accountType {get; set;}
        public IEnumerable<Balance>? balances {get; set;}
        public IEnumerable<string>? permission {get; set;}
    }
}
