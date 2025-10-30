import { IAccountState } from "../Model"
import { AccountActionType } from "./Actions";

export const createAccountReducer = () => {
	return (state: IAccountState = {}, action: { type: AccountActionType }) : IAccountState => {
		switch(action.type) {
			case "loginComplete":
				return { ...state, ...{ user: (action as any).payload.user, auth: (action as any).payload.auth }};
			case "refreshLoginComplete":
				return { ...state, ...{ auth: (action as any).payload }};
			case "setTheme":
				return { ...state, ...{ theme: (action as any).theme } };
			case "logout":
				return { theme: state.theme };
			case "updateUserSettingsComplete":
				return { ...state, ...{ user: { ...state.user, ...(action as any).payload }}};
			default:
				return state;
		};
	}
};