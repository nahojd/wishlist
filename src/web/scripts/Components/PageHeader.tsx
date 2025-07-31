import React from "react";
import { useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { setTheme } from "../Account/Actions";

export const PageHeader = () => {
	const dispatch = useDispatch();
	const user = useStateSelector(state => state.account.user);

	return <header>
		<h1>Önskelistemaskinen v3</h1>
		{ user && <p>Du är inloggad som {user.name}!</p> }
		{ user?.theme === "dark" ?
			<button className="link" onClick={() => dispatch(setTheme("light"))}>Light</button> :
			<button className="link" onClick={() => dispatch(setTheme("dark"))}>Dark</button>}
	</header>;
};