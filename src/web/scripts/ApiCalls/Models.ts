import { IErrorDictionary } from "./ApiCallReducer";

export type LoadingState = "started"|"complete"|"failed";

export interface IHydratedApiCallState extends IApiCallState {
	getLoadingState: (actionType: string) => LoadingState|undefined;
	getError: (actionType: string) => string;
	getErrors: (actionType: string) => IErrorDictionary;
	getStatus: (actionType: string) => number|undefined;
}

export interface IApiCallState
{
	callStates: {
		type: string;
		state?: LoadingState;
		status?: number;
		errors?: string|IErrorDictionary;
	}[];
}