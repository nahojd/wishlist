import React, { useEffect } from 'react';
import { createRoot } from 'react-dom/client';
import domready from "domready";
import { Provider } from 'react-redux';
import { useStateSelector } from './Model';
import { PersistGate } from 'redux-persist/integration/react';
import { persistor, store } from './Store';
import { createBrowserRouter } from 'react-router';
import { RouterProvider } from 'react-router';
import { privateRoute, publicRoutes } from './Router';
import { RefreshLogin } from './Components/RefreshLogin';



const App = () => {

	const user = useStateSelector(state => state.account.user);
	const isAuthenticated = !!user;

	const router = createBrowserRouter([
		isAuthenticated ? privateRoute : {},
		...publicRoutes
	]);

	useEffect(() => {
		const theme = user?.theme || "light";
		document.documentElement.setAttribute("data-theme", theme);
	}, [user?.theme]);

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

domready(() => {
	const root = createRoot(document.getElementById("approot"));

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
