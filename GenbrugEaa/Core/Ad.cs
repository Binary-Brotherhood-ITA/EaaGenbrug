using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Core;

public class Ad
{
    // Unikt ID for annoncen
    public int Id { get; set; }

    // ID på brugeren, som sælger varen
    public int SellerId { get; set; }

    // Status på annoncen
    public string Status { get; set; }

    // Varens navn
    public string Name { get; set; } 

    // Varens kategori
    public string Category { get; set; } 

    // Varens pris
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }

    // Beskrivelse af varen
    public string Description { get; set; } 

    // Sti til varens billede
    public string ImgPath { get; set; } 

    // Varens stand
    public string Condition { get; set; } 

    // Navn på lokationen, hvis varen har en
    public string? Location { get; set; }

    // Dato for oprettelse af annoncen
    public DateTime CreatedAt { get; set; }

    // Dato for seneste statusændring
    public DateTime StatusUpdatedAt { get; set; } 
}