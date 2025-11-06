import { IApiCallState } from "./ApiCalls/Models";
import { hydrateApiCallState } from "./ApiCalls/Utils";
import { useGenericSelector } from "./Utils/Redux";

export const getApiCallState = () => hydrateApiCallState(useStateSelector(state => state.apicalls));
export function useStateSelector<T>(selectorFunc: (state: IWishlistAppState) => T) { return useGenericSelector((state :IWishlistAppState) => selectorFunc(state)); }


export interface IWishlistAppState
{
	account: IAccountState;
	wishlist: IWishlistState;
	apicalls: IApiCallState;
}

export interface IWishlistState
{
	users?: IUser[];
}

export interface IAccountState
{
	auth?: IOAuth2AuthResponse;
	user?: IUser;
	theme?: "dark"|"light";
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
	notify: boolean;
	wishes?: IUserWish[]
	isFriend?: boolean;
}

export interface IUserWish {
	id: number;
	name?: string;
	description?: string;
	linkUrl?: string;
	tjingadBy?: IUser;
}

export interface IWish extends IUserWish {
	owner: IUser;
}