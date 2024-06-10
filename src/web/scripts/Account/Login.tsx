import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { login } from "./Actions";
import { PageHeader } from "../app";

export const LoginPage = () => {

	const dispatch = useDispatch();

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");

	const submit = () => {
		dispatch(login(email, password));
	};

	return <>
		<PageHeader />

		<article>
			<header>Logga in</header>
				<form>
					<fieldset>
						<label htmlFor="email">E-post</label>
						<input type="email" id="email"  onChange={e => setEmail(e.target.value)} value={email} />

						<label htmlFor="password">Lösenord</label>
						<input type="password" id="password" onChange={e => setPassword(e.target.value)} value={password} />
					</fieldset>
				</form>
			<footer>
				<button type="submit" onClick={submit}>Logga in</button>
			</footer>
		</article>
	</>
};