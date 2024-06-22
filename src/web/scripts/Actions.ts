import { RSAA } from "redux-api-middleware";
import { getDefaultHeaders } from "./app";

export type WishlistActionType = "getUsersStarted" | "getUsersComplete" | "getUsersFailed";

export const getUsers = () => {

	return {
		type: "getUsers",
		[RSAA]: {
			endpoint: "/api/user/list",
			headers: getDefaultHeaders(),
			method: "GET",
			types: [
				"getUsersStarted",
				"getUsersComplete",
				"getUsersFailed"
			]
		}
	};
};