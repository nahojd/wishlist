import { IApiCallState, IHydratedApiCallState } from "./Models";
import { getStore } from "../Store";

export const hydrateApiCallState = (state: IApiCallState) : IHydratedApiCallState => {
	return {
		...state,
		...{
			getLoadingState: GetLoadingState,
			getError: GetError,
			getStatus: GetStatus
		}
	}
}

const GetLoadingState = (actionType: string) => GetCallState(actionType)?.state;
const GetError = (actionType: string) => GetCallState(actionType)?.errors;
const GetStatus = (actionType: string) => GetCallState(actionType)?.status;

const GetCallState = (actionType: string) => {
	const store = getStore();
	const state = store.getState() as { apicalls: IApiCallState; };
	if (!state?.apicalls?.callStates)
		return null;

	return state.apicalls.callStates.find(x => x.type === actionType);
}