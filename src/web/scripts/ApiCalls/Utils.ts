import { IApiCallState, IHydratedApiCallState } from "./Models";
import { getStore } from "../Store";

export const hydrateApiCallState = (state: IApiCallState) : IHydratedApiCallState => {
	return {
		...state,
		...{
			getLoadingState: GetLoadingState,
			getError: GetError,
			getErrors: GetErrors,
			getStatus: GetStatus
		}
	}
}

const GetLoadingState = (actionType: string) => GetCallState(actionType)?.state;

const GetError = (actionType: string) => {
	const errors = GetCallState(actionType)?.errors;
	if (!errors)
		return null;
	if (typeof(errors) === "string")
		return errors;
	return Object.getOwnPropertyNames(errors).map(key => errors[key].join(", ")).join(", ");
};

const GetErrors = (actionType: string) => {
	const errors = GetCallState(actionType)?.errors;
	if (!errors)
		return {};
	if (typeof(errors) === "string")
		return { "error": [errors] };
	return errors;
};

const GetStatus = (actionType: string) => GetCallState(actionType)?.status;

const GetCallState = (actionType: string) => {
	const store = getStore();
	const state = store.getState() as { apicalls: IApiCallState; };
	if (!state?.apicalls?.callStates)
		return null;

	return state.apicalls.callStates.find(x => x.type === actionType);
}