using Bam.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Storage
{
    public interface IAsyncStorage : IStorage
    {
        Task<T> LoadAsync<T>(string path);

        Task<T[]> SearchAsync<T>(IStorageSearchFilter searchFilter);

        Task<IStorageResult> SaveAsync<T>(string path, T value);
    }
}
