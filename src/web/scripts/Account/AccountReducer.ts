import { IAccountState } from "../Model"
import { AccountActionType } from "./Actions";

export const createAccountReducer = () => {
	return (state: IAccountState = {}, action: { type: AccountActionType }) : IAccountState => {
		switch(action.type) {
			case "loginComplete":
				return { ...state, ...{ user: (action as any).payload.user, auth: (action as any).payload.auth }};
			case "logout":
				return {};
			default:
				return state;
		};
	}
};