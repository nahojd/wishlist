import { AccountActionType } from "./Account/Actions";
import { WishlistActionType } from "./Actions";
import { IWishlistState } from "./Model";

export const createWishlistReducer = () => {
	return (state: IWishlistState = {}, action: { type: WishlistActionType|AccountActionType }) : IWishlistState => {
		switch(action.type) {
			case "getUsersComplete":
				return { ...state, ...{ users: (action as any).payload }};
			case "logout":
				return {}; //Clear state on logout
			default:
				return state;
		};
	}
};