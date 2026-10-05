import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { VideoService } from '../_services/video-service';
import { VideoModel } from '../_models/VideoModel';

export const VideoResolver: ResolveFn<any | null> = (
  route,
  state
) => {
  const videoService = inject(VideoService);
  return videoService.videoByCategory(1);
};
