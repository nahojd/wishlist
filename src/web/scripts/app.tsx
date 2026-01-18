import React, { useEffect } from 'react';
import { createRoot } from 'react-dom/client';
import domready from "domready";
import { Provider } from 'react-redux';
import { getApiCallState, useStateSelector } from './Model';
import { PersistGate } from 'redux-persist/integration/react';
import { persistor, store } from './Store';
import { createBrowserRouter } from 'react-router';
import { RouterProvider } from 'react-router';
import { privateRoute, publicRoutes } from './Router';
import { RefreshLogin } from './Components/RefreshLogin';
import { setTheme } from './Account/Actions';
import { useDispatch } from 'react-redux';
import { getShoppingList } from './Actions';



const App = () => {

	const accountState = useStateSelector(state => state.account);
	const user = accountState.user;
	const isAuthenticated = !!user;

	const dispatch = useDispatch();
	const apicalls = getApiCallState();
	const tjingaState = apicalls.getLoadingState("tjinga");
	const avtjingaState = apicalls.getLoadingState("avtjinga");

	const router = createBrowserRouter([
		isAuthenticated ? privateRoute : {},
		...publicRoutes
	]);

	useEffect(() => {
		const theme = accountState?.theme || "light";
		document.documentElement.setAttribute("data-theme", theme);
	}, [accountState?.theme]);

	//Uppdatera shoppingslista när en önskning tjingas eller avtjingas
	useEffect(() => {
		if (tjingaState === "complete")
			dispatch(getShoppingList());
	}, [tjingaState,]);
	useEffect(() => {
		if (avtjingaState === "complete")
			dispatch(getShoppingList());
	}, [avtjingaState,]);

	return <>
		<RouterProvider router={router} />
		<RefreshLogin />
	</>;
};


export const getDefaultHeaders = () => {

	const authState = store.getState().account.auth;

	return {
		"Content-Type": "application/json",
		"Accept": "application/json",
		"Authorization": authState?.access_token ? `Bearer ${authState.access_token}` : null
	};
};

domready(async () => {
	const root = createRoot(document.getElementById("approot"));

	const themeCookie = await window?.cookieStore?.get("theme");
	if (themeCookie?.value === "dark" || themeCookie?.value === "light")
		store.dispatch(setTheme(themeCookie.value));
	else if (window?.matchMedia?.("(prefers-color-scheme:dark)").matches)
		store.dispatch(setTheme("dark"));

	root.render(
		<React.StrictMode>
			<Provider store={store}>
				<PersistGate loading={null} persistor={persistor}>
					<App />
				</PersistGate>
			</Provider>
		</React.StrictMode>
	);
})
