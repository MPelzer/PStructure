using System;
using Microsoft.Extensions.Configuration;

namespace PStructure.PersistenceLayer.Pdo.PdoCruds.SimpleCrud;

public static class SimplePdoDataGlobal
{
    private static readonly Lazy<IConfiguration> _lazyConfiguration = new(() =>
    {
        if (_initConfig == null)
            throw new InvalidOperationException("PdoDataGlobalConfig has not been initialized. Call Initialize(config) at application startup.");
        return _initConfig;
    });

    private static IConfiguration _initConfig;
    private static bool _isInitialized;

    public static IConfiguration Configuration => _lazyConfiguration.Value;

    public static void Initialize(IConfiguration configuration)
    {
        if (_isInitialized)
            throw new InvalidOperationException("PdoDataGlobalConfig has already been initialized.");

        _initConfig = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _isInitialized = true;
    }
}