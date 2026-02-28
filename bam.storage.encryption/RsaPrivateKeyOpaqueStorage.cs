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
    /// backend. Implements <see cref="INamedKeyStorage"/> for named key persistence and
    /// <see cref="IRsaPrivateKeyByteWriter"/> for writing private key bytes.
    /// </summary>
    public class RsaPrivateKeyOpaqueStorage : INamedKeyStorage, IRsaPrivateKeyByteWriter
    {
        const string DefaultKeyName = "DefaultRsaKey";
        private readonly OpaqueFsKeyValuePairStorage _opaqueFsKeyValueStorage;

        /// <summary>
        /// Initializes a new instance of <see cref="RsaPrivateKeyOpaqueStorage"/> using the specified opaque key-value pair storage.
        /// </summary>
        /// <param name="opaqueFsKeyValuePairStorage">The opaque key-value pair storage to use for persisting private key data.</param>
        public RsaPrivateKeyOpaqueStorage(OpaqueFsKeyValuePairStorage opaqueFsKeyValuePairStorage)
        {
            this._opaqueFsKeyValueStorage = opaqueFsKeyValuePairStorage;
        }

        /// <inheritdoc />
        public bool SaveNamedKey(string keyName, byte[] keyBytes)
        {
            return _opaqueFsKeyValueStorage.Save(keyName, keyBytes).Success;
        }

        /// <inheritdoc />
        public byte[]? GetNamedKey(string name)
        {
            try
            {
                IKeyValuePair kvp = this._opaqueFsKeyValueStorage.Get(name);
                return kvp?.Value;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Saves a named key and returns the full <see cref="IKeyValuePairSaveResult"/> for callers needing detailed result information.
        /// </summary>
        /// <param name="keyName">The name to associate with the key.</param>
        /// <param name="keyBytes">The raw key bytes to store.</param>
        /// <returns>The save result.</returns>
        public IKeyValuePairSaveResult SaveNamedKeyResult(string keyName, byte[] keyBytes)
        {
            return _opaqueFsKeyValueStorage.Save(keyName, keyBytes);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool WritePrivateKeyBytes(byte[] privateKeyBytes)
        {
            return SaveNamedKey(DefaultKeyName, privateKeyBytes);
        }

        protected Action<Exception> ExceptionHandler
        {
            get; set;
        } = null!;
    }
}
