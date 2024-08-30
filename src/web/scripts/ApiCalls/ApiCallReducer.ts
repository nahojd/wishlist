import { IApiCallState, LoadingState } from "./Models";

export interface IErrorDictionary
{
	[field: number]: string[]
}

export const createApiCallReducer = () => {
	return (state: IApiCallState = createInitialState(), action: { type: string, payload?: any }) : IApiCallState => {

		const match = action.type.match(/^([\S]+)(Started|Complete|Failed|Clear)$/);
		if (!match || match.length !== 3)
			return state;

		let loadingState: LoadingState;
		let errors: string|IErrorDictionary;
		let status: number;
		const type = match[1];
		switch(match[2])
		{
			case "Started":
				loadingState = "started";
				break;
			case "Complete":
				loadingState = "complete";
				break;
			case "Failed":
				loadingState = "failed";
				errors = (action.payload?.response?.errors && Object.getOwnPropertyNames(action.payload.response.errors).length > 0) ? action.payload?.response?.errors : action.payload?.response?.error;
				status = action.payload?.status;
				break;
		}

		let callStates = state.callStates.slice();
		const index = callStates.findIndex(x => x.type === type);
		if (index >= 0)
			callStates.splice(index, 1, { type, state: loadingState, errors, status });
		else
			callStates.push({ type, state: loadingState, errors, status });
		return { ...state, ...{ callStates }};
	}
}

function createInitialState() : IApiCallState {
	return {
		callStates: []
	};
}