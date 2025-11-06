import { configureStore } from "@reduxjs/toolkit";
import { Action, combineReducers, Middleware } from "redux";
import { apiMiddleware } from "redux-api-middleware";
import {
	persistStore,
	persistReducer,
	FLUSH,
	REHYDRATE,
	PAUSE,
	PERSIST,
	PURGE,
	REGISTER,
  } from 'redux-persist';
import { createAccountReducer } from "./Account/AccountReducer";
import storageSession from 'redux-persist/lib/storage/session';
import { createWishlistReducer } from "./Reducer";
import { createApiCallReducer } from "./ApiCalls/ApiCallReducer";
import { logout } from "./Account/Actions";

const authMiddleware: Middleware = store => next => async (action: Action) => {
	if (action.type.endsWith("Failed") && (action as any).payload?.status === 401) {
		console.debug("Got 401 from API -- logging out user");
		store.dispatch(logout());
	}

	next(action);
};

const reducer = combineReducers({
	apicalls: createApiCallReducer(),
	account: createAccountReducer(),
	wishlist: createWishlistReducer()
});

//https://blog.logrocket.com/persist-state-redux-persist-redux-toolkit-react/
//https://redux-toolkit.js.org/usage/usage-guide#use-with-redux-persist

const persistConfig = {
	key: 'root',
	storage: storageSession,
	blacklist: ["apicalls"] //Vi vill inte persistera apicalls, för det ska inte behövas
};
const persistedReducer = persistReducer(persistConfig, reducer);

export const store = configureStore({
	reducer: persistedReducer,
	middleware: (getDefaultMiddleware) => getDefaultMiddleware({
		serializableCheck: {
			ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER,
							"resetPasswordForEmailFailed", "loginFailed", "registerFailed", "refreshLoginFailed",
							"validatePwdResetTokenFailed", "resetPasswordFailed",
							"getUsersFailed", "getUserWishesFailed", "addWishFailed", "deleteWishFailed", "updateWishFailed",
							"updateUserSettingsFailed", "updatePasswordFailed",
							"toggleFriendStatusFailed"
			],
		}
	}).prepend(apiMiddleware, authMiddleware)
});

export const getStore = () => store;
export const persistor = persistStore(store);