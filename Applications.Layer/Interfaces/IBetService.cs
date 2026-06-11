using Applications.Layer.DTOs;

namespace Applications.Layer.Interfaces;

public interface IBetService
{
    Task<PlaceBetResponse> PlaceBetAsync(PlaceBetRequest request);
}
