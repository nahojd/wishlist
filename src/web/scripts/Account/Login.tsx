import React, { useState } from "react";
import { useDispatch } from "react-redux";
import { login } from "./Actions";

export const LoginPage = () => {

	const dispatch = useDispatch();

	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");

	const submit = () => {
		dispatch(login(email, password));
	};

	return <div>
		<h1>Logga in</h1>

		<label>Epost</label>
		<input type="email" onChange={e => setEmail(e.target.value)} value={email} />
		<br />
		<label>Lösenord</label>
		<input type="password" onChange={e => setPassword(e.target.value)} value={password} />
		<br />
		<button type="submit" onClick={submit}>Logga in</button>
	</div>
};