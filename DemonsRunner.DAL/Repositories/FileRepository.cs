﻿using DemonsRunner.DAL.Repositories.Interfaces;
using DemonsRunner.DAL.Storage.Interfaces;
using DemonsRunner.Domain.Models;
using Newtonsoft.Json;

namespace DemonsRunner.DAL.Repositories
{
    public class FileRepository : IFileRepository<PHPDemon>
    {
        private readonly IStorageFile _storageFile;
        private readonly object _locker = new object();

        public FileRepository(IStorageFile storageFile)
        {
            _storageFile = storageFile;
        }

        public IEnumerable<PHPDemon> GetAll()
        {
            if (!File.Exists(_storageFile.FullPath))
            {
                throw new InvalidOperationException("The storage file has been deleted or renamed");
            }

            lock(_locker)
            {
                using var reader = new StreamReader(_storageFile.FullPath);
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<IEnumerable<PHPDemon>>(json) ?? Enumerable.Empty<PHPDemon>();
            }
        }

        public void SaveAll(IEnumerable<PHPDemon> items)
        {
            lock (_locker)
            {
                using var writer = new StreamWriter(_storageFile.FullPath);
                string json = JsonConvert.SerializeObject(items, Formatting.Indented);
                writer.Write(json);
            }
        }
    }
}
