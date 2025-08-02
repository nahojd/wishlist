import { AccountActionType } from "./Account/Actions";
import { WishlistActionType } from "./Actions";
import { IUserWish, IWishlistState } from "./Model";

export const createWishlistReducer = () => {
	return (state: IWishlistState = {}, action: { type: WishlistActionType|AccountActionType }) : IWishlistState => {
		switch(action.type) {
			case "getUsersComplete":
				return { ...state, ...{ users: (action as any).payload }};
			case "getUserWishesComplete":
				return userWishesLoaded(state, action as any);
			case "addWishComplete":
				return wishAdded(state, action as any);
			case "logout":
				return {}; //Clear state on logout
			default:
				return state;
		};
	}
};

function userWishesLoaded(state: IWishlistState, action: { meta: { userId: number }, payload: IUserWish[]}) : IWishlistState {
	if (!state.users?.some(x => x.id === action.meta.userId))
		return state;

	const users = [...state.users];
	const index = users.findIndex(x => x.id === action.meta.userId);
	const user = {...users[index]};
	user.wishes = action.payload;
	users[index] = user;

	return { ...state, ...{ users } };
}

interface IAddedWish {
	id: number;
	name: string;
	description?: string;
	linkUrl?: string;
	owner: { id: number }
}
function wishAdded(state: IWishlistState, action: { payload: IAddedWish }) : IWishlistState {
	const users = [...state.users];
	const index = users.findIndex(x => x.id === action.payload.owner.id);
	const user = {...users[index]};

	const wishes = [...user.wishes];
	const wish = {
		id: action.payload.id,
		name: action.payload.name,
		description: action.payload.description,
		linkUrl: action.payload.linkUrl
	}
	wishes.push(wish);

	user.wishes = wishes;
	users[index] = user;

	return { ...state, ...{ users } };
}