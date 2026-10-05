using api.Helpers;

namespace api.data.interfaces;

public interface IVideoService
{
    Task<string> AddVideo(VideoDto videoDto, CancellationToken cancellationToken);
    Task<string> GetAllVideosAsync(VideoParams videoParams, CancellationToken cancellationToken);
    Task<(Stream Stream, string ContentType)> GetVideoStreamAsync(
        int id,
        CancellationToken cancellationToken
    );
    Task<string> UpdateVideo(VideoDto videoDto, CancellationToken cancellationToken);

    Task<string> AddUserToUserVideos(int id,CancellationToken cancellationToken);

    Task<string> AddVideoToUser(int Videoid,int UserId,CancellationToken cancellationToken);

    Task<FileStream> GetThumbNail(string id);

    Task<string> getVideosForCurrentUser(VideoParams videoParams, CancellationToken cancellationToken);
    Task<string> UpdateVideoAllowance(int[]? videoAllowedToSee, int userId, CancellationToken cancellationToken);
    Task<string> GetVideoByCategory(int id, CancellationToken cancellationToken);
    Task<string> GetSpecificVideoDetailsAsync(int id, CancellationToken cancellationToken);
    Task<string> DeleteVideo(int id, CancellationToken cancellationToken);
}
