import { configureStore } from '@reduxjs/toolkit';
import React from 'react';
import { createRoot } from 'react-dom/client';
import { apiMiddleware } from "redux-api-middleware";
import { createAccountReducer } from './Account/AccountReducer';
import { Provider } from 'react-redux';
import { LoginPage } from './Account/Login';
import { useGenericSelector } from './Utils/Redux';
import { IWishlistAppState } from './Model';
import { useDispatch } from 'react-redux';
import { logout } from './Account/Actions';
import { Button } from "react-bootstrap";



const App = () => {

	const user = useStateSelector(state => state.account.user);
	const dispatch = useDispatch();

	if (!user)
		return <LoginPage />;

	return <div className="container">
		<PageHeader />

		<p>Du är inloggad som {user.name}!</p>
		<Button variant="outline-primary" type="button" onClick={() => dispatch(logout())}>Logga ut</Button>
	</div>
};

export const PageHeader = () => {
	return <div className="page-header">
		<h1>Önskelistemaskinen v3</h1>
	</div>
};

export function useStateSelector<T>(selectorFunc: (state: IWishlistAppState) => T) { return useGenericSelector((state :IWishlistAppState) => selectorFunc(state)); }

export const getDefaultHeaders = () => {

	const authState = store.getState().account.auth;

	return {
		"Content-Type": "application/json",
		"Accept": "application/json",
		"Authorization": authState?.access_token ? `Bearer ${authState.access_token}` : null
	};
};



const root = createRoot(document.getElementById("approot"));

const store = configureStore({
	reducer: {
		account: createAccountReducer()
	},
	middleware: (getDefaultMiddleware) => getDefaultMiddleware().prepend(apiMiddleware)
})

root.render(<Provider store={store}>
				<App />
			</Provider>);