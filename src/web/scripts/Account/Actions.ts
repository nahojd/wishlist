import { RSAA } from "redux-api-middleware";
import { getDefaultHeaders } from "../app";

export type AccountActionType = "logout" |
	"loginStarted" | "loginComplete" | "loginFailed";

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

export const logout = () => ({ type: "logout" });