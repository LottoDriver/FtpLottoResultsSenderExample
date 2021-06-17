using LottoDriver.FtpLottoResultsSenderExample.Common.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace LottoDriver.FtpLottoResultsSenderExample.Common.FileManager
{
    public class LocalResultsFileManager : ILocalResultsFileManager
    {
        private readonly string _inputDirectoryFullPath;
        private readonly string _backupDirectoryFullPath;
        private readonly string _clientName;

        public LocalResultsFileManager(string inputDirectoryFullPath, string backupDirectoryFullPath, string clientName)
        {
            _inputDirectoryFullPath = inputDirectoryFullPath;
            _backupDirectoryFullPath = backupDirectoryFullPath;
            _clientName = clientName;
        }

        /// <inheritdoc />
        public IEnumerable<string> GetFiles()
        {
            EnsureFolderExists(_inputDirectoryFullPath);

            return Directory.GetFiles(_inputDirectoryFullPath, "*.xml");
        }

        /// <inheritdoc />
        public void MoveToBackupDirectory(string fullFileName)
        {
            EnsureFolderExists(_backupDirectoryFullPath);

            var fi = new FileInfo(fullFileName);
            var destFileName = Path.Combine(_backupDirectoryFullPath, fi.Name);

            fi.MoveTo(destFileName);
        }

        /// <inheritdoc />
        public void CreateFile(IEnumerable<LottoResultItem> lottoResultItems)
        {
            var dateTimeAsString = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff");
            var tempFileName = $"{_clientName}_Lotto_Results_{dateTimeAsString}.temp";
            var tempFileNameFullPath = Path.Combine(_inputDirectoryFullPath, tempFileName);
            var fileNameFullPath = Path.ChangeExtension(tempFileNameFullPath, ".xml");

            using (var writer = new FileStream(tempFileNameFullPath, FileMode.Create))
            {
                var serializer =  new DataContractSerializer(lottoResultItems.GetType());
                serializer.WriteObject(writer, lottoResultItems);
                writer.Close();
            }

            File.Move(tempFileNameFullPath, fileNameFullPath);
        }

        /// <inheritdoc />
        public void DeleteOldFiles()
        {
            double olderThenHours = 3 * 24; // 3 Days
            Directory.GetFiles(_backupDirectoryFullPath)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastWriteTime < DateTime.Now.AddHours(-olderThenHours))
                .ToList()
                .ForEach(f => f.Delete());
        }

        private void EnsureFolderExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}