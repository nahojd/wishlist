import React, { useEffect, useState } from "react";
import { useDispatch } from "react-redux";
import { login } from "./Actions";
import { PageHeader } from "../Components/PageHeader";
import { useNavigate, NavLink } from "react-router";
import { getApiCallState, useStateSelector } from "../Model";
import { Alert } from "../Components/Alert";
import { clearApiCallState } from "../ApiCalls/Actions";

export const LoginPage = () => {

	const user = useStateSelector(state => state.account.user);
	const isAuthenticated = !!user;

	const apicalls = getApiCallState();
	const submitState = apicalls.getLoadingState("login");

	const dispatch = useDispatch();
	const navigate = useNavigate();

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");

	const submit = (e: React.FormEvent) => {
		dispatch(login(email, password));
		e.preventDefault();
	};

	useEffect(() => {
		return () => {
			dispatch(clearApiCallState("login"));
		};
	}, []);

	useEffect(() => {
		if (isAuthenticated)
			navigate("/");
	}, [isAuthenticated]);

	return <>
		<PageHeader />

		<article>
			<header>Logga in</header>
				<form onSubmit={submit}>
					<fieldset>
						<label htmlFor="email">E-post</label>
						<input type="email" id="email" required onChange={e => setEmail(e.target.value)} value={email} />

						<label htmlFor="password">Lösenord</label>
						<input type="password" id="password" required onChange={e => setPassword(e.target.value)} value={password} />
					</fieldset>

					<Alert type="danger" show={submitState === "failed"}>Fel e-post eller lösenord!</Alert>

					<button type="submit" aria-busy={submitState === "started"}>Logga in</button>
				</form>
			<footer>
				<NavLink to="/register">Registrera konto</NavLink><br />
				<NavLink to="/forgotpassword">Glömt lösenord</NavLink>
			</footer>
		</article>
	</>
};