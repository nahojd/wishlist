import { useEffect, useRef } from "react";
import React from "react";
import { useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { refreshLogin } from "../Account/Actions";

const refreshInterval = 10 * 60 * 1000; //Var 10 minut

export const RefreshLogin = () => {

	const dispatch = useDispatch();
	const authState = useStateSelector(state => state.account.auth);
	const intervalRef = useRef<number>();

	useEffect(() => {
		if (authState?.access_token) {
			//Starta interval (om det inte redan är igång)
			if (!intervalRef.current) {
				const interval = setInterval(() => {
					dispatch(refreshLogin());
				}, refreshInterval);
				intervalRef.current = interval;
				console.debug("Started refresh login interval", interval);
			}
		}
		else if (intervalRef.current) {
			clearInterval(intervalRef.current);
			console.debug("Stopped refresh login interval", intervalRef.current);
			intervalRef.current = null;
		}

	}, [authState?.access_token]);

	return <></>;
};