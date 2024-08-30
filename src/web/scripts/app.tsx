import React from 'react';
import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';
import { useStateSelector } from './Model';
import { PersistGate } from 'redux-persist/integration/react';
import { persistor, store } from './Store';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import { privateRoute, publicRoutes } from './Router';



const App = () => {

	const user = useStateSelector(state => state.account.user);
	const isAuthenticated = !!user;

	const router = createBrowserRouter([
		isAuthenticated ? privateRoute : {},
		...publicRoutes
	]);

	return <RouterProvider router={router} />;

	// if (!user)
	// 	return <LoginPage />;

	// return <Home />;
};


export const getDefaultHeaders = () => {

	const authState = store.getState().account.auth;

	return {
		"Content-Type": "application/json",
		"Accept": "application/json",
		"Authorization": authState?.access_token ? `Bearer ${authState.access_token}` : null
	};
};



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