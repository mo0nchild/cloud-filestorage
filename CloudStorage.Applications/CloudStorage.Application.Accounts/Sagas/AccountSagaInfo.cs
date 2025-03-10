using AutoMapper;
using CloudStorage.Domain.Core.Transactions;

namespace CloudStorage.Application.Accounts.Sagas;

using RegistrateAccountState = SagaStoreEntity<AccountSagaState, Guid>;
public class AccountSagaInfo
{
    public required Guid SagaUuid { get; set; }
    public required string Status { get; set; }
    public RollbackSagaMessage? RollbackMessage { get; set; }
}

public class AccountSagaInfoProfile : Profile
{
    public AccountSagaInfoProfile()
    {
        CreateMap<RegistrateAccountState, AccountSagaInfo>()
            .ForMember(dest => dest.SagaUuid, opt => opt.MapFrom(src => src.SagaUuid))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.RollbackMessage, opt => opt.MapFrom(src => src.RollbackMessage));
    }
}