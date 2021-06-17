using LottoDriver.FtpLottoResultsSenderExample.Common.Model;
using System.Collections.Generic;

namespace LottoDriver.FtpLottoResultsSenderExample.Common.FileManager
{
    public interface ILocalResultsFileManager
    {
        /// <summary>
        /// Returns list of files with Lotto Results from Input folder.
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetFiles();

        /// <summary>
        /// Move specified file to Backup folder
        /// </summary>
        /// <param name="fullFileName"></param>
        void MoveToBackupDirectory(string fullFileName);

        /// <summary>
        /// Save provided collection with <see cref="LottoResultItem"/> to file in xml format.
        /// </summary>
        /// <param name="lottoResultItems">Collection of <see cref="LottoResultItem"/>.</param>
        void CreateFile(IEnumerable<LottoResultItem> lottoResultItems);

        /// <summary>
        /// Delete old files from backup.
        /// </summary>
        void DeleteOldFiles();
    }
}
