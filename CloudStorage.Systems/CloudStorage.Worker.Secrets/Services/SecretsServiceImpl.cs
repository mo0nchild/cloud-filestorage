using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using CloudStorage.Domain.Authorization.Settings;
using CloudStorage.Shared.Contracts;
using CloudStorage.Shared.Contracts.Secrets;

namespace CloudStorage.Worker.SecretsStorage.Services;

public class SecretsServiceImpl : SecretsService.SecretsServiceBase
{
    private readonly SecretsAccessService _secretsService;
    private readonly ILogger<SecretsServiceImpl> _logger;
    private readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<TokenOptions, SecretsInfo>();
        })
        .CreateMapper(); 

    public SecretsServiceImpl(SecretsAccessService secretsService, ILogger<SecretsServiceImpl> logger)
    {
        _secretsService = secretsService;
        _logger = logger;
    }
    public override async Task<SecretsInfo> GetSecrets(Empty request, ServerCallContext context)
    {
        var result = await _secretsService.GetTokenSecrets();
        return _mapper.Map<SecretsInfo>(result);
    }
}