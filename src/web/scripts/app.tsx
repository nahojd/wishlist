import React from 'react';
import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';
import { LoginPage } from './Account/Login';
import { useGenericSelector } from './Utils/Redux';
import { IWishlistAppState } from './Model';
import { useDispatch } from 'react-redux';
import { logout } from './Account/Actions';
import { PersistGate } from 'redux-persist/integration/react';
import { persistor, store } from './Store';



const App = () => {

	const user = useStateSelector(state => state.account.user);
	const dispatch = useDispatch();

	if (!user)
		return <LoginPage />;

	return <>
		<PageHeader />

		<p>Du är inloggad som {user.name}!</p>
		<button className="outline" type="submit" onClick={() => dispatch(logout())}>Logga ut</button>
	</>
};

export const PageHeader = () => {
	return <header>
		<h1>Önskelistemaskinen v3</h1>
	</header>
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


root.render(
	<React.StrictMode>
		<Provider store={store}>
			<PersistGate loading={null} persistor={persistor}>
				<App />
			</PersistGate>
		</Provider>
	</React.StrictMode>
);