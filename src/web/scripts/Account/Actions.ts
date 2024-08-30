import { RSAA } from "redux-api-middleware";
import { getDefaultHeaders } from "../app";

export type AccountActionType = "logout" |
	"loginStarted" | "loginComplete" | "loginFailed" |
	"registerStarted" | "registerComplete" | "registerFailed";

export const login = (email: string, password: string) => {

	const payload = { email, password };

	return {
		type: "login",
		[RSAA]: {
			endpoint: "/api/account/login",
			headers: getDefaultHeaders(),
			method: "POST",
			body: JSON.stringify(payload),
			types: [
				"loginStarted",
				"loginComplete",
				"loginFailed"
			]
		}
	};
};

export const registerUser = (registerData: { name: string, email: string, password: string, message?: string }) => {

	return {
		type: "register",
		[RSAA]: {
			endpoint: "/api/account/register",
			headers: getDefaultHeaders(),
			method: "POST",
			body: JSON.stringify(registerData),
			types: [
				"registerStarted",
				"registerComplete",
				"registerFailed"
			]
		}
	};
};


export const logout = () => ({ type: "logout" });