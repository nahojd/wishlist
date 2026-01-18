import React, { useEffect } from "react";
import { useStateSelector } from "../Model";
import { useDispatch } from "react-redux";
import { setTheme } from "../Account/Actions";
import { Link } from "react-router";
import logoImage from "url:../../images/logo.png";
import Icon from "@mdi/react";
import { mdiWeatherNight, mdiWeatherSunny } from "@mdi/js";

export const PageHeader = () => {
	const dispatch = useDispatch();
	const state = useStateSelector(state => state.account);
	const user = state.user;

	useEffect(() => {
		if (state.theme && window.cookieStore)
			window.cookieStore.set("theme", state.theme);
	}, [state.theme]);

	return <header>

		<nav>
			<Link to="/"><img className="logo" src={logoImage} alt="Önskelistemaskinen v3" /></Link>
			<ul>
				{ user && <>
					<li><Link to={`/user/${user.id}`}>Min önskelista</Link></li>
					<li><Link to="/profile">Mitt konto</Link></li>
				</>}
				<li>
					{ state?.theme === "dark" ?
						<button className="icon" onClick={() => dispatch(setTheme("light"))} title="Växla till ljust läge"><Icon path={mdiWeatherSunny} size={1} /></button> :
						<button className="icon" onClick={() => dispatch(setTheme("dark"))} title="Växla till mörkt läge"><Icon path={mdiWeatherNight} size={1} /></button>}
				</li>
			</ul>
		</nav>
	</header>;
};