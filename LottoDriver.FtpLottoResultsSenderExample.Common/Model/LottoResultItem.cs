using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LottoDriver.FtpLottoResultsSenderExample.Common.Model
{
    [DataContract]
    public class LottoResultItem
    {
        /// <summary>
        /// This is the betting company's identifier for the Lotto
        /// <remarks>
        ///this is not LottoDriver's id
        /// </remarks>
        /// </summary>
        [DataMember]
        public string LottoId { get; set; }

        [DataMember]
        public string LottoName { get; set; }

        [DataMember]
        public string LottoOfferId { get; set; }

        [DataMember]
        public string LottoOfferRound { get; set; }

        [DataMember]
        public DateTime ResultTimeUtc { get; set; }

        [DataMember]
        public string ResultString { get; set; }

        [DataMember]
        public bool IsCanceled { get; set; }

        [DataMember]
        public int BallsToGuess { get; set; }

        [DataMember]
        public int TotalBalls { get; set; }

        [DataMember]
        public IList<KeyValuePair<int, decimal>> Odds{ get; set; }
    }
}