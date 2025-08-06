import { RSAA } from "redux-api-middleware";
import { getDefaultHeaders } from "./app";

export type WishlistActionType = "getUsersStarted" | "getUsersComplete" | "getUsersFailed" |
"getUserWishesStarted" | "getUserWishesComplete" | "getUserWishesFailed" |
"addWishStarted" | "addWishComplete" | "addWishFailed" |
"updateWishStarted" | "updateWishComplete" | "updateWishFailed" |
"deleteWishStarted" | "deleteWishComplete" | "deleteWishFailed";

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

export const updateWish = (id: number, data: { name: string, description?: string, linkUrl?: string }) => {

	return {
		type: "updateWish",
		[RSAA]: {
			endpoint: `/api/wish/${id}`,
			headers: getDefaultHeaders(),
			method: "PATCH",
			body: JSON.stringify(data),
			types: [
				"updateWishStarted",
				"updateWishComplete",
				"updateWishFailed"
			]
		}
	};
};

export const deleteWish = (id: number, userId: number) => {

	const meta = { id, userId };

	return {
		type: "deleteWish",
		[RSAA]: {
			endpoint: `/api/wish/${id}`,
			headers: getDefaultHeaders(),
			method: "DELETE",
			types: [
				{ meta, type: "deleteWishStarted" },
				{ meta, type: "deleteWishComplete" },
				{ meta, type: "deleteWishFailed" }
			]
		}
	};
};