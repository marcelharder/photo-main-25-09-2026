export interface User {
    Id:number,
    UserName: string;
    Token: string;
    IsAdmin?: boolean;
    Created: Date;
    paidTill: Date;
    password:string,
    Email:string,
    PhotoAllowedToSee:number[],
    VideoAllowedToSee:number[],
    gender:string,
    PhoneNumber:string
}