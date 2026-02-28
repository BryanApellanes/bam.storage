using Bam.Encryption;

namespace Bam.Storage.Encryption;

/// <summary>
/// Orchestrates loading a named key from storage and executing an action within a protected usage context.
/// Composes <see cref="INamedKeyStorage"/> for key retrieval and <see cref="IProtectedKeyUsageContextFactory"/>
/// for creating secure usage contexts.
/// </summary>
public class NamedKeyUsageService
{
    private readonly INamedKeyStorage _keyStorage;
    private readonly IProtectedKeyUsageContextFactory _contextFactory;

    /// <summary>
    /// Initializes a new instance of <see cref="NamedKeyUsageService"/>.
    /// </summary>
    /// <param name="keyStorage">The named key storage to retrieve keys from.</param>
    /// <param name="contextFactory">The factory for creating protected key usage contexts.</param>
    public NamedKeyUsageService(INamedKeyStorage keyStorage, IProtectedKeyUsageContextFactory contextFactory)
    {
        _keyStorage = keyStorage;
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Retrieves the named key from storage and executes the specified action within a protected usage context.
    /// Does nothing if the key is not found.
    /// </summary>
    /// <param name="name">The name of the key to use.</param>
    /// <param name="action">The action to execute with the private key.</param>
    public void UseNamedKey(string name, Action<IPrivateKey> action)
    {
        byte[]? keyBytes = _keyStorage.GetNamedKey(name);
        if (keyBytes == null)
        {
            return;
        }
        using (ProtectedKeyUsageContext ctx = _contextFactory.Create(keyBytes))
        {
            ctx.UseKey(action);
        }
    }
}
