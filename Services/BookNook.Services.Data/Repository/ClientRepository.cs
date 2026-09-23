using BookNook.Data.DTO;
using BookNook.Services.Data.Mapper;

namespace BookNook.Services.Data.Repository;
using BookNook.Data;
using BookNook.Data.Repository.Interface;
using BookNook.Data.Models;

public class ClientRepository : IClientRepository
{
    private readonly BookNookContext _context;
    private readonly ClientMapper _clientMapper;
    public ClientRepository(BookNookContext context,  ClientMapper clientMapper)
    {
        _context = context;
        _clientMapper = clientMapper;
    }

    public async Task<ClientDto?> GetByIdAsync(int id)
    {
        return _clientMapper.ClientToDto(await _context.Users.FindAsync(id));
    }

  
    public void Update(ClientDto client)
    {
        _context.Users.Update(_clientMapper.DtoToModel(client));
    }

    public void Delete(ClientDto client)
    {
        _context.Users.Remove(_clientMapper.DtoToModel(client));
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
