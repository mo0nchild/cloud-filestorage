﻿namespace Pinterest.Application.Users.Infrastructures;

public interface IReserveUserImage
{
    public Task ReserveUserImageAsync(Guid imageUuid);
}