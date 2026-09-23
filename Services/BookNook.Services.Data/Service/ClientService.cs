using BookNook.Data.DTO;
using BookNook.Data.Repository.Interface;

namespace BookNook.Services.Data.Service;

public class ClientService
{
    private readonly IClientRepository _clientRepository;
    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }
    
    public async Task<ClientDto> GetByIdAsync(int id)
    {
        var client = await _clientRepository.GetByIdAsync(id);
        return client;
    }

   
}