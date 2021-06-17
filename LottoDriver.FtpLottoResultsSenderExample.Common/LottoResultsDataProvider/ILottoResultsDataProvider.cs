using LottoDriver.FtpLottoResultsSenderExample.Common.Model;
using System.Collections.Generic;

namespace LottoDriver.FtpLottoResultsSenderExample.Common.LottoResultsDataProvider
{
    public interface ILottoResultsDataProvider
    {
        /// <summary>
        /// Returns collection of <see cref="LottoResultItem"/> objects.
        /// </summary>
        /// <returns></returns>
        IEnumerable<LottoResultItem> GetLottoResultsData();
    }
}