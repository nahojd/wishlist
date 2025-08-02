import { RSAA } from "redux-api-middleware";
import { getDefaultHeaders } from "./app";

export type WishlistActionType = "getUsersStarted" | "getUsersComplete" | "getUsersFailed" |
"getUserWishesStarted" | "getUserWishesComplete" | "getUserWishesFailed" |
"addWishStarted" | "addWishComplete" | "addWishFailed";

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

export const getUserWishes = (userId: number) => {

	const meta = { userId };

	return {
		type: "getUserWishes",
		[RSAA]: {
			endpoint: `/api/wish/list/${userId}`,
			headers: getDefaultHeaders(),
			method: "GET",
			types: [
				{ meta, type: "getUserWishesStarted" },
				{ meta, type: "getUserWishesComplete" },
				{ meta, type: "getUserWishesFailed" }
			]
		}
	};
};

export const addWish = (data: { name: string, description?: string, linkUrl?: string }) => {

	return {
		type: "addWish",
		[RSAA]: {
			endpoint: `/api/wish`,
			headers: getDefaultHeaders(),
			method: "POST",
			body: JSON.stringify(data),
			types: [
				"addWishStarted",
				"addWishComplete",
				"addWishFailed"
			]
		}
	};
};