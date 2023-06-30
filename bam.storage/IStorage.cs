using Bam.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Storage
{
    public interface IStorage
    {
        T Load<T>(string path);

        T[] Search<T>(IStorageSearchFilter searchFilter);

        IStorageResult Save<T>(string path, T value);
    }
}
