using System.Threading;
using System.Threading.Tasks;

namespace LottoDriver.FtpLottoResultsSenderExample.Common
{
    public interface IFtpFileUploadManager
    {
        /// <summary>
        /// Uploads files prepared in INPUT folder to FTP server.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UploadFiles(CancellationToken cancellationToken);
    }
}