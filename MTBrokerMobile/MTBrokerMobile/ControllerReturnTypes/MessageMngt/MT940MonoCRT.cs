using System;

namespace MTBrokerMobile.ControllerReturnTypes.MessageMngt
{
    public class MT940MonoCRT
    {
        public int ID { get; set; }

        public string AccountID25 { get; set; }

        public decimal AvailableBalance64Amount { get; set; }

        public string AvailableBalance64Currency { get; set; }

        public DateTime AvailableBalance64Date { get; set; }

        public string AvailableBalance64DOrC { get; set; }

        public decimal ClosingBalance62FAmount { get; set; }

        public string ClosingBalance62FCurrency { get; set; }

        public DateTime ClosingBalance62FDate { get; set; }

        public string ClosingBalance62FDOrC { get; set; }

        public string FinBranchCode { get; set; }

        public string FinLTCode { get; set; }

        public string FinSwiftAddress { get; set; }

        public decimal OpeningBalance60FAmount { get; set; }

        public string OpeningBalance60FCurrency { get; set; }

        public DateTime OpeningBalance60FDate { get; set; }

        public string OpeningBalance60FDOrC { get; set; }

        public string SendersSwiftAddress { get; set; }

        public string SequenceNumber { get; set; }

        public string SessionNumber { get; set; }

        public string StatementOrSeqNo28CMsgSeq { get; set; }

        public string StatementOrSeqNo28CStmntSeq { get; set; }

        public string TransactionRefNo20 { get; set; }
    }
}
