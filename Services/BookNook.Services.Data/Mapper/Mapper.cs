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

    //Order line
    [MapProperty("Book.Title", nameof(OrderLineDto.BookTitle))]
    [MapProperty("Book.Author", nameof(OrderLineDto.BookAuthor))]
    public partial OrderLineDto OLToDto(OrderLine orderLine);

    [MapperIgnoreSource(nameof(OrderLineDto.BookTitle))]
    [MapperIgnoreSource(nameof(OrderLineDto.BookAuthor))]
    public partial OrderLine DtoToOL(OrderLineDto orderLineDto);

    //Order
    [MapProperty("Client.Name", nameof(OrderDto.ClientName))]
    public partial OrderDto OrderToDto(Order order);

    [MapperIgnoreSource(nameof(OrderDto.ClientName))]
    public partial Order DtoToOrder(OrderDto orderDto);

    public partial List<OrderDto> ToOrderDtoList(List<Order> orders);
}
