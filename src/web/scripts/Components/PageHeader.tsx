import React from "react";
import { useStateSelector } from "../Model";

export const PageHeader = () => {
	const user = useStateSelector(state => state.account.user);

	return <header>
		<h1>Önskelistemaskinen v3</h1>
		{ user && <p>Du är inloggad som {user.name}!</p> }
	</header>;
};