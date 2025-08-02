import { RSAA } from "redux-api-middleware";
import { getDefaultHeaders } from "../app";

export type AccountActionType = "logout" |
	"loginStarted" | "loginComplete" | "loginFailed" |
	"refreshLoginStarted" | "refreshLoginComplete" | "refreshLoginFailed" |
	"registerStarted" | "registerComplete" | "registerFailed" |
	"resetPasswordForEmailStarted" | "resetPasswordForEmailComplete" | "resetPasswordForEmailFailed" |
 	"setTheme";

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

export const refreshLogin = () => {
	return {
		type: "refreshLogin",
		[RSAA]: {
			endpoint: "/api/account/refreshlogin",
			headers: getDefaultHeaders(),
			method: "POST",
			types: [
				"refreshLoginStarted",
				"refreshLoginComplete",
				"refreshLoginFailed"
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

export const resetPasswordForEmail = (email: string) => {

	return {
		type: "resetPasswordForEmail",
		[RSAA]: {
			endpoint: `/api/account/resetpwd/${email}`,
			headers: getDefaultHeaders(),
			method: "POST",
			types: [
				"resetPasswordForEmailStarted",
				"resetPasswordForEmailComplete",
				"resetPasswordForEmailFailed"
			]
		}
	};
};

export const validatePwdResetToken = (token: string) => {

	return {
		type: "validatePwdResetToken",
		[RSAA]: {
			endpoint: `/api/account/validateresettoken?token=${token}`,
			headers: getDefaultHeaders(),
			method: "POST",
			types: [
				"validatePwdResetTokenStarted",
				"validatePwdResetTokenComplete",
				"validatePwdResetTokenFailed"
			]
		}
	};
};

export const resetPassword = (token: string, password: string) => {

	const payload = { token, password };

	return {
		type: "resetPassword",
		[RSAA]: {
			endpoint: `/api/account/resetpwd`,
			headers: getDefaultHeaders(),
			method: "POST",
			body: JSON.stringify(payload),
			types: [
				"resetPasswordStarted",
				"resetPasswordComplete",
				"resetPasswordFailed"
			]
		}
	};
};


export const logout = () => ({ type: "logout" });

export const setTheme = (theme: "light"|"dark") => ({ type: "setTheme", theme});