import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { CategoryListComponent } from './dias/categoryList/categoryList.component';
import { ImageViewerComponent } from './image-viewer/image-viewer.component';
import { AboutComponent } from './About/About.component';
import { PhotoListComponent } from './photo-list/photo-list.component';
import { TapeListComponent } from './tape-list/tape-list.component';
import { DiaListComponent } from './dias/dia-list/dia-list.component';
import { FulldiaComponent } from './dias/fulldia/fulldia.component';
import { fulldiaResolver } from './_resolvers/fulldia.resolver';
import { DashboardComponent } from './Admin/Users/dashboard/dashboard.component';
import { UserResolver } from './_resolvers/user.resolver';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { VideoListComponent } from './videos/video-list/video-list';
import { FullvideoComponent } from './videos/fullvideo/fullvideo';
import { AddVideo } from './Admin/videos/add-video/add-video';
import { UpdateVideo } from './Admin/videos/update-video/update-video';
import { DeleteVideo } from './Admin/videos/delete-video/delete-video';
import { ManageVideos } from './Admin/videos/manage-videos/manage-videos';
import { VideoResolver } from './_resolvers/video.resolver';
import { ManageImages } from './Admin/Slides/manage-images/manage-images';
import { CategoryResolver } from './_resolvers/category.resolver';
import { ManageCategoriesComponent } from './Admin/Slides/manage-categories/ManageCategoriesComponent';
import { EditCategory } from './Admin/Slides/manage-categories/edit-category/edit-category';
import { AddImageComponent } from './Admin/Slides/add-image/add-image.component';
import { Holiday } from './holiday/holiday';
import { holidayResolver } from './_resolvers/holidayResolver';
import { Fullholidayimage } from './holiday/fullholidayimage/fullholidayimage';
import { ListOfHolidaysComponent } from './Admin/Holidays/list-of-holidays/list-of-holidays.component';
import { AddHoliday } from './Admin/Holidays/add-holiday/add-holiday';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'about', component: AboutComponent },
  { path: 'categoryList', component: CategoryListComponent},
  { path: 'photoList', component: PhotoListComponent},
  { path: 'tapeList', component: TapeListComponent},
  { path: 'videoList', component: VideoListComponent},
  { path: 'editProfile', component: EditProfileComponent},
  { path: 'diaList/:id', component: DiaListComponent},
  { path: 'imageViewer/:id', component: ImageViewerComponent},
  { path: 'dashboard', component: DashboardComponent, resolve:{us:UserResolver}},
  { path: 'fulldia/:id', component: FulldiaComponent, resolve: {dia: fulldiaResolver}},
  { path: 'fullVideo/:id', component: FullvideoComponent},
  { path: 'addVideo', component: AddVideo},
  { path: 'updateVideo/:id', component: UpdateVideo},
  { path: 'deleteVideo/:id', component: DeleteVideo},
  { path: 'manageVideos', component: ManageVideos, resolve:{vid: VideoResolver}},
  { path: 'manageCategories', component: ManageCategoriesComponent, resolve:{lc: CategoryResolver}},
  { path: 'manageImages', component: ManageImages},
  { path: 'editCategory/:id', component: EditCategory},
  { path: 'addImage/:id', component: AddImageComponent},
  { path: 'holidayList', component: Holiday, resolve:{hr: holidayResolver}},
  { path: 'holidayfullimage/:id', component: Fullholidayimage},
  { path: 'adminHolidayList', component: ListOfHolidaysComponent, resolve:{hr: holidayResolver}},
 
  
  { path: '**', component: HomeComponent, pathMatch: 'full'}
];
