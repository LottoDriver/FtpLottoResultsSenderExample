using LottoDriver.FtpLottoResultsSenderExample.Common.Model;
using System;
using System.Collections.Generic;

namespace LottoDriver.FtpLottoResultsSenderExample.Common.LottoResultsDataProvider
{
    /// <inheritdoc />
    public class LottoResultsDataProvider : ILottoResultsDataProvider
    {
        public IEnumerable<LottoResultItem> GetLottoResultsData()
        {
            // For demo purpose dummy data is returned.
            // Put your logic here to retrieve this from database or any other source!

            return new List<LottoResultItem>
            {
                new LottoResultItem
                {
                    LottoId = "1",
                    LottoName = "Italy Win for Life Classico - 10/20",
                    LottoOfferId = "438002202104261200",
                    LottoOfferRound = "1",
                    ResultTimeUtc = DateTime.UtcNow,
                    ResultString = "1,2,3,4,5,11,13,14,18,19",
                    IsCanceled = false,
                    BallsToGuess = 10,
                    TotalBalls = 20
                },
                new LottoResultItem
                {
                    LottoId = "1",
                    LottoName = "Finland Keno - 20/70",
                    LottoOfferId = "424601202104261149",
                    LottoOfferRound = "1",
                    ResultTimeUtc = DateTime.UtcNow,
                    ResultString = "2,6,8,10,11,20,21,22,23,30,41,43,45,56,60,65,66,67,69,70",
                    IsCanceled = false,
                    BallsToGuess = 20,
                    TotalBalls = 70
                },
                new LottoResultItem
                {
                    LottoId = "1",
                    LottoName = "Italy 10e Lotto - 20/90",
                    LottoOfferId = "438001202104261150",
                    LottoOfferRound = "1",
                    ResultTimeUtc = DateTime.UtcNow,
                    ResultString = "6,9,14,16,22,23,28,32,37,40,42,49,53,55,56,63,65,69,74,81",
                    IsCanceled = false,
                    BallsToGuess = 20,
                    TotalBalls = 90
                }
            };
        }
    }
}