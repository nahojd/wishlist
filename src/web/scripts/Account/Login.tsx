import React, { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { login } from "./Actions";
import { PageHeader } from "../Components/PageHeader";
import { useNavigate } from "react-router-dom";
import { NavLink } from "react-router-dom";
import { useStateSelector } from "../Model";

export const LoginPage = () => {

	const user = useStateSelector(state => state.account.user);
	const isAuthenticated = !!user;

	const dispatch = useDispatch();
	const navigate = useNavigate();

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");

	const submit = () => {
		dispatch(login(email, password));
	};

	useEffect(() => {
		if (isAuthenticated)
			navigate("/");
	}, [isAuthenticated]);

	return <>
		<PageHeader />

		<article>
			<header>Logga in</header>
				<form>
					<fieldset>
						<label htmlFor="email">E-post</label>
						<input type="email" id="email" required onChange={e => setEmail(e.target.value)} value={email} />

						<label htmlFor="password">Lösenord</label>
						<input type="password" id="password" required onChange={e => setPassword(e.target.value)} value={password} />
					</fieldset>
				</form>

				<NavLink to="/register">Registrera konto</NavLink><br />
				<NavLink to="/forgotpassword">Glömt lösenord</NavLink>
			<footer>
				<button type="submit" onClick={submit}>Logga in</button>
			</footer>
		</article>
	</>
};