using BookNook.Data.DTO;
using BookNook.Data.Models;
using Riok.Mapperly.Abstractions;

namespace BookNook.Services.Data.Mapper;

[Mapper]    
public partial class Mapper
{
    //Clients
    public partial ClientDto ClientToDto(Client client);
    public partial Client DtoToModel(ClientDto clientDto);
    
    //Books
    public partial BookDto BookToDto(Book book);
    public partial Book DtoToBook(BookDto bookDto);
    public partial List<BookDto> ToBookDtoList(List<Book> objectList);
}