using Applications.Layer.DTOs;

namespace Applications.Layer.Interfaces;

public interface IStatsService
{
    Task<StatsResponse> GetStatsAsync();
}
