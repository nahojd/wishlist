import { useSelector } from "react-redux";

export function useGenericSelector<S, T>(selectorFunc: (state: S) => T) {
	return useSelector((state :S) => selectorFunc(state)) as T;
};