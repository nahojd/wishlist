import React from "react";
import { useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { setTheme } from "../Account/Actions";
import { Link } from "react-router";

export const PageHeader = () => {
	const dispatch = useDispatch();
	const state = useStateSelector(state => state.account);
	const user = state.user;

	return <header>
		<nav>
			<ul>
				<li><strong><Link to="/">Önskelistemaskinen v3</Link></strong></li>
			</ul>
			<ul>
				{ user && <>
					<li><Link to="/profile">Mitt konto</Link></li>
				</>}
				<li>
					{ state?.theme === "dark" ?
						<button className="link" onClick={() => dispatch(setTheme("light"))}>Light</button> :
						<button className="link" onClick={() => dispatch(setTheme("dark"))}>Dark</button>}
				</li>
			</ul>
		</nav>
	</header>;
};