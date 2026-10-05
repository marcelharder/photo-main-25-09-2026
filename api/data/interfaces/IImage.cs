
namespace fotoservice.data.interfaces;

public interface IImage
{

    Task<CarouselDto> getCarouselData(int id, CancellationToken cancellationToken);
   
    Task<string> addImage(ImageDto imdto, CancellationToken cancellationToken);
    Task<int> deleteImage(int id, CancellationToken cancellationToken);
    Task<string> updateImage(ImageDto image, CancellationToken cancellationToken);
    Task<ImageDto> findImage(int Id, CancellationToken cancellationToken);
    Task<PagedList<ImageDto>> findImagesByUser(ImageParams ip,CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync();
    Task<PagedList<ImageDto>> GetPhotosBySpecificCategory(ImageParams ip, CancellationToken cancellationToken);
    Task<int> GetTotalItemsInCategory(int category, CancellationToken cancellationToken);
}
