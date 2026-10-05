namespace fotoservice.data.dtos;

public class CategoryDto
{
    public int? Id { get; set; }
    public string? Description { get; set; }
    public string? Name { get; set; }
    public int? MainPhoto { get; set; }
    public int? Number_of_images { get; set; }
    public int? YearTaken {get; set;}

       
    }