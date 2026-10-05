import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { User } from '../_models/User';
import { UserService } from '../_services/user.service';

export const UserResolver: ResolveFn<User[] | null> = (
  route,
  state
) => {
  const userService = inject(UserService);
  return userService.getAllUsers();
};
