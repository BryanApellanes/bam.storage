using Bam.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Storage.Encryption
{
    public class RsaPrivateKeyOpaqueStorage : IRsaPrivateKeyByteWriter, IRsaPrivateKeyByteReader
    {
        OpaqueFsKeyValuePairStorage _opaqueFsKeyValueStorage;

        public RsaPrivateKeyOpaqueStorage(OpaqueFsKeyValuePairStorage opaqueFsKeyValuePairStorage)
        {
            this._opaqueFsKeyValueStorage = opaqueFsKeyValuePairStorage;
        }

        public RsaPublicPrivateKeyPair ReadPrivateKey(byte[] privateKeyBytes)
        {
            throw new NotImplementedException();
        }

        public bool WritePrivateKeyBytes(RsaPublicPrivateKeyPair keyPair)
        {
            try
            {
                WritePrivateKeyBytes(keyPair.Pem);
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler?.Invoke(ex);
                return false;
            }
        }

        public bool WritePrivateKeyBytes(byte[] privateKeyBytes)
        {
            throw new NotImplementedException();
        }

        protected Action<Exception> ExceptionHandler
        {
            get; set;
        }
    }
}
