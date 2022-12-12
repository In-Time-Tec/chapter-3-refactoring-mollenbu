using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        private CultureInfo cultureInfo;
        private int volumeCredits = 0;
        private string formattedResult;

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            cultureInfo = new CultureInfo("en-US");
            formattedResult = string.Format("Statement for {0}\n", invoice.Customer);
            var invoiceTotalAmount = getInvoiceTotalAmountAndVolumeCredits(invoice, plays);
            formattedResult += String.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(invoiceTotalAmount / 100));
            formattedResult += String.Format("You earned {0} credits\n", volumeCredits);
            return formattedResult;
        }

        private int getInvoiceTotalAmountAndVolumeCredits(Invoice invoice, Dictionary<string, Play> plays)
        {
            var invoiceTotalAmount = 0;
            foreach (var performance in invoice.Performances)
            {
                var play = plays[performance.PlayID];
                var playAmountOwed = getPlayAmountOwed(performance.Audience, play);
                volumeCredits += getAudienceVolumeCredits(performance.Audience, play);
                formattedResult += getFormattedLineOrder(performance.Audience, play, playAmountOwed);
                invoiceTotalAmount += playAmountOwed;
            }
            return invoiceTotalAmount;
        }

        private string getFormattedLineOrder(int audienceCount, Play play, int thisAmount)
        {
            return String.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(thisAmount / 100), audienceCount);
        }

        private static int getAudienceVolumeCredits(int audienceCount, Play play)
        {
            // add volume credits
            var volumeCredits = Math.Max(audienceCount - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == play.Type) volumeCredits += (int)Math.Floor((decimal)audienceCount / 5);
            return volumeCredits;
        }

        private static int getPlayAmountOwed(int audienceCount, Play play)
        {
            int thisAmount = 0;
            switch (play.Type)
            {
                case "tragedy":
                    thisAmount = getTragedyAmountOwed(audienceCount);
                    break;
                case "comedy":
                    thisAmount = getComedyAmountOwed(audienceCount);
                    break;
                default:
                    throw new Exception("unknown type: " + play.Type);
            }

            return thisAmount;
        }

        private static int getComedyAmountOwed(int audienceCount)
        {
            int thisAmount = 30000;
            if (audienceCount > 20)
            {
                thisAmount += 10000 + 500 * (audienceCount - 20);
            }
            thisAmount += 300 * audienceCount;
            return thisAmount;
        }

        private static int getTragedyAmountOwed(int audienceCount)
        {
            int thisAmount = 40000;
            if (audienceCount > 30)
            {
                thisAmount += 1000 * (audienceCount - 30);
            }

            return thisAmount;
        }
    }
}
