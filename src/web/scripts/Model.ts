export interface IWishlistAppState
{
	account: IAccountState;
}

export interface IAccountState
{
	auth?: IOAuth2AuthResponse;
	user?: IUser;
}


export interface ILoginResponse
{
	auth: IOAuth2AuthResponse;
	user: IUser;
}

export interface IOAuth2AuthResponse
{
	access_token: string;
	expires_in: number;
	token_type: "Bearer";
}

export interface IUser
{
	id: number;
	name: string;
	email: string;
}