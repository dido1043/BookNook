using BookNook.Data.DTO;
using BookNook.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BookNook.Services.Data.Mapper;

[Mapper]    
public partial class ClientMapper
{
    public partial ClientDto ClientToDto(Client client);
    public partial Client DtoToModel(ClientDto clientDto);
}