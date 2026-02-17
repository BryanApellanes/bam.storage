using Bam.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Storage.Encryption
{
    /// <summary>
    /// Provides opaque (encrypted) storage for RSA private keys using an <see cref="OpaqueFsKeyValuePairStorage"/>
    /// backend. Currently partially implemented.
    /// </summary>
    public class RsaPrivateKeyOpaqueStorage : IRsaPrivateKeyByteWriter, IRsaPrivateKeyByteReader
    {
        OpaqueFsKeyValuePairStorage _opaqueFsKeyValueStorage;

        /// <summary>
        /// Initializes a new instance of <see cref="RsaPrivateKeyOpaqueStorage"/> using the specified opaque key-value pair storage.
        /// </summary>
        /// <param name="opaqueFsKeyValuePairStorage">The opaque key-value pair storage to use for persisting private key data.</param>
        public RsaPrivateKeyOpaqueStorage(OpaqueFsKeyValuePairStorage opaqueFsKeyValuePairStorage)
        {
            this._opaqueFsKeyValueStorage = opaqueFsKeyValuePairStorage;
        }

        /// <summary>
        /// Reads an RSA key pair from the specified private key bytes. Not yet implemented.
        /// </summary>
        /// <param name="privateKeyBytes">The private key bytes to read.</param>
        /// <returns>The RSA public-private key pair.</returns>
        public RsaPublicPrivateKeyPair ReadPrivateKey(byte[] privateKeyBytes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes the private key bytes of the specified RSA key pair to opaque storage.
        /// Returns false and invokes the exception handler if an error occurs.
        /// </summary>
        /// <param name="keyPair">The RSA key pair whose private key bytes will be written.</param>
        /// <returns><c>true</c> if the write was successful; <c>false</c> if an error occurred.</returns>
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

        /// <summary>
        /// Writes the specified private key bytes to opaque storage. Not yet implemented.
        /// </summary>
        /// <param name="privateKeyBytes">The private key bytes to write.</param>
        /// <returns><c>true</c> if the write was successful; <c>false</c> otherwise.</returns>
        public bool WritePrivateKeyBytes(byte[] privateKeyBytes)
        {
            throw new NotImplementedException();
        }

        protected Action<Exception> ExceptionHandler
        {
            get; set;
        } = null!;
    }
}
