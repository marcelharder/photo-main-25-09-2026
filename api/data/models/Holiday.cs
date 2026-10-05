namespace fotoservice.data.models;

public class Holiday
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int CoverImageUrl { get; set; }
    public List<Location> Locations { get; set; } = new();
}

public class Location
{
    public int Id { get; set; }
    public required string Name{ get; set; }
    public required string Description { get; set; }
    public required int CoverImageUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; } 
    public List<LocationImage> Images { get; set; } = new();
    
    }
    public class LocationImage
{
    public int Id { get; set; }
    public required int ImageUrl { get; set; }
    public string Caption { get; set; }
    
    }
