import { configureStore } from "@reduxjs/toolkit";
import { combineReducers } from "redux";
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

const reducer = combineReducers({
	account: createAccountReducer()
});

//https://blog.logrocket.com/persist-state-redux-persist-redux-toolkit-react/
//https://redux-toolkit.js.org/usage/usage-guide#use-with-redux-persist

const persistConfig = {
	key: 'root',
	storage: storageSession,
};
const persistedReducer = persistReducer(persistConfig, reducer);

export const store = configureStore({
	reducer: persistedReducer,
	middleware: (getDefaultMiddleware) => getDefaultMiddleware({
		serializableCheck: {
			ignoredActions: [FLUSH, REHYDRATE, PAUSE, PERSIST, PURGE, REGISTER],
		}
	}).prepend(apiMiddleware)
})

export const persistor = persistStore(store);