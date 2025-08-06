import { AccountActionType } from "./Account/Actions";
import { WishlistActionType } from "./Actions";
import { IUserWish, IWish, IWishlistState } from "./Model";

export const createWishlistReducer = () => {
	return (state: IWishlistState = {}, action: { type: WishlistActionType|AccountActionType }) : IWishlistState => {
		switch(action.type) {
			case "getUsersComplete":
				return { ...state, ...{ users: (action as any).payload }};
			case "getUserWishesComplete":
				return userWishesLoaded(state, action as any);
			case "addWishComplete":
				return wishAdded(state, action as any);
			case "updateWishComplete":
			case "tjingaComplete":
			case "avtjingaComplete":
				return wishUpdated(state, action as any);
			case "deleteWishComplete":
				return wishDeleted(state, action as any);
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

function wishAdded(state: IWishlistState, action: { payload: IWish }) : IWishlistState {
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

function wishDeleted(state: IWishlistState, action: { meta: { id: number, userId: number } }) : IWishlistState {
	const users = [...state.users];
	const index = users.findIndex(x => x.id === action.meta.userId);
	const user = {...users[index]};

	const wishes = [...user.wishes];
	const wishIndex = wishes.findIndex(x => x.id === action.meta.id);
	if (wishIndex < 0)
		return state;

	wishes.splice(wishIndex, 1);

	user.wishes = wishes;
	users[index] = user;

	return { ...state, ...{ users } };
}

function wishUpdated(state: IWishlistState, action: { payload: IWish }) : IWishlistState {
	const users = [...state.users];
	const index = users.findIndex(x => x.id === action.payload.owner.id);
	const user = {...users[index]};

	const wishes = [...user.wishes];
	const wishIndex = wishes.findIndex(x => x.id === action.payload.id);
	if (wishIndex < 0)
		return state;

	wishes[wishIndex] = {
		id: action.payload.id,
		name: action.payload.name,
		description: action.payload.description,
		linkUrl: action.payload.linkUrl,
		tjingadBy: action.payload.tjingadBy
	};

	user.wishes = wishes;
	users[index] = user;

	return { ...state, ...{ users } };
}