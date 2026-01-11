import { RSAA } from "redux-api-middleware";
import { getDefaultHeaders } from "./app";

export type WishlistActionType = "getUsersStarted" | "getUsersComplete" | "getUsersFailed" |
"getUserWishesStarted" | "getUserWishesComplete" | "getUserWishesFailed" |
"addWishStarted" | "addWishComplete" | "addWishFailed" |
"updateWishStarted" | "updateWishComplete" | "updateWishFailed" |
"deleteWishStarted" | "deleteWishComplete" | "deleteWishFailed" |
"tjingaStarted" | "tjingaComplete" | "tjingaFailed" |
"avtjingaStarted" | "avtjingaComplete" | "avtjingaFailed" |
"getShoppingListStarted" | "getShoppingListComplete" | "getShoppingListFailed" |
"toggleFriendStatusStarted" | "toggleFriendStatusComplete" | "toggleFriendStatusFailed";

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

export const toggleFriendStatus = (userId: number) => {

	const meta = { userId };

	return {
		type: "toggleFriendStatus",
		[RSAA]: {
			endpoint: `/api/user/${userId}/toggleFriend`,
			headers: getDefaultHeaders(),
			method: "POST",
			types: [
				{ meta, type: "toggleFriendStatusStarted" },
				{ meta, type: "toggleFriendStatusComplete" },
				{ meta, type: "toggleFriendStatusFailed" }
			]
		}
	};
};

export const getShoppingList = () => {

	return {
		type: "getShoppingList",
		[RSAA]: {
			endpoint: `/api/wish/shoppinglist`,
			headers: getDefaultHeaders(),
			method: "GET",
			types: [
				{ type: "getShoppingListStarted" },
				{ type: "getShoppingListComplete" },
				{ type: "getShoppingListFailed" }
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

export const tjinga = (id: number) => {
	return {
		type: "tjinga",
		[RSAA]: {
			endpoint: `/api/wish/${id}/tjinga`,
			headers: getDefaultHeaders(),
			method: "POST",
			types: [
				"tjingaStarted",
				"tjingaComplete",
				"tjingaFailed"
			]
		}
	};
};

export const avtjinga = (id: number) => {
	return {
		type: "avtjinga",
		[RSAA]: {
			endpoint: `/api/wish/${id}/avtjinga`,
			headers: getDefaultHeaders(),
			method: "POST",
			types: [
				"avtjingaStarted",
				"avtjingaComplete",
				"avtjingaFailed"
			]
		}
	};
}