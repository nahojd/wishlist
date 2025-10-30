import React from "react";
import { useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { setTheme } from "../Account/Actions";
import { Link } from "react-router";
import logoImage from "url:../../images/logo.png";

export const PageHeader = () => {
	const dispatch = useDispatch();
	const state = useStateSelector(state => state.account);
	const user = state.user;

	return <header>

		<nav>
			<Link to="/"><img className="logo" src={logoImage} alt="Önskelistemaskinen v3" /></Link>
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