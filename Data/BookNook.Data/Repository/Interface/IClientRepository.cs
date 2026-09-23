using BookNook.Data.DTO;
using BookNook.Data.Models;

namespace BookNook.Data.Repository.Interface;

public interface IClientRepository
{
    Task<ClientDto?> GetByIdAsync(int id);
    void Update(ClientDto book);
    void Delete(ClientDto book);
    Task SaveAsync();
}
